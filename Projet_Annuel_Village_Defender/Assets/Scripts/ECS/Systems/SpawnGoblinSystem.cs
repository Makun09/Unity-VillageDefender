using ECS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Systems
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
                SpawnZoneEntity = spawnZoneEntity
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct SpawnGoblinJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;
        [ReadOnly]
        public NativeArray<float3> GoblinSpawnPoints;
        public Entity SpawnZoneEntity;

        private void Execute(in SpawnZoneProperties spawnZoneProperties, [EntityIndexInQuery] int entityIndexInQuery, RefRW<GoblinSpawnTimer> goblinSpawnTimer, RefRW<SpawnZoneRandom> spawnZoneRandom, RefRO<LocalTransform> transform)
        {
            goblinSpawnTimer.ValueRW.Value -= DeltaTime;
            if (goblinSpawnTimer.ValueRW.Value > 0f) return;
            if (GoblinSpawnPoints.Length == 0) return;

            goblinSpawnTimer.ValueRW.Value = spawnZoneProperties.GoblinSpawnRate;
            var newGoblin = ECB.Instantiate(entityIndexInQuery, spawnZoneProperties.BasicGoblinPrefab);

            var spawnPointIndex = spawnZoneRandom.ValueRW.Value.NextInt(0, GoblinSpawnPoints.Length);
            var spawnPosition = GoblinSpawnPoints[spawnPointIndex];

            var newGoblinTransform = new LocalTransform
            {
                Position = spawnPosition,
                Rotation = quaternion.identity,
                Scale = 1f
            };
            ECB.SetComponent(entityIndexInQuery, newGoblin, newGoblinTransform);
        }
    }
}
