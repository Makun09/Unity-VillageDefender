using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components
{
    public struct GoblinWalkProperties : IComponentData , IEnableableComponent
    {
        public float WalkSpeed;
    }
    
    public struct GoblinHeading : IComponentData
    {
        public float3 Value;
    }
    
    public struct NewGoblinTag : IComponentData {}
}