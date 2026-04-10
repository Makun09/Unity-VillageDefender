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
        private ComponentLookup<GoblinRiseRate> _goblinRiseRateLookup;
        private EntityQuery _aliveGoblinsQuery;
        private EntityQuery _spawnPointQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SpawnZoneProperties>();
            state.RequireForUpdate<GoblinWaveState>();
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            _goblinRiseRateLookup = state.GetComponentLookup<GoblinRiseRate>(true);

            _aliveGoblinsQuery = new EntityQueryBuilder(Allocator.Persistent)
                .WithAll<GoblinHealth>()
                .Build(ref state);
            _spawnPointQuery = new EntityQueryBuilder(Allocator.Persistent)
                .WithAll<GoblinSpawnData>()
                .Build(ref state);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _goblinRiseRateLookup.Update(ref state);

            var spawnZoneEntity = SystemAPI.GetSingletonEntity<SpawnZoneProperties>();
            var spawnPoints = _spawnPointQuery.ToComponentDataArray<GoblinSpawnData>(Allocator.Temp);
            if (spawnPoints.Length == 0) return;

            var aliveGoblinsCount = _aliveGoblinsQuery.CalculateEntityCount();
            var deltaTime = SystemAPI.Time.DeltaTime;
            var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (spawnZoneProperties, waveState, spawnZoneRandom) in
                     SystemAPI.Query<RefRO<SpawnZoneProperties>, RefRW<GoblinWaveState>, RefRW<SpawnZoneRandom>>())
            {
                ref var ws = ref waveState.ValueRW;
                var props = spawnZoneProperties.ValueRO;

                if (ws.WaveIndex <= 0)
                {
                    ws.WaveIndex = 1;
                }

                if (ws.TargetThisWave <= 0)
                {
                    ws.TargetThisWave = math.max(1, props.BaseGoblinsPerWave + (ws.WaveIndex - 1) * props.ExtraGoblinsPerWave);
                }

                if (ws.WaitingNextWave == 1)
                {
                    ws.InterWaveCooldown -= deltaTime;
                    if (ws.InterWaveCooldown > 0f)
                    {
                        continue;
                    }

                    ws.WaveIndex += 1;
                    ws.SpawnedThisWave = 0;
                    ws.TargetThisWave = math.max(1, props.BaseGoblinsPerWave + (ws.WaveIndex - 1) * props.ExtraGoblinsPerWave);
                    ws.SpawnCooldown = 0f;
                    ws.InterWaveCooldown = 0f;
                    ws.WaitingNextWave = 0;
                    continue;
                }

                if (ws.SpawnedThisWave >= ws.TargetThisWave)
                {
                    if (aliveGoblinsCount == 0)
                    {
                        ws.WaitingNextWave = 1;
                        ws.InterWaveCooldown = math.max(0f, props.TimeBetweenWaves);
                    }

                    continue;
                }

                ws.SpawnCooldown -= deltaTime;
                if (ws.SpawnCooldown > 0f)
                {
                    continue;
                }

                var newGoblin = ecb.Instantiate(props.BasicGoblinPrefab);

                var random = spawnZoneRandom.ValueRW.Value;
                var spawnPointIndex = random.NextInt(0, spawnPoints.Length);
                spawnZoneRandom.ValueRW.Value = random;
                var spawnData = spawnPoints[spawnPointIndex];

                ecb.SetComponent(newGoblin, new LocalTransform
                {
                    Position = spawnData.SpawnPosition,
                    Rotation = quaternion.identity,
                    Scale = 1f
                });

                var prefabRiseRate = _goblinRiseRateLookup[props.BasicGoblinPrefab];
                ecb.SetComponent(newGoblin, new GoblinRiseRate
                {
                    Value = prefabRiseRate.Value,
                    TargetHeight = prefabRiseRate.TargetHeight
                });

                ws.SpawnedThisWave += 1;
                ws.SpawnCooldown = math.max(0.01f, props.GoblinSpawnRate);
            }
            
            spawnPoints.Dispose();
        }
    }
}
