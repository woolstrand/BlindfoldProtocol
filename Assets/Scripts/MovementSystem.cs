using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace BlindfoldProtocol.Systems
{
    /// <summary>
    /// System that handles right-click movement commands
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(SelectionSystem))]
    public partial class MovementInputSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            // Check for right mouse button click
            if (Input.GetMouseButtonDown(1))
            {
                HandleMovementCommand();
            }

            // Update movement
            float deltaTime = SystemAPI.Time.DeltaTime;
            foreach (var (transform, moveTo, entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<BlindfoldProtocol.Components.MoveTo>>().WithEntityAccess())
            {
                float3 currentPos = transform.ValueRO.Position;
                float3 targetPos = moveTo.ValueRO.TargetPosition;
                float speed = moveTo.ValueRO.Speed;

                float3 direction = targetPos - currentPos;
                float distance = math.length(direction);

                if (distance < 0.1f)
                {
                    // Reached target
                    transform.ValueRW.Position = targetPos;
                    EntityManager.RemoveComponent<BlindfoldProtocol.Components.MoveTo>(entity);
                }
                else
                {
                    // Move towards target
                    float3 moveDir = math.normalize(direction);
                    float moveDistance = math.min(speed * deltaTime, distance);
                    transform.ValueRW.Position = currentPos + moveDir * moveDistance;
                }
            }
        }

        private void HandleMovementCommand()
        {
            var camera = Camera.main;
            if (camera == null)
                return;

            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Raycast to find the target position on the ground
            if (Physics.Raycast(ray, out hit, 100f))
            {
                var entityManager = EntityManager;

                // Add MoveTo component to selected entities
                foreach (var (cubeTag, entity) in SystemAPI.Query<RefRO<BlindfoldProtocol.Components.CubeTag>>().WithAll<BlindfoldProtocol.Components.Selected>().WithEntityAccess())
                {
                    var moveTo = new BlindfoldProtocol.Components.MoveTo
                    {
                        TargetPosition = new float3(hit.point.x, 0.5f, hit.point.z), // Keep cube at 0.5 height
                        Speed = 5f
                    };

                    if (entityManager.HasComponent<BlindfoldProtocol.Components.MoveTo>(entity))
                    {
                        entityManager.SetComponentData(entity, moveTo);
                    }
                    else
                    {
                        entityManager.AddComponentData(entity, moveTo);
                    }

                    Debug.Log($"Move command to position: {hit.point}");
                }
            }
        }
    }
}
