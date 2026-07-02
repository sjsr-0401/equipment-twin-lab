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
        private Text telemetryText;
        private Text alarmText;
        private Text eventText;
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
                new Vector2(0.66f, 0.18f),
                new Vector2(0.97f, 0.93f),
                Background);

            CreateText(panel, "Operator Interface", new Vector2(0.06f, 0.91f), new Vector2(0.94f, 0.99f), 28, TextPrimary, TextAnchor.MiddleCenter);

            CreateCommandButton(panel, "START", new Vector2(0.07f, 0.82f), new Vector2(0.28f, 0.89f), Success);
            CreateCommandButton(panel, "STOP", new Vector2(0.31f, 0.82f), new Vector2(0.52f, 0.89f), Warning);
            CreateCommandButton(panel, "FAULT", new Vector2(0.55f, 0.82f), new Vector2(0.76f, 0.89f), Precursor);
            CreateCommandButton(panel, "RESET", new Vector2(0.79f, 0.82f), new Vector2(0.98f, 0.89f), Reactant);

            var recipeCard = CreatePanel(panel, "Recipe Card", new Vector2(0.06f, 0.58f), new Vector2(0.94f, 0.79f), SurfaceRaised);
            CreateText(recipeCard, "RECIPE", new Vector2(0.05f, 0.68f), new Vector2(0.95f, 0.94f), 16, TextMuted, TextAnchor.MiddleLeft);
            currentStepText = CreateText(recipeCard, "Current Step", new Vector2(0.05f, 0.22f), new Vector2(0.95f, 0.72f), 28, TextPrimary, TextAnchor.MiddleLeft);
            recipeText = CreateText(recipeCard, "Recipe", new Vector2(0.05f, 0.04f), new Vector2(0.95f, 0.23f), 14, TextMuted, TextAnchor.MiddleLeft);

            var telemetryCard = CreatePanel(panel, "Telemetry Card", new Vector2(0.06f, 0.31f), new Vector2(0.94f, 0.54f), Surface);
            CreateText(telemetryCard, "LIVE TELEMETRY", new Vector2(0.05f, 0.74f), new Vector2(0.95f, 0.95f), 16, TextMuted, TextAnchor.MiddleLeft);
            telemetryText = CreateText(telemetryCard, "Telemetry", new Vector2(0.05f, 0.08f), new Vector2(0.95f, 0.76f), 20, TextPrimary, TextAnchor.MiddleLeft);

            var alarmCard = CreatePanel(panel, "Alarm Card", new Vector2(0.06f, 0.12f), new Vector2(0.94f, 0.27f), Surface);
            alarmCardImage = alarmCard.GetComponent<Image>();
            alarmText = CreateText(alarmCard, "NO ALARM", new Vector2(0.05f, 0.10f), new Vector2(0.95f, 0.90f), 24, TextPrimary, TextAnchor.MiddleLeft);

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

            if (telemetryText != null)
            {
                telemetryText.text =
                    $"Pressure   {visualState.ChamberPressureMtorr:0.#} mTorr\n" +
                    $"Temp       {visualState.WaferTemperatureC:0.#} C\n" +
                    $"Film       {visualState.EstimatedThicknessAngstrom:0.###} A\n" +
                    $"Valve      {ActiveValveText(visualState)}";
            }

            if (alarmText != null)
            {
                alarmText.text = visualState.HasFault
                    ? $"ALARM ACTIVE\n{FaultArea(visualState.StepName)}\nHold sequence"
                    : "NO ALARM\nInterlocks nominal";
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

        private Text CreateText(Transform parent, string value, Vector2 anchorMin, Vector2 anchorMax, int fontSize, Color color, TextAnchor anchor)
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
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
        }

        private void CreateCommandButton(Transform parent, string label, Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            var button = CreatePanel(parent, $"{label} Button", anchorMin, anchorMax, color);
            CreateText(button, label, new Vector2(0f, 0f), new Vector2(1f, 1f), 14, Background, TextAnchor.MiddleCenter);
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
    }
}
