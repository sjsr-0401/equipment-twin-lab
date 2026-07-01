using UnityEngine;

namespace EquipmentTwin.Unity.Processes
{
    public sealed class MolyAldProcessPlayer : MonoBehaviour
    {
        [Header("Timeline source")]
        [SerializeField] private TextAsset timelineAsset;
        [SerializeField] private string streamingAssetsRelativePath = "moly-ald-timeline.sample.json";

        [Header("Playback")]
        [SerializeField] private bool playOnStart = true;
        [SerializeField] private bool loop;
        [SerializeField] private float playbackSpeed = 1.0f;
        [SerializeField] private float minimumStepSeconds = 0.25f;

        private MolyAldTimelineDocumentDto timeline;
        private int currentStepIndex;
        private float elapsedInCurrentStepSeconds;
        private bool isPlaying;
        private string loadError = string.Empty;

        public MolyAldTimelineDocumentDto Timeline => timeline;

        public MolyAldTimelineStepDto CurrentStep
        {
            get
            {
                if (timeline == null || timeline.steps == null || timeline.steps.Length == 0)
                {
                    return null;
                }

                return timeline.steps[Mathf.Clamp(currentStepIndex, 0, timeline.steps.Length - 1)];
            }
        }

        public bool IsPlaying => isPlaying;

        public string LoadError => loadError;

        public float NormalizedStepProgress
        {
            get
            {
                var step = CurrentStep;
                if (step == null)
                {
                    return 0f;
                }

                var duration = GetStepDurationSeconds(step);
                return Mathf.Clamp01(elapsedInCurrentStepSeconds / duration);
            }
        }

        private void Start()
        {
            LoadTimeline();

            if (playOnStart && string.IsNullOrWhiteSpace(loadError))
            {
                Play();
            }
        }

        private void Update()
        {
            if (!isPlaying || CurrentStep == null)
            {
                return;
            }

            elapsedInCurrentStepSeconds += Time.deltaTime * Mathf.Max(0f, playbackSpeed);

            if (elapsedInCurrentStepSeconds >= GetStepDurationSeconds(CurrentStep))
            {
                AdvanceStep();
            }
        }

        public void LoadTimeline()
        {
            try
            {
                loadError = string.Empty;
                timeline = timelineAsset != null
                    ? MolyAldTimelineLoader.FromTextAsset(timelineAsset)
                    : MolyAldTimelineLoader.FromStreamingAssetsFile(streamingAssetsRelativePath);

                currentStepIndex = 0;
                elapsedInCurrentStepSeconds = 0f;
            }
            catch (System.Exception ex)
            {
                timeline = null;
                isPlaying = false;
                loadError = ex.Message;
                Debug.LogError($"Failed to load ALD timeline: {ex.Message}", this);
            }
        }

        public void Play()
        {
            if (timeline == null || timeline.steps == null || timeline.steps.Length == 0)
            {
                return;
            }

            isPlaying = true;
        }

        public void Pause()
        {
            isPlaying = false;
        }

        public void Restart()
        {
            currentStepIndex = 0;
            elapsedInCurrentStepSeconds = 0f;
            Play();
        }

        public void AdvanceStep()
        {
            elapsedInCurrentStepSeconds = 0f;

            if (timeline == null || timeline.steps == null || timeline.steps.Length == 0)
            {
                isPlaying = false;
                return;
            }

            if (currentStepIndex < timeline.steps.Length - 1)
            {
                currentStepIndex++;
                return;
            }

            if (loop)
            {
                currentStepIndex = 0;
                return;
            }

            isPlaying = false;
        }

        private float GetStepDurationSeconds(MolyAldTimelineStepDto step)
        {
            return Mathf.Max(minimumStepSeconds, step.durationMilliseconds / 1000f);
        }
    }
}
