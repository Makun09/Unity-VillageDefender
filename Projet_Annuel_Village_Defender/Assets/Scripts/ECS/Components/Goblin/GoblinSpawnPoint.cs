using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components
{
    /// <summary>
    /// Data for a single goblin spawn point
    /// </summary>
    public struct GoblinSpawnData
    {
        public float3 SpawnPosition;  // Position under ground where goblin spawns
        public float TargetHeight;     // Y position where goblin should stop rising (ground level)
    }
    
    [ChunkSerializable]
    public struct GoblinSpawnPoint : IComponentData
    {
        public NativeArray<GoblinSpawnData> Value;
    }
}
