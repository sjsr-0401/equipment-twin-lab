using UnityEngine;

namespace EquipmentTwin.Unity.Processes
{
    public sealed class MolyAldProcessPlayer : MonoBehaviour
    {
        private static readonly string[] PublicFaultScenarioNames =
        {
            "pumpdown-timeout",
            "temperature-not-stable",
            "precursor-dose-timeout",
            "purge-timeout"
        };

        [Header("Timeline source")]
        [SerializeField] private TextAsset timelineAsset;
        [SerializeField] private string streamingAssetsRelativePath = "moly-ald-timeline.sample.json";
        [SerializeField] private string faultTimelineDirectory = "faults";
        [SerializeField] private string faultTimelineFilePrefix = "moly-ald-timeline.";
        [SerializeField] private string faultTimelineFileExtension = ".json";

        [Header("Playback")]
        [SerializeField] private bool playOnStart = true;
        [SerializeField] private bool loop;
        [SerializeField] private float playbackSpeed = 1.0f;
        [SerializeField] private float minimumStepSeconds = 0.25f;

        [Header("Fault scenario")]
        [SerializeField] private string selectedFaultScenarioName = "precursor-dose-timeout";

        private MolyAldTimelineDocumentDto timeline;
        private int currentStepIndex;
        private float elapsedInCurrentStepSeconds;
        private bool isPlaying;
        private bool operatorFaultActive;
        private bool faultTimelineReplayActive;
        private string activeFaultScenarioName = string.Empty;
        private string activeTimelineRelativePath = string.Empty;
        private string faultReplayError = string.Empty;
        private string loadError = string.Empty;

        public MolyAldTimelineDocumentDto Timeline => timeline;

        public int CurrentStepIndex => currentStepIndex;

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

        public bool OperatorFaultActive => operatorFaultActive;

        public string SelectedFaultScenarioName => NormalizeFaultScenarioName(selectedFaultScenarioName);

        public string ActiveFaultScenarioName
        {
            get
            {
                if (!operatorFaultActive)
                {
                    return string.Empty;
                }

                return string.IsNullOrWhiteSpace(activeFaultScenarioName)
                    ? SelectedFaultScenarioName
                    : activeFaultScenarioName;
            }
        }

        public bool FaultTimelineReplayActive => faultTimelineReplayActive;

        public string ActiveTimelineRelativePath => activeTimelineRelativePath;

