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

        [Header("Generated visuals")]
        [SerializeField] private Renderer basePlateRenderer;
        [SerializeField] private Renderer chamberRenderer;
        [SerializeField] private Renderer waferRenderer;
        [SerializeField] private Renderer filmRenderer;
        [SerializeField] private Renderer pressureColumnRenderer;
        [SerializeField] private Transform pressureNeedle;
        [SerializeField] private Renderer precursorLineRenderer;
        [SerializeField] private Renderer reactantLineRenderer;
        [SerializeField] private Renderer purgeLineRenderer;
        [SerializeField] private Renderer precursorValveRenderer;
        [SerializeField] private Renderer reactantValveRenderer;
        [SerializeField] private Renderer purgeValveRenderer;
        [SerializeField] private TextMesh titleLabel;
        [SerializeField] private TextMesh stepLabel;
        [SerializeField] private TextMesh valueLabel;

        private static readonly Color BasePlate = new Color(0.075f, 0.09f, 0.115f);
        private static readonly Color ChamberAtAtmosphere = new Color(0.35f, 0.35f, 0.38f);
        private static readonly Color ChamberAtVacuum = new Color(0.12f, 0.28f, 0.55f);
        private static readonly Color WaferCold = new Color(0.55f, 0.58f, 0.62f);
        private static readonly Color WaferHot = new Color(1.0f, 0.42f, 0.08f);
        private static readonly Color FilmLow = new Color(0.15f, 0.55f, 1.0f);
        private static readonly Color FilmFull = new Color(1.0f, 0.82f, 0.12f);
        private static readonly Color ValveOff = new Color(0.18f, 0.18f, 0.18f);
        private static readonly Color PrecursorOn = new Color(0.95f, 0.55f, 0.12f);
        private static readonly Color ReactantOn = new Color(0.25f, 0.75f, 1.0f);
        private static readonly Color PurgeOn = new Color(0.2f, 0.9f, 0.35f);
        private static readonly Color Alarm = new Color(0.95f, 0.08f, 0.08f);
        private static readonly Color TextColor = new Color(0.92f, 0.92f, 0.92f);

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
                var root = new GameObject("Generated Moly ALD Visual");
                sceneRoot = root.transform;
                sceneRoot.SetParent(transform, false);
                sceneRoot.localPosition = Vector3.zero;
            }

            if (basePlateRenderer == null)
            {
                basePlateRenderer = CreatePrimitive(
                    "Equipment Base Plate",
                    PrimitiveType.Cube,
                    new Vector3(0f, -0.06f, 0f),
                    new Vector3(6.2f, 0.08f, 3.25f),
                    BasePlate);
            }

            if (chamberRenderer == null)
            {
                chamberRenderer = CreatePrimitive(
                    "Chamber Body",
                    PrimitiveType.Cylinder,
                    new Vector3(0f, 0.55f, 0f),
                    new Vector3(2.3f, 0.28f, 2.3f),
                    ChamberAtAtmosphere);
            }

            if (waferRenderer == null)
            {
                waferRenderer = CreatePrimitive(
                    "Wafer",
                    PrimitiveType.Cylinder,
                    new Vector3(0f, 0.92f, 0f),
                    new Vector3(1.55f, 0.035f, 1.55f),
                    WaferCold);
            }

            if (filmRenderer == null)
            {
                filmRenderer = CreatePrimitive(
                    "Film Thickness Overlay",
                    PrimitiveType.Cylinder,
                    new Vector3(0f, 1.02f, 0f),
                    new Vector3(0.25f, 0.025f, 0.25f),
                    FilmLow);
            }

            if (pressureColumnRenderer == null)
            {
                pressureColumnRenderer = CreatePrimitive(
                    "Vacuum Level Column",
                    PrimitiveType.Cube,
                    new Vector3(-2.9f, 0.7f, 0f),
                    new Vector3(0.18f, 0.55f, 0.18f),
                    ChamberAtVacuum);
            }

            if (pressureNeedle == null)
            {
                var needleRenderer = CreatePrimitive(
                    "Vacuum Gauge Needle",
                    PrimitiveType.Cube,
                    new Vector3(-2.9f, 2.0f, 0f),
                    new Vector3(0.75f, 0.04f, 0.04f),
                    FilmFull);
                pressureNeedle = needleRenderer.transform;
            }

            if (precursorLineRenderer == null)
            {
                precursorLineRenderer = CreateProcessLine(
                    "Metal Precursor Gas Line",
                    new Vector3(1.8f, 1.25f, 0.85f),
                    PrecursorOn);
            }

            if (reactantLineRenderer == null)
            {
                reactantLineRenderer = CreateProcessLine(
                    "Reactant Gas Line",
                    new Vector3(1.8f, 0.75f, 0f),
                    ReactantOn);
            }

            if (purgeLineRenderer == null)
            {
                purgeLineRenderer = CreateProcessLine(
                    "Purge Gas Line",
                    new Vector3(1.8f, 0.25f, -0.85f),
                    PurgeOn);
            }

            if (precursorValveRenderer == null)
            {
                precursorValveRenderer = CreateValve("Metal Precursor Valve", new Vector3(2.8f, 1.25f, 0.85f));
            }

            if (reactantValveRenderer == null)
            {
                reactantValveRenderer = CreateValve("Reactant Valve", new Vector3(2.8f, 0.75f, 0f));
            }

            if (purgeValveRenderer == null)
            {
                purgeValveRenderer = CreateValve("Purge Valve", new Vector3(2.8f, 0.25f, -0.85f));
            }

            if (createLabels && titleLabel == null)
            {
                titleLabel = CreateLabel("Equipment Twin - Public Moly ALD Replay", new Vector3(0f, 2.9f, 0.05f), 0.052f);
            }

            if (createLabels && stepLabel == null)
            {
                stepLabel = CreateLabel("Step", new Vector3(0f, 2.55f, 0.05f), 0.082f);
            }

            if (createLabels && valueLabel == null)
            {
                valueLabel = CreateLabel("Values", new Vector3(0f, 2.2f, 0.05f), 0.046f);
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
            UpdateFilm(visualState.ThicknessRatio);
            UpdatePressure(visualState.VacuumRatio);
            UpdateValves(visualState);
            UpdateProcessLines(visualState);
            UpdateLabels(visualState);
        }

        private Renderer CreateValve(string name, Vector3 position)
        {
            return CreatePrimitive(name, PrimitiveType.Sphere, position, new Vector3(0.28f, 0.28f, 0.28f), ValveOff);
        }

        private Renderer CreateProcessLine(string name, Vector3 position, Color color)
        {
            return CreatePrimitive(name, PrimitiveType.Cube, position, new Vector3(1.75f, 0.035f, 0.035f), color);
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
            var labelObject = new GameObject(name);
            labelObject.transform.SetParent(sceneRoot, false);
            labelObject.transform.localPosition = localPosition;
            labelObject.transform.localRotation = Quaternion.Euler(55f, 0f, 0f);

            var label = labelObject.AddComponent<TextMesh>();
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.characterSize = characterSize;
            label.fontSize = 48;
            label.color = TextColor;
            label.text = name;
            return label;
        }

        private void UpdateFilm(float thicknessRatio)
        {
            if (filmRenderer == null)
            {
                return;
            }

            var diameter = Mathf.Lerp(0.25f, 1.65f, thicknessRatio);
            filmRenderer.transform.localScale = new Vector3(diameter, 0.025f, diameter);
            SetColor(filmRenderer, Color.Lerp(FilmLow, FilmFull, thicknessRatio));
        }

        private void UpdatePressure(float vacuumRatio)
        {
            if (pressureColumnRenderer != null)
            {
                var height = Mathf.Lerp(0.12f, 1.2f, vacuumRatio);
                pressureColumnRenderer.transform.localScale = new Vector3(0.18f, height, 0.18f);
                pressureColumnRenderer.transform.localPosition = new Vector3(-2.9f, 0.12f + height, 0f);
                SetColor(pressureColumnRenderer, Color.Lerp(ChamberAtAtmosphere, ChamberAtVacuum, vacuumRatio));
            }

            if (pressureNeedle != null)
            {
                pressureNeedle.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(65f, -65f, vacuumRatio));
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
                ? new Vector3(0.38f, 0.38f, 0.38f)
                : new Vector3(0.25f, 0.25f, 0.25f);
            SetColor(renderer, isOpen ? openColor : ValveOff);
        }

        private void UpdateLine(Renderer renderer, bool isOpen, Color openColor)
        {
            if (renderer == null)
            {
                return;
            }

            renderer.transform.localScale = isOpen
                ? new Vector3(1.9f, 0.055f, 0.055f)
                : new Vector3(1.55f, 0.03f, 0.03f);
            SetColor(renderer, isOpen ? openColor : Color.Lerp(ValveOff, openColor, 0.25f));
        }

        private void UpdateLabels(MolyAldVisualState visualState)
        {
            if (stepLabel != null)
            {
                stepLabel.text = visualState.StepLabel;
            }

            if (valueLabel != null)
            {
                valueLabel.text = visualState.ValueLabel;
            }
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
