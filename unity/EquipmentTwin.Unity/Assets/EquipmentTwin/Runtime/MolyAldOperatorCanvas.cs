using UnityEngine;
using UnityEngine.UI;

namespace EquipmentTwin.Unity.Processes
{
    [DisallowMultipleComponent]
    public sealed class MolyAldOperatorCanvas : MonoBehaviour
    {
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
        private Text eventText;
        private Text hmiStateText;
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
                processTemperatureC);
            ApplyVisualState(visualState);
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

            CreateCommandButton(panel, "START", new Vector2(0.06f, 0.745f), new Vector2(0.28f, 0.805f), Success, Background);
            CreateCommandButton(panel, "STOP", new Vector2(0.305f, 0.745f), new Vector2(0.525f, 0.805f), Stop, TextPrimary);
            CreateCommandButton(panel, "FAULT", new Vector2(0.55f, 0.745f), new Vector2(0.77f, 0.805f), Warning, Background);
            CreateCommandButton(panel, "RESET", new Vector2(0.795f, 0.745f), new Vector2(0.94f, 0.805f), NeutralButton, TextPrimary);

            var recipeCard = CreatePanel(panel, "Recipe Card", new Vector2(0.06f, 0.615f), new Vector2(0.94f, 0.725f), SurfaceRaised);
            CreateText(recipeCard, "CURRENT STEP", new Vector2(0.05f, 0.58f), new Vector2(0.48f, 0.92f), 12, TextMuted, TextAnchor.MiddleLeft, FontStyle.Bold);
            CreateText(recipeCard, "RECIPE / CYCLE", new Vector2(0.52f, 0.58f), new Vector2(0.95f, 0.92f), 12, TextMuted, TextAnchor.MiddleRight, FontStyle.Bold);
            currentStepText = CreateText(recipeCard, "Current Step", new Vector2(0.05f, 0.12f), new Vector2(0.52f, 0.60f), 22, TextPrimary, TextAnchor.MiddleLeft, FontStyle.Bold);
            recipeText = CreateText(recipeCard, "Recipe", new Vector2(0.50f, 0.12f), new Vector2(0.95f, 0.60f), 13, TextMuted, TextAnchor.MiddleRight);

            var telemetryCard = CreatePanel(panel, "Instrumentation Card", new Vector2(0.06f, 0.315f), new Vector2(0.94f, 0.595f), Surface);
            CreateText(telemetryCard, "PROCESS INSTRUMENTS", new Vector2(0.05f, 0.84f), new Vector2(0.95f, 0.98f), 13, TextMuted, TextAnchor.MiddleLeft, FontStyle.Bold);
            pressureInstrument = CreateInstrumentRow(telemetryCard, "Pressure", "mTorr", "normal 800-900", new Vector2(0.04f, 0.58f), new Vector2(0.96f, 0.82f), Primary);
            temperatureInstrument = CreateInstrumentRow(telemetryCard, "Temp", "C", "normal 245-255", new Vector2(0.04f, 0.32f), new Vector2(0.96f, 0.56f), Warning);
            filmInstrument = CreateInstrumentRow(telemetryCard, "Film", "A", "target progress", new Vector2(0.04f, 0.06f), new Vector2(0.96f, 0.30f), Success);

            var alarmCard = CreatePanel(panel, "Alarm Card", new Vector2(0.06f, 0.12f), new Vector2(0.94f, 0.295f), Surface);
            alarmCardImage = alarmCard.GetComponent<Image>();
            CreateText(alarmCard, "ALARM PRIORITY", new Vector2(0.05f, 0.72f), new Vector2(0.95f, 0.95f), 12, TextMuted, TextAnchor.MiddleLeft, FontStyle.Bold);
            alarmText = CreateText(alarmCard, "NO ALARM", new Vector2(0.05f, 0.42f), new Vector2(0.95f, 0.76f), 25, TextPrimary, TextAnchor.MiddleLeft, FontStyle.Bold);
            alarmDetailText = CreateText(alarmCard, "Interlocks nominal", new Vector2(0.05f, 0.18f), new Vector2(0.62f, 0.45f), 15, TextPrimary, TextAnchor.MiddleLeft);
            alarmCodeText = CreateText(alarmCard, "PRI 0 | CODE ----", new Vector2(0.60f, 0.18f), new Vector2(0.95f, 0.45f), 13, TextMuted, TextAnchor.MiddleRight, FontStyle.Bold);

            eventText = CreateText(panel, "Event", new Vector2(0.07f, 0.03f), new Vector2(0.94f, 0.10f), 14, TextMuted, TextAnchor.MiddleLeft);
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
                currentStepText.text = SplitCamelCase(visualState.StepName);
            }

            if (recipeText != null)
            {
                recipeText.text = $"{ShortRecipeName(visualState.RecipeName)}  |  Step {visualState.StepIndex}/{visualState.TotalSteps}  |  Cycle {CycleText(visualState)}";
            }

            if (hmiStateText != null)
            {
                hmiStateText.text = visualState.HasFault
                    ? "HELD  |  OPERATOR ACTION REQUIRED"
                    : "RUNNING  |  INTERLOCK OK";
                hmiStateText.color = visualState.HasFault ? Alarm : Success;
            }

            UpdateInstruments(visualState);

            if (alarmText != null)
            {
                alarmText.text = visualState.HasFault ? "ALARM ACTIVE" : "NO ALARM";
            }

            if (alarmDetailText != null)
            {
                alarmDetailText.text = visualState.HasFault
                    ? $"{FaultArea(visualState.StepName)} fault | hold sequence"
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

            if (eventText != null)
            {
                eventText.text = $"EVENT: {SplitCamelCase(visualState.StepName)} | {ActiveValveText(visualState)} valve";
            }

            UpdateTimeline(visualState);
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

        private void CreateCommandButton(Transform parent, string label, Vector2 anchorMin, Vector2 anchorMax, Color color, Color textColor)
        {
            var button = CreatePanel(parent, $"{label} Button", anchorMin, anchorMax, color);
            CreateText(button, label, new Vector2(0f, 0f), new Vector2(1f, 1f), 14, textColor, TextAnchor.MiddleCenter, FontStyle.Bold);
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
