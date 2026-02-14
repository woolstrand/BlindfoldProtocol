using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace BlindfoldProtocol
{
    /// <summary>
    /// Bridge between ECS and GameObject world for rendering
    /// </summary>
    public class GameObjectBridge : MonoBehaviour
    {
        private EntityManager entityManager;
        private Entity cubeEntity;
        private Entity meadowEntity;

        private GameObject cubeGameObject;
        private GameObject meadowGameObject;
        private GameObject selectionBox;

        void Start()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            entityManager = world.EntityManager;

            // Create visual representations
            CreateMeadowVisual();
            CreateCubeVisual();
            CreateSelectionBox();

            // Find entities (wait a frame for them to be created)
            Invoke(nameof(FindEntities), 0.1f);
        }

        void FindEntities()
        {
            var query = entityManager.CreateEntityQuery(typeof(BlindfoldProtocol.Components.CubeTag));
            var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);
            if (entities.Length > 0)
            {
                cubeEntity = entities[0];
            }
            entities.Dispose();

            var meadowQuery = entityManager.CreateEntityQuery(typeof(BlindfoldProtocol.Components.MeadowTag));
            var meadowEntities = meadowQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
            if (meadowEntities.Length > 0)
            {
                meadowEntity = meadowEntities[0];
            }
            meadowEntities.Dispose();
        }

        void CreateMeadowVisual()
        {
            meadowGameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meadowGameObject.name = "Meadow";
            meadowGameObject.transform.position = new Vector3(0, 0, 0);
            meadowGameObject.transform.localScale = new Vector3(10, 1, 10);

            var renderer = meadowGameObject.GetComponent<Renderer>();
            // Use standard shader or create simple material
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard") ?? Shader.Find("Diffuse");
            var material = new Material(shader);
            material.color = new Color(0.2f, 0.8f, 0.2f); // Green color
            renderer.material = material;
        }

        void CreateCubeVisual()
        {
            cubeGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubeGameObject.name = "PlayerCube";
            cubeGameObject.transform.position = new Vector3(0, 0.5f, 0);
            cubeGameObject.transform.localScale = Vector3.one;

            var renderer = cubeGameObject.GetComponent<Renderer>();
            // Use standard shader or create simple material
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard") ?? Shader.Find("Diffuse");
            var material = new Material(shader);
            material.color = new Color(0.8f, 0.2f, 0.2f); // Red color
            renderer.material = material;
        }

        void CreateSelectionBox()
        {
            selectionBox = new GameObject("SelectionBox");
            var lineRenderer = selectionBox.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.yellow;
            lineRenderer.endColor = Color.yellow;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.positionCount = 5;
            lineRenderer.loop = true;
            lineRenderer.useWorldSpace = true;
            selectionBox.SetActive(false);
        }

        void Update()
        {
            if (!entityManager.Exists(cubeEntity))
                return;

            // Sync cube position from ECS to GameObject
            if (entityManager.HasComponent<LocalTransform>(cubeEntity))
            {
                var transform = entityManager.GetComponentData<LocalTransform>(cubeEntity);
                cubeGameObject.transform.position = new Vector3(transform.Position.x, transform.Position.y, transform.Position.z);
                cubeGameObject.transform.rotation = new Quaternion(transform.Rotation.value.x, transform.Rotation.value.y, transform.Rotation.value.z, transform.Rotation.value.w);
                cubeGameObject.transform.localScale = Vector3.one * transform.Scale;
            }

            // Update selection box
            bool isSelected = entityManager.HasComponent<BlindfoldProtocol.Components.Selected>(cubeEntity);
            selectionBox.SetActive(isSelected);

            if (isSelected)
            {
                UpdateSelectionBox();
            }
        }

        void UpdateSelectionBox()
        {
            var cubePos = cubeGameObject.transform.position;
            var size = 0.6f;
            var height = 0.5f;

            var lineRenderer = selectionBox.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, cubePos + new Vector3(-size, height, -size));
            lineRenderer.SetPosition(1, cubePos + new Vector3(size, height, -size));
            lineRenderer.SetPosition(2, cubePos + new Vector3(size, height, size));
            lineRenderer.SetPosition(3, cubePos + new Vector3(-size, height, size));
            lineRenderer.SetPosition(4, cubePos + new Vector3(-size, height, -size));
        }
    }
}
