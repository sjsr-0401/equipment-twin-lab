using UnityEngine;

namespace EquipmentTwin.Unity.Processes
{
    [DisallowMultipleComponent]
    public sealed class MolyAldPrimitiveVisualizer : MonoBehaviour
    {
        [Header("Source")]
        [SerializeField] private MolyAldProcessPlayer player;

        [Header("Auto scene")]
        [SerializeField] private bool autoCreateScene = true;
        [SerializeField] private bool createLabels = true;
        [SerializeField] private Transform sceneRoot;

        [Header("Pressure mapping")]
        [SerializeField] private float processPressureMtorr = 850f;
        [SerializeField] private float atmospherePressureMtorr = 760000f;

        [Header("Temperature mapping")]
        [SerializeField] private float roomTemperatureC = 25f;
        [SerializeField] private float processTemperatureC = 250f;

        [Header("Generated equipment visuals")]
        [SerializeField] private Renderer floorRenderer;
        [SerializeField] private Renderer backWallRenderer;
        [SerializeField] private Renderer mainCabinetRenderer;
        [SerializeField] private Renderer cabinetWindowRenderer;
        [SerializeField] private Renderer loadPortRenderer;
        [SerializeField] private Renderer transferRobotRenderer;
        [SerializeField] private Renderer chamberRenderer;
        [SerializeField] private Renderer waferRenderer;
        [SerializeField] private Renderer filmRenderer;
        [SerializeField] private Renderer pumpRenderer;
        [SerializeField] private Renderer exhaustLineRenderer;
        [SerializeField] private Renderer gasCabinetRenderer;
        [SerializeField] private Renderer precursorBottleRenderer;
        [SerializeField] private Renderer reactantBottleRenderer;
        [SerializeField] private Renderer purgeBottleRenderer;
        [SerializeField] private Renderer precursorLineRenderer;
        [SerializeField] private Renderer reactantLineRenderer;
        [SerializeField] private Renderer purgeLineRenderer;
        [SerializeField] private Renderer precursorValveRenderer;
        [SerializeField] private Renderer reactantValveRenderer;
        [SerializeField] private Renderer purgeValveRenderer;
        [SerializeField] private Renderer alarmBeaconRenderer;
        [SerializeField] private Renderer vacuumGaugeRenderer;
        [SerializeField] private Transform pressureNeedle;

        [Header("Generated operator interface")]
        [SerializeField] private Renderer operatorPanelRenderer;
        [SerializeField] private Renderer recipeCardRenderer;
        [SerializeField] private Renderer telemetryCardRenderer;
        [SerializeField] private Renderer alarmCardRenderer;
        [SerializeField] private Renderer eventLogCardRenderer;
        [SerializeField] private Renderer startButtonRenderer;
        [SerializeField] private Renderer stopButtonRenderer;
        [SerializeField] private Renderer faultButtonRenderer;
        [SerializeField] private Renderer resetButtonRenderer;
        [SerializeField] private Renderer processFlowPanelRenderer;
        [SerializeField] private Renderer progressTrackRenderer;
        [SerializeField] private Renderer progressFillRenderer;
        [SerializeField] private Renderer[] processFlowRenderers = new Renderer[0];

        [Header("Generated labels")]
        [SerializeField] private TextMesh titleLabel;
        [SerializeField] private TextMesh subtitleLabel;
        [SerializeField] private TextMesh equipmentLabel;
        [SerializeField] private TextMesh chamberLabel;
        [SerializeField] private TextMesh loadPortLabel;
        [SerializeField] private TextMesh gasPanelLabel;
        [SerializeField] private TextMesh pumpLabel;
        [SerializeField] private TextMesh operatorTitleLabel;
        [SerializeField] private TextMesh recipeLabel;
        [SerializeField] private TextMesh telemetryLabel;
        [SerializeField] private TextMesh alarmLabel;
        [SerializeField] private TextMesh buttonLabel;
        [SerializeField] private TextMesh processFlowLabel;
        [SerializeField] private TextMesh eventLogLabel;
        [SerializeField] private TextMesh architectureLabel;

