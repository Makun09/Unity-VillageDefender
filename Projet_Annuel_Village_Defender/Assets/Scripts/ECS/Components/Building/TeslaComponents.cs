using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components.Building
{
    /// <summary>
    /// Marker placed on Tesla-type tower entities.
    /// Tells TeslaSystem to apply continuous arc damage instead of projectiles.
    /// </summary>
    public struct TeslaTag : IComponentData { }

    /// <summary>
    /// Dynamic buffer written by TeslaSystem each frame.
    /// Contains world positions of currently arced targets so TeslaArcVisual
    /// can draw the lightning lines without querying ECS itself.
    /// </summary>
    public struct TeslaCurrentTarget : IBufferElementData
    {
        public float3 Position;
        public Entity GoblinEntity;
    }
}
