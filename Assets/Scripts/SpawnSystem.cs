using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;

namespace BlindfoldProtocol.Systems
{
    /// <summary>
    /// System that spawns the initial game entities (meadow and cube)
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class SpawnSystem : SystemBase
    {
        private bool hasSpawned = false;

        protected override void OnUpdate()
        {
            if (hasSpawned)
                return;

            hasSpawned = true;

            // Create meadow (green plane)
            CreateMeadow();

            // Create player cube
            CreateCube();
        }

        private void CreateMeadow()
        {
            var entityManager = EntityManager;

            // Create meadow entity
            var meadowEntity = entityManager.CreateEntity();

            entityManager.AddComponentData(meadowEntity, new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            });

            entityManager.AddComponent<BlindfoldProtocol.Components.MeadowTag>(meadowEntity);

            Debug.Log("Meadow created");
        }

        private void CreateCube()
        {
            var entityManager = EntityManager;

            // Create cube entity
            var cubeEntity = entityManager.CreateEntity();

            entityManager.AddComponentData(cubeEntity, new LocalTransform
            {
                Position = new float3(0, 0.5f, 0), // Slightly above ground
                Rotation = quaternion.identity,
                Scale = 1f
            });

            entityManager.AddComponent<BlindfoldProtocol.Components.CubeTag>(cubeEntity);

            Debug.Log("Cube created");
        }
    }
}