        private static readonly Color Background = new Color(0.035f, 0.042f, 0.055f);
        private static readonly Color Floor = new Color(0.055f, 0.065f, 0.08f);
        private static readonly Color Wall = new Color(0.075f, 0.088f, 0.11f);
        private static readonly Color CabinetDark = new Color(0.12f, 0.135f, 0.16f);
        private static readonly Color CabinetMid = new Color(0.20f, 0.23f, 0.27f);
        private static readonly Color CabinetTrim = new Color(0.44f, 0.51f, 0.60f);
        private static readonly Color Glass = new Color(0.05f, 0.20f, 0.30f, 0.82f);
        private static readonly Color Panel = new Color(0.025f, 0.032f, 0.045f);
        private static readonly Color PanelCard = new Color(0.055f, 0.067f, 0.085f);
        private static readonly Color TextColor = new Color(0.92f, 0.95f, 0.98f);
        private static readonly Color MutedTextColor = new Color(0.62f, 0.70f, 0.78f);
        private static readonly Color ChamberAtAtmosphere = new Color(0.34f, 0.36f, 0.40f);
        private static readonly Color ChamberAtVacuum = new Color(0.10f, 0.29f, 0.58f);
        private static readonly Color WaferCold = new Color(0.60f, 0.64f, 0.70f);
        private static readonly Color WaferHot = new Color(1.0f, 0.45f, 0.12f);
        private static readonly Color FilmLow = new Color(0.18f, 0.62f, 1.0f);
        private static readonly Color FilmFull = new Color(1.0f, 0.82f, 0.16f);
        private static readonly Color ValveOff = new Color(0.16f, 0.17f, 0.18f);
        private static readonly Color PrecursorOn = new Color(0.96f, 0.58f, 0.14f);
        private static readonly Color ReactantOn = new Color(0.24f, 0.78f, 1.0f);
        private static readonly Color PurgeOn = new Color(0.22f, 0.90f, 0.38f);
        private static readonly Color Alarm = new Color(0.96f, 0.08f, 0.10f);
        private static readonly Color Safe = new Color(0.17f, 0.75f, 0.42f);
        private static readonly Color Warning = new Color(1.0f, 0.72f, 0.18f);
        private static readonly Color InactiveStep = new Color(0.20f, 0.23f, 0.28f);
        private static readonly Color ActiveStep = new Color(0.18f, 0.66f, 1.0f);

        private static readonly string[] ProcessFlowNames =
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

            if (autoCreateScene)
            {
                EnsureScene();
            }
        }

        private void LateUpdate()
        {
            UpdateVisuals();
        }

        public void RefreshVisuals()
        {
            UpdateVisuals();
        }

        public void EnsureScene()
        {
            if (sceneRoot == null)
            {
                var root = new GameObject("Generated User-Friendly ALD Operator Console");
                sceneRoot = root.transform;
                sceneRoot.SetParent(transform, false);
                sceneRoot.localPosition = new Vector3(0f, -0.55f, 0f);
            }

            EnsureEnvironment();
            EnsureEquipmentBody();
            EnsureGasAndPumpHardware();
            EnsureOperatorInterface();
            EnsureLabels();
            EnsureProcessFlowBlocks();
            SetStaticLabelText();
        }

        private void EnsureEnvironment()
        {
            if (floorRenderer == null)
            {
                floorRenderer = CreatePrimitive(
                    "Cleanroom Floor Plate",
                    PrimitiveType.Cube,
                    new Vector3(0f, -0.08f, 0.1f),
                    new Vector3(9.5f, 0.05f, 3.7f),
                    Floor);
            }

            if (backWallRenderer == null)
            {
                backWallRenderer = CreatePrimitive(
                    "Dark Back Wall",
                    PrimitiveType.Cube,
                    new Vector3(0f, 1.35f, 1.25f),
                    new Vector3(9.5f, 2.7f, 0.06f),
                    Wall);
            }
        }

