using UnityEngine;

namespace EquipmentTwin.Unity.Processes
{
    [DisallowMultipleComponent]
    public sealed class MolyAldImportedModelVisualBinding : MonoBehaviour
    {
        [Header("Source")]
        [SerializeField] private MolyAldProcessPlayer player;

        [Header("Pressure mapping")]
        [SerializeField] private float processPressureMtorr = 850f;
        [SerializeField] private float atmospherePressureMtorr = 760000f;

        [Header("Temperature mapping")]
        [SerializeField] private float roomTemperatureC = 25f;
        [SerializeField] private float processTemperatureC = 250f;

        [Header("Imported model bindings")]
        [SerializeField] private Renderer chamberRenderer;
        [SerializeField] private Renderer waferRenderer;
        [SerializeField] private Renderer filmRenderer;
        [SerializeField] private Transform filmScaleRoot;
        [SerializeField] private Renderer pressureIndicatorRenderer;
        [SerializeField] private Transform pressureNeedle;
        [SerializeField] private Renderer precursorLineRenderer;
        [SerializeField] private Renderer reactantLineRenderer;
        [SerializeField] private Renderer purgeLineRenderer;
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

        private bool hasFilmBaseScale;
        private Vector3 filmBaseScale;

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
        }

        private void LateUpdate()
        {
            RefreshVisuals();
        }

        public void RefreshVisuals()
        {
            if (player == null || player.CurrentStep == null)
            {
                return;
            }

            var visualState = MolyAldVisualStateMapper.FromTimeline(
                player.Timeline,
                player.CurrentStep,
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
            SetColor(filmRenderer, Color.Lerp(FilmLow, FilmFull, visualState.ThicknessRatio));
            SetColor(pressureIndicatorRenderer, Color.Lerp(ChamberAtAtmosphere, ChamberAtVacuum, visualState.VacuumRatio));
            UpdateFilmScale(visualState.ThicknessRatio);
            UpdatePressureNeedle(visualState.VacuumRatio);
            UpdateValve(precursorValveRenderer, visualState.MetalPrecursorOpen, PrecursorOn);
            UpdateValve(reactantValveRenderer, visualState.ReactantOpen, ReactantOn);
            UpdateValve(purgeValveRenderer, visualState.PurgeOpen, PurgeOn);
            UpdateLine(precursorLineRenderer, visualState.MetalPrecursorOpen, PrecursorOn);
            UpdateLine(reactantLineRenderer, visualState.ReactantOpen, ReactantOn);
            UpdateLine(purgeLineRenderer, visualState.PurgeOpen, PurgeOn);
            UpdateLabels(visualState);
        }

        private void UpdateFilmScale(float thicknessRatio)
        {
            if (filmScaleRoot == null)
            {
                return;
            }

            if (!hasFilmBaseScale)
            {
                filmBaseScale = filmScaleRoot.localScale;
                hasFilmBaseScale = true;
            }

            var diameterMultiplier = Mathf.Lerp(0.15f, 1f, thicknessRatio);
            filmScaleRoot.localScale = new Vector3(
                filmBaseScale.x * diameterMultiplier,
                filmBaseScale.y,
                filmBaseScale.z * diameterMultiplier);
        }

        private void UpdatePressureNeedle(float vacuumRatio)
        {
            if (pressureNeedle == null)
            {
                return;
            }

            pressureNeedle.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(65f, -65f, vacuumRatio));
        }

        private void UpdateValve(Renderer renderer, bool isOpen, Color openColor)
        {
            if (renderer == null)
            {
                return;
            }

            SetColor(renderer, isOpen ? openColor : ValveOff);
        }

        private void UpdateLine(Renderer renderer, bool isOpen, Color openColor)
        {
            if (renderer == null)
            {
                return;
            }

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
