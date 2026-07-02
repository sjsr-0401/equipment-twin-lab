using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using EquipmentTwin.Core;
using EquipmentTwin.Core.Processes;
using EquipmentTwin.Hmi.Wpf.Models;
using EquipmentTwin.Hmi.Wpf.Services;

namespace EquipmentTwin.Hmi.Wpf.ViewModels;

public sealed class OperatorConsoleViewModel : ObservableObject
{
    private static readonly Brush BackgroundBrush = Brush("#16191D");
    private static readonly Brush SurfaceBrush = Brush("#20252B");
    private static readonly Brush SurfaceRaisedBrush = Brush("#2B3138");
    private static readonly Brush SurfaceActiveBrush = Brush("#3A424B");
    private static readonly Brush ChamberNormalBrush = Brush("#202A31");
    private static readonly Brush ChamberAlarmBrush = Brush("#4A2428");
    private static readonly Brush TextPrimaryBrush = Brush("#E5E8EB");
    private static readonly Brush TextMutedBrush = Brush("#A6ADB5");
    private static readonly Brush NeutralSignalBrush = Brush("#8D97A3");
    private static readonly Brush WarningBrush = Brush("#DCA84A");
    private static readonly Brush AlarmBrush = Brush("#D14B4B");
    private static readonly Brush AlarmSurfaceBrush = Brush("#5A2428");

    private readonly MolyAldRecipeService recipeService = new();
    private readonly MolyAldRunner runner = new(new ManualClock(new DateTimeOffset(2026, 7, 1, 0, 0, 0, TimeSpan.Zero)));
    private readonly DispatcherTimer playbackTimer;
    private MolyAldRecipe? recipe;
    private MolyAldTimelineDocument? timeline;
    private int currentStepIndex;
    private bool isRunning;
    private bool isAlarmActive;
    private string selectedFaultScenario = string.Empty;

    public OperatorConsoleViewModel()
    {
        StartCommand = new RelayCommand(Start);
        PauseCommand = new RelayCommand(Pause);
        ResetCommand = new RelayCommand(Reset);
        FaultReplayCommand = new RelayCommand(FaultReplay);
        StepForwardCommand = new RelayCommand(StepForward);
        OpenUnityViewerCommand = new RelayCommand(OpenUnityViewer);

        playbackTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(750)
        };
        playbackTimer.Tick += (_, _) => StepForwardFromTimer();

