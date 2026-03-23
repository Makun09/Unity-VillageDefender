using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Authorings.Enemy.Goblin
{
    public struct GoblinTargetPosition : IComponentData
    {
        public float3 Value;
    }
}