using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EquipmentTwin.Unity.Processes
{
    [DisallowMultipleComponent]
    public sealed class MolyAldOperatorCanvas : MonoBehaviour
    {
        private const int OperatorActionLogCapacity = 3;

        [Header("Source")]
        [SerializeField] private MolyAldProcessPlayer player;

        [Header("Auto UI")]
        [SerializeField] private bool autoCreateCanvas = true;
        [SerializeField] private Canvas canvas;

        [Header("Pressure mapping")]
        [SerializeField] private float processPressureMtorr = 850f;
        [SerializeField] private float atmospherePressureMtorr = 760000f;

        [Header("Temperature mapping")]
        [SerializeField] private float roomTemperatureC = 25f;
        [SerializeField] private float processTemperatureC = 250f;

        private RectTransform progressFill;
        private Image alarmCardImage;
        private Text recipeText;
        private Text currentStepText;
        private Text alarmText;
        private Text alarmDetailText;
        private Text alarmCodeText;
        private Text faultScenarioText;
        private Text eventText;
        private Text hmiStateText;
        private Image operatorActionLogCardImage;
        private Text[] operatorActionLogTexts = new Text[0];
        private readonly string[] operatorActionLogEntries = new string[OperatorActionLogCapacity];
        private int operatorActionLogEntryCount;
        private Text schematicStepText;
        private Text schematicMetaText;
        private Text schematicFlowText;
        private Text precursorValveText;
        private Text reactantValveText;
        private Text purgeValveText;
        private Image chamberImage;
        private Image showerheadImage;
        private Image filmFillImage;
        private Image precursorValveImage;
        private Image reactantValveImage;
        private Image purgeValveImage;
        private Image precursorLineImage;
        private Image reactantLineImage;
        private Image purgeLineImage;
        private Image exhaustLineImage;
        private Image gateValveImage;
        private Image pumpImage;
        private Image[] gasDistributionDots = new Image[0];
        private Image[] gasFlowPulses = new Image[0];
        private Image[] exhaustFlowPulses = new Image[0];
        private Image startButtonImage;
        private Image stopButtonImage;
        private Image faultButtonImage;
        private Image resetButtonImage;
        private Text startButtonText;
        private Text stopButtonText;
        private Text faultButtonText;
        private Text resetButtonText;
        private InstrumentView pressureInstrument;
        private InstrumentView temperatureInstrument;
        private InstrumentView filmInstrument;
        private Text[] timelineLabels = new Text[0];
        private Image[] timelineChips = new Image[0];

        private static readonly Color Background = Hex(0x0B, 0x0F, 0x14);
        private static readonly Color Surface = Hex(0x15, 0x1C, 0x24);
        private static readonly Color SurfaceRaised = Hex(0x1D, 0x27, 0x33);
        private static readonly Color Border = Hex(0x2F, 0x3C, 0x4C);
        private static readonly Color TextPrimary = Hex(0xEA, 0xF0, 0xF7);
        private static readonly Color TextMuted = Hex(0x9A, 0xA8, 0xB7);
        private static readonly Color Primary = Hex(0x2E, 0xA8, 0xFF);
        private static readonly Color Success = Hex(0x2A, 0xD1, 0x7D);
        private static readonly Color Warning = Hex(0xFF, 0xB8, 0x4D);
        private static readonly Color Alarm = Hex(0xFF, 0x3B, 0x3B);
        private static readonly Color Stop = Hex(0xA8, 0x2D, 0x2D);
        private static readonly Color NeutralButton = Hex(0x2B, 0x3A, 0x4A);
        private static readonly Color NormalBand = Hex(0x20, 0x6A, 0x4A);
        private static readonly Color GaugeTrack = Hex(0x0F, 0x15, 0x1B);
        private static readonly Color SchematicBackground = Hex(0x07, 0x09, 0x0C);
        private static readonly Color SchematicMetal = Hex(0x50, 0x5B, 0x66);
        private static readonly Color SchematicGlass = Hex(0x12, 0x21, 0x2C);
        private static readonly Color Precursor = Hex(0xF7, 0xA8, 0x3B);
        private static readonly Color Reactant = Hex(0x38, 0xCF, 0xFF);
        private static readonly Color Purge = Hex(0x31, 0xD8, 0x6B);

        private static readonly string[] TimelineSteps =
        {
            "Load",
            "Pump",
            "Temp",
            "Precursor",
            "Purge",
            "Reactant",
            "Complete"
        };

        private void Reset()
        {
            player = GetComponent<MolyAldProcessPlayer>();
        }

        private void Awake()
        {
            if (player == null)
            {
                player = GetComponent<MolyAldProcessPlayer>();
            }

            if (autoCreateCanvas)
            {
                EnsureCanvas();
            }
        }

        private void LateUpdate()
        {
            RefreshCanvas();
        }

        public void EnsureCanvas()
        {
            if (canvas != null)
            {
                EnsureEventSystem();
                return;
            }

            var canvasObject = new GameObject("Moly ALD Canvas Operator Panel");
            canvasObject.transform.SetParent(transform, false);

            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = ResolveCamera();
            canvas.planeDistance = 1f;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();
            EnsureEventSystem();

            BuildProcessSchematic(canvasObject.transform);
            BuildOperatorPanel(canvasObject.transform);
            BuildTimeline(canvasObject.transform);
        }

        public void RefreshCanvas()
        {
            if (player == null)
            {
                return;
            }

            if (canvas == null && autoCreateCanvas)
            {
                EnsureCanvas();
            }

            if (canvas != null && canvas.worldCamera == null)
            {
                canvas.worldCamera = ResolveCamera();
            }

            var timeline = player.Timeline;
            var step = player.CurrentStep;
            if (timeline == null || step == null)
            {
                return;
            }

            var visualState = MolyAldVisualStateMapper.FromTimeline(
                timeline,
                step,
                processPressureMtorr,
                atmospherePressureMtorr,
                roomTemperatureC,
                processTemperatureC,
                player.OperatorFaultActive);
            ApplyVisualState(visualState);
        }

        public int OperatorActionLogEntryCount => operatorActionLogEntryCount;

        public void ClearOperatorActionLog()
        {
            for (var index = 0; index < operatorActionLogEntries.Length; index++)
            {
                operatorActionLogEntries[index] = string.Empty;
            }

            operatorActionLogEntryCount = 0;
            RefreshOperatorActionLog();
        }

        public void RecordOperatorAction(string action, string detail)
        {
            var safeAction = string.IsNullOrWhiteSpace(action) ? "EVENT" : action.Trim();
            var safeDetail = string.IsNullOrWhiteSpace(detail) ? "-" : detail.Trim();
            var entry = $"{DateTime.Now:HH:mm:ss}  {safeAction}  |  {safeDetail}";

            for (var index = operatorActionLogEntries.Length - 1; index > 0; index--)
            {
                operatorActionLogEntries[index] = operatorActionLogEntries[index - 1];
            }

            operatorActionLogEntries[0] = entry;
            operatorActionLogEntryCount = Mathf.Min(operatorActionLogEntries.Length, operatorActionLogEntryCount + 1);
            RefreshOperatorActionLog();
        }

        private void BuildProcessSchematic(Transform parent)
        {
            var panel = CreatePanel(
                parent,
                "Process Schematic Main View",
                new Vector2(0.00f, 0.19f),
                new Vector2(0.65f, 0.94f),
                SchematicBackground);

            CreateText(panel, "Synthetic Moly ALD — Process Schematic", new Vector2(0.02f, 0.91f), new Vector2(0.96f, 0.985f), 22, TextPrimary, TextAnchor.MiddleLeft, FontStyle.Bold);
            CreateText(panel, "public-reference HMI | not vendor CAD or process copy", new Vector2(0.02f, 0.865f), new Vector2(0.96f, 0.92f), 12, TextMuted, TextAnchor.MiddleLeft);

            var runBadge = CreatePanel(panel, "Schematic Run Badge", new Vector2(0.72f, 0.92f), new Vector2(0.96f, 0.97f), SurfaceRaised);
            schematicMetaText = CreateText(runBadge, "RUNNING | OK", new Vector2(0.05f, 0f), new Vector2(0.95f, 1f), 12, Success, TextAnchor.MiddleCenter, FontStyle.Bold);

            var schematicArea = CreatePanel(panel, "Connected ALD Schematic Area", new Vector2(0.03f, 0.06f), new Vector2(0.97f, 0.84f), Background);

            CreateText(schematicArea, "GAS DELIVERY", new Vector2(0.20f, 0.86f), new Vector2(0.78f, 0.94f), 11, TextMuted, TextAnchor.MiddleCenter, FontStyle.Bold);
            CreateText(schematicArea, "PRECURSOR", new Vector2(0.23f, 0.78f), new Vector2(0.36f, 0.84f), 9, TextMuted, TextAnchor.MiddleCenter);
            CreateText(schematicArea, "REACTANT", new Vector2(0.43f, 0.78f), new Vector2(0.56f, 0.84f), 9, Reactant, TextAnchor.MiddleCenter);
            CreateText(schematicArea, "PURGE N2", new Vector2(0.62f, 0.78f), new Vector2(0.75f, 0.84f), 9, TextMuted, TextAnchor.MiddleCenter);

            precursorLineImage = CreatePanel(schematicArea, "Precursor Gas Line", new Vector2(0.295f, 0.62f), new Vector2(0.303f, 0.78f), SchematicMetal).GetComponent<Image>();
            reactantLineImage = CreatePanel(schematicArea, "Reactant Gas Line", new Vector2(0.495f, 0.62f), new Vector2(0.503f, 0.78f), Reactant).GetComponent<Image>();
            purgeLineImage = CreatePanel(schematicArea, "Purge Gas Line", new Vector2(0.685f, 0.62f), new Vector2(0.693f, 0.78f), SchematicMetal).GetComponent<Image>();

            precursorValveImage = CreatePanel(schematicArea, "Precursor Valve Symbol", new Vector2(0.268f, 0.705f), new Vector2(0.330f, 0.755f), SurfaceRaised).GetComponent<Image>();
            reactantValveImage = CreatePanel(schematicArea, "Reactant Valve Symbol", new Vector2(0.468f, 0.705f), new Vector2(0.530f, 0.755f), Reactant).GetComponent<Image>();
            purgeValveImage = CreatePanel(schematicArea, "Purge Valve Symbol", new Vector2(0.658f, 0.705f), new Vector2(0.720f, 0.755f), SurfaceRaised).GetComponent<Image>();
            precursorValveText = CreateText(precursorValveImage.transform, "PRE\nOFF", new Vector2(0f, 0f), new Vector2(1f, 1f), 8, TextMuted, TextAnchor.MiddleCenter);
            reactantValveText = CreateText(reactantValveImage.transform, "RCT\nON", new Vector2(0f, 0f), new Vector2(1f, 1f), 8, Background, TextAnchor.MiddleCenter, FontStyle.Bold);
            purgeValveText = CreateText(purgeValveImage.transform, "PRG\nOFF", new Vector2(0f, 0f), new Vector2(1f, 1f), 8, TextMuted, TextAnchor.MiddleCenter);

            chamberImage = CreatePanel(schematicArea, "Vacuum Chamber Schematic", new Vector2(0.18f, 0.30f), new Vector2(0.76f, 0.68f), SchematicGlass).GetComponent<Image>();
            CreateText(chamberImage.transform, "VACUUM CHAMBER", new Vector2(0.04f, 0.82f), new Vector2(0.45f, 0.98f), 9, TextMuted, TextAnchor.MiddleLeft, FontStyle.Bold);

            showerheadImage = CreatePanel(chamberImage.transform, "Showerhead Gas Distributor", new Vector2(0.18f, 0.67f), new Vector2(0.84f, 0.75f), SchematicMetal).GetComponent<Image>();
            CreateText(showerheadImage.transform, "showerhead", new Vector2(0f, 0f), new Vector2(1f, 1f), 8, TextPrimary, TextAnchor.MiddleCenter);

            gasDistributionDots = new Image[9];
            for (var index = 0; index < 9; index++)
            {
                var x = 0.22f + index * 0.065f;
                gasDistributionDots[index] = CreatePanel(chamberImage.transform, $"Gas Distribution Dot {index + 1}", new Vector2(x, 0.57f), new Vector2(x + 0.012f, 0.59f), Reactant).GetComponent<Image>();
            }

            gasFlowPulses = new Image[5];
            for (var index = 0; index < gasFlowPulses.Length; index++)
            {
                gasFlowPulses[index] = CreatePanel(schematicArea, $"Animated Gas Flow Pulse {index + 1}", new Vector2(0.49f, 0.66f), new Vector2(0.51f, 0.70f), Reactant).GetComponent<Image>();
            }

            CreateText(chamberImage.transform, "wafer + film", new Vector2(0.35f, 0.34f), new Vector2(0.65f, 0.44f), 9, TextPrimary, TextAnchor.MiddleCenter, FontStyle.Bold);
            var wafer = CreatePanel(chamberImage.transform, "Wafer Base", new Vector2(0.22f, 0.24f), new Vector2(0.78f, 0.31f), Primary);
            filmFillImage = CreatePanel(wafer, "Film Thickness Fill", new Vector2(0f, 0.74f), new Vector2(1f, 1f), Success).GetComponent<Image>();
            var heater = CreatePanel(chamberImage.transform, "Susceptor Heater", new Vector2(0.20f, 0.16f), new Vector2(0.80f, 0.24f), Warning);
            CreateText(heater, "susceptor heater 250 C", new Vector2(0f, 0f), new Vector2(1f, 1f), 8, Background, TextAnchor.MiddleCenter, FontStyle.Bold);

            CreatePanel(schematicArea, "Load Port", new Vector2(0.045f, 0.40f), new Vector2(0.145f, 0.58f), SurfaceRaised);
            CreatePanel(schematicArea, "Load Port Slot 1", new Vector2(0.065f, 0.52f), new Vector2(0.125f, 0.55f), GaugeTrack);
            CreatePanel(schematicArea, "Load Port Slot 2", new Vector2(0.065f, 0.45f), new Vector2(0.125f, 0.48f), GaugeTrack);
            CreateText(schematicArea, "LOAD PORT", new Vector2(0.04f, 0.34f), new Vector2(0.16f, 0.39f), 8, TextMuted, TextAnchor.MiddleCenter);
            CreatePanel(schematicArea, "Transfer Link", new Vector2(0.145f, 0.485f), new Vector2(0.18f, 0.492f), Border);

            CreatePanel(schematicArea, "Pressure Temperature Tap", new Vector2(0.12f, 0.25f), new Vector2(0.18f, 0.258f), Primary);
            CreateText(schematicArea, "P/T tap", new Vector2(0.09f, 0.18f), new Vector2(0.20f, 0.24f), 8, Primary, TextAnchor.MiddleCenter);

            exhaustLineImage = CreatePanel(schematicArea, "Exhaust Line", new Vector2(0.76f, 0.38f), new Vector2(0.86f, 0.392f), SchematicMetal).GetComponent<Image>();
            var gate = CreatePanel(schematicArea, "Gate Valve", new Vector2(0.84f, 0.34f), new Vector2(0.89f, 0.43f), SurfaceRaised);
            gateValveImage = gate.GetComponent<Image>();
            CreateText(gate, "GATE", new Vector2(0f, 0f), new Vector2(1f, 1f), 7, TextMuted, TextAnchor.MiddleCenter);
            var pump = CreatePanel(schematicArea, "Vacuum Pump", new Vector2(0.88f, 0.27f), new Vector2(0.96f, 0.39f), SurfaceRaised);
            pumpImage = pump.GetComponent<Image>();
            CreateText(pump, "PUMP", new Vector2(0f, 0f), new Vector2(1f, 1f), 8, TextPrimary, TextAnchor.MiddleCenter, FontStyle.Bold);

            exhaustFlowPulses = new Image[4];
            for (var index = 0; index < exhaustFlowPulses.Length; index++)
            {
                exhaustFlowPulses[index] = CreatePanel(schematicArea, $"Animated Exhaust Flow Pulse {index + 1}", new Vector2(0.77f, 0.374f), new Vector2(0.79f, 0.398f), Primary).GetComponent<Image>();
            }

            schematicStepText = CreateText(schematicArea, "STEP: Dose Reactant", new Vector2(0.20f, 0.07f), new Vector2(0.77f, 0.14f), 13, TextPrimary, TextAnchor.MiddleCenter, FontStyle.Bold);
            schematicFlowText = CreateText(schematicArea, "FLOW: Reactant pulse -> chamber", new Vector2(0.72f, 0.08f), new Vector2(0.96f, 0.14f), 10, Reactant, TextAnchor.MiddleCenter, FontStyle.Bold);
        }

        private void BuildOperatorPanel(Transform parent)
        {
            var panel = CreatePanel(
                parent,
                "Canvas Operator Panel",
                new Vector2(0.655f, 0.16f),
                new Vector2(0.975f, 0.94f),
                Background);

            CreateText(panel, "MOLY ALD HMI", new Vector2(0.06f, 0.93f), new Vector2(0.94f, 0.99f), 24, TextPrimary, TextAnchor.MiddleCenter, FontStyle.Bold);
            CreateText(panel, "operator console | synthetic public-reference process", new Vector2(0.06f, 0.885f), new Vector2(0.94f, 0.93f), 12, TextMuted, TextAnchor.MiddleCenter);

            var stateStrip = CreatePanel(panel, "Run State Strip", new Vector2(0.06f, 0.82f), new Vector2(0.94f, 0.875f), SurfaceRaised);
            hmiStateText = CreateText(stateStrip, "RUNNING  |  INTERLOCK OK", new Vector2(0.05f, 0f), new Vector2(0.95f, 1f), 15, Success, TextAnchor.MiddleCenter, FontStyle.Bold);

            CreateCommandButton(panel, "START", new Vector2(0.06f, 0.745f), new Vector2(0.28f, 0.805f), Success, Background, OnStartClicked, out startButtonImage, out startButtonText);
            CreateCommandButton(panel, "STOP", new Vector2(0.305f, 0.745f), new Vector2(0.525f, 0.805f), Stop, TextPrimary, OnPauseClicked, out stopButtonImage, out stopButtonText);
            CreateCommandButton(panel, "FAULT", new Vector2(0.55f, 0.745f), new Vector2(0.77f, 0.805f), Warning, Background, OnFaultClicked, out faultButtonImage, out faultButtonText);
            CreateCommandButton(panel, "RESET", new Vector2(0.795f, 0.745f), new Vector2(0.94f, 0.805f), NeutralButton, TextPrimary, OnResetClicked, out resetButtonImage, out resetButtonText);
            faultScenarioText = CreateText(panel, "FAULT SCENARIO: precursor-dose-timeout", new Vector2(0.06f, 0.724f), new Vector2(0.94f, 0.744f), 9, Warning, TextAnchor.MiddleLeft, FontStyle.Bold);
            faultScenarioText.verticalOverflow = VerticalWrapMode.Overflow;
            faultScenarioText.horizontalOverflow = HorizontalWrapMode.Overflow;

            var recipeCard = CreatePanel(panel, "Recipe Card", new Vector2(0.06f, 0.615f), new Vector2(0.94f, 0.725f), SurfaceRaised);
            CreateText(recipeCard, "CURRENT STEP", new Vector2(0.05f, 0.58f), new Vector2(0.48f, 0.92f), 12, TextMuted, TextAnchor.MiddleLeft, FontStyle.Bold);
            CreateText(recipeCard, "RECIPE / CYCLE", new Vector2(0.52f, 0.58f), new Vector2(0.95f, 0.92f), 12, TextMuted, TextAnchor.MiddleRight, FontStyle.Bold);
            currentStepText = CreateText(recipeCard, "Current Step", new Vector2(0.05f, 0.12f), new Vector2(0.52f, 0.60f), 22, TextPrimary, TextAnchor.MiddleLeft, FontStyle.Bold);
            recipeText = CreateText(recipeCard, "Recipe", new Vector2(0.50f, 0.12f), new Vector2(0.95f, 0.60f), 13, TextMuted, TextAnchor.MiddleRight);

            var telemetryCard = CreatePanel(panel, "Instrumentation Card", new Vector2(0.06f, 0.335f), new Vector2(0.94f, 0.595f), Surface);
            CreateText(telemetryCard, "PROCESS INSTRUMENTS", new Vector2(0.05f, 0.84f), new Vector2(0.95f, 0.98f), 13, TextMuted, TextAnchor.MiddleLeft, FontStyle.Bold);
            pressureInstrument = CreateInstrumentRow(telemetryCard, "Pressure", "mTorr", "normal 800-900", new Vector2(0.04f, 0.58f), new Vector2(0.96f, 0.82f), Primary);
            temperatureInstrument = CreateInstrumentRow(telemetryCard, "Temp", "C", "normal 245-255", new Vector2(0.04f, 0.32f), new Vector2(0.96f, 0.56f), Warning);
            filmInstrument = CreateInstrumentRow(telemetryCard, "Film", "A", "target progress", new Vector2(0.04f, 0.06f), new Vector2(0.96f, 0.30f), Success);

            var alarmCard = CreatePanel(panel, "Alarm Card", new Vector2(0.06f, 0.175f), new Vector2(0.94f, 0.315f), Surface);
            alarmCardImage = alarmCard.GetComponent<Image>();
            CreateText(alarmCard, "ALARM PRIORITY", new Vector2(0.05f, 0.72f), new Vector2(0.95f, 0.95f), 12, TextMuted, TextAnchor.MiddleLeft, FontStyle.Bold);
            alarmText = CreateText(alarmCard, "NO ALARM", new Vector2(0.05f, 0.39f), new Vector2(0.95f, 0.72f), 20, TextPrimary, TextAnchor.MiddleLeft, FontStyle.Bold);
            alarmText.verticalOverflow = VerticalWrapMode.Overflow;
            alarmText.horizontalOverflow = HorizontalWrapMode.Overflow;
            alarmDetailText = CreateText(alarmCard, "Interlocks nominal", new Vector2(0.05f, 0.13f), new Vector2(0.62f, 0.42f), 12, TextPrimary, TextAnchor.MiddleLeft);
            alarmCodeText = CreateText(alarmCard, "PRI 0 | CODE ----", new Vector2(0.60f, 0.13f), new Vector2(0.95f, 0.42f), 11, TextMuted, TextAnchor.MiddleRight, FontStyle.Bold);

            var operatorLogCard = CreatePanel(panel, "Operator Action Log Card", new Vector2(0.06f, 0.02f), new Vector2(0.94f, 0.16f), Surface);
            operatorActionLogCardImage = operatorLogCard.GetComponent<Image>();
            CreateText(operatorLogCard, "OPERATOR ACTION LOG", new Vector2(0.05f, 0.76f), new Vector2(0.95f, 0.98f), 11, TextMuted, TextAnchor.MiddleLeft, FontStyle.Bold);
            eventText = CreateText(operatorLogCard, "LIVE EVENT: -", new Vector2(0.05f, 0.56f), new Vector2(0.95f, 0.76f), 10, TextMuted, TextAnchor.MiddleLeft);

            operatorActionLogTexts = new Text[OperatorActionLogCapacity];
            operatorActionLogTexts[0] = CreateText(operatorLogCard, "-", new Vector2(0.05f, 0.37f), new Vector2(0.95f, 0.55f), 9, TextPrimary, TextAnchor.MiddleLeft);
            operatorActionLogTexts[1] = CreateText(operatorLogCard, "-", new Vector2(0.05f, 0.19f), new Vector2(0.95f, 0.37f), 9, TextMuted, TextAnchor.MiddleLeft);
            operatorActionLogTexts[2] = CreateText(operatorLogCard, "-", new Vector2(0.05f, 0.01f), new Vector2(0.95f, 0.19f), 9, TextMuted, TextAnchor.MiddleLeft);
            RecordOperatorAction("SYSTEM", "HMI ready");
        }

        private void BuildTimeline(Transform parent)
        {
            var panel = CreatePanel(
                parent,
                "Canvas Process Timeline",
                new Vector2(0.04f, 0.04f),
                new Vector2(0.96f, 0.17f),
                Background);

            CreateText(panel, "PROCESS TIMELINE", new Vector2(0.02f, 0.58f), new Vector2(0.22f, 0.94f), 15, TextMuted, TextAnchor.MiddleLeft);

            var track = CreatePanel(panel, "Timeline Track", new Vector2(0.02f, 0.12f), new Vector2(0.98f, 0.32f), SurfaceRaised);
            progressFill = CreatePanel(track, "Timeline Progress Fill", new Vector2(0f, 0f), new Vector2(0.01f, 1f), Primary);

            timelineLabels = new Text[TimelineSteps.Length];
            timelineChips = new Image[TimelineSteps.Length];

            const float startX = 0.23f;
            const float endX = 0.98f;
            var width = (endX - startX) / TimelineSteps.Length;
            for (var index = 0; index < TimelineSteps.Length; index++)
            {
                var minX = startX + width * index + 0.005f;
                var maxX = startX + width * (index + 1) - 0.005f;
                var chip = CreatePanel(panel, $"Timeline Chip {TimelineSteps[index]}", new Vector2(minX, 0.52f), new Vector2(maxX, 0.92f), SurfaceRaised);
                timelineChips[index] = chip.GetComponent<Image>();
                timelineLabels[index] = CreateText(chip, TimelineSteps[index], new Vector2(0.04f, 0.05f), new Vector2(0.96f, 0.95f), 13, TextPrimary, TextAnchor.MiddleCenter);
            }
        }

        private void ApplyVisualState(MolyAldVisualState visualState)
        {
            if (visualState == null)
            {
                return;
            }

            if (currentStepText != null)
            {
                currentStepText.text = DisplayStepName(visualState.StepName);
            }

            if (recipeText != null)
            {
                recipeText.text = $"{ShortRecipeName(visualState.RecipeName)}  |  Step {visualState.StepIndex}/{visualState.TotalSteps}  |  Cycle {CycleText(visualState)}";
            }

            if (hmiStateText != null)
            {
                hmiStateText.text = RunStateText(visualState);
                hmiStateText.color = RunStateColor(visualState);
            }

            UpdateInstruments(visualState);
            UpdateProcessSchematic(visualState);
            UpdateCommandButtons(visualState);

            if (alarmText != null)
            {
                alarmText.text = visualState.HasFault ? "ALARM ACTIVE" : "NO ALARM";
            }

            if (alarmDetailText != null)
            {
                alarmDetailText.text = visualState.HasFault
                    ? $"{ActiveFaultScenarioName()} | {FaultArea(visualState.StepName)} {FaultModeText()}"
                    : "Interlocks nominal";
            }

            if (alarmCodeText != null)
            {
                alarmCodeText.text = visualState.HasFault
                    ? $"PRI 1 | CODE {FaultCode(visualState.StepName)}"
                    : "PRI 0 | CODE ----";
                alarmCodeText.color = visualState.HasFault ? Warning : TextMuted;
            }

            if (alarmCardImage != null)
            {
                alarmCardImage.color = visualState.HasFault
                    ? Color.Lerp(SurfaceRaised, Alarm, 0.65f)
                    : Color.Lerp(Surface, Success, 0.12f);
            }

            if (operatorActionLogCardImage != null)
            {
                operatorActionLogCardImage.color = visualState.HasFault
                    ? Color.Lerp(Surface, Alarm, 0.28f)
                    : Surface;
            }

            if (eventText != null)
            {
                var playback = player != null && player.FaultTimelineReplayActive
                    ? "FAULT REPLAY"
                    : player != null && player.IsPlaying ? "RUN" : "HOLD";
                eventText.text = $"LIVE EVENT: {SplitCamelCase(visualState.StepName)} | {ActiveValveText(visualState)} valve | {playback}";
            }

            UpdateFaultScenarioText(visualState);
            UpdateTimeline(visualState);
        }

        private void UpdateProcessSchematic(MolyAldVisualState visualState)
        {
            if (schematicStepText != null)
            {
                schematicStepText.text = $"STEP: {SplitCamelCase(visualState.StepName)} | Valve: {ActiveValveText(visualState)} | Film {visualState.EstimatedThicknessAngstrom:0.##} A";
                schematicStepText.color = visualState.HasFault ? Alarm : TextPrimary;
            }

            if (schematicMetaText != null)
            {
                schematicMetaText.text = visualState.HasFault
                    ? "HELD | ALARM"
                    : player != null && !player.IsPlaying ? "PAUSED | READY" : "RUNNING | OK";
                schematicMetaText.color = visualState.HasFault
                    ? Alarm
                    : player != null && !player.IsPlaying ? Warning : Success;
            }

            SetValveState(
                precursorValveImage,
                precursorValveText,
                precursorLineImage,
                visualState.MetalPrecursorOpen,
                Precursor,
                "PRE");
            SetValveState(
                reactantValveImage,
                reactantValveText,
                reactantLineImage,
                visualState.ReactantOpen,
                Reactant,
                "RCT");
            SetValveState(
                purgeValveImage,
                purgeValveText,
                purgeLineImage,
                visualState.PurgeOpen,
                Purge,
                "PRG");

            UpdateGasFlowPulses(visualState);
            UpdateExhaustFlowPulses(visualState);

            if (chamberImage != null)
            {
                var faultPulse = FaultPulse();
                chamberImage.color = visualState.HasFault
                    ? Color.Lerp(SchematicGlass, Alarm, 0.35f + faultPulse * 0.35f)
                    : SchematicGlass;
            }

            if (showerheadImage != null)
            {
                showerheadImage.color = ActiveValveColor(visualState);
            }

            if (filmFillImage != null)
            {
                filmFillImage.color = visualState.ThicknessRatio >= 0.98f ? Success : Primary;
                SetHorizontalAnchors(filmFillImage.rectTransform, 0f, Mathf.Clamp01(visualState.ThicknessRatio));
            }

            if (exhaustLineImage != null)
            {
                exhaustLineImage.color = visualState.HasFault ? Color.Lerp(SchematicMetal, Alarm, 0.80f) : SchematicMetal;
            }

            if (gateValveImage != null)
            {
                gateValveImage.color = visualState.HasFault ? Color.Lerp(SurfaceRaised, Alarm, 0.70f) : SurfaceRaised;
            }

            if (pumpImage != null)
            {
                pumpImage.color = visualState.HasFault ? Color.Lerp(SurfaceRaised, Alarm, 0.70f) : SurfaceRaised;
            }
        }

        private void UpdateGasFlowPulses(MolyAldVisualState visualState)
        {
            var activeGas = visualState.MetalPrecursorOpen || visualState.ReactantOpen || visualState.PurgeOpen;
            var activeColor = ActiveValveColor(visualState);
            var activeX = ActiveValveX(visualState);
            var phase = Mathf.Repeat(Time.unscaledTime * 0.80f, 1f);

            for (var index = 0; index < gasFlowPulses.Length; index++)
            {
                var pulse = gasFlowPulses[index];
                if (pulse == null)
                {
                    continue;
                }

                pulse.enabled = activeGas || visualState.HasFault;
                if (!pulse.enabled)
                {
                    continue;
                }

                var local = Mathf.Repeat(phase + index / (float)gasFlowPulses.Length, 1f);
                var color = visualState.HasFault ? Alarm : activeColor;
                pulse.color = Color.Lerp(color, TextPrimary, index == 0 ? 0.25f : 0f);

                if (local < 0.48f)
                {
                    var y = Mathf.Lerp(0.68f, 0.61f, local / 0.48f);
                    SetRectAnchor(pulse.rectTransform, activeX, y, 0.010f, 0.022f);
                }
                else
                {
                    var x = Mathf.Lerp(0.34f, 0.65f, (local - 0.48f) / 0.52f);
                    SetRectAnchor(pulse.rectTransform, x, 0.535f, 0.014f, 0.014f);
                }
            }

            for (var index = 0; index < gasDistributionDots.Length; index++)
            {
                var dot = gasDistributionDots[index];
                if (dot == null)
                {
                    continue;
                }

                if (!activeGas && !visualState.HasFault)
                {
                    dot.color = Color.Lerp(SchematicMetal, SchematicGlass, 0.35f);
                    continue;
                }

                var pulse = 0.35f + 0.65f * Mathf.PingPong(Time.unscaledTime * 1.6f + index * 0.17f, 1f);
                dot.color = visualState.HasFault
                    ? Color.Lerp(SchematicMetal, Alarm, pulse)
                    : Color.Lerp(SchematicMetal, activeColor, pulse);
            }

            if (schematicFlowText != null)
            {
                schematicFlowText.text = visualState.HasFault
                    ? "FLOW: held by alarm"
                    : activeGas
                        ? $"FLOW: {ActiveValveText(visualState)} pulse -> chamber"
                        : "FLOW: idle / closed";
                schematicFlowText.color = visualState.HasFault ? Alarm : activeGas ? activeColor : TextMuted;
            }
        }

        private void UpdateExhaustFlowPulses(MolyAldVisualState visualState)
        {
            var pumpActive = IsPumpFlowActive(visualState);
            var phase = Mathf.Repeat(Time.unscaledTime * 0.65f, 1f);

            for (var index = 0; index < exhaustFlowPulses.Length; index++)
            {
                var pulse = exhaustFlowPulses[index];
                if (pulse == null)
                {
                    continue;
                }

                pulse.enabled = pumpActive || visualState.HasFault;
                if (!pulse.enabled)
                {
                    continue;
                }

                var local = Mathf.Repeat(phase + index / (float)exhaustFlowPulses.Length, 1f);
                var x = Mathf.Lerp(0.765f, 0.945f, local);
                SetRectAnchor(pulse.rectTransform, x, 0.386f, 0.024f, 0.016f);
                pulse.color = visualState.HasFault
                    ? Color.Lerp(Alarm, TextPrimary, 0.20f)
                    : Color.Lerp(Primary, TextPrimary, index == 0 ? 0.25f : 0f);
            }

            if (exhaustLineImage != null && pumpActive && !visualState.HasFault)
            {
                exhaustLineImage.color = Color.Lerp(SchematicMetal, Primary, 0.55f);
            }

            if (gateValveImage != null && pumpActive && !visualState.HasFault)
            {
                gateValveImage.color = Color.Lerp(SurfaceRaised, Primary, 0.35f);
            }

            if (pumpImage != null && pumpActive && !visualState.HasFault)
            {
                pumpImage.color = Color.Lerp(SurfaceRaised, Primary, 0.25f);
            }
        }

        private void UpdateInstruments(MolyAldVisualState visualState)
        {
            var pressureStatus = RangeStatus(visualState.ChamberPressureMtorr, 800f, 900f, "LOW", "OK", "HI");
            var pressureRatio = Mathf.Clamp01(visualState.ChamberPressureMtorr / 1000f);
            UpdateInstrument(
                pressureInstrument,
                visualState.ChamberPressureMtorr.ToString("0"),
                "mTorr",
                pressureStatus,
                "normal 800-900",
                pressureRatio,
                StatusColor(pressureStatus),
                0.80f,
                0.90f);

            var temperatureStatus = RangeStatus(visualState.WaferTemperatureC, 245f, 255f, "COOL", "OK", "HOT");
            var temperatureRatio = Mathf.Clamp01(visualState.WaferTemperatureC / 300f);
            UpdateInstrument(
                temperatureInstrument,
                visualState.WaferTemperatureC.ToString("0"),
                "C",
                temperatureStatus,
                "normal 245-255",
                temperatureRatio,
                StatusColor(temperatureStatus),
                245f / 300f,
                255f / 300f);

            var filmStatus = visualState.ThicknessRatio >= 0.98f ? "TARGET" : visualState.ThicknessRatio > 0.05f ? "GROWING" : "WAIT";
            UpdateInstrument(
                filmInstrument,
                visualState.EstimatedThicknessAngstrom.ToString("0.##"),
                "A",
                filmStatus,
                "target 100%",
                Mathf.Clamp01(visualState.ThicknessRatio),
                filmStatus == "TARGET" ? Success : Primary,
                0.92f,
                1.00f);

            // Valve state is kept in the event line for now. The instrument card stays focused on numeric process variables.
        }

        private static void SetValveState(Image valveImage, Text valveText, Image lineImage, bool isOpen, Color activeColor, string label)
        {
            if (valveImage != null)
            {
                valveImage.color = isOpen ? activeColor : SurfaceRaised;
            }

            if (valveText != null)
            {
                valveText.text = isOpen ? $"{label}\nON" : $"{label}\nOFF";
                valveText.color = isOpen ? Background : TextMuted;
                valveText.fontStyle = isOpen ? FontStyle.Bold : FontStyle.Normal;
            }

            if (lineImage != null)
            {
                lineImage.color = isOpen ? activeColor : SchematicMetal;
            }
        }

        private static void SetRectAnchor(RectTransform rectTransform, float centerX, float centerY, float width, float height)
        {
            rectTransform.anchorMin = new Vector2(centerX - width * 0.5f, centerY - height * 0.5f);
            rectTransform.anchorMax = new Vector2(centerX + width * 0.5f, centerY + height * 0.5f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        private static float ActiveValveX(MolyAldVisualState visualState)
        {
            if (visualState.MetalPrecursorOpen)
            {
                return 0.299f;
            }

            if (visualState.ReactantOpen)
            {
                return 0.499f;
            }

            if (visualState.PurgeOpen)
            {
                return 0.689f;
            }

            return 0.499f;
        }

        private static bool IsPumpFlowActive(MolyAldVisualState visualState)
        {
            if (visualState.HasFault)
            {
                return true;
            }

            var stepName = visualState.StepName ?? string.Empty;
            return
                stepName.IndexOf("Pump", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                stepName.IndexOf("Purge", System.StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static float FaultPulse()
        {
            return 0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * 7.0f);
        }

        private void UpdateTimeline(MolyAldVisualState visualState)
        {
            var activeIndex = ProcessFlowIndex(visualState.StepName);
            for (var index = 0; index < timelineChips.Length; index++)
            {
                var isActive = index == activeIndex;
                var isPast = activeIndex >= 0 && index < activeIndex;
                if (timelineChips[index] != null)
                {
                    timelineChips[index].color = isActive ? Primary : isPast ? Success : SurfaceRaised;
                }

                if (timelineLabels[index] != null)
                {
                    timelineLabels[index].color = isActive || isPast ? TextPrimary : TextMuted;
                    timelineLabels[index].fontStyle = isActive ? FontStyle.Bold : FontStyle.Normal;
                }
            }

            if (progressFill != null && visualState.TotalSteps > 0)
            {
                var ratio = Mathf.Clamp01(visualState.StepIndex / (float)visualState.TotalSteps);
                progressFill.anchorMax = new Vector2(Mathf.Max(0.01f, ratio), 1f);
                progressFill.offsetMin = Vector2.zero;
                progressFill.offsetMax = Vector2.zero;
            }
        }

        private InstrumentView CreateInstrumentRow(
            Transform parent,
            string label,
            string unit,
            string rangeLabel,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Color accent)
        {
            var row = CreatePanel(parent, $"{label} Instrument Row", anchorMin, anchorMax, SurfaceRaised);
            CreateText(row, label.ToUpperInvariant(), new Vector2(0.03f, 0.52f), new Vector2(0.25f, 0.96f), 12, TextMuted, TextAnchor.MiddleLeft, FontStyle.Bold);

            var valueText = CreateText(row, string.Empty, new Vector2(0.28f, 0.48f), new Vector2(0.32f, 0.96f), 10, TextPrimary, TextAnchor.MiddleRight, FontStyle.Bold);

            var unitText = CreateText(row, unit, new Vector2(0.31f, 0.48f), new Vector2(0.66f, 0.96f), 18, TextPrimary, TextAnchor.MiddleRight, FontStyle.Bold);
            unitText.horizontalOverflow = HorizontalWrapMode.Overflow;
            unitText.verticalOverflow = VerticalWrapMode.Overflow;
            var statusText = CreateText(row, "OK", new Vector2(0.74f, 0.50f), new Vector2(0.96f, 0.96f), 14, accent, TextAnchor.MiddleRight, FontStyle.Bold);
            statusText.verticalOverflow = VerticalWrapMode.Overflow;
            var rangeText = CreateText(row, rangeLabel, new Vector2(0.03f, 0.05f), new Vector2(0.34f, 0.38f), 10, TextMuted, TextAnchor.MiddleLeft);

            var track = CreatePanel(row, $"{label} Range Track", new Vector2(0.36f, 0.12f), new Vector2(0.96f, 0.30f), GaugeTrack);
            var normalBand = CreatePanel(track, $"{label} Normal Range Band", new Vector2(0f, 0f), new Vector2(1f, 1f), NormalBand);
            var fill = CreatePanel(track, $"{label} Actual Value Fill", new Vector2(0f, 0f), new Vector2(0.01f, 1f), accent);

            var dot = CreatePanel(row, $"{label} Status Dot", new Vector2(0.69f, 0.62f), new Vector2(0.71f, 0.84f), accent);

            return new InstrumentView(valueText, unitText, statusText, rangeText, fill.GetComponent<Image>(), normalBand.GetComponent<Image>(), dot.GetComponent<Image>());
        }

        private void UpdateInstrument(
            InstrumentView instrument,
            string value,
            string unit,
            string status,
            string rangeLabel,
            float fillRatio,
            Color statusColor,
            float normalMin,
            float normalMax)
        {
            if (instrument == null)
            {
                return;
            }

            if (instrument.ValueText != null)
            {
                instrument.ValueText.text = string.Empty;
            }

            if (instrument.UnitText != null)
            {
                instrument.UnitText.text = string.IsNullOrWhiteSpace(unit)
                    ? value
                    : $"{value} {unit}";
            }

            if (instrument.StatusText != null)
            {
                instrument.StatusText.text = status;
                instrument.StatusText.color = statusColor;
            }

            if (instrument.RangeText != null)
            {
                instrument.RangeText.text = rangeLabel;
            }

            if (instrument.FillImage != null)
            {
                instrument.FillImage.color = statusColor;
                SetHorizontalAnchors(instrument.FillImage.rectTransform, 0f, Mathf.Clamp01(fillRatio));
            }

            if (instrument.NormalBandImage != null)
            {
                SetHorizontalAnchors(instrument.NormalBandImage.rectTransform, Mathf.Clamp01(normalMin), Mathf.Clamp01(normalMax));
            }

            if (instrument.StatusDotImage != null)
            {
                instrument.StatusDotImage.color = statusColor;
            }
        }

        private static void SetHorizontalAnchors(RectTransform rectTransform, float minX, float maxX)
        {
            rectTransform.anchorMin = new Vector2(minX, 0f);
            rectTransform.anchorMax = new Vector2(Mathf.Max(minX + 0.01f, maxX), 1f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        private RectTransform CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            var gameObject = new GameObject(name);
            gameObject.transform.SetParent(parent, false);
            var rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            var image = gameObject.AddComponent<Image>();
            image.color = color;
            return rectTransform;
        }

        private Text CreateText(Transform parent, string value, Vector2 anchorMin, Vector2 anchorMax, int fontSize, Color color, TextAnchor anchor, FontStyle fontStyle = FontStyle.Normal)
        {
            var gameObject = new GameObject($"{value} Text");
            gameObject.transform.SetParent(parent, false);
            var rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            var text = gameObject.AddComponent<Text>();
            text.text = value;
            text.font = ResolveFont();
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = anchor;
            text.fontStyle = fontStyle;
            text.lineSpacing = 0.92f;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
        }

        private Button CreateCommandButton(
            Transform parent,
            string label,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Color color,
            Color textColor,
            System.Action onClick,
            out Image buttonImage,
            out Text buttonText)
        {
            var buttonRect = CreatePanel(parent, $"{label} Button", anchorMin, anchorMax, color);
            buttonImage = buttonRect.GetComponent<Image>();
            buttonText = CreateText(buttonRect, label, new Vector2(0f, 0f), new Vector2(1f, 1f), 14, textColor, TextAnchor.MiddleCenter, FontStyle.Bold);

            var button = buttonRect.gameObject.AddComponent<Button>();
            button.targetGraphic = buttonImage;
            button.transition = Selectable.Transition.None;

            if (onClick != null)
            {
                button.onClick.AddListener(() => onClick());
            }

            return button;
        }

        private void OnStartClicked()
        {
            if (!TryResolvePlayer())
            {
                return;
            }

            var wasFaultHeld = player.OperatorFaultActive;
            player.Play();
            RecordOperatorAction(
                player.IsPlaying ? "START" : "START BLOCKED",
                wasFaultHeld ? "fault held - reset required" : "timeline running");
            RefreshCanvas();
        }

        private void OnPauseClicked()
        {
            if (!TryResolvePlayer())
            {
                return;
            }

            player.Pause();
            RecordOperatorAction("STOP", "timeline held by operator");
            RefreshCanvas();
        }

        private void OnFaultClicked()
        {
            if (!TryResolvePlayer())
            {
                return;
            }

            player.ToggleOperatorFault();
            RecordOperatorAction(
                player.OperatorFaultActive ? "FAULT" : "FAULT CLEAR",
                player.OperatorFaultActive ? FaultActionDetail() : "override cleared");
            RefreshCanvas();
        }

        private void OnResetClicked()
        {
            if (!TryResolvePlayer())
            {
                return;
            }

            player.ResetToStart();
            RecordOperatorAction("RESET", "fault cleared, returned to first step");
            RefreshCanvas();
        }

        private bool TryResolvePlayer()
        {
            if (player == null)
            {
                player = GetComponent<MolyAldProcessPlayer>();
            }

            return player != null;
        }

        private void EnsureEventSystem()
        {
            if (FindObjectOfType<EventSystem>() != null)
            {
                return;
            }

            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        private void UpdateCommandButtons(MolyAldVisualState visualState)
        {
            var playing = player != null && player.IsPlaying;
            var fault = visualState != null && visualState.HasFault;
            var replay = player != null && player.FaultTimelineReplayActive;
            var faultLabel = fault
                ? (replay ? "FAULT\nREPLAY" : "FAULT\nACTIVE")
                : "FAULT";

            SetCommandButton(startButtonImage, startButtonText, playing ? "RUNNING" : "START", playing ? Color.Lerp(Success, TextPrimary, 0.20f) : Success, Background);
            SetCommandButton(stopButtonImage, stopButtonText, playing ? "STOP" : "PAUSED", playing ? Stop : Color.Lerp(NeutralButton, Warning, 0.30f), TextPrimary);
            SetCommandButton(faultButtonImage, faultButtonText, faultLabel, fault ? Alarm : Warning, fault ? TextPrimary : Background);
            SetCommandButton(resetButtonImage, resetButtonText, "RESET", NeutralButton, TextPrimary);
        }

        private void UpdateFaultScenarioText(MolyAldVisualState visualState)
        {
            if (faultScenarioText == null)
            {
                return;
            }

            var scenarioName = SelectedFaultScenarioName();
            var isFault = visualState != null && visualState.HasFault;
            var isReplay = player != null && player.FaultTimelineReplayActive;
            faultScenarioText.text = isFault
                ? $"{(isReplay ? "REPLAYED" : "ACTIVE")} FAULT SCENARIO: {ActiveFaultScenarioName()}"
                : $"SELECTED FAULT SCENARIO: {scenarioName}";
            faultScenarioText.color = isFault ? Alarm : Warning;
        }

        private static void SetCommandButton(Image image, Text text, string label, Color backgroundColor, Color textColor)
        {
            if (image != null)
            {
                image.color = backgroundColor;
            }

            if (text != null)
            {
                text.text = label;
                text.color = textColor;
            }
        }

        private void RefreshOperatorActionLog()
        {
            for (var index = 0; index < operatorActionLogTexts.Length; index++)
            {
                var text = operatorActionLogTexts[index];
                if (text == null)
                {
                    continue;
                }

                var hasEntry = index < operatorActionLogEntryCount && !string.IsNullOrWhiteSpace(operatorActionLogEntries[index]);
                text.text = hasEntry ? operatorActionLogEntries[index] : "-";
                text.color = hasEntry && index == 0 ? TextPrimary : TextMuted;
                text.fontStyle = hasEntry && index == 0 ? FontStyle.Bold : FontStyle.Normal;
            }
        }

        private string RunStateText(MolyAldVisualState visualState)
        {
            if (visualState != null && visualState.HasFault)
            {
                return "HELD  |  OPERATOR ACTION REQUIRED";
            }

            return player != null && !player.IsPlaying
                ? "PAUSED  |  READY"
                : "RUNNING  |  INTERLOCK OK";
        }

        private Color RunStateColor(MolyAldVisualState visualState)
        {
            if (visualState != null && visualState.HasFault)
            {
                return Alarm;
            }

            return player != null && !player.IsPlaying ? Warning : Success;
        }

        private string SelectedFaultScenarioName()
        {
            return player != null ? player.SelectedFaultScenarioName : "precursor-dose-timeout";
        }

        private string ActiveFaultScenarioName()
        {
            if (player == null || string.IsNullOrWhiteSpace(player.ActiveFaultScenarioName))
            {
                return SelectedFaultScenarioName();
            }

            return player.ActiveFaultScenarioName;
        }

        private string FaultModeText()
        {
            return player != null && player.FaultTimelineReplayActive ? "replay" : "hold";
        }

        private string FaultActionDetail()
        {
            if (player == null)
            {
                return $"{SelectedFaultScenarioName()} selected";
            }

            if (!string.IsNullOrWhiteSpace(player.FaultReplayError))
            {
                return $"{ActiveFaultScenarioName()} fallback hold: {player.FaultReplayError}";
            }

            if (player.FaultTimelineReplayActive && player.CurrentStep != null)
            {
                return $"{ActiveFaultScenarioName()} replay {DisplayStepName(player.CurrentStep.step)}";
            }

            return $"{ActiveFaultScenarioName()} selected";
        }

        private Camera ResolveCamera()
        {
            if (Camera.main != null)
            {
                return Camera.main;
            }

            return FindObjectOfType<Camera>();
        }

        private static Font ResolveFont()
        {
            return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }

        private static int ProcessFlowIndex(string stepName)
        {
            if (string.IsNullOrWhiteSpace(stepName))
            {
                return -1;
            }

            if (stepName.IndexOf("Load", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return 0;
            }

            if (stepName.IndexOf("Pump", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return 1;
            }

            if (stepName.IndexOf("Temperature", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                stepName.IndexOf("Stabilize", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return 2;
            }

            if (stepName.IndexOf("Precursor", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return 3;
            }

            if (stepName.IndexOf("Purge", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return 4;
            }

            if (stepName.IndexOf("Reactant", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return 5;
            }

            if (stepName.IndexOf("Complete", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                stepName.IndexOf("Transfer", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return 6;
            }

            return -1;
        }

        private static string CycleText(MolyAldVisualState visualState)
        {
            if (!visualState.HasCycle)
            {
                return "-";
            }

            return visualState.CycleCount > 0
                ? $"{visualState.Cycle}/{visualState.CycleCount}"
                : visualState.Cycle.ToString();
        }

        private static string ActiveValveText(MolyAldVisualState visualState)
        {
            if (visualState.MetalPrecursorOpen)
            {
                return "Precursor";
            }

            if (visualState.ReactantOpen)
            {
                return "Reactant";
            }

            if (visualState.PurgeOpen)
            {
                return "Purge";
            }

            return "None";
        }

        private static Color ActiveValveColor(MolyAldVisualState visualState)
        {
            if (visualState.MetalPrecursorOpen)
            {
                return Precursor;
            }

            if (visualState.ReactantOpen)
            {
                return Reactant;
            }

            if (visualState.PurgeOpen)
            {
                return Purge;
            }

            return SchematicMetal;
        }

        private static string RangeStatus(float value, float low, float high, string lowLabel, string normalLabel, string highLabel)
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

        private static Color StatusColor(string status)
        {
            if (status == "OK" || status == "TARGET" || status == "OPEN")
            {
                return Success;
            }

            if (status == "GROWING" || status == "WAIT" || status == "CLOSED")
            {
                return Primary;
            }

            if (status == "LOW" || status == "COOL")
            {
                return Warning;
            }

            return Alarm;
        }

        private static string FaultCode(string stepName)
        {
            if (string.IsNullOrWhiteSpace(stepName))
            {
                return "ALD-UNK";
            }

            if (stepName.IndexOf("Pump", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "VAC-101";
            }

            if (stepName.IndexOf("Temperature", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                stepName.IndexOf("Stabilize", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "TMP-201";
            }

            if (stepName.IndexOf("Precursor", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                stepName.IndexOf("Reactant", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                stepName.IndexOf("Purge", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "GAS-301";
            }

            return "SEQ-001";
        }

        private static string FaultArea(string stepName)
        {
            if (string.IsNullOrWhiteSpace(stepName))
            {
                return "Unknown";
            }

            if (stepName.IndexOf("Pump", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Pump / exhaust";
            }

            if (stepName.IndexOf("Temperature", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                stepName.IndexOf("Stabilize", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Thermal chamber";
            }

            if (stepName.IndexOf("Precursor", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                stepName.IndexOf("Reactant", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                stepName.IndexOf("Purge", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Gas delivery";
            }

            return "Process module";
        }

        private static string ShortRecipeName(string recipeName)
        {
            if (string.IsNullOrWhiteSpace(recipeName))
            {
                return "public synthetic ALD";
            }

            return recipeName.Replace("public-", string.Empty).Replace("-demo", string.Empty);
        }

        private static string SplitCamelCase(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "-";
            }

            return System.Text.RegularExpressions.Regex.Replace(value, "([a-z])([A-Z])", "$1 $2");
        }

        private static string DisplayStepName(string stepName)
        {
            if (string.Equals(stepName, "DoseMetalPrecursor", StringComparison.OrdinalIgnoreCase))
            {
                return "Dose Precursor";
            }

            if (string.Equals(stepName, "PurgeAfterPrecursor", StringComparison.OrdinalIgnoreCase))
            {
                return "Purge Precursor";
            }

            if (string.Equals(stepName, "PurgeAfterReactant", StringComparison.OrdinalIgnoreCase))
            {
                return "Purge Reactant";
            }

            if (string.Equals(stepName, "StabilizeTemperature", StringComparison.OrdinalIgnoreCase))
            {
                return "Stabilize Temp";
            }

            return SplitCamelCase(stepName);
        }

        private static Color Hex(byte red, byte green, byte blue)
        {
            return new Color(red / 255f, green / 255f, blue / 255f);
        }

        private sealed class InstrumentView
        {
            public InstrumentView(
                Text valueText,
                Text unitText,
                Text statusText,
                Text rangeText,
                Image fillImage,
                Image normalBandImage,
                Image statusDotImage)
            {
                ValueText = valueText;
                UnitText = unitText;
                StatusText = statusText;
                RangeText = rangeText;
                FillImage = fillImage;
                NormalBandImage = normalBandImage;
                StatusDotImage = statusDotImage;
            }

            public Text ValueText { get; }

            public Text UnitText { get; }

            public Text StatusText { get; }

            public Text RangeText { get; }

            public Image FillImage { get; }

            public Image NormalBandImage { get; }

            public Image StatusDotImage { get; }
        }
    }
}
