using ECS.Components.Enemy.SimpleGoblin;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

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
            state.RequireForUpdate<GoblinBounty>();
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

        private void Execute(Entity entity, [EntityIndexInQuery] int sortKey, in GoblinHealth health, in GoblinBounty bounty)
        {
            if (health.Value <= 0)
            {
                var rewardEntity = ECB.CreateEntity(sortKey);
                ECB.AddComponent(sortKey, rewardEntity, new ECS.Components.Enemy.SimpleGoblin.GoblinDeathRewardEvent
                {
                    Value = math.max(0, bounty.Value)
                });
                ECB.DestroyEntity(sortKey, entity);
            }
        }
    }


}