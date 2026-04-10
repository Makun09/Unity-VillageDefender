using ECS.Components.Enemy.SimpleGoblin;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace ECS.Authoring.Enemy
{
    public class SpawnZone : MonoBehaviour
    {
        public float2 fieldDimensions;
        public int numberSpawnPoints;
        public GameObject spawnPrefab;
        public uint randomSeed;
        public GameObject basicGoblinPrefab;
        public float goblinSpawnRate = 2f;
        public int baseGoblinsPerWave = 5;
        public int extraGoblinsPerWave = 5;
        public float timeBetweenWaves = 20f;
    }
    
    public class SpawnZoneBaker : Baker<SpawnZone>
    {
        public override void Bake(SpawnZone authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SpawnZoneProperties
            {
                FieldDimensions = authoring.fieldDimensions,
                NumberSpawnPoints = authoring.numberSpawnPoints,
                EnemySpawnPrefab = GetEntity(authoring.spawnPrefab, TransformUsageFlags.Dynamic),
                BasicGoblinPrefab = GetEntity(authoring.basicGoblinPrefab, TransformUsageFlags.Dynamic),
                GoblinSpawnRate = authoring.goblinSpawnRate,
                BaseGoblinsPerWave = math.max(1, authoring.baseGoblinsPerWave),
                ExtraGoblinsPerWave = math.max(0, authoring.extraGoblinsPerWave),
                TimeBetweenWaves = math.max(0f, authoring.timeBetweenWaves)
            });
            AddComponent(entity, new SpawnZoneRandom
            {
                Value = Random.CreateFromIndex(authoring.randomSeed),
            });
            AddComponent<GoblinSpawnTimer>(entity);
            AddComponent(entity, new GoblinWaveState
            {
                WaveIndex = 1,
                SpawnedThisWave = 0,
                TargetThisWave = math.max(1, authoring.baseGoblinsPerWave),
                SpawnCooldown = 0f,
                InterWaveCooldown = 0f,
                WaitingNextWave = 0
            });
        }
    }
}