        private void EnsureEquipmentBody()
        {
            if (mainCabinetRenderer == null)
            {
                mainCabinetRenderer = CreatePrimitive(
                    "Synthetic ALD Equipment Cabinet",
                    PrimitiveType.Cube,
                    new Vector3(-2.45f, 0.95f, 0.05f),
                    new Vector3(3.55f, 1.9f, 1.45f),
                    CabinetDark);
            }

            if (cabinetWindowRenderer == null)
            {
                cabinetWindowRenderer = CreatePrimitive(
                    "Blue Tinted Service Window",
                    PrimitiveType.Cube,
                    new Vector3(-2.45f, 1.08f, -0.70f),
                    new Vector3(2.95f, 1.20f, 0.055f),
                    Glass);
            }

            if (loadPortRenderer == null)
            {
                loadPortRenderer = CreatePrimitive(
                    "Dual Load Port Facade",
                    PrimitiveType.Cube,
                    new Vector3(-4.15f, 0.62f, -0.62f),
                    new Vector3(0.72f, 1.10f, 0.62f),
                    CabinetMid);
                CreatePrimitive(
                    "Load Port Slot Upper",
                    PrimitiveType.Cube,
                    new Vector3(-4.15f, 0.92f, -0.98f),
                    new Vector3(0.46f, 0.20f, 0.05f),
                    CabinetTrim);
                CreatePrimitive(
                    "Load Port Slot Lower",
                    PrimitiveType.Cube,
                    new Vector3(-4.15f, 0.47f, -0.98f),
                    new Vector3(0.46f, 0.20f, 0.05f),
                    CabinetTrim);
            }

            if (transferRobotRenderer == null)
            {
                transferRobotRenderer = CreatePrimitive(
                    "Wafer Transfer Robot Hub",
                    PrimitiveType.Cylinder,
                    new Vector3(-3.18f, 0.47f, -0.42f),
                    new Vector3(0.34f, 0.10f, 0.34f),
                    CabinetTrim);
                CreatePrimitive(
                    "Wafer Transfer Arm",
                    PrimitiveType.Cube,
                    new Vector3(-2.82f, 0.58f, -0.42f),
                    new Vector3(0.78f, 0.055f, 0.12f),
                    CabinetTrim);
            }

            if (chamberRenderer == null)
            {
                chamberRenderer = CreatePrimitive(
                    "Process Chamber",
                    PrimitiveType.Cylinder,
                    new Vector3(-2.05f, 0.80f, -0.42f),
                    new Vector3(1.18f, 0.24f, 1.18f),
                    ChamberAtAtmosphere);
            }

            if (waferRenderer == null)
            {
                waferRenderer = CreatePrimitive(
                    "Wafer On Heated Stage",
                    PrimitiveType.Cylinder,
                    new Vector3(-2.05f, 1.10f, -0.42f),
                    new Vector3(0.84f, 0.035f, 0.84f),
                    WaferCold);
            }

            if (filmRenderer == null)
            {
                filmRenderer = CreatePrimitive(
                    "Moly Film Overlay",
                    PrimitiveType.Cylinder,
                    new Vector3(-2.05f, 1.16f, -0.42f),
                    new Vector3(0.18f, 0.025f, 0.18f),
                    FilmLow);
            }

            if (vacuumGaugeRenderer == null)
            {
                vacuumGaugeRenderer = CreatePrimitive(
                    "Vacuum Gauge Dial",
                    PrimitiveType.Cylinder,
                    new Vector3(-0.92f, 1.42f, -0.70f),
                    new Vector3(0.34f, 0.035f, 0.34f),
                    CabinetTrim);
                vacuumGaugeRenderer.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            }

            if (pressureNeedle == null)
            {
                var needle = CreatePrimitive(
                    "Vacuum Gauge Needle",
                    PrimitiveType.Cube,
                    new Vector3(-0.92f, 1.42f, -0.96f),
                    new Vector3(0.45f, 0.035f, 0.035f),
                    FilmFull);
                pressureNeedle = needle.transform;
            }

            if (alarmBeaconRenderer == null)
            {
                alarmBeaconRenderer = CreatePrimitive(
                    "Alarm Beacon",
                    PrimitiveType.Sphere,
                    new Vector3(-0.95f, 2.02f, -0.58f),
                    new Vector3(0.18f, 0.18f, 0.18f),
                    Safe);
            }
        }

