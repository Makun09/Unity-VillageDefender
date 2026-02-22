using ECS.Components;
using ECS.Components.Goblin;
using Unity.Burst;
using Unity.Entities;

namespace ECS.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(GoblinTargetSystem))]
    public partial struct GoblinWalkSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GoblinWalkProperties>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var deltaTime = SystemAPI.Time.DeltaTime;
            
            new GoblinWalkJob
            {
                DeltaTime = deltaTime,
                StopDistanceSq = 0.5f * 0.5f,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    [WithAll(typeof(GoblinWalkProperties))]
    public partial struct GoblinWalkJob : IJobEntity
    {
        public float DeltaTime;
        public float StopDistanceSq;
        public EntityCommandBuffer.ParallelWriter ECB;
        
        private void Execute(GoblinWalkAspect goblin, [EntityIndexInQuery] int sortKey)
        {
            goblin.Walk(DeltaTime);
            
            if (goblin.IsInStoppingRange(goblin.Heading, StopDistanceSq))
            {
                ECB.SetComponentEnabled<GoblinWalkProperties>(sortKey, goblin.Entity, false);
            }
        }
    }
}