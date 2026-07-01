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
        [SerializeField] private Renderer chamberRenderer;
        [SerializeField] private Renderer waferRenderer;
        [SerializeField] private Renderer filmRenderer;
        [SerializeField] private Renderer pressureColumnRenderer;
        [SerializeField] private Transform pressureNeedle;
        [SerializeField] private Renderer precursorValveRenderer;
        [SerializeField] private Renderer reactantValveRenderer;
        [SerializeField] private Renderer purgeValveRenderer;
        [SerializeField] private TextMesh stepLabel;
        [SerializeField] private TextMesh valueLabel;

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

        public void EnsureScene()
        {
            if (sceneRoot == null)
            {
                var root = new GameObject("Generated Moly ALD Visual");
                sceneRoot = root.transform;
                sceneRoot.SetParent(transform, false);
                sceneRoot.localPosition = Vector3.zero;
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
                    new Vector3(-2.9f, 1.95f, 0f),
                    new Vector3(0.75f, 0.04f, 0.04f),
                    FilmFull);
                pressureNeedle = needleRenderer.transform;
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

            if (createLabels && stepLabel == null)
            {
                stepLabel = CreateLabel("Step Label", new Vector3(0f, 2.35f, 0f), 0.15f);
            }

            if (createLabels && valueLabel == null)
            {
                valueLabel = CreateLabel("Value Label", new Vector3(0f, 2.05f, 0f), 0.105f);
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

            var pressureRatio = PressureRatio(step.chamberPressureMtorr);
            var vacuumRatio = 1f - pressureRatio;
            var temperatureRatio = Mathf.InverseLerp(roomTemperatureC, processTemperatureC, step.waferTemperatureC);
            var thicknessRatio = ThicknessRatio(timeline, step);
            var hasFault = timeline != null && (!timeline.success || !step.success);

            SetColor(chamberRenderer, hasFault ? Alarm : Color.Lerp(ChamberAtVacuum, ChamberAtAtmosphere, pressureRatio));
            SetColor(waferRenderer, Color.Lerp(WaferCold, WaferHot, temperatureRatio));
            UpdateFilm(thicknessRatio);
            UpdatePressure(vacuumRatio);
            UpdateValves(step);
            UpdateLabels(timeline, step, pressureRatio, thicknessRatio);
        }

        private Renderer CreateValve(string name, Vector3 position)
        {
            return CreatePrimitive(name, PrimitiveType.Sphere, position, new Vector3(0.28f, 0.28f, 0.28f), ValveOff);
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

        private void UpdateValves(MolyAldTimelineStepDto step)
        {
            var valves = step.valves;
            var precursorOpen = valves != null && valves.metalPrecursor;
            var reactantOpen = valves != null && valves.reactant;
            var purgeOpen = valves != null && valves.purge;

            UpdateValve(precursorValveRenderer, precursorOpen, PrecursorOn);
            UpdateValve(reactantValveRenderer, reactantOpen, ReactantOn);
            UpdateValve(purgeValveRenderer, purgeOpen, PurgeOn);
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

        private void UpdateLabels(
            MolyAldTimelineDocumentDto timeline,
            MolyAldTimelineStepDto step,
            float pressureRatio,
            float thicknessRatio)
        {
            if (stepLabel != null)
            {
                var totalSteps = timeline != null && timeline.steps != null ? timeline.steps.Length : 0;
                stepLabel.text = $"{step.index}/{totalSteps} {step.step}";
            }

            if (valueLabel != null)
            {
                var cycle = step.HasCycle ? step.cycle.ToString() : "-";
                valueLabel.text =
                    $"cycle {cycle} | pressure {step.chamberPressureMtorr:0.#} mTorr | temp {step.waferTemperatureC:0.#} C\n" +
                    $"vacuum {(1f - pressureRatio):P0} | film {thicknessRatio:P0} | valves {ValveText(step)}";
            }
        }

        private string ValveText(MolyAldTimelineStepDto step)
        {
            if (step.valves == null)
            {
                return "none";
            }

            return
                $"P:{OnOff(step.valves.metalPrecursor)} " +
                $"R:{OnOff(step.valves.reactant)} " +
                $"G:{OnOff(step.valves.purge)}";
        }

        private float PressureRatio(float pressureMtorr)
        {
            var low = Mathf.Min(processPressureMtorr, atmospherePressureMtorr);
            var high = Mathf.Max(processPressureMtorr, atmospherePressureMtorr);
            return Mathf.InverseLerp(low, high, Mathf.Clamp(pressureMtorr, low, high));
        }

        private static float ThicknessRatio(MolyAldTimelineDocumentDto timeline, MolyAldTimelineStepDto step)
        {
            if (timeline == null || timeline.targetThicknessAngstrom <= 0f)
            {
                return 0f;
            }

            return Mathf.Clamp01(step.estimatedThicknessAngstrom / timeline.targetThicknessAngstrom);
        }

        private static void SetColor(Renderer renderer, Color color)
        {
            if (renderer == null)
            {
                return;
            }

            renderer.material.color = color;
        }

        private static string OnOff(bool value)
        {
            return value ? "ON" : "OFF";
        }
    }
}