        private void EnsureGasAndPumpHardware()
        {
            if (gasCabinetRenderer == null)
            {
                gasCabinetRenderer = CreatePrimitive(
                    "Gas Delivery Cabinet",
                    PrimitiveType.Cube,
                    new Vector3(-0.55f, 0.87f, 0.32f),
                    new Vector3(0.92f, 1.55f, 0.96f),
                    CabinetMid);
            }

            if (precursorBottleRenderer == null)
            {
                precursorBottleRenderer = CreateGasBottle("Precursor Bottle", new Vector3(-0.78f, 0.70f, 0.02f), PrecursorOn);
            }

            if (reactantBottleRenderer == null)
            {
                reactantBottleRenderer = CreateGasBottle("Reactant Bottle", new Vector3(-0.53f, 0.70f, 0.32f), ReactantOn);
            }

            if (purgeBottleRenderer == null)
            {
                purgeBottleRenderer = CreateGasBottle("Purge Bottle", new Vector3(-0.28f, 0.70f, 0.62f), PurgeOn);
            }

            if (precursorLineRenderer == null)
            {
                precursorLineRenderer = CreateProcessLine(
                    "Metal Precursor Gas Line",
                    new Vector3(-1.28f, 1.28f, -0.07f),
                    PrecursorOn);
            }

            if (reactantLineRenderer == null)
            {
                reactantLineRenderer = CreateProcessLine(
                    "Reactant Gas Line",
                    new Vector3(-1.28f, 1.03f, -0.36f),
                    ReactantOn);
            }

            if (purgeLineRenderer == null)
            {
                purgeLineRenderer = CreateProcessLine(
                    "Purge Gas Line",
                    new Vector3(-1.28f, 0.78f, -0.65f),
                    PurgeOn);
            }

            if (precursorValveRenderer == null)
            {
                precursorValveRenderer = CreateValve("Metal Precursor Valve", new Vector3(-0.75f, 1.28f, -0.07f));
            }

            if (reactantValveRenderer == null)
            {
                reactantValveRenderer = CreateValve("Reactant Valve", new Vector3(-0.75f, 1.03f, -0.36f));
            }

            if (purgeValveRenderer == null)
            {
                purgeValveRenderer = CreateValve("Purge Valve", new Vector3(-0.75f, 0.78f, -0.65f));
            }

            if (pumpRenderer == null)
            {
                pumpRenderer = CreatePrimitive(
                    "Dry Pump And Exhaust Box",
                    PrimitiveType.Cube,
                    new Vector3(-0.72f, 0.33f, -0.86f),
                    new Vector3(0.82f, 0.46f, 0.45f),
                    CabinetDark);
            }

            if (exhaustLineRenderer == null)
            {
                exhaustLineRenderer = CreatePrimitive(
                    "Vacuum Exhaust Line",
                    PrimitiveType.Cube,
                    new Vector3(-1.36f, 0.42f, -0.86f),
                    new Vector3(1.05f, 0.055f, 0.09f),
                    ChamberAtVacuum);
            }
        }

