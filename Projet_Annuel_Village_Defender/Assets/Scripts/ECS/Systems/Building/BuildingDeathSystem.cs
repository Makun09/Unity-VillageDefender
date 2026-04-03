using ECS.Components.Building;
using ECS.Systems.Enemy.AgressiveGoblin;
using Unity.Burst;
using Unity.Entities;

namespace ECS.Systems.Building
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(GoblinAttackTowerSystem))]
    [UpdateBefore(typeof(TowerFireSystem))]
    public partial struct BuildingDeathSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BuildingHealth>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

            state.Dependency = new BuildingDeathJob
            {
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            }.ScheduleParallel(state.Dependency);
        }
    }

    [BurstCompile]
    public partial struct BuildingDeathJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;

        private void Execute(Entity entity, [EntityIndexInQuery] int sortKey, in BuildingHealth health)
        {
            if (health.Value <= 0f)
            {
                ECB.DestroyEntity(sortKey, entity);
            }
        }
    }
}

