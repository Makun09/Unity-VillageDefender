using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components.Enemy.SimpleGoblin
{
    public struct GoblinSpawnData
    {
        public float3 SpawnPosition;  
        public float TargetHeight;   
    }
    
    [ChunkSerializable]
    public struct GoblinSpawnPoint : IComponentData
    {
        public NativeArray<GoblinSpawnData> Value;
    }
}
