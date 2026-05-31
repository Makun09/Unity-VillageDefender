using ECS.Components.Enemy.SimpleGoblin;
using Player;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Systems.Enemy.SimpleGoblin
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(GoblinDeathSystem))]
    public partial struct GoblinRewardPayoutSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GoblinDeathRewardEvent>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            var totalReward = 0;

            foreach (var (reward, entity) in SystemAPI.Query<RefRO<GoblinDeathRewardEvent>>().WithEntityAccess())
            {
                totalReward += math.max(0, reward.ValueRO.Value);
                ecb.DestroyEntity(entity);
            }

            if (totalReward > 0)
            {
                PlayerMoneyManager.Instance?.AddMoney(totalReward);
            }
        }
    }
}

