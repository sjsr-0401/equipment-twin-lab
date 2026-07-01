using UnityEngine;

namespace EquipmentTwin.Unity.Processes
{
    public sealed class MolyAldProcessHud : MonoBehaviour
    {
        [SerializeField] private MolyAldProcessPlayer player;
        [SerializeField] private int width = 520;
        [SerializeField] private int height = 280;

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

        private void OnGUI()
        {
            if (player == null)
            {
                GUILayout.BeginArea(new Rect(12, 12, width, 80), GUI.skin.box);
                GUILayout.Label("MolyAldProcessHud: player reference is missing.");
                GUILayout.EndArea();
                return;
            }

            GUILayout.BeginArea(new Rect(12, 12, width, height), GUI.skin.box);
            GUILayout.Label("Equipment Twin - Public Moly ALD Timeline");

            if (!string.IsNullOrWhiteSpace(player.LoadError))
            {
                GUILayout.Label($"Load error: {player.LoadError}");
                GUILayout.EndArea();
                return;
            }

            var timeline = player.Timeline;
            var step = player.CurrentStep;
            if (timeline == null || step == null)
            {
                GUILayout.Label("Timeline is not loaded.");
                GUILayout.EndArea();
                return;
            }

            GUILayout.Label($"Recipe: {timeline.recipeName}");
            GUILayout.Label($"Result: {(timeline.success ? "PASS" : "FAIL")} / Final: {timeline.finalStep}");
            GUILayout.Label($"Step: {step.index}/{timeline.steps.Length} {step.step}");
            GUILayout.Label($"Cycle: {(step.HasCycle ? step.cycle.ToString() : "-")} / {timeline.cycleCount}");
            GUILayout.Label($"Progress: {player.NormalizedStepProgress:P0} / Playing: {player.IsPlaying}");
            GUILayout.Space(6);
            GUILayout.Label($"Pressure: {step.chamberPressureMtorr:0.#} mTorr");
            GUILayout.Label($"Wafer temperature: {step.waferTemperatureC:0.#} C");
            GUILayout.Label($"Film thickness: {step.estimatedThicknessAngstrom:0.###} A / target {timeline.targetThicknessAngstrom:0.###} A");
            GUILayout.Space(6);
            GUILayout.Label($"Valves - Precursor: {OnOff(step.valves.metalPrecursor)}, Reactant: {OnOff(step.valves.reactant)}, Purge: {OnOff(step.valves.purge)}");
            GUILayout.Label($"Message: {step.message}");
            GUILayout.Space(6);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(player.IsPlaying ? "Pause" : "Play"))
            {
                if (player.IsPlaying)
                {
                    player.Pause();
                }
                else
                {
                    player.Play();
                }
            }

            if (GUILayout.Button("Restart"))
            {
                player.Restart();
            }

            if (GUILayout.Button("Next Step"))
            {
                player.AdvanceStep();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private static string OnOff(bool value)
        {
            return value ? "ON" : "OFF";
        }
    }
}
