using ECS.Components;
using ECS.Components.Goblin;
using ECS.Components.Tower;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

namespace ECS.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(GoblinRiseSystem))]
    [UpdateBefore(typeof(GoblinWalkSystem))]
    public partial struct GoblinTargetSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TowerTag>();
            state.RequireForUpdate<VillageTag>();
            state.RequireForUpdate<GoblinHeading>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var towerQuery = SystemAPI.QueryBuilder().WithAll<TowerTag, LocalTransform>().Build();
            var towerCount = towerQuery.CalculateEntityCount();
            
            if (towerCount == 0)
            {
                return;
            }
            
            var towerPositions = new NativeArray<float3>(towerCount, Allocator.TempJob);
            
            int index = 0;
            foreach (var towerTransform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<TowerTag>())
            {
                towerPositions[index] = towerTransform.ValueRO.Position;
                index++;
            }
            
            state.Dependency = new GoblinFindTargetJob
            {
                TowerPositions = towerPositions
            }.ScheduleParallel(state.Dependency);
            
            towerPositions.Dispose(state.Dependency);
        }

        public void OnDestroy(ref SystemState state)
        {
        }
    }
    
    [BurstCompile]
    public partial struct GoblinFindTargetJob : IJobEntity
    {
        [ReadOnly] public NativeArray<float3> TowerPositions;

        public void Execute(ref GoblinHeading heading, in LocalTransform transform)
        {
            var goblinPosition = transform.Position;
            float3 closestTower = TowerPositions[0];
            float distMin = float.MaxValue;
    
            for (int i = 0; i < TowerPositions.Length; i++)
            {
                float distanceSq = math.distancesq(goblinPosition, TowerPositions[i]);
        
                if (distanceSq < distMin)
                {
                    distMin = distanceSq;
                    closestTower = TowerPositions[i];
                }
            }
    
            heading.Value = closestTower;
        }
    }
}
