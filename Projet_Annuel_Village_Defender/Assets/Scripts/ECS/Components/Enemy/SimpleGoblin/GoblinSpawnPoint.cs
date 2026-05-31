using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components.Enemy.SimpleGoblin
{
    public struct GoblinSpawnData : IComponentData
    {
        public float3 SpawnPosition;  
        public float TargetHeight;
    }
}
