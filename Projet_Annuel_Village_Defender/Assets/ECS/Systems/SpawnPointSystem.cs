using ECS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct SpawnPointSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SpawnZoneProperties>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            var spawnZoneEntity = SystemAPI.GetSingletonEntity<SpawnZoneProperties>();
            var spawnZone = SystemAPI.GetAspect<SpawnZoneAspect>(spawnZoneEntity);

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var spawnPoints = new NativeList<float3>(Allocator.Persistent);
            var spawnPointOffset = new float3(0f, -2f, 1f);

            for (var i = 0; i < spawnZone.NumberSpawnPointToSpawn; i++)
            {
                var newSpawnPoint = ecb.Instantiate(spawnZone.SpawnPointPrefab);
                var newSpawnPointTransform = spawnZone.GetRandomSpawnPointTransform();
                ecb.SetComponent(newSpawnPoint, newSpawnPointTransform);
                var newGobelinSpawnPoint = newSpawnPointTransform.Position + spawnPointOffset;
                spawnPoints.Add(newGobelinSpawnPoint);
            }

            spawnZone.GoblinSpawnPoints = spawnPoints.AsArray();
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}