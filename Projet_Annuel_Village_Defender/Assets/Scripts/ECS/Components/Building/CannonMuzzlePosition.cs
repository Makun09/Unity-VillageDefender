using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components.Building
{
    /// <summary>
    /// Position world de la bouche du canon.
    /// Écrit par CannonOrigin (MonoBehaviour), lu par TowerFireSystem comme
    /// point de départ des boulets à la place de la position ECS de la tour.
    /// </summary>
    public struct CannonMuzzlePosition : IComponentData
    {
        public float3 WorldPosition;
    }
}
