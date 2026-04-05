using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components.Enemy.SimpleGoblin
{
    public struct SpawnZoneProperties : IComponentData
    {
        public float2 FieldDimensions;
        public int NumberSpawnPoints;
        public Entity EnemySpawnPrefab;
        public Entity BasicGoblinPrefab;
        public float GoblinSpawnRate;

        public int BaseGoblinsPerWave;
        public int ExtraGoblinsPerWave;
        public float TimeBetweenWaves;
    }

    public struct GoblinSpawnTimer : IComponentData
    {
        public float Value;
    }
}