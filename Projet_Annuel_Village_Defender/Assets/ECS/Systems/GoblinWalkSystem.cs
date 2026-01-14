using ECS.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Systems
{
    [BurstCompile]
    [UpdateAfter(typeof(GoblinRiseSystem))]
    public partial struct GoblinWalkSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var deltaTime = SystemAPI.Time.DeltaTime;
            //var targetEntity = SystemAPI.GetSingletonEntity<TowerTag>(); // Example target entity if needed
            
            new GoblinWalkJob
            {
                DeltaTime = deltaTime,
                StopDistance = 5.5f * 5.5f,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct GoblinWalkJob : IJobEntity
    {
        public float DeltaTime;
        public float StopDistance;
        public EntityCommandBuffer.ParallelWriter ECB;
        
        [BurstCompile]
        private void Execute(GoblinWalkAspect goblin, [EntityIndexInQuery] int SortKey)
        {
            goblin.Walk(DeltaTime);
            if (goblin.IsInStoppingRange(float3.zero, StopDistance))
            {
                ECB.SetComponentEnabled<GoblinWalkProperties>(SortKey, goblin.Entity, false);
            }
        }
    }
}