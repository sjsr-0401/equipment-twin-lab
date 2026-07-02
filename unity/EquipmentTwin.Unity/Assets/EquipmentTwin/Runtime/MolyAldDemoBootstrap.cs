using UnityEngine;

namespace EquipmentTwin.Unity.Processes
{
    [DisallowMultipleComponent]
    public sealed class MolyAldDemoBootstrap : MonoBehaviour
    {
        [SerializeField] private bool ensurePlayer = true;
        [SerializeField] private bool ensureHud = true;
        [SerializeField] private bool ensurePrimitiveVisualizer = true;
        [SerializeField] private bool ensureCameraAndLight = true;

        private void Awake()
        {
            if (ensurePlayer)
            {
                EnsureComponent<MolyAldProcessPlayer>();
            }

            if (ensureHud)
            {
                EnsureComponent<MolyAldProcessHud>();
            }

            if (ensurePrimitiveVisualizer)
            {
                EnsureComponent<MolyAldPrimitiveVisualizer>();
            }

            if (ensureCameraAndLight)
            {
                EnsureSceneView();
            }
        }

        private T EnsureComponent<T>() where T : Component
        {
            var component = GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }

        private void EnsureSceneView()
        {
            if (Camera.main == null && FindObjectOfType<Camera>() == null)
            {
                var cameraObject = new GameObject("Moly ALD Demo Camera");
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

            if (FindObjectOfType<Light>() == null)
            {
                var lightObject = new GameObject("Moly ALD Demo Directional Light");
                lightObject.transform.rotation = Quaternion.Euler(50f, -35f, 0f);

                var light = lightObject.AddComponent<Light>();
                light.type = LightType.Directional;
                light.intensity = 1.30f;
            }
        }
    }
}