        private void EnsureOperatorInterface()
        {
            if (operatorPanelRenderer == null)
            {
                operatorPanelRenderer = CreatePrimitive(
                    "Operator Interface Panel",
                    PrimitiveType.Cube,
                    new Vector3(2.35f, 1.28f, -0.95f),
                    new Vector3(2.95f, 2.48f, 0.10f),
                    Panel);
            }

            if (recipeCardRenderer == null)
            {
                recipeCardRenderer = CreatePanelCard("Recipe And Step Card", new Vector3(2.35f, 1.95f, -1.03f), new Vector3(2.55f, 0.62f, 0.045f));
            }

            if (telemetryCardRenderer == null)
            {
                telemetryCardRenderer = CreatePanelCard("Telemetry Card", new Vector3(2.35f, 1.22f, -1.03f), new Vector3(2.55f, 0.62f, 0.045f));
            }

            if (alarmCardRenderer == null)
            {
                alarmCardRenderer = CreatePanelCard("Alarm Card", new Vector3(2.35f, 0.50f, -1.03f), new Vector3(2.55f, 0.42f, 0.045f));
            }

            if (eventLogCardRenderer == null)
            {
                eventLogCardRenderer = CreatePanelCard("Event Log Card", new Vector3(2.35f, 0.23f, -1.03f), new Vector3(2.55f, 0.22f, 0.045f));
            }

            if (startButtonRenderer == null)
            {
                startButtonRenderer = CreateButton("Start Normal Run Button", new Vector3(1.48f, 2.62f, -1.07f), Safe);
            }

            if (stopButtonRenderer == null)
            {
                stopButtonRenderer = CreateButton("Stop Button", new Vector3(2.06f, 2.62f, -1.07f), Warning);
            }

            if (faultButtonRenderer == null)
            {
                faultButtonRenderer = CreateButton("Fault Select Button", new Vector3(2.64f, 2.62f, -1.07f), PrecursorOn);
            }

            if (resetButtonRenderer == null)
            {
                resetButtonRenderer = CreateButton("Reset Alarm Button", new Vector3(3.22f, 2.62f, -1.07f), ReactantOn);
            }

            if (processFlowPanelRenderer == null)
            {
                processFlowPanelRenderer = CreatePrimitive(
                    "Process Flow Panel Background",
                    PrimitiveType.Cube,
                    new Vector3(0.05f, 0.16f, -1.42f),
                    new Vector3(8.45f, 0.05f, 0.48f),
                    Panel);
            }

            if (progressTrackRenderer == null)
            {
                progressTrackRenderer = CreatePrimitive(
                    "Timeline Progress Track",
                    PrimitiveType.Cube,
                    new Vector3(0.05f, 0.36f, -1.63f),
                    new Vector3(7.40f, 0.035f, 0.045f),
                    InactiveStep);
            }

            if (progressFillRenderer == null)
            {
                progressFillRenderer = CreatePrimitive(
                    "Timeline Progress Fill",
                    PrimitiveType.Cube,
                    new Vector3(-3.65f, 0.37f, -1.67f),
                    new Vector3(0.01f, 0.055f, 0.055f),
                    ActiveStep);
            }
        }

        private void EnsureLabels()
        {
            if (!createLabels)
            {
                return;
            }

            if (titleLabel == null)
            {
                titleLabel = CreateLabel(
                    "Synthetic Moly ALD Module",
                    new Vector3(-2.45f, 2.72f, -0.95f),
                    0.050f);
            }

            if (subtitleLabel == null)
            {
                subtitleLabel = CreateLabel(
                    "Public-reference equipment style | not a vendor CAD copy",
                    new Vector3(-2.45f, 2.48f, -0.95f),
                    0.027f,
                    MutedTextColor);
            }

            if (equipmentLabel == null)
            {
                equipmentLabel = CreateLabel(
                    "3D Equipment View",
                    new Vector3(-2.60f, 2.18f, -0.98f),
                    0.031f,
                    MutedTextColor);
            }

            if (chamberLabel == null)
            {
                chamberLabel = CreateLabel("Process chamber\nwafer + film", new Vector3(-2.05f, 1.62f, -1.05f), 0.025f);
            }

            if (loadPortLabel == null)
            {
                loadPortLabel = CreateLabel("Load port", new Vector3(-4.15f, 1.33f, -1.05f), 0.024f);
            }

            if (gasPanelLabel == null)
            {
                gasPanelLabel = CreateLabel("Gas / valve panel", new Vector3(-0.55f, 1.72f, -0.98f), 0.024f);
            }

            if (pumpLabel == null)
            {
                pumpLabel = CreateLabel("Vacuum pump", new Vector3(-0.72f, 0.74f, -1.16f), 0.023f);
            }

            // Operator panel, telemetry, alarm, event log, and timeline text are rendered by
            // MolyAldOperatorCanvas. Keep TextMesh only for equipment labels inside the 3D view.
        }

