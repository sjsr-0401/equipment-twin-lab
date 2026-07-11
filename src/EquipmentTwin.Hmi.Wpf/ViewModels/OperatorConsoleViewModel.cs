using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using EquipmentTwin.Core;
using EquipmentTwin.Core.Alarms;
using EquipmentTwin.Core.Processes;
using EquipmentTwin.Hmi.Wpf.Models;
using EquipmentTwin.Hmi.Wpf.Services;

namespace EquipmentTwin.Hmi.Wpf.ViewModels;

public sealed class OperatorConsoleViewModel : ObservableObject
{
    private const int MaxTraceEntries = 80;
    private const int MaxServerPayloadPreviewLength = 1800;
    private const double TrendWidth = 170;
    private const double TrendHeight = 30;

    private static readonly Brush BackgroundBrush = Brush("#0B0F14");
    private static readonly Brush SurfaceBrush = Brush("#151C24");
    private static readonly Brush SurfaceRaisedBrush = Brush("#1D2733");
    private static readonly Brush BorderBrush = Brush("#2F3C4C");
    private static readonly Brush TextPrimaryBrush = Brush("#EAF0F7");
    private static readonly Brush TextMutedBrush = Brush("#9AA8B7");
    private static readonly Brush PrimaryBrush = Brush("#2EA8FF");
    private static readonly Brush SuccessBrush = Brush("#2AD17D");
    private static readonly Brush WarningBrush = Brush("#FFB84D");
    private static readonly Brush AlarmBrush = Brush("#FF3B3B");
    private static readonly Brush PrecursorBrush = Brush("#F7A83B");
    private static readonly Brush ReactantBrush = Brush("#38CFFF");
    private static readonly Brush PurgeBrush = Brush("#31D86B");

    private readonly MolyAldRecipeService recipeService = new();
    private readonly AlarmResponseGuideService alarmGuideService = new();
    private readonly AlarmIssueReportExportService issueReportExportService = new();
    private readonly AlarmIssueReportOutboxService issueReportOutboxService = new();
    private readonly MockServerPayloadSender mockServerPayloadSender = new();
    private readonly MolyAldRunner runner = new(new ManualClock(new DateTimeOffset(2026, 7, 1, 0, 0, 0, TimeSpan.Zero)));
    private readonly DispatcherTimer playbackTimer;
    private AlarmResponseGuideCatalog? alarmGuideCatalog;
    private AlarmResponseGuide? activeAlarmGuide;
    private AlarmGuideChoiceRowViewModel? selectedAlarmGuideChoice;
    private MolyAldRecipe? recipe;
    private MolyAldTimelineDocument? timeline;
    private int currentStepIndex;
    private bool isRunning;
    private bool isAlarmActive;
    private bool useKorean;
    private string selectedFaultScenario = string.Empty;
    private string? lastIssueReportPath;
    private string? lastServerOutboxPath;
    private AlarmIssueReportOutboxResult? lastServerOutboxState;
    private string? lastServerPayloadPreview;
    private string? lastMockServerSendStatus;
    private bool lastMockServerSendSucceeded;
    private bool isSendingServerPayload;
    private string? mockServerHealthStatus;
    private bool mockServerHealthOnline;
    private bool isCheckingMockServerHealth;

    public OperatorConsoleViewModel()
    {
        StartCommand = new RelayCommand(Start);
        PauseCommand = new RelayCommand(Pause);
        ResetCommand = new RelayCommand(Reset);
        FaultReplayCommand = new RelayCommand(FaultReplay);
        StepForwardCommand = new RelayCommand(StepForward);
        OpenUnityViewerCommand = new RelayCommand(OpenUnityViewer);
        ExportIssueReportCommand = new RelayCommand(ExportIssueReport);
        QueueIssueReportCommand = new RelayCommand(QueueIssueReport);
        OpenReportFolderCommand = new RelayCommand(OpenReportFolder);
        OpenLatestReportCommand = new RelayCommand(OpenLatestReport);
        OpenServerOutboxFolderCommand = new RelayCommand(OpenServerOutboxFolder);
        OpenLatestServerPayloadCommand = new RelayCommand(OpenLatestServerPayload);
        CheckMockServerHealthCommand = new RelayCommand(CheckMockServerHealth);
        SendLatestServerPayloadCommand = new RelayCommand(SendLatestServerPayload);
        ToggleLanguageCommand = new RelayCommand(ToggleLanguage);

        playbackTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(750)
        };
        playbackTimer.Tick += (_, _) => StepForwardFromTimer();

