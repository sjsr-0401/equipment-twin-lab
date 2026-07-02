using System;
using System.IO;
using EquipmentTwin.Unity.Processes;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EquipmentTwin.Unity.EditorTools
{
    public static class MolyAldEditorSmokeTest
    {
        public const string SuccessMarker = "EQUIPMENT_TWIN_UNITY_SMOKE_TEST_PASS";
        public const string ScreenshotMarker = "EQUIPMENT_TWIN_UNITY_SCREENSHOT_SAVED";
        public const string FaultScreenshotMarker = "EQUIPMENT_TWIN_UNITY_FAULT_SCREENSHOT_SAVED";
        public const string RecoveryScreenshotMarker = "EQUIPMENT_TWIN_UNITY_RECOVERY_SCREENSHOT_SAVED";
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

        [MenuItem("Equipment Twin/Capture Moly ALD Demo Screenshot")]
        public static void CaptureScreenshotFromMenu()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                Debug.Log("Moly ALD screenshot capture was cancelled.");
                return;
            }

            CaptureScreenshot();
        }

        [MenuItem("Equipment Twin/Capture Moly ALD Fault Screenshot")]
        public static void CaptureFaultScreenshotFromMenu()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                Debug.Log("Moly ALD fault screenshot capture was cancelled.");
                return;
            }

            CaptureFaultScreenshot();
        }

        [MenuItem("Equipment Twin/Capture Moly ALD Recovery Screenshot")]
        public static void CaptureRecoveryScreenshotFromMenu()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                Debug.Log("Moly ALD recovery screenshot capture was cancelled.");
                return;
            }

            CaptureRecoveryScreenshot();
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

        public static void RunBatchScreenshotCapture()
        {
            try
            {
                RunSmokeTest();
                CaptureScreenshot();
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunBatchFaultScreenshotCapture()
        {
            try
            {
                RunSmokeTest();
                CaptureFaultScreenshot();
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunBatchRecoveryScreenshotCapture()
        {
            try
            {
                RunSmokeTest();
                CaptureRecoveryScreenshot();
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
            PrepareDemoStateForCapture(root);

            var visualizer = root.GetComponent<MolyAldPrimitiveVisualizer>();
            if (visualizer == null)
            {
                throw new InvalidOperationException("MolyAldPrimitiveVisualizer was not created.");
            }

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

            var operatorCanvas = root.GetComponent<MolyAldOperatorCanvas>();
            if (operatorCanvas == null)
            {
                throw new InvalidOperationException("MolyAldOperatorCanvas was not created.");
            }

            operatorCanvas.EnsureCanvas();
            operatorCanvas.RefreshCanvas();

            if (UnityEngine.Object.FindObjectOfType<Canvas>() == null)
            {
                throw new InvalidOperationException("Canvas operator panel was not created.");
            }

            ValidateVisualStateMapper(root);
            ValidateOperatorControls(root);

            Debug.Log(
                $"{SuccessMarker}: recipe={timeline.recipeName}, steps={timeline.steps.Length}, renderers={renderers.Length}");
        }

        public static string CaptureScreenshot()
        {
            var root = FindDemoRoot();
            if (root == null)
            {
                root = CreateDemoScene();
            }

            var visualizer = root.GetComponent<MolyAldPrimitiveVisualizer>();
            if (visualizer == null)
            {
                throw new InvalidOperationException("MolyAldPrimitiveVisualizer was not found for screenshot capture.");
            }

            PrepareDemoStateForCapture(root);

            var camera = Camera.main != null ? Camera.main : UnityEngine.Object.FindObjectOfType<Camera>();
            if (camera == null)
            {
                CreateCamera();
                camera = Camera.main != null ? Camera.main : UnityEngine.Object.FindObjectOfType<Camera>();
            }

            if (camera == null)
            {
                throw new InvalidOperationException("No camera is available for screenshot capture.");
            }

            var outputPath = ResolveScreenshotPath();
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            RenderCameraToPng(camera, outputPath, 1280, 720);
            Debug.Log($"{ScreenshotMarker}: {outputPath}");
            return outputPath;
        }

        public static string CaptureFaultScreenshot()
        {
            var root = FindDemoRoot();
            if (root == null)
            {
                root = CreateDemoScene();
            }

            var visualizer = root.GetComponent<MolyAldPrimitiveVisualizer>();
            if (visualizer == null)
            {
                throw new InvalidOperationException("MolyAldPrimitiveVisualizer was not found for fault screenshot capture.");
            }

            PrepareFaultDemoStateForCapture(root);

            var camera = Camera.main != null ? Camera.main : UnityEngine.Object.FindObjectOfType<Camera>();
            if (camera == null)
            {
                CreateCamera();
                camera = Camera.main != null ? Camera.main : UnityEngine.Object.FindObjectOfType<Camera>();
            }

            if (camera == null)
            {
                throw new InvalidOperationException("No camera is available for fault screenshot capture.");
            }

            var outputPath = ResolveFaultScreenshotPath();
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            RenderCameraToPng(camera, outputPath, 1280, 720);
            Debug.Log($"{FaultScreenshotMarker}: {outputPath}");
            return outputPath;
        }

        public static string CaptureRecoveryScreenshot()
        {
            var root = FindDemoRoot();
            if (root == null)
            {
                root = CreateDemoScene();
            }

            var visualizer = root.GetComponent<MolyAldPrimitiveVisualizer>();
            if (visualizer == null)
            {
                throw new InvalidOperationException("MolyAldPrimitiveVisualizer was not found for recovery screenshot capture.");
            }

            PrepareRecoveryDemoStateForCapture(root);

            var camera = Camera.main != null ? Camera.main : UnityEngine.Object.FindObjectOfType<Camera>();
            if (camera == null)
            {
                CreateCamera();
                camera = Camera.main != null ? Camera.main : UnityEngine.Object.FindObjectOfType<Camera>();
            }

            if (camera == null)
            {
                throw new InvalidOperationException("No camera is available for recovery screenshot capture.");
            }

            var outputPath = ResolveRecoveryScreenshotPath();
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            RenderCameraToPng(camera, outputPath, 1280, 720);
            Debug.Log($"{RecoveryScreenshotMarker}: {outputPath}");
            return outputPath;
        }

        public static GameObject CreateDemoScene()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var root = new GameObject("Moly ALD Demo Root");
            root.AddComponent<MolyAldProcessPlayer>();
            root.AddComponent<MolyAldProcessHud>();
            root.AddComponent<MolyAldPrimitiveVisualizer>();
            root.AddComponent<MolyAldOperatorCanvas>();
            root.AddComponent<MolyAldDemoBootstrap>();

            CreateCamera();
            CreateLight();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Selection.activeGameObject = root;
            return root;
        }

        private static void PrepareDemoStateForCapture(GameObject root)
        {
            var player = root.GetComponent<MolyAldProcessPlayer>();
            if (player == null)
            {
                throw new InvalidOperationException("MolyAldProcessPlayer was not found.");
            }

            player.LoadTimeline();
            MoveToRepresentativeStep(player);
            player.Play();

            var visualizer = root.GetComponent<MolyAldPrimitiveVisualizer>();
            if (visualizer == null)
            {
                throw new InvalidOperationException("MolyAldPrimitiveVisualizer was not found.");
            }

            visualizer.EnsureScene();
            visualizer.RefreshVisuals();

            var operatorCanvas = root.GetComponent<MolyAldOperatorCanvas>();
            if (operatorCanvas != null)
            {
                operatorCanvas.EnsureCanvas();
                operatorCanvas.ClearOperatorActionLog();
                operatorCanvas.RecordOperatorAction("START", "normal process running");
                operatorCanvas.RefreshCanvas();
            }
        }

        private static void PrepareFaultDemoStateForCapture(GameObject root)
        {
            PrepareDemoStateForCapture(root);

            var player = root.GetComponent<MolyAldProcessPlayer>();
            if (player == null)
            {
                throw new InvalidOperationException("MolyAldProcessPlayer was not found for fault screenshot capture.");
            }

            if (!player.OperatorFaultActive)
            {
                player.SelectFaultScenario("precursor-dose-timeout");
                player.ToggleOperatorFault();
            }

            var operatorCanvas = root.GetComponent<MolyAldOperatorCanvas>();
            if (operatorCanvas == null)
            {
                throw new InvalidOperationException("MolyAldOperatorCanvas was not found for fault screenshot capture.");
            }

            operatorCanvas.EnsureCanvas();
            operatorCanvas.RecordOperatorAction("FAULT", $"{player.SelectedFaultScenarioName} selected");
            operatorCanvas.RefreshCanvas();
        }

        private static void PrepareRecoveryDemoStateForCapture(GameObject root)
        {
            PrepareFaultDemoStateForCapture(root);

            var player = root.GetComponent<MolyAldProcessPlayer>();
            if (player == null)
            {
                throw new InvalidOperationException("MolyAldProcessPlayer was not found for recovery screenshot capture.");
            }

            player.ResetToStart();

            var operatorCanvas = root.GetComponent<MolyAldOperatorCanvas>();
            if (operatorCanvas == null)
            {
                throw new InvalidOperationException("MolyAldOperatorCanvas was not found for recovery screenshot capture.");
            }

            operatorCanvas.EnsureCanvas();
            operatorCanvas.RecordOperatorAction("RESET", "fault cleared, ready at first step");
            operatorCanvas.RefreshCanvas();
        }

        private static void ValidateVisualStateMapper(GameObject root)
        {
            var player = root.GetComponent<MolyAldProcessPlayer>();
            if (player == null)
            {
                throw new InvalidOperationException("MolyAldProcessPlayer was not found for visual-state validation.");
            }

            var visualState = MolyAldVisualStateMapper.FromTimeline(
                player.Timeline,
                player.CurrentStep,
                850f,
                760000f,
                25f,
                250f);

            if (visualState == null)
            {
                throw new InvalidOperationException("Visual state mapper returned null.");
            }

            if (string.IsNullOrWhiteSpace(visualState.StepLabel))
            {
                throw new InvalidOperationException("Visual state step label is empty.");
            }

            if (!visualState.ReactantOpen)
            {
                throw new InvalidOperationException(
                    $"Expected the representative visual state to show reactant flow, but step was {visualState.StepName}.");
            }
        }

        private static void ValidateOperatorControls(GameObject root)
        {
            if (UnityEngine.Object.FindObjectOfType<EventSystem>() == null)
            {
                throw new InvalidOperationException("EventSystem was not created for Canvas button interaction.");
            }

            var buttons = UnityEngine.Object.FindObjectsOfType<Button>();
            if (buttons.Length < 4)
            {
                throw new InvalidOperationException($"Expected at least 4 Canvas command buttons, but found {buttons.Length}.");
            }

            var operatorCanvas = root.GetComponent<MolyAldOperatorCanvas>();
            if (operatorCanvas == null)
            {
                throw new InvalidOperationException("MolyAldOperatorCanvas was not found for operator action log validation.");
            }

            operatorCanvas.ClearOperatorActionLog();
            operatorCanvas.RecordOperatorAction("TEST", "operator log smoke");
            if (operatorCanvas.OperatorActionLogEntryCount < 1)
            {
                throw new InvalidOperationException("Operator action log did not record a smoke-test event.");
            }

            var player = root.GetComponent<MolyAldProcessPlayer>();
            if (player == null)
            {
                throw new InvalidOperationException("MolyAldProcessPlayer was not found for operator-control validation.");
            }

            player.Play();
            if (!player.IsPlaying)
            {
                throw new InvalidOperationException("Play command did not set the process player to running.");
            }

            player.Pause();
            if (player.IsPlaying)
            {
                throw new InvalidOperationException("Pause command did not stop the process player.");
            }

            player.ToggleOperatorFault();
            if (!player.OperatorFaultActive)
            {
                throw new InvalidOperationException("Fault selector did not enable the operator fault override.");
            }

            if (string.IsNullOrWhiteSpace(player.SelectedFaultScenarioName))
            {
                throw new InvalidOperationException("Fault selector did not keep a selected public fault scenario name.");
            }

            var faultState = MolyAldVisualStateMapper.FromTimeline(
                player.Timeline,
                player.CurrentStep,
                850f,
                760000f,
                25f,
                250f,
                player.OperatorFaultActive);

            if (faultState == null || !faultState.HasFault)
            {
                throw new InvalidOperationException("Forced operator fault was not reflected in the visual state mapper.");
            }

            player.ResetToStart();
            if (player.OperatorFaultActive || player.IsPlaying || player.CurrentStepIndex != 0)
            {
                throw new InvalidOperationException("Reset command did not clear fault, stop playback, and return to step 0.");
            }
        }

        private static void MoveToRepresentativeStep(MolyAldProcessPlayer player)
        {
            var timeline = player.Timeline;
            if (timeline == null || timeline.steps == null || timeline.steps.Length == 0)
            {
                return;
            }

            var targetIndex = 0;
            for (var index = 0; index < timeline.steps.Length; index++)
            {
                var step = timeline.steps[index];
                if (step != null && string.Equals(step.step, "DoseReactant", StringComparison.OrdinalIgnoreCase))
                {
                    targetIndex = index;
                    break;
                }
            }

            for (var index = 0; index < targetIndex; index++)
            {
                player.AdvanceStep();
            }
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
            cameraObject.transform.position = new Vector3(0f, 3.85f, -8.20f);
            cameraObject.transform.rotation = Quaternion.Euler(28f, 0f, 0f);

            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.035f, 0.042f, 0.055f);
            camera.orthographic = true;
            camera.orthographicSize = 2.55f;
            camera.fieldOfView = 46f;
        }

        private static GameObject FindDemoRoot()
        {
            var player = UnityEngine.Object.FindObjectOfType<MolyAldProcessPlayer>();
            return player != null ? player.gameObject : null;
        }

        private static string ResolveScreenshotPath()
        {
            var args = Environment.GetCommandLineArgs();
            for (var index = 0; index < args.Length - 1; index++)
            {
                if (string.Equals(args[index], "-equipmentTwinScreenshot", StringComparison.OrdinalIgnoreCase))
                {
                    return Path.GetFullPath(args[index + 1]);
                }
            }

            return Path.GetFullPath(Path.Combine(
                Application.dataPath,
                "..",
                "..",
                "..",
                "artifacts",
                "unity-demo",
                "moly-ald-demo.png"));
        }

        private static string ResolveFaultScreenshotPath()
        {
            var args = Environment.GetCommandLineArgs();
            for (var index = 0; index < args.Length - 1; index++)
            {
                if (string.Equals(args[index], "-equipmentTwinFaultScreenshot", StringComparison.OrdinalIgnoreCase))
                {
                    return Path.GetFullPath(args[index + 1]);
                }
            }

            return Path.GetFullPath(Path.Combine(
                Application.dataPath,
                "..",
                "..",
                "..",
                "artifacts",
                "unity-demo",
                "moly-ald-demo-fault.png"));
        }

        private static string ResolveRecoveryScreenshotPath()
        {
            var args = Environment.GetCommandLineArgs();
            for (var index = 0; index < args.Length - 1; index++)
            {
                if (string.Equals(args[index], "-equipmentTwinRecoveryScreenshot", StringComparison.OrdinalIgnoreCase))
                {
                    return Path.GetFullPath(args[index + 1]);
                }
            }

            return Path.GetFullPath(Path.Combine(
                Application.dataPath,
                "..",
                "..",
                "..",
                "artifacts",
                "unity-demo",
                "moly-ald-demo-recovery.png"));
        }

        private static void RenderCameraToPng(Camera camera, string outputPath, int width, int height)
        {
            var previousTargetTexture = camera.targetTexture;
            var previousActiveTexture = RenderTexture.active;
            var renderTexture = new RenderTexture(width, height, 24);
            var texture = new Texture2D(width, height, TextureFormat.RGB24, false);

            try
            {
                camera.targetTexture = renderTexture;
                RenderTexture.active = renderTexture;
                camera.Render();
                texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                texture.Apply();
                File.WriteAllBytes(outputPath, texture.EncodeToPNG());
            }
            finally
            {
                camera.targetTexture = previousTargetTexture;
                RenderTexture.active = previousActiveTexture;
                UnityEngine.Object.DestroyImmediate(texture);
                UnityEngine.Object.DestroyImmediate(renderTexture);
            }
        }

        private static void CreateLight()
        {
            var lightObject = new GameObject("Moly ALD Smoke Test Directional Light");
            lightObject.transform.rotation = Quaternion.Euler(50f, -35f, 0f);

            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.30f;
        }
    }
}
