using Unity.Entities;

namespace ECS.Components
{
    public struct GoblinRiseRate : IComponentData
    {
        public float Value;
        public float TargetHeight; // The Y position where the goblin should stop rising
    }
}