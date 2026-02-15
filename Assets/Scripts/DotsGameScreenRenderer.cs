using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

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

    public struct GameScreenEntityTag : IComponentData
    {
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

    public sealed class DotsGameScreenRenderer : BlindfoldProtocol.App.IGameScreenRenderer
    {
        private bool _initialized;
        private RuntimeSceneConfig _config;

        public void Show()
        {
            EnsureInitialized();
            var camera = SetupCamera(_config);
            EnsureCameraController(camera, true);
            SpawnSceneIfMissing(_config);
        }

        public void Hide()
        {
            var mainCamera = Camera.main;
            if (mainCamera != null)
            {
                EnsureCameraController(mainCamera, false);
            }

            if (!TryGetEntityManager(out var entityManager))
            {
                return;
            }

            using var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<GameScreenEntityTag>());
            entityManager.DestroyEntity(query);
        }

        private void EnsureInitialized()
        {
            if (_initialized)
            {
                return;
            }

            _config = RuntimeSceneDefaults.Create();
            _initialized = true;
        }

        private void SpawnSceneIfMissing(RuntimeSceneConfig config)
        {
            if (!TryGetEntityManager(out var entityManager))
            {
                return;
            }

            using var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<GameScreenEntityTag>());
            if (!query.IsEmptyIgnoreFilter)
            {
                return;
            }

            var cubeMesh = GetPrimitiveMesh(PrimitiveType.Cube);
            var groundMesh = CreateGroundMesh(config.GroundSize);

            var groundMaterial = CreateUnlitMaterial(new Color(0.19f, 0.57f, 0.22f), "RuntimeGroundMaterial");
            var cubeMaterial = CreateUnlitMaterial(new Color(0.76f, 0.76f, 0.76f), "RuntimeCubeMaterial");

            CreateRenderableEntity(
                entityManager,
                mesh: groundMesh,
                material: groundMaterial,
                position: config.GroundPosition,
                rotation: quaternion.identity,
                scale: 1f);

            CreateRenderableEntity(
                entityManager,
                mesh: cubeMesh,
                material: cubeMaterial,
                position: config.CubePosition,
                rotation: quaternion.identity,
                scale: config.CubeSize);
        }

        private static bool TryGetEntityManager(out EntityManager entityManager)
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null)
            {
                entityManager = default;
                return false;
            }

            entityManager = world.EntityManager;
            return true;
        }

        private static void CreateRenderableEntity(EntityManager entityManager, Mesh mesh, Material material, float3 position, quaternion rotation, float scale)
        {
            var entity = entityManager.CreateEntity(typeof(LocalTransform), typeof(GameScreenEntityTag));

            var renderMeshDescription = new RenderMeshDescription(
                shadowCastingMode: ShadowCastingMode.Off,
                receiveShadows: false,
                motionVectorGenerationMode: MotionVectorGenerationMode.ForceNoMotion,
                layer: 0,
                renderingLayerMask: uint.MaxValue,
                lightProbeUsage: LightProbeUsage.Off,
                staticShadowCaster: false);

            var renderMeshArray = new RenderMeshArray(new[] { material }, new[] { mesh });

            RenderMeshUtility.AddComponents(
                entity,
                entityManager,
                renderMeshDescription,
                renderMeshArray,
                MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));

            entityManager.SetComponentData(entity, LocalTransform.FromPositionRotationScale(position, rotation, scale));
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

        private static Mesh GetPrimitiveMesh(PrimitiveType primitiveType)
        {
            var temporary = GameObject.CreatePrimitive(primitiveType);
            var mesh = temporary.GetComponent<MeshFilter>().sharedMesh;
            Object.Destroy(temporary);
            return mesh;
        }

        private static Mesh CreateGroundMesh(float size)
        {
            var half = size * 0.5f;

            var mesh = new Mesh
            {
                name = "RuntimeGroundMesh",
                vertices = new[]
                {
                    new Vector3(-half, 0f, -half),
                    new Vector3(-half, 0f, half),
                    new Vector3(half, 0f, half),
                    new Vector3(half, 0f, -half)
                },
                uv = new[]
                {
                    new Vector2(0f, 0f),
                    new Vector2(0f, 1f),
                    new Vector2(1f, 1f),
                    new Vector2(1f, 0f)
                },
                triangles = new[] { 0, 1, 2, 0, 2, 3 },
                normals = new[]
                {
                    Vector3.up,
                    Vector3.up,
                    Vector3.up,
                    Vector3.up
                }
            };

            mesh.RecalculateBounds();
            return mesh;
        }

        private static Camera SetupCamera(RuntimeSceneConfig config)
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
            return mainCamera;
        }

        private static void EnsureCameraController(Camera camera, bool enabled)
        {
            var controller = camera.GetComponent<GameCameraController>();
            if (controller == null)
            {
                controller = camera.gameObject.AddComponent<GameCameraController>();
            }

            controller.Configure(
                panSpeed: 20f,
                zoomStep: 1.2f,
                minZoom: 3f,
                maxZoom: 30f);
            controller.enabled = enabled;
        }
    }
}
