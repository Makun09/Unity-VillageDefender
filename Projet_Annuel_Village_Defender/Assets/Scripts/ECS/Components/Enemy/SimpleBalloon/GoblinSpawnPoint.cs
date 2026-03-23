using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components
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
