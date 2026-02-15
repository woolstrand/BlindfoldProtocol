using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace BlindfoldProtocol.Runtime
{
    public struct RuntimeSceneConfig : IComponentData
    {
        public float GroundSize;
        public float CubeSize;
        public float3 GroundPosition;
        public float3 CubePosition;
        public float3 CameraPosition;
        public float3 CameraEuler;
    }

    public enum RuntimeObjectKind : byte
    {
        Ground = 1,
        Cube = 2
    }

    public struct RuntimeSceneObject : IComponentData
    {
        public RuntimeObjectKind Kind;
    }

    public static class RuntimeSceneDefaults
    {
        public static RuntimeSceneConfig Create()
        {
            return new RuntimeSceneConfig
            {
                GroundSize = 20f,
                CubeSize = 1f,
                GroundPosition = new float3(0f, 0f, 0f),
                CubePosition = new float3(0f, 0.5f, 0f),
                CameraPosition = new float3(0f, 12f, 0f),
                CameraEuler = new float3(90f, 0f, 0f)
            };
        }
    }

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class RuntimeDotsBootstrapSystem : SystemBase
    {
        private bool _initialized;

        protected override void OnCreate()
        {
            base.OnCreate();

            if (!SystemAPI.TryGetSingletonEntity<RuntimeSceneConfig>(out _))
            {
                var configEntity = EntityManager.CreateEntity(typeof(RuntimeSceneConfig));
                EntityManager.SetComponentData(configEntity, RuntimeSceneDefaults.Create());
            }
        }

        protected override void OnUpdate()
        {
            if (_initialized)
            {
                return;
            }

            var config = SystemAPI.GetSingleton<RuntimeSceneConfig>();

            SetupCamera(config);
            SpawnScene(config);

            _initialized = true;
            Enabled = false;
        }

        private void SpawnScene(RuntimeSceneConfig config)
        {
            CreateSceneEntity(
                kind: RuntimeObjectKind.Ground,
                position: config.GroundPosition,
                rotation: quaternion.identity,
                scale: 1f);

            CreateSceneEntity(
                kind: RuntimeObjectKind.Cube,
                position: config.CubePosition,
                rotation: quaternion.identity,
                scale: config.CubeSize);

            BuildVisualScene(config);
        }

        private void CreateSceneEntity(RuntimeObjectKind kind, float3 position, quaternion rotation, float scale)
        {
            var entity = EntityManager.CreateEntity(typeof(RuntimeSceneObject), typeof(LocalTransform));

            EntityManager.SetComponentData(entity, new RuntimeSceneObject { Kind = kind });
            EntityManager.SetComponentData(entity, LocalTransform.FromPositionRotationScale(position, rotation, scale));
        }

        private static void BuildVisualScene(RuntimeSceneConfig config)
        {
            const string rootName = "RuntimeDotsView";
            var root = GameObject.Find(rootName);
            if (root == null)
            {
                root = new GameObject(rootName);
            }

            EnsureLandscape(root.transform, config);
            EnsureCube(root.transform, config);
        }

        private static void SetupCamera(RuntimeSceneConfig config)
        {
            var mainCamera = Camera.main;
            if (mainCamera == null)
            {
                var existingCamera = Object.FindFirstObjectByType<Camera>();
                if (existingCamera != null)
                {
                    existingCamera.tag = "MainCamera";
                    mainCamera = existingCamera;
                }
            }

            if (mainCamera == null)
            {
                var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
                cameraObject.tag = "MainCamera";
                mainCamera = cameraObject.GetComponent<Camera>();
            }

            mainCamera.transform.position = config.CameraPosition;
            mainCamera.transform.rotation = Quaternion.Euler(config.CameraEuler);
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = math.max(5f, config.GroundSize * 0.4f);
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = new Color(0.45f, 0.72f, 0.95f);
        }

        private static void EnsureLandscape(Transform root, RuntimeSceneConfig config)
        {
            var landscape = root.Find("Landscape");
            GameObject landscapeObject;

            if (landscape == null)
            {
                landscapeObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                landscapeObject.name = "Landscape";
                landscapeObject.transform.SetParent(root, false);
                var collider = landscapeObject.GetComponent<Collider>();
                if (collider != null)
                {
                    Object.Destroy(collider);
                }
            }
            else
            {
                landscapeObject = landscape.gameObject;
            }

            landscapeObject.transform.position = new Vector3(config.GroundPosition.x, config.GroundPosition.y - 0.05f, config.GroundPosition.z);
            landscapeObject.transform.localScale = new Vector3(config.GroundSize, 0.1f, config.GroundSize);

            var renderer = landscapeObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = CreateUnlitMaterial(new Color(0.19f, 0.57f, 0.22f), "RuntimeLandscapeMaterial");
            }
        }

        private static void EnsureCube(Transform root, RuntimeSceneConfig config)
        {
            var cube = root.Find("Cube");
            GameObject cubeObject;

            if (cube == null)
            {
                cubeObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cubeObject.name = "Cube";
                cubeObject.transform.SetParent(root, false);
                var collider = cubeObject.GetComponent<Collider>();
                if (collider != null)
                {
                    Object.Destroy(collider);
                }
            }
            else
            {
                cubeObject = cube.gameObject;
            }

            cubeObject.transform.position = config.CubePosition;
            cubeObject.transform.localScale = new Vector3(config.CubeSize, config.CubeSize, config.CubeSize);

            var renderer = cubeObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = CreateUnlitMaterial(new Color(0.76f, 0.76f, 0.76f), "RuntimeCubeMaterial");
            }
        }

        private static Material CreateUnlitMaterial(Color color, string materialName)
        {
            var shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Color");
            var material = new Material(shader)
            {
                name = materialName,
                color = color
            };

            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", color);
            }

            if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", color);
            }

            return material;
        }
    }
}
