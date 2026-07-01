using UnityEngine;

namespace EquipmentTwin.Unity.Processes
{
    public sealed class MolyAldProcessHud : MonoBehaviour
    {
        [SerializeField] private MolyAldProcessPlayer player;
        [SerializeField] private bool logStepSummary = true;

        private string lastSummary = string.Empty;

        public string LastSummary => lastSummary;

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
            var summary = BuildSummary();
            if (!string.Equals(summary, lastSummary))
            {
                lastSummary = summary;
                if (logStepSummary && !string.IsNullOrWhiteSpace(lastSummary))
                {
                    Debug.Log(lastSummary);
                }
            }
        }

        private string BuildSummary()
        {
            if (player == null)
            {
                return "MolyAldProcessHud: player reference is missing.";
            }

            if (!string.IsNullOrWhiteSpace(player.LoadError))
            {
                return $"MolyAldProcessHud load error: {player.LoadError}";
            }

            var timeline = player.Timeline;
            var step = player.CurrentStep;
            if (timeline == null || step == null)
            {
                return "MolyAldProcessHud: timeline is not loaded.";
            }

            return $"Equipment Twin - {timeline.recipeName}: {step.index}/{timeline.steps.Length} {step.step}, " +
                   $"cycle {(step.HasCycle ? step.cycle.ToString() : "-")}/{timeline.cycleCount}, " +
                   $"pressure {step.chamberPressureMtorr:0.#} mTorr, temp {step.waferTemperatureC:0.#} C, " +
                   $"thickness {step.estimatedThicknessAngstrom:0.###}/{timeline.targetThicknessAngstrom:0.###} A, " +
                   $"valves P:{OnOff(step.valves.metalPrecursor)} R:{OnOff(step.valves.reactant)} Pu:{OnOff(step.valves.purge)}";
        }

        private static string OnOff(bool value)
        {
            return value ? "ON" : "OFF";
        }
    }
}
