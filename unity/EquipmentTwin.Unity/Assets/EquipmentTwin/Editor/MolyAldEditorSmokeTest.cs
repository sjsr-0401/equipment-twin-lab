using System;
using System.IO;
using EquipmentTwin.Unity.Processes;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EquipmentTwin.Unity.EditorTools
{
    public static class MolyAldEditorSmokeTest
    {
        public const string SuccessMarker = "EQUIPMENT_TWIN_UNITY_SMOKE_TEST_PASS";
        public const string TimelineFileName = "moly-ald-timeline.sample.json";

        [MenuItem("Equipment Twin/Create Moly ALD Demo Scene")]
        public static void CreateDemoSceneFromMenu()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                Debug.Log("Create demo scene was cancelled.");
                return;
            }

            CreateDemoScene();
            Debug.Log("Created Moly ALD demo scene. Press Play to run the timeline.");
        }

        [MenuItem("Equipment Twin/Run Moly ALD Smoke Test")]
        public static void RunSmokeTestFromMenu()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                Debug.Log("Moly ALD smoke test was cancelled.");
                return;
            }

            RunSmokeTest();
        }

        public static void RunBatchSmokeTest()
        {
            try
            {
                RunSmokeTest();
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunSmokeTest()
        {
            var timeline = LoadSampleTimeline();
            ValidateTimeline(timeline);

            var root = CreateDemoScene();
            var visualizer = root.GetComponent<MolyAldPrimitiveVisualizer>();
            if (visualizer == null)
            {
                throw new InvalidOperationException("MolyAldPrimitiveVisualizer was not created.");
            }

            visualizer.EnsureScene();

            var renderers = root.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length < 6)
            {
                throw new InvalidOperationException(
                    $"Expected at least 6 generated renderers, but found {renderers.Length}.");
            }

            if (root.GetComponent<MolyAldProcessPlayer>() == null)
            {
                throw new InvalidOperationException("MolyAldProcessPlayer was not created.");
            }

            if (root.GetComponent<MolyAldProcessHud>() == null)
            {
                throw new InvalidOperationException("MolyAldProcessHud was not created.");
            }

            if (root.GetComponent<MolyAldDemoBootstrap>() == null)
            {
                throw new InvalidOperationException("MolyAldDemoBootstrap was not created.");
            }

            Debug.Log(
                $"{SuccessMarker}: recipe={timeline.recipeName}, steps={timeline.steps.Length}, renderers={renderers.Length}");
        }

        public static GameObject CreateDemoScene()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var root = new GameObject("Moly ALD Demo Root");
            root.AddComponent<MolyAldProcessPlayer>();
            root.AddComponent<MolyAldProcessHud>();
            root.AddComponent<MolyAldPrimitiveVisualizer>();
            root.AddComponent<MolyAldDemoBootstrap>();

            CreateCamera();
            CreateLight();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Selection.activeGameObject = root;
            return root;
        }

        private static MolyAldTimelineDocumentDto LoadSampleTimeline()
        {
            var path = Path.Combine(Application.streamingAssetsPath, TimelineFileName);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Sample timeline was not found: {path}", path);
            }

            return MolyAldTimelineLoader.FromJson(File.ReadAllText(path));
        }

        private static void ValidateTimeline(MolyAldTimelineDocumentDto timeline)
        {
            if (timeline == null)
            {
                throw new InvalidOperationException("Timeline failed to parse.");
            }

            if (timeline.steps == null || timeline.steps.Length == 0)
            {
                throw new InvalidOperationException("Timeline contains no steps.");
            }

            if (string.IsNullOrWhiteSpace(timeline.recipeName))
            {
                throw new InvalidOperationException("Timeline recipeName is empty.");
            }
        }

        private static void CreateCamera()
        {
            var cameraObject = new GameObject("Moly ALD Smoke Test Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 4f, -6.4f);
            cameraObject.transform.rotation = Quaternion.Euler(32f, 0f, 0f);

            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.04f, 0.045f, 0.055f);
        }

        private static void CreateLight()
        {
            var lightObject = new GameObject("Moly ALD Smoke Test Directional Light");
            lightObject.transform.rotation = Quaternion.Euler(50f, -35f, 0f);

            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.15f;
        }
    }
}
