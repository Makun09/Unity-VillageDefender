using ECS.Components.Enemy.SimpleGoblin;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

namespace ECS.Systems.Enemy.SimpleGoblin
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]

    public partial struct GoblinDeathSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GoblinHealth>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

            state.Dependency = new GoblinDeathJob
            {
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            }.ScheduleParallel(state.Dependency);            
        }
    }

    [BurstCompile]
    public partial struct GoblinDeathJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;
        
        private void Execute(Entity entity, [EntityIndexInQuery] int sortKey, in GoblinHealth health)
        {
            if (health.Value <= 0)
            {
                ECB.DestroyEntity(sortKey, entity);
            }
        }
    }
    
    
}