using ECS.Authorings.Enemy.Goblin;
using ECS.Components.Goblin;
using ECS.Components.Enemy.AgressiveGoblin;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Systems.Enemy.AgressiveGoblin
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(SimpleGoblin.GoblinRiseSystem))]
    public partial struct GoblinWalkSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GoblinWalkProperties>();
            state.RequireForUpdate<GoblinTargetPosition>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var deltaTime = SystemAPI.Time.DeltaTime;
            var targetPosition = SystemAPI.GetSingleton<GoblinTargetPosition>().Value;
            
            new GoblinWalkJob
            {
                DeltaTime = deltaTime,
                StopDistanceSq = 0.5f,
                TargetPosition = targetPosition,
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
        public float3 TargetPosition;
        public EntityCommandBuffer.ParallelWriter ECB;
        
        private void Execute(GoblinWalkAspect goblin, [EntityIndexInQuery] int sortKey)
        {
            goblin.SetHeading(TargetPosition);
            
            goblin.Walk(DeltaTime);
            
            if (goblin.IsInStoppingRange(goblin.Heading, StopDistanceSq))
            {
                ECB.AddComponent<GoblinReachedTarget>(sortKey, goblin.Entity);
            }
        }
    }
}