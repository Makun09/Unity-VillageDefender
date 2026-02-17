using ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace ECS.Authoring
{
    public class SpawnZone : MonoBehaviour
    {
        public float2 FieldDimensions;
        public int NumberSpawnPoints;
        public GameObject SpawnPrefab;
        public uint RandomSeed;
        public GameObject BasicGoblinPrefab;
        public float GoblinSpawnRate = 2f;
        public float GoblinRiseRate = 1f;
    }
    
    public class SpawnZoneBaker : Baker<SpawnZone>
    {
        public override void Bake(SpawnZone authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SpawnZoneProperties
            {
                FieldDimensions = authoring.FieldDimensions,
                NumberSpawnPoints = authoring.NumberSpawnPoints,
                EnemySpawnPrefab = GetEntity(authoring.SpawnPrefab, TransformUsageFlags.Dynamic),
                BasicGoblinPrefab = GetEntity(authoring.BasicGoblinPrefab, TransformUsageFlags.Dynamic),
                GoblinSpawnRate = authoring.GoblinSpawnRate,
                GoblinRiseRate = authoring.GoblinRiseRate
            });
            AddComponent(entity, new SpawnZoneRandom
            {
                Value = Random.CreateFromIndex(authoring.RandomSeed),
            });
            AddComponent<GoblinSpawnPoint>(entity);
            AddComponent<GoblinSpawnTimer>(entity);
        }
    }
}