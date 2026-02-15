using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components
{
    [ChunkSerializable]
    public struct GoblinSpawnPoint : IComponentData
    {
        public NativeArray<float3> Value;
    }
}
