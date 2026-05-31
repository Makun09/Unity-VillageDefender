using Unity.Entities;

namespace ECS.Components.Enemy.AgressiveGoblin
{
    public struct GoblinGravityState : IComponentData, IEnableableComponent
    {
        public float VerticalSpeed;
        public float GravityAcceleration;
        public float MaxFallSpeed;
    }
}

