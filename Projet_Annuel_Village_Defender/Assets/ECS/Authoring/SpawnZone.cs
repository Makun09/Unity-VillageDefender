using ECS.Components;
using Unity.Collections;
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
    }
    
    public class SpawnZoneBaker : Baker <SpawnZone>
    {
        public override void Bake(SpawnZone authoring)
        {
            AddComponent(new SpawnZoneProperties
            {
                FieldDimensions = authoring.FieldDimensions,
                NumberSpawnPoints = authoring.NumberSpawnPoints,
                EnemySpawnPrefab = GetEntity(authoring.SpawnPrefab),
                BasicGoblinPrefab = GetEntity(authoring.BasicGoblinPrefab),
                GoblinSpawnRate = authoring.GoblinSpawnRate
            });
            AddComponent(new SpawnZoneRandom
            {
                Value = Random.CreateFromIndex(authoring.RandomSeed),
            });
            AddComponent<GoblinSpawnPoint>();
            AddComponent<GoblinSpawnTimer>();
        }
    }
}