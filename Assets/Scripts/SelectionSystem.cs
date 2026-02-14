using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace BlindfoldProtocol.Systems
{
    /// <summary>
    /// System that handles mouse selection of the cube
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class SelectionSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            // Check for left mouse button click
            if (Input.GetMouseButtonDown(0))
            {
                HandleSelection();
            }
        }

        private void HandleSelection()
        {
            var camera = Camera.main;
            if (camera == null)
                return;

            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // First, deselect all entities
            var entityManager = EntityManager;
            foreach (var (cubeTag, entity) in SystemAPI.Query<RefRO<BlindfoldProtocol.Components.CubeTag>>().WithAll<BlindfoldProtocol.Components.Selected>().WithEntityAccess())
            {
                entityManager.RemoveComponent<BlindfoldProtocol.Components.Selected>(entity);
            }

            // Perform raycast to check if cube was clicked
            if (Physics.Raycast(ray, out hit, 100f))
            {
                // Check if we hit an entity with CubeTag
                // In a full implementation, we'd need to track game objects to entities
                // For now, we'll select the cube if any collider was hit
                foreach (var (cubeTag, entity) in SystemAPI.Query<RefRO<BlindfoldProtocol.Components.CubeTag>>().WithEntityAccess())
                {
                    if (!entityManager.HasComponent<BlindfoldProtocol.Components.Selected>(entity))
                    {
                        entityManager.AddComponent<BlindfoldProtocol.Components.Selected>(entity);
                        Debug.Log("Cube selected");
                    }
                }
            }
        }
    }
}