        Trace("VM", "OperatorConsoleViewModel.ctor()");
        LoadRecipeAndTimeline();
    }

    public ObservableCollection<string> FaultScenarios { get; } = new();

    public ObservableCollection<StepRowViewModel> Steps { get; } = new();

    public ObservableCollection<InstrumentTrendRowViewModel> InstrumentTrends { get; } = new();

    public ObservableCollection<OperatorLogEntry> OperatorLog { get; } = new();

    public ObservableCollection<EngineeringTraceEntry> EngineeringTrace { get; } = new();

    public ObservableCollection<AlarmGuideCheckRowViewModel> AlarmGuideChecks { get; } = new();

    public ObservableCollection<AlarmGuideChoiceRowViewModel> AlarmGuideChoices { get; } = new();

    public ObservableCollection<string> AlarmGuideEscalationConditions { get; } = new();

    public ICommand StartCommand { get; }

    public ICommand PauseCommand { get; }

    public ICommand ResetCommand { get; }

    public ICommand FaultReplayCommand { get; }

    public ICommand StepForwardCommand { get; }

    public ICommand OpenUnityViewerCommand { get; }

    public ICommand ExportIssueReportCommand { get; }

    public ICommand QueueIssueReportCommand { get; }

    public ICommand OpenReportFolderCommand { get; }

    public ICommand OpenLatestReportCommand { get; }

    public ICommand OpenServerOutboxFolderCommand { get; }

    public ICommand OpenLatestServerPayloadCommand { get; }

    public ICommand CheckMockServerHealthCommand { get; }

    public ICommand SendLatestServerPayloadCommand { get; }

    public ICommand ToggleLanguageCommand { get; }

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

    public string Subtitle => L(
        "Main operator console | Core-driven synthetic public-reference process",
        "메인 오퍼레이터 콘솔 | Core 기반 공개 자료 참조 합성 공정");

    public string LanguageToggleText => useKorean ? "EN" : "한국어";

    public string OperationsTabHeader => L("OPERATE", "공정 운전");

    public string AlarmReportTabHeader => L("ALARM / REPORT", "알람/리포트");

    public string OperatorLogTabHeader => L("LOG", "작업 로그");

    public string SchematicTitle => L(
        "Synthetic Moly ALD — Process Schematic",
        "Synthetic Moly ALD — 공정 구성도");

    public string SchematicSubtitle => L(
        "public-reference HMI | not vendor CAD or process copy",
        "공개 자료 기반 HMI | 제조사 CAD/공정 복제 아님");

    public string EquipmentModuleLayoutLabel => L("EQUIPMENT MODULE LAYOUT", "장비 모듈 구성");

    public string ProcessTimelineLabel => L("PROCESS TIMELINE", "공정 진행도");

    public string StartButtonText => L("START", "시작");

    public string StopButtonText => L("STOP", "정지");

    public string StepButtonText => L("STEP", "다음 STEP");

    public string FaultReplayButtonText => L("FAULT REPLAY", "FAULT 재현");

    public string ResetButtonText => L("RESET", "초기화");

    public string FaultScenarioSelectorLabel => L("FAULT SCENARIO SELECTOR", "FAULT 시나리오 선택");

    public string CurrentStepLabel => L("CURRENT STEP", "현재 STEP");

    public string RecipeCycleLabel => "RECIPE / CYCLE";

    public string ProcessInstrumentsLabel => L("PROCESS INSTRUMENTS", "공정 계측값");

    public string InstrumentTrendLabel => L("RECENT TREND", "최근 Trend");

    public string PressureLabel => "Pressure";

    public string TempLabel => "Temp";

    public string FilmLabel => "Film";

    public string AlarmPriorityLabel => L("ALARM PRIORITY", "알람 우선순위");

    public string AlarmResponseGuideLabel => L("ALARM RESPONSE GUIDE", "알람 대응 가이드");

    public string ResponseChoicesLabel => L("RESPONSE CHOICES", "대응 선택지");

    public string ExportIssueReportButtonText => L("EXPORT ISSUE REPORT", "이슈 리포트 저장");

    public string QueueReportButtonText => L("QUEUE REPORT TO SERVER", "서버 전송 대기열 저장");

    public string LatestIssueReportLabel => L("LATEST ISSUE REPORT", "최신 이슈 리포트");

    public string LatestIssueReportPathText => string.IsNullOrWhiteSpace(lastIssueReportPath)
        ? L("No exported issue report yet", "아직 저장된 이슈 리포트 없음")
        : lastIssueReportPath;

    public string OpenReportFolderButtonText => L("OPEN REPORT FOLDER", "리포트 폴더 열기");

    public string OpenLatestReportButtonText => L("OPEN LATEST REPORT", "최신 리포트 열기");

    public string LatestServerPayloadLabel => L("LATEST SERVER OUTBOX PAYLOAD", "최신 서버 대기열 payload");

    public string LatestServerPayloadPathText => string.IsNullOrWhiteSpace(lastServerOutboxPath)
        ? L("No queued server payload yet", "아직 서버 대기열 payload 없음")
        : lastServerOutboxPath;

    public string ServerPayloadPreviewLabel => L("PAYLOAD PREVIEW", "payload 미리보기");

    public string LatestServerPayloadPreviewText => string.IsNullOrWhiteSpace(lastServerPayloadPreview)
        ? L(
            "Queue a server payload to preview the JSON envelope here.",
            "서버 payload를 저장하면 여기에 JSON envelope 미리보기가 표시됩니다.")
        : lastServerPayloadPreview;

    public string OpenServerOutboxFolderButtonText => L("OPEN OUTBOX FOLDER", "대기열 폴더 열기");

    public string OpenLatestServerPayloadButtonText => L("OPEN LATEST PAYLOAD", "최신 payload 열기");

    public string OutboxDeliveryStateLabel => L("OUTBOX DELIVERY STATE", "전송 대기열 상태");

    public string LatestServerOutboxStateText => lastServerOutboxState == null
        ? L("Status: no payload", "상태: payload 없음")
        : L(
            $"Status: {lastServerOutboxState.Status.ToUpperInvariant()}",
            $"상태: {LocalizeOutboxStatus(lastServerOutboxState.Status)}");

    public string LatestServerOutboxAttemptText
    {
        get
        {
            if (lastServerOutboxState == null)
            {
                return L("Attempts: 0 | Last attempt: -", "시도 횟수: 0회 | 마지막 시도: -");
            }

            var lastAttempt = lastServerOutboxState.LastAttemptAt?.ToLocalTime().ToString("HH:mm:ss") ?? "-";
            return L(
                $"Attempts: {lastServerOutboxState.AttemptCount} | Last attempt: {lastAttempt}",
                $"시도 횟수: {lastServerOutboxState.AttemptCount}회 | 마지막 시도: {lastAttempt}");
        }
    }

    public string LatestServerOutboxErrorText => string.IsNullOrWhiteSpace(lastServerOutboxState?.LastError)
        ? L("Last error: -", "마지막 오류: -")
        : L(
            $"Last error: {lastServerOutboxState.LastError}",
            $"마지막 오류: {lastServerOutboxState.LastError}");

    public Brush LatestServerOutboxStateBrush => lastServerOutboxState?.Status switch
    {
        AlarmIssueReportOutboxStatuses.Sent => SuccessBrush,
        AlarmIssueReportOutboxStatuses.Failed => AlarmBrush,
        AlarmIssueReportOutboxStatuses.Sending => WarningBrush,
        AlarmIssueReportOutboxStatuses.Queued => PrimaryBrush,
        _ => TextMutedBrush
    };

    public bool CanSendLatestServerPayload =>
        !isSendingServerPayload &&
        !string.IsNullOrWhiteSpace(lastServerOutboxPath) &&
        File.Exists(lastServerOutboxPath) &&
        lastServerOutboxState?.Status is not AlarmIssueReportOutboxStatuses.Sent and
        not AlarmIssueReportOutboxStatuses.Sending;

    public string SendLatestServerPayloadButtonText
    {
        get
        {
            if (isSendingServerPayload)
            {
                return L("SENDING...", "전송 중...");
            }

            return lastServerOutboxState?.Status switch
            {
                AlarmIssueReportOutboxStatuses.Failed => L("RETRY SEND", "재전송"),
                AlarmIssueReportOutboxStatuses.Sent => L("ALREADY SENT", "전송 완료"),
                _ => L("SEND TO MOCK SERVER", "Mock Server로 전송")
            };
        }
    }

    public string LatestMockServerSendStatus => string.IsNullOrWhiteSpace(lastMockServerSendStatus)
        ? L(
            "Latest mock-server send: not sent yet",
            "최근 mock server 전송: 아직 없음")
        : lastMockServerSendStatus;

    public string MockServerConnectionLabel => L("MOCK SERVER CONNECTION", "Mock Server 연결 상태");

    public string CheckMockServerHealthButtonText => isCheckingMockServerHealth
        ? L("CHECKING...", "확인 중...")
        : L("CHECK SERVER", "서버 확인");

    public string MockServerHealthBadgeText
    {
        get
        {
            if (isCheckingMockServerHealth)
            {
                return L("CHECKING", "확인 중");
            }

            if (mockServerHealthOnline)
            {
                return L("ONLINE", "연결 가능");
            }

            return string.IsNullOrWhiteSpace(mockServerHealthStatus)
                ? L("NOT CHECKED", "미확인")
                : L("OFFLINE", "연결 실패");
        }
    }

    public string MockServerHealthStatusText => string.IsNullOrWhiteSpace(mockServerHealthStatus)
        ? L(
            "Press check before sending, or start EquipmentTwin.MockServer and send a payload.",
            "전송 전 서버 확인을 누르거나 EquipmentTwin.MockServer를 실행한 뒤 payload를 전송하세요.")
        : mockServerHealthStatus;

    public Brush MockServerHealthBrush
    {
        get
        {
            if (isCheckingMockServerHealth)
            {
                return WarningBrush;
            }

            if (mockServerHealthOnline)
            {
                return SuccessBrush;
            }

            return string.IsNullOrWhiteSpace(mockServerHealthStatus) ? TextMutedBrush : AlarmBrush;
        }
    }

    public string EscalationConditionsLabel => L("ESCALATION CONDITIONS", "엔지니어 검토 요청 조건");

    public string OperatorActionLogLabel => L("OPERATOR ACTION LOG", "오퍼레이터 작업 로그");

    public string OpenUnityViewerButtonText => L("Open optional Unity viewer folder", "Unity viewer 폴더 열기");

    public string TimelineDebugTableLabel => L(
        "TIMELINE DEBUG TABLE — Core-generated process steps",
        "공정 Timeline 디버그 표 — Core 생성 Step");

    public string EngineeringTraceConsoleLabel => L(
        "ENGINEERING TRACE CONSOLE",
        "엔지니어링 Trace 콘솔");

    public string RunStateText
    {
        get
        {
            if (isAlarmActive)
            {
                return L("HELD | OPERATOR ACTION REQUIRED", "정지 유지 | 오퍼레이터 조치 필요");
            }

            return isRunning
                ? L("RUNNING | INTERLOCK OK", "운전 중 | Interlock 정상")
                : L("PAUSED | READY", "일시정지 | 준비");
        }
    }

    public Brush RunStateBrush => isAlarmActive ? AlarmBrush : isRunning ? SuccessBrush : WarningBrush;

    public string CurrentStepName => CurrentStep == null
        ? "-"
        : LocalizeStepName(StepRowViewModel.DisplayStepName(CurrentStep.Step));

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

    public string PressureStatus => CurrentStep == null
        ? "-"
        : RangeStatus(
            CurrentStep.ChamberPressureMtorr,
            800,
            900,
            L("LOW", "낮음"),
            L("OK", "정상"),
            L("HI", "높음"));

    public string TemperatureStatus => CurrentStep == null
        ? "-"
        : RangeStatus(
            CurrentStep.WaferTemperatureC,
            245,
            255,
            L("COOL", "낮음"),
            L("OK", "정상"),
            L("HOT", "높음"));

    public string FilmStatus => CurrentStep == null
        ? "-"
        : CurrentStep.EstimatedThicknessAngstrom >= (timeline?.TargetThicknessAngstrom ?? 0)
            ? L("TARGET", "목표")
            : L("GROWING", "성장 중");

    public Brush PressureStatusBrush => StatusBrush(PressureStatus);

    public Brush TemperatureStatusBrush => StatusBrush(TemperatureStatus);

    public Brush FilmStatusBrush => StatusBrush(FilmStatus);

    public string AlarmIcon => isAlarmActive ? "!" : "OK";

    public Brush AlarmIconBrush => isAlarmActive ? WarningBrush : SuccessBrush;

    public string AlarmTitle => isAlarmActive
        ? L("ALARM ACTIVE", "알람 발생")
        : L("NO ALARM", "알람 없음");

    public string AlarmDetail => isAlarmActive && CurrentStep != null
        ? L(
            $"{SelectedFaultScenario} | {FaultArea(CurrentStep.Step)} replay",
            $"{SelectedFaultScenario} | {FaultArea(CurrentStep.Step)} 재현")
        : L("Interlocks nominal", "Interlock 정상");

    public string AlarmCode => isAlarmActive && CurrentStep != null
        ? $"PRI 1 | CODE {ActiveAlarmCode}"
        : "PRI 0 | CODE ----";

    public Brush AlarmCodeBrush => WarningBrush;

    public Brush AlarmCardBrush => isAlarmActive ? AlarmBrush : Brush("#173D34");

    public string SchematicDiagnosticText
    {
        get
        {
            if (!isAlarmActive)
            {
                return L("SCHEMATIC FOCUS · NORMAL", "구성도 진단 · 정상");
            }

            var alarmCode = ActiveAlarmCode;
            return alarmCode.ToUpperInvariant() switch
            {
                "GAS-301" => L(
                    "FAULT FOCUS · GAS-301 · GAS BOX / DELIVERY",
                    "FAULT 진단 · GAS-301 · GAS BOX / 공급 라인"),
                "VAC-101" => L(
                    "FAULT FOCUS · VAC-101 · EXHAUST / PUMP",
                    "FAULT 진단 · VAC-101 · EXHAUST / PUMP"),
                "TMP-201" => L(
                    "FAULT FOCUS · TMP-201 · CHAMBER HEATER",
                    "FAULT 진단 · TMP-201 · CHAMBER HEATER"),
                _ => L(
                    $"FAULT FOCUS · {alarmCode} · PROCESS SEQUENCE",
                    $"FAULT 진단 · {alarmCode} · PROCESS SEQUENCE")
            };
        }
    }

    public Brush SchematicDiagnosticBrush => isAlarmActive ? AlarmBrush : SuccessBrush;

    public Brush GasModuleBorderBrush => IsActiveAlarmCode("GAS-301") ? AlarmBrush : BorderBrush;

    public Brush GasDeliveryLineBrush => IsActiveAlarmCode("GAS-301")
        ? AlarmBrush
        : isAlarmActive ? TextMutedBrush : FlowBrush;

    public Brush ChamberBorderBrush => IsActiveAlarmCode("TMP-201")
        ? AlarmBrush
        : isAlarmActive ? BorderBrush : FlowBrush;

    public Brush HeaterDiagnosticBrush => IsActiveAlarmCode("TMP-201") ? AlarmBrush : WarningBrush;

    public Brush ExhaustModuleBorderBrush => IsActiveAlarmCode("VAC-101") ? AlarmBrush : BorderBrush;

    public Brush VacuumPathBrush => IsActiveAlarmCode("VAC-101")
        ? AlarmBrush
        : isAlarmActive ? TextMutedBrush : FlowBrush;

    public string WaferTransferStatusText
    {
        get
        {
            if (isAlarmActive && IsWaferInChamber)
            {
                return L(
                    "WAFER · HELD IN CHAMBER BY ALARM",
                    "WAFER · 알람으로 CHAMBER 내부 Hold");
            }

            return CurrentProcessStep switch
            {
                MolyAldProcessStep.LoadWafer => L(
                    "WAFER · TRANSFER IN TO CHAMBER",
                    "WAFER · CHAMBER로 이송 중"),
                MolyAldProcessStep.TransferOut => L(
                    "WAFER · TRANSFER OUT TO LOAD PORT",
                    "WAFER · LOAD PORT로 복귀 중"),
                MolyAldProcessStep.Complete => L(
                    "WAFER · RETURNED TO LOAD PORT",
                    "WAFER · LOAD PORT 복귀 완료"),
                _ when IsWaferInChamber => L(
                    "WAFER · IN PROCESS CHAMBER",
                    "WAFER · PROCESS CHAMBER 내부"),
                _ => L(
                    "WAFER · POSITION NOT ACTIVE",
                    "WAFER · 활성 위치 없음")
            };
        }
    }

    public string TransferGateText => IsTransferGateOpen
        ? L("GATE OPEN", "GATE 열림")
        : L("GATE CLOSED", "GATE 닫힘");

    public Brush TransferGateBrush => IsTransferGateOpen ? PrimaryBrush : TextMutedBrush;

    public double LoadPortWaferOpacity => CurrentProcessStep is MolyAldProcessStep.Idle or MolyAldProcessStep.Complete
        ? 1.0
        : 0.12;

    public double TransferInWaferOpacity => CurrentProcessStep == MolyAldProcessStep.LoadWafer ? 1.0 : 0.0;

    public double ChamberWaferOpacity => IsWaferInChamber ? 1.0 : 0.12;

    public double TransferOutWaferOpacity => CurrentProcessStep == MolyAldProcessStep.TransferOut ? 1.0 : 0.0;

    public bool IsGasFlowActive =>
        !isAlarmActive &&
        (CurrentStep?.Valves.MetalPrecursor == true ||
         CurrentStep?.Valves.Reactant == true ||
         CurrentStep?.Valves.Purge == true);

    public bool IsVacuumFlowActive =>
        !isAlarmActive &&
        CurrentProcessStep is
            MolyAldProcessStep.PumpDown or
            MolyAldProcessStep.StabilizeTemperature or
            MolyAldProcessStep.DoseMetalPrecursor or
            MolyAldProcessStep.PurgeAfterPrecursor or
            MolyAldProcessStep.DoseReactant or
            MolyAldProcessStep.PurgeAfterReactant or
            MolyAldProcessStep.PostPurge;

    public bool IsHeaterActive =>
        !isAlarmActive &&
        CurrentProcessStep is
            MolyAldProcessStep.StabilizeTemperature or
            MolyAldProcessStep.DoseMetalPrecursor or
            MolyAldProcessStep.PurgeAfterPrecursor or
            MolyAldProcessStep.DoseReactant or
            MolyAldProcessStep.PurgeAfterReactant or
            MolyAldProcessStep.PostPurge;

    public bool IsWaferTransferInActive =>
        !isAlarmActive && CurrentProcessStep == MolyAldProcessStep.LoadWafer;

    public bool IsWaferTransferOutActive =>
        !isAlarmActive && CurrentProcessStep == MolyAldProcessStep.TransferOut;

    public bool IsTransferGateOpenVisual => IsTransferGateOpen;

    public bool IsPrecursorValveOpen => !isAlarmActive && CurrentStep?.Valves.MetalPrecursor == true;

    public bool IsReactantValveOpen => !isAlarmActive && CurrentStep?.Valves.Reactant == true;

    public bool IsPurgeValveOpen => !isAlarmActive && CurrentStep?.Valves.Purge == true;

    public Brush PrecursorValveBrush => CurrentStep?.Valves.MetalPrecursor == true ? PrecursorBrush : SurfaceRaisedBrush;

    public Brush ReactantValveBrush => CurrentStep?.Valves.Reactant == true ? ReactantBrush : SurfaceRaisedBrush;

    public Brush PurgeValveBrush => CurrentStep?.Valves.Purge == true ? PurgeBrush : SurfaceRaisedBrush;

    public string PrecursorValveText => CurrentStep?.Valves.MetalPrecursor == true ? "PRE\nON" : "PRE\nOFF";

    public string ReactantValveText => CurrentStep?.Valves.Reactant == true ? "RCT\nON" : "RCT\nOFF";

    public string PurgeValveText => CurrentStep?.Valves.Purge == true ? "PRG\nON" : "PRG\nOFF";

    public string FlowText
    {
        get
        {
            if (isAlarmActive)
            {
                return L("FLOW: held by alarm", "FLOW: 알람으로 Hold");
            }

            if (CurrentStep?.Valves.MetalPrecursor == true)
            {
                return L("FLOW: precursor pulse -> chamber", "FLOW: Precursor 주입 -> Chamber");
            }

            if (CurrentStep?.Valves.Reactant == true)
            {
                return L("FLOW: reactant pulse -> chamber", "FLOW: Reactant 주입 -> Chamber");
            }

            if (CurrentStep?.Valves.Purge == true)
            {
                return L("FLOW: purge N2 -> exhaust", "FLOW: Purge N2 -> Exhaust");
            }

            return L("FLOW: idle / closed", "FLOW: 대기 / 닫힘");
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

            if (CurrentStep?.Valves.MetalPrecursor == true)
            {
                return PrecursorBrush;
            }

            if (CurrentStep?.Valves.Reactant == true)
            {
                return ReactantBrush;
            }

            if (CurrentStep?.Valves.Purge == true)
            {
                return PurgeBrush;
            }

            return TextMutedBrush;
        }
    }

    public double TimelineProgress => timeline == null || timeline.Steps.Count == 0
        ? 0
        : ClampPercent((currentStepIndex + 1.0) / timeline.Steps.Count * 100.0);

    public string SelectorStatus => isAlarmActive
        ? L(
            "locked during alarm | press RESET before changing scenario",
            "알람 중에는 잠김 | RESET 후 시나리오 변경 가능")
        : L(
            "choose scenario, then press FAULT REPLAY",
            "시나리오 선택 후 FAULT 재현 실행");

    public string TimelineSource => isAlarmActive
        ? L($"fault replay JSON: {SelectedFaultScenario}", $"fault replay JSON: {SelectedFaultScenario}")
        : L("normal process timeline", "정상 공정 Timeline");

    public string EngineeringTraceStatus => L(
        $"{EngineeringTrace.Count}/{MaxTraceEntries} trace entries | latest first",
        $"{EngineeringTrace.Count}/{MaxTraceEntries} trace | 최신순");

    public string AlarmGuideTitle => activeAlarmGuide == null
        ? L("TROUBLESHOOTING GUIDE", "트러블슈팅 가이드")
        : $"{activeAlarmGuide.AlarmCode} | {LocalizeGuideText(activeAlarmGuide.Title)}";

    public string AlarmGuideSeverity => LocalizeAlarmGuideSeverity(activeAlarmGuide?.Severity);

    public Brush AlarmGuideSeverityBrush => activeAlarmGuide?.Severity switch
    {
        EquipmentTwin.Core.Alarms.AlarmGuideSeverity.Critical => AlarmBrush,
        EquipmentTwin.Core.Alarms.AlarmGuideSeverity.Warning => WarningBrush,
        EquipmentTwin.Core.Alarms.AlarmGuideSeverity.Info => PrimaryBrush,
        _ => TextMutedBrush
    };

    public string AlarmGuideSummary => activeAlarmGuide == null
        ? L(
            "Run FAULT REPLAY to load the operator troubleshooting guide for the active alarm.",
            "FAULT 재현을 실행하면 현재 알람에 맞는 오퍼레이터 대응 가이드가 로드됩니다.")
        : LocalizeGuideText(activeAlarmGuide.Summary);

    public string AlarmGuideStatus => activeAlarmGuide == null
        ? L("No active alarm guide", "활성 알람 가이드 없음")
        : L(
            $"{AlarmGuideChecks.Count(check => check.IsChecked)}/{AlarmGuideChecks.Count} checks complete | {AlarmGuideChoices.Count} response choices",
            $"점검 {AlarmGuideChecks.Count(check => check.IsChecked)}/{AlarmGuideChecks.Count} 완료 | 대응 선택지 {AlarmGuideChoices.Count}개");

    public string SelectedAlarmGuideChoiceText => selectedAlarmGuideChoice == null
        ? L("Selected response: none", "선택한 대응: 없음")
        : L(
            $"Selected response: {selectedAlarmGuideChoice.Id} | {selectedAlarmGuideChoice.Label}",
            $"선택한 대응: {selectedAlarmGuideChoice.Id} | {selectedAlarmGuideChoice.Label}");

    public string IssueReportStatus => activeAlarmGuide == null
        ? L(
            "Run FAULT REPLAY before exporting an issue report",
            "이슈 리포트 저장 전 FAULT 재현을 먼저 실행하세요")
        : L(
            "Exports alarm, checklist, selected response, step snapshot, and engineering trace",
            "알람, 점검 결과, 선택한 대응, 현재 공정 정보, Engineering Trace를 저장합니다");

    public string ServerOutboxStatus => activeAlarmGuide == null
        ? L(
            "Run FAULT REPLAY before queueing a server payload",
            "서버 payload 저장 전 FAULT 재현을 먼저 실행하세요")
        : L(
            "Queues the same issue report payload to the local server outbox",
            "동일한 이슈 리포트 Payload를 로컬 서버 전송 대기열에 저장합니다");

    public string WorkflowStepperLabel => L("ALARM WORKFLOW", "알람 처리 흐름");

    public string WorkflowAlarmStepTitle => L("1 ALARM", "1 알람");

    public string WorkflowChecklistStepTitle => L("2 CHECK", "2 점검");

    public string WorkflowResponseStepTitle => L("3 RESPONSE", "3 대응");

    public string WorkflowReportStepTitle => L("4 REPORT", "4 리포트");

    public string WorkflowQueueStepTitle => L("5 QUEUE", "5 대기열");

    public string WorkflowSendStepTitle => L("6 SEND", "6 전송");

    public string WorkflowAlarmStepStatus => activeAlarmGuide == null
        ? L("WAIT", "대기")
        : L("LOADED", "로드됨");

    public string WorkflowAlarmStepDetail => activeAlarmGuide == null
        ? L("Run FAULT REPLAY", "FAULT 재현 실행")
        : ActiveAlarmCode;

    public Brush WorkflowAlarmStepBrush => activeAlarmGuide == null ? TextMutedBrush : AlarmBrush;

    public string WorkflowChecklistStepStatus
    {
        get
        {
            if (activeAlarmGuide == null)
            {
                return L("WAIT", "대기");
            }

            return IsWorkflowChecklistComplete
                ? L("DONE", "완료")
                : L("ACTIVE", "진행");
        }
    }

    public string WorkflowChecklistStepDetail => activeAlarmGuide == null
        ? L("No checklist", "체크리스트 없음")
        : $"{AlarmGuideChecks.Count(check => check.IsChecked)}/{AlarmGuideChecks.Count}";

    public Brush WorkflowChecklistStepBrush => WorkflowStepBrush(IsWorkflowChecklistComplete, activeAlarmGuide != null);

    public string WorkflowResponseStepStatus => selectedAlarmGuideChoice == null
        ? L("WAIT", "대기")
        : L("SELECTED", "선택됨");

    public string WorkflowResponseStepDetail => selectedAlarmGuideChoice == null
        ? L("Choose response", "대응 선택")
        : selectedAlarmGuideChoice.Id;

    public Brush WorkflowResponseStepBrush => WorkflowStepBrush(selectedAlarmGuideChoice != null, IsWorkflowChecklistComplete);

    public string WorkflowReportStepStatus => string.IsNullOrWhiteSpace(lastIssueReportPath)
        ? L("WAIT", "대기")
        : L("SAVED", "저장됨");

    public string WorkflowReportStepDetail => string.IsNullOrWhiteSpace(lastIssueReportPath)
        ? L("Export issue report", "이슈 리포트 저장")
        : Path.GetFileName(lastIssueReportPath);

    public Brush WorkflowReportStepBrush => WorkflowStepBrush(!string.IsNullOrWhiteSpace(lastIssueReportPath), selectedAlarmGuideChoice != null);

    public string WorkflowQueueStepStatus => string.IsNullOrWhiteSpace(lastServerOutboxPath)
        ? L("WAIT", "대기")
        : L("QUEUED", "저장됨");

    public string WorkflowQueueStepDetail => string.IsNullOrWhiteSpace(lastServerOutboxPath)
        ? L("Queue server payload", "서버 대기열 저장")
        : Path.GetFileName(lastServerOutboxPath);

    public Brush WorkflowQueueStepBrush => WorkflowStepBrush(!string.IsNullOrWhiteSpace(lastServerOutboxPath), !string.IsNullOrWhiteSpace(lastIssueReportPath));

    public string WorkflowSendStepStatus
    {
        get
        {
            if (lastServerOutboxState?.Status == AlarmIssueReportOutboxStatuses.Sent)
            {
                return L("HTTP OK", "전송 성공");
            }

            if (lastServerOutboxState?.Status == AlarmIssueReportOutboxStatuses.Sending || isSendingServerPayload)
            {
                return L("SENDING", "전송 중");
            }

            if (lastServerOutboxState?.Status == AlarmIssueReportOutboxStatuses.Failed)
            {
                return L("FAILED", "전송 실패");
            }

            if (isCheckingMockServerHealth)
            {
                return L("CHECKING", "확인 중");
            }

            if (HasKnownMockServerHealth && !mockServerHealthOnline)
            {
                return L("OFFLINE", "연결 실패");
            }

            if (mockServerHealthOnline)
            {
                return L("ONLINE", "연결 가능");
            }

            if (!string.IsNullOrWhiteSpace(lastMockServerSendStatus))
            {
                return L("CHECK", "확인 필요");
            }

            return L("WAIT", "대기");
        }
    }

    public string WorkflowSendStepDetail
    {
        get
        {
            if (lastServerOutboxState?.Status == AlarmIssueReportOutboxStatuses.Sent)
            {
                return L("Mock server received", "Mock Server 수신");
            }

            if (lastServerOutboxState?.Status == AlarmIssueReportOutboxStatuses.Sending || isSendingServerPayload)
            {
                return L("Sending queued payload", "대기열 payload 전송 중");
            }

            if (lastServerOutboxState?.Status == AlarmIssueReportOutboxStatuses.Failed)
            {
                return L(
                    $"Retry available; attempt {lastServerOutboxState.AttemptCount}",
                    $"재전송 가능; {lastServerOutboxState.AttemptCount}회 시도");
            }

            if (lastServerOutboxState?.Status == AlarmIssueReportOutboxStatuses.Queued)
            {
                return L("Queued payload ready", "대기열 payload 전송 준비");
            }

            if (isCheckingMockServerHealth)
            {
                return L("Checking /health", "/health 확인 중");
            }

            if (HasKnownMockServerHealth && !mockServerHealthOnline)
            {
                return L("Start Mock Server", "Mock Server 실행 필요");
            }

            if (mockServerHealthOnline)
            {
                return string.IsNullOrWhiteSpace(lastServerOutboxPath)
                    ? L("Server ready; queue payload", "서버 준비됨; payload 저장 필요")
                    : L("Server ready to receive", "서버 전송 준비 완료");
            }

            return string.IsNullOrWhiteSpace(lastServerOutboxPath)
                ? L("Queue payload first", "payload 먼저 저장")
                : L("Check server, then send", "서버 확인 후 전송");
        }
    }

    public Brush WorkflowSendStepBrush
    {
        get
        {
            if (lastServerOutboxState?.Status == AlarmIssueReportOutboxStatuses.Sent)
            {
                return SuccessBrush;
            }

            if (lastServerOutboxState?.Status == AlarmIssueReportOutboxStatuses.Failed)
            {
                return AlarmBrush;
            }

            if (lastServerOutboxState?.Status == AlarmIssueReportOutboxStatuses.Sending ||
                isSendingServerPayload ||
                isCheckingMockServerHealth)
            {
                return WarningBrush;
            }

            if (lastServerOutboxState?.Status == AlarmIssueReportOutboxStatuses.Queued)
            {
                return PrimaryBrush;
            }

            if (HasKnownMockServerHealth && !mockServerHealthOnline)
            {
                return AlarmBrush;
            }

            if (mockServerHealthOnline)
            {
                return PrimaryBrush;
            }

            return string.IsNullOrWhiteSpace(lastMockServerSendStatus)
                ? TextMutedBrush
                : WarningBrush;
        }
    }

    public Brush AlarmGuideCardBrush => activeAlarmGuide == null ? SurfaceBrush : SurfaceRaisedBrush;

    private bool IsWorkflowChecklistComplete =>
        activeAlarmGuide != null &&
        AlarmGuideChecks.Count > 0 &&
        AlarmGuideChecks.All(check => !check.Required || check.IsChecked);

    private bool HasKnownMockServerHealth => !string.IsNullOrWhiteSpace(mockServerHealthStatus);

    private string ActiveAlarmCode => activeAlarmGuide?.AlarmCode
        ?? (CurrentStep == null ? "----" : FaultCode(CurrentStep.Step));

    private bool IsActiveAlarmCode(string alarmCode) =>
        isAlarmActive && string.Equals(ActiveAlarmCode, alarmCode, StringComparison.OrdinalIgnoreCase);

    private MolyAldProcessStep CurrentProcessStep =>
        Enum.TryParse<MolyAldProcessStep>(CurrentStep?.Step, ignoreCase: true, out var step)
            ? step
            : MolyAldProcessStep.Idle;

    private bool IsTransferGateOpen =>
        !isAlarmActive &&
        (CurrentProcessStep is MolyAldProcessStep.LoadWafer or MolyAldProcessStep.TransferOut);

    private bool IsWaferInChamber => CurrentProcessStep is
        MolyAldProcessStep.PumpDown or
        MolyAldProcessStep.StabilizeTemperature or
        MolyAldProcessStep.DoseMetalPrecursor or
        MolyAldProcessStep.PurgeAfterPrecursor or
        MolyAldProcessStep.DoseReactant or
        MolyAldProcessStep.PurgeAfterReactant or
        MolyAldProcessStep.PostPurge;

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
            Trace("LOAD", "LoadRecipeAndTimeline()");
            recipe = recipeService.LoadRecipe();
            Trace("LOAD", $"MolyAldRecipeService.LoadRecipe() recipe={recipe.Name} faults={recipe.FaultScenarios.Count}");
            alarmGuideCatalog = alarmGuideService.LoadCatalog();
            Trace("LOAD", $"AlarmResponseGuideService.LoadCatalog() guides={alarmGuideCatalog.Guides.Count}");
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
            AddLog("GUIDE", $"{alarmGuideCatalog.Guides.Count} alarm response guides loaded");
        }
        catch (Exception ex)
        {
            AddLog("LOAD ERROR", ex.Message);
        }
    }

    private void Start()
    {
        Trace("CALL", "Start()");
        if (recipe == null)
        {
            AddLog("START BLOCKED", "recipe not loaded");
            Trace("BLOCK", "Start() blocked: recipe not loaded");
            return;
        }

        LoadNormalTimeline();
        isAlarmActive = false;
        isRunning = true;
        playbackTimer.Start();
        AddLog("START", "normal process running");
        Trace("STATE", "normal playback started");
        RefreshComputedProperties();
    }

    private void Pause()
    {
        Trace("CALL", "Pause()");
        isRunning = false;
        playbackTimer.Stop();
        AddLog("STOP", "timeline held by operator");
        Trace("STATE", $"playback paused at index={currentStepIndex}");
        RefreshComputedProperties();
    }

    private void Reset()
    {
        Trace("CALL", "Reset()");
        LoadNormalTimeline();
        currentStepIndex = 0;
        isAlarmActive = false;
        isRunning = false;
        playbackTimer.Stop();
        AddLog("RESET", "fault cleared, returned to first step");
        Trace("STATE", "alarm guide and playback state reset");
        RefreshComputedProperties();
    }

    private void FaultReplay()
    {
        Trace("CALL", $"FaultReplay() scenario={selectedFaultScenario}");
        if (recipe == null)
        {
            AddLog("FAULT BLOCKED", "recipe not loaded");
            Trace("BLOCK", "FaultReplay() blocked: recipe not loaded");
            return;
        }

        try
        {
            ClearReportWorkflowState();
            RefreshIssueReportProperties();
            RefreshServerOutboxProperties();
            var result = runner.Run(recipe, selectedFaultScenario);
            Trace("CORE", $"MolyAldRunner.Run(recipe, fault={selectedFaultScenario}) success={result.Success} final={result.FinalStep}");
            timeline = MolyAldTimelineDocument.FromRunResult(result);
            Trace("CORE", $"MolyAldTimelineDocument.FromRunResult() steps={timeline.Steps.Count}");
            currentStepIndex = FindFirstFailedStepIndex(timeline);
            TraceCurrentStep("FAULT STEP");
            isAlarmActive = true;
            isRunning = false;
            playbackTimer.Stop();
            RefreshStepRows();
            LoadActiveAlarmGuide(result);
            AddLog("FAULT REPLAY", $"{selectedFaultScenario} replay {CurrentStepName}");
            RefreshComputedProperties();
        }
        catch (Exception ex)
        {
            AddLog("FAULT ERROR", ex.Message);
            Trace("ERROR", $"FaultReplay() {ex.GetType().Name}: {ex.Message}");
        }
    }

    private void StepForward()
    {
        Trace("CALL", "StepForward()");
        if (isAlarmActive)
        {
            AddLog("STEP BLOCKED", "reset required during active alarm");
            Trace("BLOCK", "StepForward() blocked: active alarm requires reset");
            return;
        }

        StepForwardFromTimer();
    }

    private void StepForwardFromTimer()
    {
        if (timeline == null || timeline.Steps.Count == 0)
        {
            Trace("BLOCK", "StepForwardFromTimer() ignored: no timeline");
            return;
        }

        if (currentStepIndex < timeline.Steps.Count - 1)
        {
            currentStepIndex++;
            TraceCurrentStep("STEP");
        }
        else
        {
            isRunning = false;
            playbackTimer.Stop();
            AddLog("COMPLETE", "normal process sequence complete");
            Trace("STATE", "timeline complete; playback stopped");
        }

        RefreshComputedProperties();
    }

    private void OpenUnityViewer()
    {
        var unityProjectPath = ResolveUnityProjectPath();
        if (unityProjectPath == null)
        {
            AddLog("UNITY", "optional viewer folder not found");
            Trace("UNITY", "optional viewer folder not found");
            return;
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = unityProjectPath,
            UseShellExecute = true
        });
        AddLog("UNITY", "optional 3D viewer folder opened");
        Trace("UNITY", $"Process.Start(folder={unityProjectPath})");
    }

    private void LoadNormalTimeline()
    {
        if (recipe == null)
        {
            return;
        }

        var result = runner.Run(recipe);
        Trace("CORE", $"MolyAldRunner.Run(recipe) success={result.Success} steps={result.Steps.Count}");
        timeline = MolyAldTimelineDocument.FromRunResult(result);
        Trace("CORE", $"MolyAldTimelineDocument.FromRunResult() source={timeline.Source}");
        currentStepIndex = 0;
        ClearActiveAlarmGuide();
        ClearReportWorkflowState();
        RefreshStepRows();
        RefreshIssueReportProperties();
        RefreshServerOutboxProperties();
        TraceCurrentStep("LOAD STEP");
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

    private void RefreshInstrumentTrends()
    {
        InstrumentTrends.Clear();
        if (timeline == null || timeline.Steps.Count == 0)
        {
            return;
        }

        var stepCount = Math.Clamp(currentStepIndex + 1, 1, timeline.Steps.Count);
        var visibleSteps = timeline.Steps.Take(stepCount).ToArray();

        InstrumentTrends.Add(CreateInstrumentTrendRow(
            "Pressure",
            PressureValue,
            visibleSteps.Select(step => step.ChamberPressureMtorr),
            value => $"{value:0} mTorr",
            PrimaryBrush));

        InstrumentTrends.Add(CreateInstrumentTrendRow(
            "Temp",
            TemperatureValue,
            visibleSteps.Select(step => step.WaferTemperatureC),
            value => $"{value:0} C",
            WarningBrush));

        InstrumentTrends.Add(CreateInstrumentTrendRow(
            "Film",
            FilmValue,
            visibleSteps.Select(step => step.EstimatedThicknessAngstrom),
            value => $"{value:0.0} A",
            SuccessBrush));
    }

    private static InstrumentTrendRowViewModel CreateInstrumentTrendRow(
        string label,
        string currentValue,
        IEnumerable<double> values,
        Func<double, string> formatValue,
        Brush trendBrush)
    {
        var valueList = values.ToArray();
        var rangeText = valueList.Length == 0
            ? "-"
            : $"{formatValue(valueList.Min())} - {formatValue(valueList.Max())}";

        return new InstrumentTrendRowViewModel(
            label,
            currentValue,
            rangeText,
            CreateTrendPoints(valueList),
            trendBrush);
    }

    private static PointCollection CreateTrendPoints(IReadOnlyList<double> values)
    {
        var points = new PointCollection();
        if (values.Count == 0)
        {
            return points;
        }

        var min = values.Min();
        var max = values.Max();
        if (Math.Abs(max - min) < 0.0001)
        {
            min -= 1;
            max += 1;
        }

        if (values.Count == 1)
        {
            var y = TrendY(values[0], min, max);
            points.Add(new Point(0, y));
            points.Add(new Point(TrendWidth, y));
            return points;
        }

        for (var index = 0; index < values.Count; index++)
        {
            var x = index / (values.Count - 1.0) * TrendWidth;
            points.Add(new Point(x, TrendY(values[index], min, max)));
        }

        return points;
    }

    private static double TrendY(double value, double min, double max)
    {
        var normalized = (value - min) / (max - min);
        return TrendHeight - Math.Clamp(normalized, 0, 1) * TrendHeight;
    }

    private void AddLog(string action, string detail)
    {
        OperatorLog.Insert(0, new OperatorLogEntry(action, detail));
        while (OperatorLog.Count > 8)
        {
            OperatorLog.RemoveAt(OperatorLog.Count - 1);
        }
    }

    private void LoadActiveAlarmGuide(MolyAldRunResult result)
    {
        ClearActiveAlarmGuide();

        if (result.FaultScenario == null)
        {
            Trace("GUIDE", "LoadActiveAlarmGuide() skipped: no fault scenario");
            return;
        }

        if (alarmGuideCatalog == null)
        {
            AddLog("GUIDE ERROR", "alarm guide catalog not loaded");
            Trace("GUIDE", "LoadActiveAlarmGuide() failed: catalog not loaded");
            return;
        }

        var guideCode = MolyAldAlarmGuideCodes.FromFaultKind(result.FaultScenario.Kind);
        Trace("GUIDE", $"MolyAldAlarmGuideCodes.FromFaultKind({result.FaultScenario.Kind}) => {guideCode}");
        activeAlarmGuide = alarmGuideCatalog.FindGuide(guideCode);
        Trace("GUIDE", $"AlarmResponseGuideCatalog.FindGuide({guideCode}) title={activeAlarmGuide.Title}");

        PopulateActiveAlarmGuideRows(Array.Empty<string>(), null);
        AddLog("GUIDE", $"{activeAlarmGuide.AlarmCode} guide loaded");
    }

    private void PopulateActiveAlarmGuideRows(IReadOnlyCollection<string> checkedIds, string? selectedChoiceId)
    {
        AlarmGuideChecks.Clear();
        AlarmGuideChoices.Clear();
        AlarmGuideEscalationConditions.Clear();
        selectedAlarmGuideChoice = null;

        if (activeAlarmGuide == null)
        {
            return;
        }

        foreach (var check in activeAlarmGuide.Checks)
        {
            AlarmGuideChecks.Add(new AlarmGuideCheckRowViewModel(
                check,
                OnAlarmGuideCheckChanged,
                LocalizeGuideText(check.Label),
                useKorean ? (check.Required ? "필수" : "선택") : null,
                checkedIds.Contains(check.Id)));
        }

        foreach (var choice in activeAlarmGuide.Choices)
        {
            var row = new AlarmGuideChoiceRowViewModel(
                choice,
                OnAlarmGuideChoiceSelected,
                LocalizeGuideText(choice.Label),
                LocalizeGuideText(choice.NextAction),
                useKorean ? (choice.RequiresEngineer ? "엔지니어" : "작업자") : null);
            AlarmGuideChoices.Add(row);

            if (string.Equals(choice.Id, selectedChoiceId, StringComparison.OrdinalIgnoreCase))
            {
                selectedAlarmGuideChoice = row;
            }
        }

        foreach (var condition in activeAlarmGuide.EscalationConditions)
        {
            AlarmGuideEscalationConditions.Add(LocalizeGuideText(condition));
        }
    }

    private void ClearActiveAlarmGuide()
    {
        activeAlarmGuide = null;
        selectedAlarmGuideChoice = null;
        AlarmGuideChecks.Clear();
        AlarmGuideChoices.Clear();
        AlarmGuideEscalationConditions.Clear();
    }

    private void ClearReportWorkflowState()
    {
        lastIssueReportPath = null;
        lastServerOutboxPath = null;
        lastServerOutboxState = null;
        lastServerPayloadPreview = null;
        lastMockServerSendStatus = null;
        lastMockServerSendSucceeded = false;
    }

    private void OnAlarmGuideCheckChanged(AlarmGuideCheckRowViewModel check, bool isChecked)
    {
        var state = isChecked ? "confirmed" : "cleared";
        AddLog("CHECK", $"{check.Id} {state}");
        Trace("CHECK", $"{check.Id} {state} required={check.Required}");
        RefreshAlarmGuideProperties();
    }

    private void OnAlarmGuideChoiceSelected(AlarmGuideChoiceRowViewModel choice)
    {
        selectedAlarmGuideChoice = choice;
        AddLog(choice.RequiresEngineer ? "ESCALATE" : "ACTION", $"{choice.Id}: {choice.NextAction}");
        Trace(choice.RequiresEngineer ? "ESCALATE" : "ACTION", $"{choice.Id} next={choice.NextAction}");
        RefreshAlarmGuideProperties();
    }

    private void ExportIssueReport()
    {
        Trace("CALL", "ExportIssueReport()");
        var request = BuildIssueReportRequest();
        if (request == null)
        {
            AddLog("EXPORT BLOCKED", "run FAULT REPLAY first");
            Trace("BLOCK", "ExportIssueReport() blocked: no active alarm guide");
            RefreshAlarmGuideProperties();
            return;
        }

        try
        {
            var result = issueReportExportService.Export(request);
            lastIssueReportPath = result.MarkdownPath;
            AddLog("EXPORT", Path.GetFileName(result.MarkdownPath));
            Trace("EXPORT", $"Issue report exported json={result.JsonPath} md={result.MarkdownPath}");
            RefreshIssueReportProperties();
        }
        catch (Exception ex)
        {
            AddLog("EXPORT ERROR", ex.Message);
            Trace("ERROR", $"ExportIssueReport() {ex.GetType().Name}: {ex.Message}");
        }

        RefreshAlarmGuideProperties();
    }

    private void OpenReportFolder()
    {
        Trace("CALL", "OpenReportFolder()");

        var reportFolderPath = ResolveReportFolderPath();
        Directory.CreateDirectory(reportFolderPath);

        Process.Start(new ProcessStartInfo
        {
            FileName = reportFolderPath,
            UseShellExecute = true
        });

        AddLog("REPORT FOLDER", "opened");
        Trace("REPORT", $"Opened report folder path={reportFolderPath}");
    }

    private void OpenLatestReport()
    {
        Trace("CALL", "OpenLatestReport()");

        if (string.IsNullOrWhiteSpace(lastIssueReportPath) || !File.Exists(lastIssueReportPath))
        {
            AddLog("OPEN BLOCKED", "export report first");
            Trace("BLOCK", "OpenLatestReport() blocked: no exported report");
            RefreshIssueReportProperties();
            return;
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = lastIssueReportPath,
            UseShellExecute = true
        });

        AddLog("REPORT OPEN", Path.GetFileName(lastIssueReportPath));
        Trace("REPORT", $"Opened latest issue report path={lastIssueReportPath}");
    }

    private void QueueIssueReport()
    {
        Trace("CALL", "QueueIssueReport()");
        var request = BuildIssueReportRequest();
        if (request == null)
        {
            AddLog("QUEUE BLOCKED", "run FAULT REPLAY first");
            Trace("BLOCK", "QueueIssueReport() blocked: no active alarm guide");
            RefreshAlarmGuideProperties();
            return;
        }

        try
        {
            var result = issueReportOutboxService.QueueAlarmIssueReport(request);
            lastServerOutboxPath = result.Path;
            lastServerOutboxState = result;
            lastServerPayloadPreview = ReadServerPayloadPreview(result.Path);
            lastMockServerSendSucceeded = false;
            lastMockServerSendStatus = null;
            AddLog("SERVER QUEUE", $"{result.Status} {result.EnvelopeId[..8]}");
            Trace("SERVER", $"Queued issue report envelope={result.EnvelopeId} path={result.Path}");
            RefreshServerOutboxProperties();
        }
        catch (Exception ex)
        {
            AddLog("QUEUE ERROR", ex.Message);
            Trace("ERROR", $"QueueIssueReport() {ex.GetType().Name}: {ex.Message}");
        }

        RefreshAlarmGuideProperties();
    }

    private void OpenServerOutboxFolder()
    {
        Trace("CALL", "OpenServerOutboxFolder()");

        var outboxFolderPath = ResolveServerOutboxFolderPath();
        Directory.CreateDirectory(outboxFolderPath);

        Process.Start(new ProcessStartInfo
        {
            FileName = outboxFolderPath,
            UseShellExecute = true
        });

        AddLog("OUTBOX FOLDER", "opened");
        Trace("SERVER", $"Opened server outbox folder path={outboxFolderPath}");
    }

    private void OpenLatestServerPayload()
    {
        Trace("CALL", "OpenLatestServerPayload()");

        if (string.IsNullOrWhiteSpace(lastServerOutboxPath) || !File.Exists(lastServerOutboxPath))
        {
            AddLog("OPEN BLOCKED", "queue server payload first");
            Trace("BLOCK", "OpenLatestServerPayload() blocked: no queued payload");
            RefreshServerOutboxProperties();
            return;
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = lastServerOutboxPath,
            UseShellExecute = true
        });

        AddLog("PAYLOAD OPEN", Path.GetFileName(lastServerOutboxPath));
        Trace("SERVER", $"Opened latest server payload path={lastServerOutboxPath}");
    }

    private async void CheckMockServerHealth()
    {
        Trace("CALL", "CheckMockServerHealth()");

        if (isCheckingMockServerHealth)
        {
            Trace("BLOCK", "CheckMockServerHealth() blocked: already checking");
            return;
        }

        isCheckingMockServerHealth = true;
        mockServerHealthStatus = L(
            "Checking mock server health...",
            "Mock Server 상태 확인 중...");
        RefreshServerOutboxProperties();

        var result = await mockServerPayloadSender.CheckHealthAsync();
        mockServerHealthOnline = result.Online;
        mockServerHealthStatus = result.Online
            ? L(
                $"Online: HTTP {result.StatusCode} -> {result.Endpoint}",
                $"연결 가능: HTTP {result.StatusCode} -> {result.Endpoint}")
            : L(
                $"Offline: {result.Message}. Start EquipmentTwin.MockServer first.",
                $"연결 실패: {result.Message}. EquipmentTwin.MockServer를 먼저 실행하세요.");
        isCheckingMockServerHealth = false;

        AddLog(result.Online ? "SERVER ONLINE" : "SERVER OFFLINE", result.StatusCode?.ToString() ?? "no response");
        Trace("SERVER", $"CheckMockServerHealth() online={result.Online} status={result.StatusCode} endpoint={result.Endpoint}");
        RefreshServerOutboxProperties();
    }

    private async void SendLatestServerPayload()
    {
        Trace("CALL", "SendLatestServerPayload()");

        if (isSendingServerPayload)
        {
            Trace("BLOCK", "SendLatestServerPayload() blocked: already sending");
            return;
        }

        if (string.IsNullOrWhiteSpace(lastServerOutboxPath) || !File.Exists(lastServerOutboxPath))
        {
            lastMockServerSendSucceeded = false;
            lastMockServerSendStatus = L(
                "Send blocked: queue a server payload first.",
                "전송 차단: 서버 payload를 먼저 대기열에 저장하세요.");
            AddLog("SEND BLOCKED", "queue server payload first");
            Trace("BLOCK", "SendLatestServerPayload() blocked: no queued payload");
            RefreshServerOutboxProperties();
            return;
        }

        try
        {
            lastServerOutboxState = issueReportOutboxService.Read(lastServerOutboxPath);
        }
        catch (Exception ex)
        {
            lastMockServerSendSucceeded = false;
            lastMockServerSendStatus = L(
                $"Send blocked: {ex.Message}",
                $"전송 차단: {ex.Message}");
            AddLog("SEND BLOCKED", ex.Message);
            Trace("BLOCK", $"SendLatestServerPayload() could not read outbox: {ex.Message}");
            RefreshServerOutboxProperties();
            return;
        }

        if (lastServerOutboxState.Status == AlarmIssueReportOutboxStatuses.Sent)
        {
            lastMockServerSendSucceeded = true;
            lastMockServerSendStatus = L(
                "Send blocked: this payload was already sent.",
                "전송 차단: 이미 전송 완료된 payload입니다.");
            AddLog("SEND BLOCKED", "payload already sent");
            Trace("BLOCK", "SendLatestServerPayload() blocked: outbox status is sent");
            RefreshServerOutboxProperties();
            return;
        }

        isSendingServerPayload = true;
        lastMockServerSendSucceeded = false;
        lastMockServerSendStatus = L(
            "Sending latest payload to mock server...",
            "최신 payload를 mock server로 전송 중...");
        RefreshServerOutboxProperties();

        var attemptStarted = false;
        try
        {
            lastServerOutboxState = issueReportOutboxService.BeginSendAttempt(lastServerOutboxPath);
            attemptStarted = true;
            lastServerPayloadPreview = ReadServerPayloadPreview(lastServerOutboxPath);
            RefreshServerOutboxProperties();

            var result = await mockServerPayloadSender.SendAlarmIssueReportAsync(lastServerOutboxPath);
            lastServerOutboxState = result.Success
                ? issueReportOutboxService.MarkSent(lastServerOutboxPath)
                : issueReportOutboxService.MarkFailed(
                    lastServerOutboxPath,
                    $"HTTP {result.StatusCode}: {result.ResponseBody}");
            lastServerPayloadPreview = ReadServerPayloadPreview(lastServerOutboxPath);
            lastMockServerSendSucceeded = lastServerOutboxState.Status == AlarmIssueReportOutboxStatuses.Sent;
            mockServerHealthOnline = result.Success;
            mockServerHealthStatus = result.Success
                ? L(
                    $"Online: latest send returned HTTP {result.StatusCode}",
                    $"연결 가능: 최근 전송 HTTP {result.StatusCode}")
                : L(
                    $"Server responded with HTTP {result.StatusCode}",
                    $"서버 응답: HTTP {result.StatusCode}");
            lastMockServerSendStatus = result.Success
                ? L(
                    $"Send success: HTTP {result.StatusCode} -> {result.Endpoint} | attempt {lastServerOutboxState.AttemptCount}",
                    $"전송 성공: HTTP {result.StatusCode} -> {result.Endpoint} | {lastServerOutboxState.AttemptCount}번째 시도")
                : L(
                    $"Send failed: HTTP {result.StatusCode} -> {result.Endpoint} | retry available",
                    $"전송 실패: HTTP {result.StatusCode} -> {result.Endpoint} | 재전송 가능");

            AddLog(result.Success ? "SEND OK" : "SEND FAIL", $"HTTP {result.StatusCode}");
            Trace(
                "SERVER",
                $"SendLatestServerPayload() status={result.StatusCode} endpoint={result.Endpoint} " +
                $"attempt={lastServerOutboxState.AttemptCount} outbox={lastServerOutboxState.Status} response={result.ResponseBody}");
        }
        catch (Exception ex)
        {
            if (attemptStarted)
            {
                try
                {
                    lastServerOutboxState = issueReportOutboxService.MarkFailed(lastServerOutboxPath, ex.Message);
                    lastServerPayloadPreview = ReadServerPayloadPreview(lastServerOutboxPath);
                }
                catch (Exception stateException)
                {
                    Trace("ERROR", $"MarkFailed() {stateException.GetType().Name}: {stateException.Message}");
                }
            }

            lastMockServerSendSucceeded = false;
            mockServerHealthOnline = false;
            mockServerHealthStatus = L(
                $"Offline: {ex.Message}. Start EquipmentTwin.MockServer first.",
                $"연결 실패: {ex.Message}. EquipmentTwin.MockServer를 먼저 실행하세요.");
            lastMockServerSendStatus = L(
                $"Send failed: {ex.Message}. Retry is available after the server recovers.",
                $"전송 실패: {ex.Message}. 서버 복구 후 재전송할 수 있습니다.");
            AddLog("SEND ERROR", ex.Message);
            Trace(
                "ERROR",
                $"SendLatestServerPayload() {ex.GetType().Name}: {ex.Message} " +
                $"attempt={lastServerOutboxState?.AttemptCount.ToString() ?? "-"} outbox={lastServerOutboxState?.Status ?? "unknown"}");
        }
        finally
        {
            isSendingServerPayload = false;
            RefreshServerOutboxProperties();
        }
    }

    private AlarmIssueReportExportRequest? BuildIssueReportRequest()
    {
        if (activeAlarmGuide == null || timeline == null || CurrentStep == null)
        {
            return null;
        }

        var currentStep = CurrentStep;
        var selectedChoice = selectedAlarmGuideChoice == null
            ? null
            : new AlarmIssueReportChoice(
                selectedAlarmGuideChoice.Id,
                selectedAlarmGuideChoice.Label,
                selectedAlarmGuideChoice.NextAction,
                selectedAlarmGuideChoice.RequiresEngineer);

        return new AlarmIssueReportExportRequest(
            useKorean ? "ko" : "en",
            timeline.RecipeName,
            SelectedFaultScenario,
            activeAlarmGuide.AlarmCode,
            LocalizeGuideText(activeAlarmGuide.Title),
            activeAlarmGuide.Severity.ToString(),
            LocalizeGuideText(activeAlarmGuide.Summary),
            CurrentStepName,
            currentStep.Index,
            timeline.Steps.Count,
            currentStep.Cycle?.ToString() ?? "-",
            currentStep.ChamberPressureMtorr,
            currentStep.WaferTemperatureC,
            currentStep.EstimatedThicknessAngstrom,
            currentStep.Valves.MetalPrecursor,
            currentStep.Valves.Reactant,
            currentStep.Valves.Purge,
            AlarmGuideChecks
                .Select(check => new AlarmIssueReportCheck(check.Id, check.Label, check.Required, check.IsChecked))
                .ToArray(),
            selectedChoice,
            AlarmGuideEscalationConditions.ToArray(),
            EngineeringTrace
                .Select(entry => new AlarmIssueReportTraceEntry(entry.Time, entry.Source, entry.Message))
                .ToArray());
    }

    private void ToggleLanguage()
    {
        var checkedIds = AlarmGuideChecks
            .Where(check => check.IsChecked)
            .Select(check => check.Id)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var selectedChoiceId = selectedAlarmGuideChoice?.Id;

        useKorean = !useKorean;
        if (activeAlarmGuide != null)
        {
            PopulateActiveAlarmGuideRows(checkedIds, selectedChoiceId);
        }

        AddLog("LANG", useKorean ? "한국어 UI" : "English UI");
        Trace("UI", useKorean ? "language switched to Korean" : "language switched to English");
        OnPropertyChanged(string.Empty);
    }

    private string L(string english, string korean)
    {
        return useKorean ? korean : english;
    }

    private static string LocalizeOutboxStatus(string status)
    {
        return status switch
        {
            AlarmIssueReportOutboxStatuses.Queued => "대기",
            AlarmIssueReportOutboxStatuses.Sending => "전송 중",
            AlarmIssueReportOutboxStatuses.Sent => "전송 완료",
            AlarmIssueReportOutboxStatuses.Failed => "전송 실패",
            _ => status
        };
    }

    private string LocalizeAlarmGuideSeverity(AlarmGuideSeverity? severity)
    {
        if (severity == null)
        {
            return L("STANDBY", "대기");
        }

        if (!useKorean)
        {
            return severity.Value.ToString().ToUpperInvariant();
        }

        return severity.Value switch
        {
            EquipmentTwin.Core.Alarms.AlarmGuideSeverity.Critical => "위험",
            EquipmentTwin.Core.Alarms.AlarmGuideSeverity.Warning => "경고",
            EquipmentTwin.Core.Alarms.AlarmGuideSeverity.Info => "정보",
            _ => "대기"
        };
    }

    private string LocalizeStepName(string stepName)
    {
        if (!useKorean)
        {
            return stepName;
        }

        return stepName switch
        {
            "Load Wafer" => "Wafer 로드",
            "Pump Down" => "Pump Down",
            "Stabilize Temp" => "Temp 안정화",
            "Dose Precursor" => "Precursor 주입",
            "Dose Reactant" => "Reactant 주입",
            "Post Purge" => "Post Purge",
            "Transfer Out" => "반출",
            "Complete" => "완료",
            _ => stepName
        };
    }

    private string LocalizeGuideText(string text)
    {
        if (!useKorean)
        {
            return text;
        }

        return text switch
        {
            "Pumpdown Timeout" => "Pumpdown 제한 시간 초과",
            "The synthetic chamber pressure did not reach the demo process setpoint before the pumpdown step timed out." =>
                "모의 Pumpdown 공정의 제한 시간 안에 Chamber 압력이 목표값까지 낮아지지 않았습니다.",
            "Confirm the chamber door/interlock state is closed before retrying pumpdown." =>
                "Pumpdown 재시도 전에 Chamber Door와 Interlock이 닫힘 상태인지 확인합니다.",
            "Confirm the vacuum pump command is ON in the HMI or trace log." =>
                "HMI 또는 Trace Log에서 Vacuum Pump 명령이 ON인지 확인합니다.",
            "Compare the current pressure reading with the expected pumpdown trend." =>
                "현재 압력값이 정상적인 Pumpdown 추세로 감소하는지 확인합니다.",
            "Check whether the synthetic exhaust path or gate valve state is blocking pumpdown." =>
                "모의 배기 경로 또는 Gate Valve 상태가 Pumpdown을 막고 있는지 확인합니다.",
            "Door/interlock was not ready" => "Chamber Door 또는 Interlock 준비 안 됨",
            "Secure the chamber/interlock, reset the demo alarm, and retry pumpdown." =>
                "Chamber Door와 Interlock을 정상 상태로 만든 뒤 알람을 Reset하고 Pumpdown을 재시도합니다.",
            "Pump command is ON but pressure remains high" => "Pump 명령은 ON이지만 압력이 높게 유지됨",
            "Export the issue report with pressure snapshot and escalate to engineering review." =>
                "압력 Snapshot이 포함된 Issue Report를 저장하고 엔지니어에게 검토를 요청합니다.",
            "Pressure reading looks inconsistent" => "압력값이 비정상적으로 보임",
            "Capture the issue report and flag the pressure signal for sensor/IO path review." =>
                "Issue Report를 저장하고 압력 신호의 Sensor/IO 경로 검토를 요청합니다.",
            "Pump command is ON but chamber pressure does not trend downward." =>
                "Pump 명령이 ON인데도 Chamber 압력이 낮아지지 않습니다.",
            "Pressure value is frozen, out of range, or inconsistent with the expected pumpdown step." =>
                "압력값이 고정되거나 허용 범위를 벗어나거나 현재 Pumpdown 단계와 일치하지 않습니다.",
            "The same pumpdown timeout repeats after interlock reset." =>
                "Interlock을 Reset한 뒤에도 같은 Pumpdown 시간 초과가 반복됩니다.",

            "Temperature Not Stable" => "Wafer 온도 안정화 실패",
            "The synthetic wafer temperature did not reach the demo stabilization band before ALD cycles started." =>
                "ALD Cycle을 시작하기 전에 모의 Wafer 온도가 설정된 안정화 범위에 도달하지 않았습니다.",
            "Confirm the recipe temperature setpoint shown on the HMI." =>
                "HMI에 표시된 Recipe 온도 설정값을 확인합니다.",
            "Confirm the current wafer temperature reading and whether it is trending toward setpoint." =>
                "현재 Wafer 온도가 설정값 방향으로 변하고 있는지 확인합니다.",
            "Confirm the alarm occurred during the temperature stabilization step." =>
                "알람이 온도 안정화 단계에서 발생했는지 확인합니다.",
            "Temperature is still ramping" => "온도가 아직 상승 또는 하강 중",
            "Hold the process and review whether the stabilization timeout is too short for this demo recipe." =>
                "공정을 Hold하고 현재 Demo Recipe의 안정화 제한 시간이 너무 짧은지 검토합니다.",
            "Temperature value is flat or unrealistic" => "온도값이 고정되었거나 현실적이지 않음",
            "Export the issue report and review the synthetic sensor or trace generation path." =>
                "Issue Report를 저장하고 모의 Sensor 또는 Trace 생성 경로를 검토합니다.",
            "Setpoint and reading look normal after reset" => "Reset 후 설정값과 현재값 모두 정상",
            "Reset the demo alarm and retry the normal process timeline." =>
                "알람을 Reset하고 정상 공정 Timeline을 다시 실행합니다.",
            "Temperature does not move toward the setpoint during stabilization." =>
                "안정화 중 온도가 설정값 방향으로 변하지 않습니다.",
            "Temperature jumps abruptly without a matching process state change." =>
                "공정 상태 변화 없이 온도가 갑자기 크게 변합니다.",
            "The same stabilization alarm repeats after a retry." =>
                "재시도 후에도 같은 온도 안정화 알람이 반복됩니다.",

            "Gas Delivery Step Timeout" => "Gas 공급 단계 제한 시간 초과",
            "A synthetic precursor, reactant, or purge step failed to complete within the demo process timing rule." =>
                "모의 Precursor/Reactant/Purge 단계가 설정된 제한 시간 안에 완료되지 않았습니다.",
            "Confirm which valve or gas delivery step was active when the alarm occurred." =>
                "알람이 발생했을 때 어떤 Valve와 Gas 공급 단계가 동작 중이었는지 확인합니다.",
            "Confirm the ALD cycle number shown in the HMI and report." =>
                "HMI와 Issue Report에 표시된 ALD Cycle 번호를 확인합니다.",
            "Check whether chamber pressure changed as expected during the gas delivery step." =>
                "Gas 공급 단계에서 Chamber 압력이 예상대로 변했는지 확인합니다.",
            "Confirm the active recipe step matches the expected ALD dose/purge sequence." =>
                "현재 Recipe 단계가 예상한 ALD 주입/Purge 순서와 일치하는지 확인합니다.",
            "Expected valve was not active" => "명령된 Valve가 동작하지 않음",
            "Capture the issue report and review command-to-valve mapping in the sequence logic." =>
                "Issue Report를 저장하고 Sequence Logic의 Command-to-Valve 연결을 검토합니다.",
            "Valve active but process response missing" => "Valve는 동작하지만 공정 응답이 없음",
            "Escalate with the process snapshot, active valve state, and trace log." =>
                "공정 Snapshot, 활성 Valve 상태, Trace Log와 함께 엔지니어에게 검토를 요청합니다.",
            "State looks normal after reset" => "Reset 후 장비 상태 정상",
            "Reset the demo alarm and rerun the fault scenario or normal timeline for comparison." =>
                "알람을 Reset하고 비교를 위해 Fault Scenario 또는 정상 Timeline을 다시 실행합니다.",
            "Valve state does not match the active ALD step." =>
                "Valve 상태가 현재 ALD 단계와 일치하지 않습니다.",
            "Gas delivery step fails repeatedly on the same cycle." =>
                "같은 ALD Cycle에서 Gas 공급 단계가 반복해서 실패합니다.",
            "Pressure or film trend is inconsistent with the active dose/purge step." =>
                "압력 또는 막 두께 추세가 현재 주입/Purge 단계와 일치하지 않습니다.",

            "Sequence State Mismatch" => "Sequence 상태 불일치",
            "The synthetic process reached an unexpected sequence state or a generic step-level fault without a more specific guide." =>
                "모의 공정이 예상하지 못한 Sequence 상태에 도달했거나 전용 가이드가 없는 Step 오류가 발생했습니다.",
            "Confirm the current process state and the last accepted operator command." =>
                "현재 공정 상태와 마지막으로 정상 처리된 작업자 명령을 확인합니다.",
            "Confirm the previous successful step before the mismatch." =>
                "상태 불일치 직전의 마지막 정상 Step을 확인합니다.",
            "Review the engineering trace for the first rejected or failed transition." =>
                "Engineering Trace에서 처음 거부되거나 실패한 상태 전환을 확인합니다.",
            "Mismatch followed an operator command" => "작업자 명령 후 상태 불일치 발생",
            "Record the command and state snapshot, then review whether the command should be blocked earlier in the HMI." =>
                "명령과 상태 Snapshot을 기록하고 해당 명령을 HMI에서 더 일찍 차단해야 하는지 검토합니다.",
            "Cause is not clear from the HMI" => "HMI만으로 원인을 확인할 수 없음",
            "Export the issue report and escalate with trace, current state, and previous step." =>
                "Issue Report를 저장하고 Trace, 현재 상태, 이전 Step과 함께 엔지니어에게 검토를 요청합니다.",
            "The same sequence mismatch is reproducible." =>
                "같은 Sequence 상태 불일치가 반복 재현됩니다.",
            "The HMI allows an operation that should be blocked by the current state." =>
                "현재 상태에서 차단되어야 하는 조작을 HMI가 허용합니다.",
            "The engineering trace shows a rejected transition that is not explained by the guide." =>
                "Engineering Trace에 가이드로 설명되지 않는 거부된 상태 전환이 기록됩니다.",

            _ => text
        };
    }

    private void TraceCurrentStep(string source)
    {
        if (CurrentStep == null)
        {
            Trace(source, "currentStep=null");
            return;
        }

        var valves = CurrentStep.Valves;
        Trace(
            source,
            $"idx={CurrentStep.Index} step={CurrentStep.Step} cycle={CurrentStep.Cycle?.ToString() ?? "-"} " +
            $"ok={CurrentStep.Success} p={CurrentStep.ChamberPressureMtorr:0}mTorr " +
            $"t={CurrentStep.WaferTemperatureC:0}C film={CurrentStep.EstimatedThicknessAngstrom:0.0}A " +
            $"valves(P={valves.MetalPrecursor},R={valves.Reactant},G={valves.Purge})");
    }

    private void Trace(string source, string message)
    {
        EngineeringTrace.Insert(0, new EngineeringTraceEntry(source, message));
        while (EngineeringTrace.Count > MaxTraceEntries)
        {
            EngineeringTrace.RemoveAt(EngineeringTrace.Count - 1);
        }

        OnPropertyChanged(nameof(EngineeringTraceStatus));
    }

    private void RefreshAlarmGuideProperties()
    {
        OnPropertyChanged(nameof(AlarmGuideTitle));
        OnPropertyChanged(nameof(AlarmGuideSeverity));
        OnPropertyChanged(nameof(AlarmGuideSeverityBrush));
        OnPropertyChanged(nameof(AlarmGuideSummary));
        OnPropertyChanged(nameof(AlarmGuideStatus));
        OnPropertyChanged(nameof(SelectedAlarmGuideChoiceText));
        OnPropertyChanged(nameof(IssueReportStatus));
        OnPropertyChanged(nameof(ServerOutboxStatus));
        OnPropertyChanged(nameof(AlarmGuideCardBrush));
        OnPropertyChanged(nameof(AlarmCode));
        RefreshWorkflowProperties();
    }

    private void RefreshIssueReportProperties()
    {
        OnPropertyChanged(nameof(LatestIssueReportPathText));
        RefreshWorkflowProperties();
    }

    private void RefreshServerOutboxProperties()
    {
        OnPropertyChanged(nameof(LatestServerPayloadPathText));
        OnPropertyChanged(nameof(LatestServerPayloadPreviewText));
        OnPropertyChanged(nameof(LatestServerOutboxStateText));
        OnPropertyChanged(nameof(LatestServerOutboxAttemptText));
        OnPropertyChanged(nameof(LatestServerOutboxErrorText));
        OnPropertyChanged(nameof(LatestServerOutboxStateBrush));
        OnPropertyChanged(nameof(CanSendLatestServerPayload));
        OnPropertyChanged(nameof(CheckMockServerHealthButtonText));
        OnPropertyChanged(nameof(MockServerHealthBadgeText));
        OnPropertyChanged(nameof(MockServerHealthStatusText));
        OnPropertyChanged(nameof(MockServerHealthBrush));
        OnPropertyChanged(nameof(SendLatestServerPayloadButtonText));
        OnPropertyChanged(nameof(LatestMockServerSendStatus));
        RefreshWorkflowProperties();
    }

    private void RefreshWorkflowProperties()
    {
        OnPropertyChanged(nameof(WorkflowStepperLabel));
        OnPropertyChanged(nameof(WorkflowAlarmStepStatus));
        OnPropertyChanged(nameof(WorkflowAlarmStepDetail));
        OnPropertyChanged(nameof(WorkflowAlarmStepBrush));
        OnPropertyChanged(nameof(WorkflowChecklistStepStatus));
        OnPropertyChanged(nameof(WorkflowChecklistStepDetail));
        OnPropertyChanged(nameof(WorkflowChecklistStepBrush));
        OnPropertyChanged(nameof(WorkflowResponseStepStatus));
        OnPropertyChanged(nameof(WorkflowResponseStepDetail));
        OnPropertyChanged(nameof(WorkflowResponseStepBrush));
        OnPropertyChanged(nameof(WorkflowReportStepStatus));
        OnPropertyChanged(nameof(WorkflowReportStepDetail));
        OnPropertyChanged(nameof(WorkflowReportStepBrush));
        OnPropertyChanged(nameof(WorkflowQueueStepStatus));
        OnPropertyChanged(nameof(WorkflowQueueStepDetail));
        OnPropertyChanged(nameof(WorkflowQueueStepBrush));
        OnPropertyChanged(nameof(WorkflowSendStepStatus));
        OnPropertyChanged(nameof(WorkflowSendStepDetail));
        OnPropertyChanged(nameof(WorkflowSendStepBrush));
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
        OnPropertyChanged(nameof(InstrumentTrendLabel));
        RefreshInstrumentTrends();
        OnPropertyChanged(nameof(AlarmIcon));
        OnPropertyChanged(nameof(AlarmIconBrush));
        OnPropertyChanged(nameof(AlarmTitle));
        OnPropertyChanged(nameof(AlarmDetail));
        OnPropertyChanged(nameof(AlarmCode));
        OnPropertyChanged(nameof(AlarmCodeBrush));
        OnPropertyChanged(nameof(AlarmCardBrush));
        OnPropertyChanged(nameof(SchematicDiagnosticText));
        OnPropertyChanged(nameof(SchematicDiagnosticBrush));
        OnPropertyChanged(nameof(GasModuleBorderBrush));
        OnPropertyChanged(nameof(GasDeliveryLineBrush));
        OnPropertyChanged(nameof(ChamberBorderBrush));
        OnPropertyChanged(nameof(HeaterDiagnosticBrush));
        OnPropertyChanged(nameof(ExhaustModuleBorderBrush));
        OnPropertyChanged(nameof(VacuumPathBrush));
        OnPropertyChanged(nameof(WaferTransferStatusText));
        OnPropertyChanged(nameof(TransferGateText));
        OnPropertyChanged(nameof(TransferGateBrush));
        OnPropertyChanged(nameof(LoadPortWaferOpacity));
        OnPropertyChanged(nameof(TransferInWaferOpacity));
        OnPropertyChanged(nameof(ChamberWaferOpacity));
        OnPropertyChanged(nameof(TransferOutWaferOpacity));
        OnPropertyChanged(nameof(IsGasFlowActive));
        OnPropertyChanged(nameof(IsVacuumFlowActive));
        OnPropertyChanged(nameof(IsHeaterActive));
        OnPropertyChanged(nameof(IsWaferTransferInActive));
        OnPropertyChanged(nameof(IsWaferTransferOutActive));
        OnPropertyChanged(nameof(IsTransferGateOpenVisual));
        OnPropertyChanged(nameof(IsPrecursorValveOpen));
        OnPropertyChanged(nameof(IsReactantValveOpen));
        OnPropertyChanged(nameof(IsPurgeValveOpen));
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
        OnPropertyChanged(nameof(EngineeringTraceStatus));
        RefreshAlarmGuideProperties();
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

    private static string ResolveReportFolderPath()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "EquipmentTwinLab.sln")))
            {
                return Path.Combine(current.FullName, "artifacts", "alarm-reports");
            }

            current = current.Parent;
        }

        return Path.Combine(AppContext.BaseDirectory, "artifacts", "alarm-reports");
    }

    private static string ReadServerPayloadPreview(string path)
    {
        var text = File.ReadAllText(path);
        if (text.Length <= MaxServerPayloadPreviewLength)
        {
            return text;
        }

        return text[..MaxServerPayloadPreviewLength] +
            Environment.NewLine +
            "... preview truncated";
    }

    private static string ResolveServerOutboxFolderPath()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "EquipmentTwinLab.sln")))
            {
                return Path.Combine(current.FullName, "artifacts", "server-outbox");
            }

            current = current.Parent;
        }

        return Path.Combine(AppContext.BaseDirectory, "artifacts", "server-outbox");
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
            return lowLabel;
        }

        if (value > high)
        {
            return highLabel;
        }

        return normalLabel;
    }

    private static Brush StatusBrush(string status)
    {
        return status is "OK" or "TARGET" or "정상" or "목표" ? SuccessBrush :
            status is "GROWING" or "WAIT" or "성장 중" or "대기" ? PrimaryBrush :
            status is "LOW" or "COOL" or "낮음" ? WarningBrush :
            AlarmBrush;
    }

    private static Brush WorkflowStepBrush(bool complete, bool active)
    {
        if (complete)
        {
            return SuccessBrush;
        }

        return active ? WarningBrush : TextMutedBrush;
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