        public string FaultReplayError => faultReplayError;

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
                operatorFaultActive = false;
                faultTimelineReplayActive = false;
                activeFaultScenarioName = string.Empty;
                activeTimelineRelativePath = timelineAsset != null ? "[TextAsset]" : streamingAssetsRelativePath;
                faultReplayError = string.Empty;
                selectedFaultScenarioName = SelectedFaultScenarioName;
            }
            catch (System.Exception ex)
            {
                timeline = null;
                isPlaying = false;
                operatorFaultActive = false;
                faultTimelineReplayActive = false;
                activeFaultScenarioName = string.Empty;
                activeTimelineRelativePath = string.Empty;
                faultReplayError = string.Empty;
                loadError = ex.Message;
                Debug.LogError($"Failed to load ALD timeline: {ex.Message}", this);
            }
        }

        public void Play()
        {
            if (operatorFaultActive || timeline == null || timeline.steps == null || timeline.steps.Length == 0)
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
            if (faultTimelineReplayActive)
            {
                LoadTimeline();
            }

            currentStepIndex = 0;
            elapsedInCurrentStepSeconds = 0f;
            operatorFaultActive = false;
            faultTimelineReplayActive = false;
            activeFaultScenarioName = string.Empty;
            faultReplayError = string.Empty;
            Play();
        }

        public void ResetToStart()
        {
            if (faultTimelineReplayActive)
            {
                LoadTimeline();
            }

            currentStepIndex = 0;
            elapsedInCurrentStepSeconds = 0f;
            operatorFaultActive = false;
            faultTimelineReplayActive = false;
            activeFaultScenarioName = string.Empty;
            faultReplayError = string.Empty;
            Pause();
        }

        public void ToggleOperatorFault()
        {
            if (operatorFaultActive)
            {
                ClearOperatorFault();
                return;
            }

            ActivateSelectedFaultScenario();
        }

        public void ActivateSelectedFaultScenario()
        {
            selectedFaultScenarioName = SelectedFaultScenarioName;
            activeFaultScenarioName = selectedFaultScenarioName;
            faultReplayError = string.Empty;

            if (!TryLoadSelectedFaultTimeline(selectedFaultScenarioName, out var replayError))
            {
                faultReplayError = replayError;
                Debug.LogError($"Failed to replay ALD fault timeline '{selectedFaultScenarioName}': {replayError}", this);
            }

            operatorFaultActive = true;
            Pause();
        }

        public void ClearOperatorFault()
        {
            operatorFaultActive = false;
            activeFaultScenarioName = string.Empty;
            faultReplayError = string.Empty;

            if (faultTimelineReplayActive)
            {
                LoadTimeline();
            }
        }

        public void SelectFaultScenario(string scenarioName)
        {
            selectedFaultScenarioName = NormalizeFaultScenarioName(scenarioName);
        }

        public void SelectNextFaultScenario()
        {
            var current = SelectedFaultScenarioName;
            var nextIndex = 0;

            for (var index = 0; index < PublicFaultScenarioNames.Length; index++)
            {
                if (string.Equals(PublicFaultScenarioNames[index], current, System.StringComparison.OrdinalIgnoreCase))
                {
                    nextIndex = (index + 1) % PublicFaultScenarioNames.Length;
                    break;
                }
            }

            selectedFaultScenarioName = PublicFaultScenarioNames[nextIndex];
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

        private bool TryLoadSelectedFaultTimeline(string scenarioName, out string replayError)
        {
            replayError = string.Empty;
            var previousTimeline = timeline;
            var previousStepIndex = currentStepIndex;
            var previousElapsed = elapsedInCurrentStepSeconds;
            var previousTimelinePath = activeTimelineRelativePath;
            var previousReplayActive = faultTimelineReplayActive;

            try
            {
                var relativePath = BuildFaultTimelineRelativePath(scenarioName);
                var replayTimeline = MolyAldTimelineLoader.FromStreamingAssetsFile(relativePath);

                if (replayTimeline.success)
                {
                    throw new System.InvalidOperationException($"Fault replay timeline '{relativePath}' is marked success=true.");
                }

                if (!string.Equals(replayTimeline.faultScenarioName, scenarioName, System.StringComparison.OrdinalIgnoreCase))
                {
                    throw new System.InvalidOperationException(
                        $"Fault replay timeline '{relativePath}' contains scenario '{replayTimeline.faultScenarioName}', expected '{scenarioName}'.");
                }

                timeline = replayTimeline;
                currentStepIndex = FindFirstFailedStepIndex(replayTimeline);
                elapsedInCurrentStepSeconds = 0f;
                activeTimelineRelativePath = relativePath;
                activeFaultScenarioName = replayTimeline.faultScenarioName;
                faultTimelineReplayActive = true;
                loadError = string.Empty;
                return true;
            }
            catch (System.Exception ex)
            {
                timeline = previousTimeline;
                currentStepIndex = previousStepIndex;
                elapsedInCurrentStepSeconds = previousElapsed;
                activeTimelineRelativePath = previousTimelinePath;
                faultTimelineReplayActive = previousReplayActive;
                replayError = ex.Message;
                return false;
            }
        }

        private string BuildFaultTimelineRelativePath(string scenarioName)
        {
            var safeDirectory = string.IsNullOrWhiteSpace(faultTimelineDirectory)
                ? string.Empty
                : faultTimelineDirectory.Trim().Trim('/', '\\');
            var fileName = $"{faultTimelineFilePrefix}{scenarioName}{faultTimelineFileExtension}";

            return string.IsNullOrWhiteSpace(safeDirectory)
                ? fileName
                : $"{safeDirectory}/{fileName}";
        }

        private static int FindFirstFailedStepIndex(MolyAldTimelineDocumentDto replayTimeline)
        {
            if (replayTimeline == null || replayTimeline.steps == null || replayTimeline.steps.Length == 0)
            {
                return 0;
            }

            for (var index = 0; index < replayTimeline.steps.Length; index++)
            {
                if (!replayTimeline.steps[index].success)
                {
                    return index;
                }
            }

            return replayTimeline.steps.Length - 1;
        }

        private static string NormalizeFaultScenarioName(string scenarioName)
        {
            if (string.IsNullOrWhiteSpace(scenarioName))
            {
                return PublicFaultScenarioNames[0];
            }

            for (var index = 0; index < PublicFaultScenarioNames.Length; index++)
            {
                if (string.Equals(PublicFaultScenarioNames[index], scenarioName.Trim(), System.StringComparison.OrdinalIgnoreCase))
                {
                    return PublicFaultScenarioNames[index];
                }
            }

            return PublicFaultScenarioNames[0];
        }
    }
}