        private void EnsureProcessFlowBlocks()
        {
            if (processFlowRenderers != null && processFlowRenderers.Length == ProcessFlowNames.Length)
            {
                return;
            }

            processFlowRenderers = new Renderer[ProcessFlowNames.Length];
            var startX = -3.38f;
            const float spacing = 0.82f;

            for (var index = 0; index < ProcessFlowNames.Length; index++)
            {
                processFlowRenderers[index] = CreatePrimitive(
                    $"Process Step {ProcessFlowNames[index]}",
                    PrimitiveType.Cube,
                    new Vector3(startX + spacing * index, 0.23f, -1.55f),
                    new Vector3(0.64f, 0.065f, 0.19f),
                    InactiveStep);
            }
        }

        private void SetStaticLabelText()
        {
            if (buttonLabel != null)
            {
                buttonLabel.text = "START        STOP        FAULT        RESET";
            }

            if (processFlowLabel != null)
            {
                processFlowLabel.text = "Timeline: Load | Pump | Temp | Precursor | Purge | Reactant | Complete";
            }
        }

        private void UpdateVisuals()
        {
            if (player == null)
            {
                return;
            }

            var timeline = player.Timeline;
            var step = player.CurrentStep;
            if (step == null)
            {
                return;
            }

            if (autoCreateScene)
            {
                EnsureScene();
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

        public void ApplyVisualState(MolyAldVisualState visualState)
        {
            if (visualState == null)
            {
                return;
            }

            SetColor(
                chamberRenderer,
                visualState.HasFault ? Alarm : Color.Lerp(ChamberAtVacuum, ChamberAtAtmosphere, visualState.PressureRatio));
            SetColor(waferRenderer, Color.Lerp(WaferCold, WaferHot, visualState.TemperatureRatio));
            SetColor(alarmBeaconRenderer, visualState.HasFault ? Alarm : Safe);
            UpdateFilm(visualState.ThicknessRatio);
            UpdatePressure(visualState.VacuumRatio);
            UpdateValves(visualState);
            UpdateProcessLines(visualState);
            UpdateOperatorCards(visualState);
            UpdateProcessFlow(visualState);
            UpdateProgressBar(visualState);
            UpdateFaultHighlights(visualState);
        }

        private Renderer CreatePanelCard(string name, Vector3 position, Vector3 scale)
        {
            return CreatePrimitive(name, PrimitiveType.Cube, position, scale, PanelCard);
        }

        private Renderer CreateButton(string name, Vector3 position, Color color)
        {
            return CreatePrimitive(name, PrimitiveType.Cube, position, new Vector3(0.46f, 0.15f, 0.055f), color);
        }

        private Renderer CreateValve(string name, Vector3 position)
        {
            return CreatePrimitive(name, PrimitiveType.Sphere, position, new Vector3(0.20f, 0.20f, 0.20f), ValveOff);
        }

        private Renderer CreateGasBottle(string name, Vector3 position, Color color)
        {
            return CreatePrimitive(name, PrimitiveType.Cylinder, position, new Vector3(0.14f, 0.45f, 0.14f), color);
        }

        private Renderer CreateProcessLine(string name, Vector3 position, Color color)
        {
            return CreatePrimitive(name, PrimitiveType.Cube, position, new Vector3(1.04f, 0.035f, 0.035f), color);
        }

        private Renderer CreatePrimitive(string name, PrimitiveType type, Vector3 localPosition, Vector3 localScale, Color color)
        {
            var gameObject = GameObject.CreatePrimitive(type);
            gameObject.name = name;
            gameObject.transform.SetParent(sceneRoot, false);
            gameObject.transform.localPosition = localPosition;
            gameObject.transform.localScale = localScale;

            var renderer = gameObject.GetComponent<Renderer>();
            SetColor(renderer, color);
            return renderer;
        }

        private TextMesh CreateLabel(string name, Vector3 localPosition, float characterSize)
        {
            return CreateLabel(name, localPosition, characterSize, TextColor);
        }

        private TextMesh CreateLabel(string name, Vector3 localPosition, float characterSize, Color color)
        {
            var labelObject = new GameObject(name);
            labelObject.transform.SetParent(sceneRoot, false);
            labelObject.transform.localPosition = localPosition;
            labelObject.transform.localRotation = Quaternion.Euler(55f, 0f, 0f);

            var label = labelObject.AddComponent<TextMesh>();
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.characterSize = characterSize;
            label.fontSize = 48;
            label.lineSpacing = 0.82f;
            label.color = color;
            label.text = name;
            return label;
        }

        private void UpdateFilm(float thicknessRatio)
        {
            if (filmRenderer == null)
            {
                return;
            }

            var diameter = Mathf.Lerp(0.16f, 0.92f, thicknessRatio);
            filmRenderer.transform.localScale = new Vector3(diameter, 0.025f, diameter);
            SetColor(filmRenderer, Color.Lerp(FilmLow, FilmFull, thicknessRatio));
        }

        private void UpdatePressure(float vacuumRatio)
        {
            if (vacuumGaugeRenderer != null)
            {
                SetColor(vacuumGaugeRenderer, Color.Lerp(CabinetTrim, ChamberAtVacuum, vacuumRatio));
            }

            if (pressureNeedle != null)
            {
                pressureNeedle.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(60f, -58f, vacuumRatio));
            }

            if (exhaustLineRenderer != null)
            {
                SetColor(exhaustLineRenderer, Color.Lerp(CabinetTrim, ChamberAtVacuum, vacuumRatio));
            }
        }

        private void UpdateValves(MolyAldVisualState visualState)
        {
            UpdateValve(precursorValveRenderer, visualState.MetalPrecursorOpen, PrecursorOn);
            UpdateValve(reactantValveRenderer, visualState.ReactantOpen, ReactantOn);
            UpdateValve(purgeValveRenderer, visualState.PurgeOpen, PurgeOn);
        }

        private void UpdateProcessLines(MolyAldVisualState visualState)
        {
            UpdateLine(precursorLineRenderer, visualState.MetalPrecursorOpen, PrecursorOn);
            UpdateLine(reactantLineRenderer, visualState.ReactantOpen, ReactantOn);
            UpdateLine(purgeLineRenderer, visualState.PurgeOpen, PurgeOn);
        }

        private void UpdateValve(Renderer renderer, bool isOpen, Color openColor)
        {
            if (renderer == null)
            {
                return;
            }

            renderer.transform.localScale = isOpen
                ? new Vector3(0.29f, 0.29f, 0.29f)
                : new Vector3(0.19f, 0.19f, 0.19f);
            SetColor(renderer, isOpen ? openColor : ValveOff);
        }

        private void UpdateLine(Renderer renderer, bool isOpen, Color openColor)
        {
            if (renderer == null)
            {
                return;
            }

            renderer.transform.localScale = isOpen
                ? new Vector3(1.12f, 0.055f, 0.055f)
                : new Vector3(0.98f, 0.028f, 0.028f);
            SetColor(renderer, isOpen ? openColor : Color.Lerp(ValveOff, openColor, 0.22f));
        }

        private void UpdateOperatorCards(MolyAldVisualState visualState)
        {
            if (recipeLabel != null)
            {
                recipeLabel.text =
                    "RECIPE\n" +
                    $"{ShortRecipeName(visualState.RecipeName)}\n" +
                    $"Step {visualState.StepIndex}/{visualState.TotalSteps}\n" +
                    $"{SplitCamelCase(visualState.StepName)}\n" +
                    $"Cycle {CycleText(visualState)}";
            }

            if (telemetryLabel != null)
            {
                telemetryLabel.text =
                    "LIVE\n" +
                    $"Pressure {visualState.ChamberPressureMtorr:0.#} mTorr\n" +
                    $"Temp {visualState.WaferTemperatureC:0.#} C\n" +
                    $"Film {visualState.EstimatedThicknessAngstrom:0.###} A\n" +
                    $"Valve {ActiveValveText(visualState)}";
            }

            if (alarmLabel != null)
            {
                alarmLabel.text = visualState.HasFault
                    ? $"ALARM ACTIVE\n{FaultArea(visualState.StepName)}\nHold sequence"
                    : "NO ALARM\nInterlocks nominal\nReady for next step";
            }

            SetColor(alarmCardRenderer, visualState.HasFault ? Color.Lerp(PanelCard, Alarm, 0.45f) : PanelCard);
        }

        private void UpdateProcessFlow(MolyAldVisualState visualState)
        {
            if (processFlowRenderers == null)
            {
                return;
            }

            var activeIndex = ProcessFlowIndex(visualState.StepName);
            for (var index = 0; index < processFlowRenderers.Length; index++)
            {
                var isActive = index == activeIndex;
                var isPast = activeIndex >= 0 && index < activeIndex;
                SetColor(processFlowRenderers[index], isActive ? ActiveStep : isPast ? Safe : InactiveStep);
                processFlowRenderers[index].transform.localScale = isActive
                    ? new Vector3(0.70f, 0.085f, 0.22f)
                    : new Vector3(0.64f, 0.065f, 0.19f);
            }

            if (eventLogLabel != null)
            {
                eventLogLabel.text = $"EVENT: {SplitCamelCase(visualState.StepName)} | {ActiveValveText(visualState)} valve";
            }
        }

        private void UpdateProgressBar(MolyAldVisualState visualState)
        {
            if (progressFillRenderer == null || visualState.TotalSteps <= 0)
            {
                return;
            }

            const float fullWidth = 7.40f;
            var ratio = Mathf.Clamp01(visualState.StepIndex / (float)visualState.TotalSteps);
            var width = Mathf.Max(0.05f, fullWidth * ratio);
            progressFillRenderer.transform.localScale = new Vector3(width, 0.055f, 0.055f);
            progressFillRenderer.transform.localPosition = new Vector3(-3.65f + width * 0.5f, 0.37f, -1.67f);
        }

        private void UpdateFaultHighlights(MolyAldVisualState visualState)
        {
            if (!visualState.HasFault)
            {
                SetColor(pumpRenderer, CabinetDark);
                SetColor(gasCabinetRenderer, CabinetMid);
                return;
            }

            var faultArea = FaultArea(visualState.StepName);
            SetColor(pumpRenderer, faultArea == "Pump / exhaust" ? Alarm : CabinetDark);
            SetColor(gasCabinetRenderer, faultArea == "Gas delivery" ? Alarm : CabinetMid);
            SetColor(chamberRenderer, faultArea == "Thermal chamber" ? Alarm : Alarm);
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

        private static string ValveSummary(MolyAldVisualState visualState)
        {
            return
                $"P:{OnOff(visualState.MetalPrecursorOpen)} " +
                $"R:{OnOff(visualState.ReactantOpen)} " +
                $"Pu:{OnOff(visualState.PurgeOpen)}";
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

        private static string OnOff(bool value)
        {
            return value ? "ON" : "OFF";
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

        private static void SetColor(Renderer renderer, Color color)
        {
            if (renderer == null)
            {
                return;
            }

            if (renderer.sharedMaterial == null || renderer.sharedMaterial.name.StartsWith("Default", System.StringComparison.OrdinalIgnoreCase))
            {
                renderer.sharedMaterial = new Material(Shader.Find("Standard"));
            }

            renderer.sharedMaterial.color = color;
        }
    }
}
