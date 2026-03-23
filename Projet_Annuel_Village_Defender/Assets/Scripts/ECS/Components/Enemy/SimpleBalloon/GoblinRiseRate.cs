using Unity.Entities;

namespace ECS.Components
{
    public struct GoblinRiseRate : IComponentData, IEnableableComponent
    {
        public float Value;
        public float TargetHeight;
    }
}