        LoadRecipeAndTimeline();
    }

    public ObservableCollection<string> FaultScenarios { get; } = new();

    public ObservableCollection<StepRowViewModel> Steps { get; } = new();

    public ObservableCollection<OperatorLogEntry> OperatorLog { get; } = new();

    public ICommand StartCommand { get; }

    public ICommand PauseCommand { get; }

    public ICommand ResetCommand { get; }

    public ICommand FaultReplayCommand { get; }

    public ICommand StepForwardCommand { get; }

    public ICommand OpenUnityViewerCommand { get; }

    public string SelectedFaultScenario
    {
        get => selectedFaultScenario;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            if (isAlarmActive)
            {
                AddLog("SELECT BLOCKED", $"{value} locked during active alarm");
                OnPropertyChanged();
                RefreshComputedProperties();
                return;
            }

            if (SetProperty(ref selectedFaultScenario, value))
            {
                AddLog("SELECT FAULT", $"{selectedFaultScenario} ready");
                RefreshComputedProperties();
            }
        }
    }

    public Brush PageBackground => BackgroundBrush;

    public string Title => "MOLY ALD WPF HMI";

    public string Subtitle => "Main operator console | Core-driven synthetic public-reference process";

    public string RunStateText
    {
        get
        {
            if (isAlarmActive)
            {
                return "HELD | OPERATOR ACTION REQUIRED";
            }

            return isRunning ? "RUNNING | INTERLOCK OK" : "PAUSED | READY";
        }
    }

    public Brush RunStateBrush => isAlarmActive ? AlarmBrush : isRunning ? TextPrimaryBrush : TextMutedBrush;

    public string CurrentStepName => CurrentStep == null ? "-" : StepRowViewModel.DisplayStepName(CurrentStep.Step);

    public string RecipeCycleText
    {
        get
        {
            if (timeline == null || CurrentStep == null)
            {
                return "-";
            }

            var cycle = CurrentStep.Cycle.HasValue
                ? $"{CurrentStep.Cycle}/{timeline.CycleCount}"
                : "-";
            return $"{ShortRecipeName(timeline.RecipeName)} | Step {CurrentStep.Index}/{timeline.Steps.Count} | Cycle {cycle}";
        }
    }

    public string PressureValue => CurrentStep == null ? "-" : $"{CurrentStep.ChamberPressureMtorr:0} mTorr";

    public string TemperatureValue => CurrentStep == null ? "-" : $"{CurrentStep.WaferTemperatureC:0} C";

    public string FilmValue => CurrentStep == null ? "-" : $"{CurrentStep.EstimatedThicknessAngstrom:0.0} A";

    public double PressureProgress => CurrentStep == null ? 0 : ClampPercent(CurrentStep.ChamberPressureMtorr / 1000.0 * 100.0);

    public double TemperatureProgress => CurrentStep == null ? 0 : ClampPercent(CurrentStep.WaferTemperatureC / 300.0 * 100.0);

    public double FilmProgress => CurrentStep == null || timeline == null ? 0 : ClampPercent(CurrentStep.EstimatedThicknessAngstrom / timeline.TargetThicknessAngstrom * 100.0);

    public string PressureStatus => CurrentStep == null ? "-" : RangeStatus(CurrentStep.ChamberPressureMtorr, 800, 900, "LOW", "OK", "HI");

    public string TemperatureStatus => CurrentStep == null ? "-" : RangeStatus(CurrentStep.WaferTemperatureC, 245, 255, "COOL", "OK", "HOT");

    public string FilmStatus => CurrentStep == null ? "-" : CurrentStep.EstimatedThicknessAngstrom >= (timeline?.TargetThicknessAngstrom ?? 0) ? "OK TARGET" : "… GROWING";

    public Brush PressureStatusBrush => StatusBrush(PressureStatus);

    public Brush TemperatureStatusBrush => StatusBrush(TemperatureStatus);

    public Brush FilmStatusBrush => StatusBrush(FilmStatus);

    public string AlarmIcon => isAlarmActive ? "!" : "OK";

    public Brush AlarmIconBrush => isAlarmActive ? WarningBrush : TextMutedBrush;

    public string AlarmTitle => isAlarmActive ? "ALARM ACTIVE" : "NO ACTIVE ALARM";

    public string AlarmDetail => isAlarmActive && CurrentStep != null
        ? $"{SelectedFaultScenario} | {FaultArea(CurrentStep.Step)} replay"
        : "Interlocks nominal | no abnormal condition";

    public string AlarmCode => isAlarmActive && CurrentStep != null
        ? $"PRI 1 | CODE {FaultCode(CurrentStep.Step)}"
        : "PRI 0 | CODE ----";

    public Brush AlarmCodeBrush => isAlarmActive ? WarningBrush : TextMutedBrush;

    public Brush AlarmCardBrush => isAlarmActive ? AlarmSurfaceBrush : SurfaceRaisedBrush;

    public Brush ChamberBrush => isAlarmActive ? ChamberAlarmBrush : ChamberNormalBrush;

    public Brush PrecursorValveBrush => CurrentStep?.Valves.MetalPrecursor == true ? SurfaceActiveBrush : SurfaceRaisedBrush;

    public Brush ReactantValveBrush => CurrentStep?.Valves.Reactant == true ? SurfaceActiveBrush : SurfaceRaisedBrush;

    public Brush PurgeValveBrush => CurrentStep?.Valves.Purge == true ? SurfaceActiveBrush : SurfaceRaisedBrush;

    public string PrecursorValveText => CurrentStep?.Valves.MetalPrecursor == true ? "PRE\nOPEN" : "PRE\nCLOSED";

    public string ReactantValveText => CurrentStep?.Valves.Reactant == true ? "RCT\nOPEN" : "RCT\nCLOSED";

    public string PurgeValveText => CurrentStep?.Valves.Purge == true ? "PRG\nOPEN" : "PRG\nCLOSED";

    public string FlowText
    {
        get
        {
            if (isAlarmActive)
            {
                return "ALARM HOLD: flow inhibited";
            }

            if (CurrentStep?.Valves.MetalPrecursor == true)
            {
                return "OPEN: precursor pulse -> chamber";
            }

            if (CurrentStep?.Valves.Reactant == true)
            {
                return "OPEN: reactant pulse -> chamber";
            }

            if (CurrentStep?.Valves.Purge == true)
            {
                return "OPEN: purge N2 -> exhaust";
            }

            return "CLOSED: idle / all valves closed";
        }
    }

    public Brush FlowBrush
    {
        get
        {
            if (isAlarmActive)
            {
                return AlarmBrush;
            }

            if (CurrentStep?.Valves.MetalPrecursor == true ||
                CurrentStep?.Valves.Reactant == true ||
                CurrentStep?.Valves.Purge == true)
            {
                return TextPrimaryBrush;
            }

            return TextMutedBrush;
        }
    }

    public double TimelineProgress => timeline == null || timeline.Steps.Count == 0
        ? 0
        : ClampPercent((currentStepIndex + 1.0) / timeline.Steps.Count * 100.0);

    public string SelectorStatus => isAlarmActive
        ? "locked during alarm | press RESET before changing scenario"
        : "choose scenario, then press FAULT REPLAY";

    public string TimelineSource => isAlarmActive
        ? $"fault replay JSON: {SelectedFaultScenario}"
        : "normal process timeline";

    private MolyAldTimelineStep? CurrentStep
    {
        get
        {
            if (timeline == null || timeline.Steps.Count == 0)
            {
                return null;
            }

            return timeline.Steps[Math.Clamp(currentStepIndex, 0, timeline.Steps.Count - 1)];
        }
    }

    private void LoadRecipeAndTimeline()
    {
        try
        {
            recipe = recipeService.LoadRecipe();
            FaultScenarios.Clear();
            foreach (var fault in recipe.FaultScenarios)
            {
                FaultScenarios.Add(fault.Name);
            }

            selectedFaultScenario = FaultScenarios.FirstOrDefault(name =>
                string.Equals(name, "precursor-dose-timeout", StringComparison.OrdinalIgnoreCase))
                ?? FaultScenarios.FirstOrDefault()
                ?? string.Empty;
            OnPropertyChanged(nameof(SelectedFaultScenario));

            LoadNormalTimeline();
            AddLog("SYSTEM", "WPF HMI ready");
        }
        catch (Exception ex)
        {
            AddLog("LOAD ERROR", ex.Message);
        }
    }

    private void Start()
    {
        if (recipe == null)
        {
            AddLog("START BLOCKED", "recipe not loaded");
            return;
        }

        LoadNormalTimeline();
        isAlarmActive = false;
        isRunning = true;
        playbackTimer.Start();
        AddLog("START", "normal process running");
        RefreshComputedProperties();
    }

    private void Pause()
    {
        isRunning = false;
        playbackTimer.Stop();
        AddLog("STOP", "timeline held by operator");
        RefreshComputedProperties();
    }

    private void Reset()
    {
        LoadNormalTimeline();
        currentStepIndex = 0;
        isAlarmActive = false;
        isRunning = false;
        playbackTimer.Stop();
        AddLog("RESET", "fault cleared, returned to first step");
        RefreshComputedProperties();
    }

    private void FaultReplay()
    {
        if (recipe == null)
        {
            AddLog("FAULT BLOCKED", "recipe not loaded");
            return;
        }

        try
        {
            var result = runner.Run(recipe, selectedFaultScenario);
            timeline = MolyAldTimelineDocument.FromRunResult(result);
            currentStepIndex = FindFirstFailedStepIndex(timeline);
            isAlarmActive = true;
            isRunning = false;
            playbackTimer.Stop();
            RefreshStepRows();
            AddLog("FAULT REPLAY", $"{selectedFaultScenario} replay {CurrentStepName}");
            RefreshComputedProperties();
        }
        catch (Exception ex)
        {
            AddLog("FAULT ERROR", ex.Message);
        }
    }

    private void StepForward()
    {
        if (isAlarmActive)
        {
            AddLog("STEP BLOCKED", "reset required during active alarm");
            return;
        }

        StepForwardFromTimer();
    }

    private void StepForwardFromTimer()
    {
        if (timeline == null || timeline.Steps.Count == 0)
        {
            return;
        }

        if (currentStepIndex < timeline.Steps.Count - 1)
        {
            currentStepIndex++;
        }
        else
        {
            isRunning = false;
            playbackTimer.Stop();
            AddLog("COMPLETE", "normal process sequence complete");
        }

        RefreshComputedProperties();
    }

    private void OpenUnityViewer()
    {
        var unityProjectPath = ResolveUnityProjectPath();
        if (unityProjectPath == null)
        {
            AddLog("UNITY", "optional viewer folder not found");
            return;
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = unityProjectPath,
            UseShellExecute = true
        });
        AddLog("UNITY", "optional 3D viewer folder opened");
    }

    private void LoadNormalTimeline()
    {
        if (recipe == null)
        {
            return;
        }

        var result = runner.Run(recipe);
        timeline = MolyAldTimelineDocument.FromRunResult(result);
        currentStepIndex = 0;
        RefreshStepRows();
        RefreshComputedProperties();
    }

    private void RefreshStepRows()
    {
        Steps.Clear();
        if (timeline == null)
        {
            return;
        }

        foreach (var step in timeline.Steps)
        {
            Steps.Add(new StepRowViewModel(step));
        }
    }

    private void AddLog(string action, string detail)
    {
        OperatorLog.Insert(0, new OperatorLogEntry(action, detail));
        while (OperatorLog.Count > 8)
        {
            OperatorLog.RemoveAt(OperatorLog.Count - 1);
        }
    }

    private void RefreshComputedProperties()
    {
        OnPropertyChanged(nameof(RunStateText));
        OnPropertyChanged(nameof(RunStateBrush));
        OnPropertyChanged(nameof(CurrentStepName));
        OnPropertyChanged(nameof(RecipeCycleText));
        OnPropertyChanged(nameof(PressureValue));
        OnPropertyChanged(nameof(TemperatureValue));
        OnPropertyChanged(nameof(FilmValue));
        OnPropertyChanged(nameof(PressureProgress));
        OnPropertyChanged(nameof(TemperatureProgress));
        OnPropertyChanged(nameof(FilmProgress));
        OnPropertyChanged(nameof(PressureStatus));
        OnPropertyChanged(nameof(TemperatureStatus));
        OnPropertyChanged(nameof(FilmStatus));
        OnPropertyChanged(nameof(PressureStatusBrush));
        OnPropertyChanged(nameof(TemperatureStatusBrush));
        OnPropertyChanged(nameof(FilmStatusBrush));
        OnPropertyChanged(nameof(AlarmIcon));
        OnPropertyChanged(nameof(AlarmIconBrush));
        OnPropertyChanged(nameof(AlarmTitle));
        OnPropertyChanged(nameof(AlarmDetail));
        OnPropertyChanged(nameof(AlarmCode));
        OnPropertyChanged(nameof(AlarmCodeBrush));
        OnPropertyChanged(nameof(AlarmCardBrush));
        OnPropertyChanged(nameof(ChamberBrush));
        OnPropertyChanged(nameof(PrecursorValveBrush));
        OnPropertyChanged(nameof(ReactantValveBrush));
        OnPropertyChanged(nameof(PurgeValveBrush));
        OnPropertyChanged(nameof(PrecursorValveText));
        OnPropertyChanged(nameof(ReactantValveText));
        OnPropertyChanged(nameof(PurgeValveText));
        OnPropertyChanged(nameof(FlowText));
        OnPropertyChanged(nameof(FlowBrush));
        OnPropertyChanged(nameof(TimelineProgress));
        OnPropertyChanged(nameof(SelectorStatus));
        OnPropertyChanged(nameof(TimelineSource));
    }

    private static int FindFirstFailedStepIndex(MolyAldTimelineDocument document)
    {
        for (var index = 0; index < document.Steps.Count; index++)
        {
            if (!document.Steps[index].Success)
            {
                return index;
            }
        }

        return document.Steps.Count == 0 ? 0 : document.Steps.Count - 1;
    }

    private static string? ResolveUnityProjectPath()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            var candidate = Path.Combine(current.FullName, "unity", "EquipmentTwin.Unity");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            current = current.Parent;
        }

        return null;
    }

    private static string ShortRecipeName(string recipeName)
    {
        return string.IsNullOrWhiteSpace(recipeName)
            ? "public synthetic ALD"
            : recipeName.Replace("public-", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("-demo", string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    private static string RangeStatus(double value, double low, double high, string lowLabel, string normalLabel, string highLabel)
    {
        if (value < low)
        {
            return $"▼ {lowLabel}";
        }

        if (value > high)
        {
            return $"▲ {highLabel}";
        }

        return normalLabel;
    }

    private static Brush StatusBrush(string status)
    {
        if (status.Contains("HI", StringComparison.OrdinalIgnoreCase) ||
            status.Contains("HOT", StringComparison.OrdinalIgnoreCase))
        {
            return AlarmBrush;
        }

        if (status.Contains("LOW", StringComparison.OrdinalIgnoreCase) ||
            status.Contains("COOL", StringComparison.OrdinalIgnoreCase))
        {
            return WarningBrush;
        }

        return status.Contains("OK", StringComparison.OrdinalIgnoreCase)
            ? TextPrimaryBrush
            : TextMutedBrush;
    }

    private static string FaultCode(string stepName)
    {
        if (stepName.Contains("Pump", StringComparison.OrdinalIgnoreCase))
        {
            return "VAC-101";
        }

        if (stepName.Contains("Temperature", StringComparison.OrdinalIgnoreCase) ||
            stepName.Contains("Stabilize", StringComparison.OrdinalIgnoreCase))
        {
            return "TMP-201";
        }

        if (stepName.Contains("Precursor", StringComparison.OrdinalIgnoreCase) ||
            stepName.Contains("Reactant", StringComparison.OrdinalIgnoreCase) ||
            stepName.Contains("Purge", StringComparison.OrdinalIgnoreCase))
        {
            return "GAS-301";
        }

        return "SEQ-001";
    }

    private static string FaultArea(string stepName)
    {
        if (stepName.Contains("Pump", StringComparison.OrdinalIgnoreCase))
        {
            return "Pump / exhaust";
        }

        if (stepName.Contains("Temperature", StringComparison.OrdinalIgnoreCase) ||
            stepName.Contains("Stabilize", StringComparison.OrdinalIgnoreCase))
        {
            return "Thermal chamber";
        }

        if (stepName.Contains("Precursor", StringComparison.OrdinalIgnoreCase) ||
            stepName.Contains("Reactant", StringComparison.OrdinalIgnoreCase) ||
            stepName.Contains("Purge", StringComparison.OrdinalIgnoreCase))
        {
            return "Gas delivery";
        }

        return "Process module";
    }

    private static double ClampPercent(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            return 0;
        }

        return Math.Clamp(value, 0, 100);
    }

    private static SolidColorBrush Brush(string hex)
    {
        var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
        brush.Freeze();
        return brush;
    }
}
