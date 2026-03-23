using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components
{
    public struct SpawnZoneProperties : IComponentData
    {
        public float2 FieldDimensions;
        public int NumberSpawnPoints;
        public Entity EnemySpawnPrefab;
        public Entity BasicGoblinPrefab;
        public float GoblinSpawnRate;
    }

    public struct GoblinSpawnTimer : IComponentData
    {
        public float Value;
    }
}