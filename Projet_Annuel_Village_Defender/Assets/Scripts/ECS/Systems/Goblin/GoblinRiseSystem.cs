using ECS.Components;
using ECS.Components.Goblin;
using Unity.Burst;
using Unity.Entities;

namespace ECS.Systems
{
    [BurstCompile]
    [UpdateAfter(typeof(SpawnGoblinSystem))]
    public partial struct GoblinRiseSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            new GoblinRiseSystemJob
            {
                DeltaTime = deltaTime,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct GoblinRiseSystemJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;
        
        private void Execute(GoblinRiseAspect goblin, [EntityIndexInQuery] int sortKey)
        {
            goblin.Rise(DeltaTime);
            
            if (!goblin.IsAboveGround)
            {
                return;
            }
            
            goblin.SetAtGroundLevel();
            ECB.RemoveComponent<GoblinRiseRate>(sortKey, goblin.Entity);
            ECB.SetComponentEnabled<GoblinWalkProperties>(sortKey, goblin.Entity, true);
        }
    }
}