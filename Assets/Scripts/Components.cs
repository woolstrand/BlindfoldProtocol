using Unity.Entities;
using Unity.Mathematics;

namespace BlindfoldProtocol.Components
{
    /// <summary>
    /// Component tag for the player cube
    /// </summary>
    public struct CubeTag : IComponentData
    {
    }

    /// <summary>
    /// Component indicating the entity is selected
    /// </summary>
    public struct Selected : IComponentData
    {
    }

    /// <summary>
    /// Component for entities that can move to a target position
    /// </summary>
    public struct MoveTo : IComponentData
    {
        public float3 TargetPosition;
        public float Speed;
    }

    /// <summary>
    /// Component for meadow ground
    /// </summary>
    public struct MeadowTag : IComponentData
    {
    }
}
