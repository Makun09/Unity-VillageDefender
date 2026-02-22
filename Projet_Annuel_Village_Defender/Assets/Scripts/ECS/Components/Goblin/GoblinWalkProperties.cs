using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components.Goblin
{
    public struct GoblinWalkProperties : IComponentData , IEnableableComponent
    {
        public float WalkSpeed;
    }
    
    public struct GoblinHeading : IComponentData
    {
        public float3 Value;
    }
}