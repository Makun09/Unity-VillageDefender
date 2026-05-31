using Unity.Entities;

namespace ECS.Components.Enemy.SimpleGoblin
{
    public struct GoblinRiseRate : IComponentData, IEnableableComponent
    {
        public float Value;
        public float TargetHeight;
    }
}