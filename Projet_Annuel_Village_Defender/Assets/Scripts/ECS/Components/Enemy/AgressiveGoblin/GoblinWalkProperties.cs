using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components.Enemy.AgressiveGoblin
{
    public struct GoblinWalkProperties : IComponentData , IEnableableComponent
    {
        public float WalkSpeed;
    }
    
    public struct GoblinHeading : IComponentData , IEnableableComponent
    {
        public float3 Value;
    }
}