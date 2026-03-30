using ECS.Components.Enemy.SimpleGoblin;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Systems.Enemy.SimpleGoblin
{
    public partial struct SpawnGoblinSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SpawnZoneProperties>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            var spawnZoneEntity = SystemAPI.GetSingletonEntity<SpawnZoneProperties>();
            var spawnZone = SystemAPI.GetAspect<SpawnZoneAspect>(spawnZoneEntity);

            if (spawnZone.GoblinSpawnPoints.Length == 0) return;

            var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
            new SpawnGoblinJob
            {
                DeltaTime = deltaTime,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                GoblinSpawnPoints = spawnZone.GoblinSpawnPoints,
                SpawnZoneEntity = spawnZoneEntity,
                GoblinRiseRateLookup = state.GetComponentLookup<GoblinRiseRate>(true)
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct SpawnGoblinJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;
        [ReadOnly] public NativeArray<GoblinSpawnData> GoblinSpawnPoints;
        public Entity SpawnZoneEntity;
        [ReadOnly] public ComponentLookup<GoblinRiseRate> GoblinRiseRateLookup;

        private void Execute(in SpawnZoneProperties spawnZoneProperties, [EntityIndexInQuery] int entityIndexInQuery, RefRW<GoblinSpawnTimer> goblinSpawnTimer, RefRW<SpawnZoneRandom> spawnZoneRandom)
        {
            goblinSpawnTimer.ValueRW.Value -= DeltaTime;
            if (goblinSpawnTimer.ValueRW.Value > 0f) return;
            if (GoblinSpawnPoints.Length == 0) return;

            goblinSpawnTimer.ValueRW.Value = spawnZoneProperties.GoblinSpawnRate;
            var newGoblin = ECB.Instantiate(entityIndexInQuery, spawnZoneProperties.BasicGoblinPrefab);

            var spawnPointIndex = spawnZoneRandom.ValueRW.Value.NextInt(0, GoblinSpawnPoints.Length);
            var spawnData = GoblinSpawnPoints[spawnPointIndex];

            var newGoblinTransform = new LocalTransform
            {
                Position = spawnData.SpawnPosition,
                Rotation = quaternion.identity,
                Scale = 1f
            };
            ECB.SetComponent(entityIndexInQuery, newGoblin, newGoblinTransform);

            // Read both riseRate and targetHeight from the prefab (baked by GoblinBaker from the Goblin inspector)
            var prefabRiseRate = GoblinRiseRateLookup[spawnZoneProperties.BasicGoblinPrefab];
            ECB.SetComponent(entityIndexInQuery, newGoblin, new GoblinRiseRate
            {
                Value = prefabRiseRate.Value,
                TargetHeight = prefabRiseRate.TargetHeight
            });
        }
    }
}
