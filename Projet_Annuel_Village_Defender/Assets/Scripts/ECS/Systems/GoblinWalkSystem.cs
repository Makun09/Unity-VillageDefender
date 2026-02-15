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
                StopDistanceSq = 0.5f * 0.5f, // Distance d'arrêt au carré (1.5 unités)
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct GoblinWalkJob : IJobEntity
    {
        public float DeltaTime;
        public float StopDistanceSq;
        public EntityCommandBuffer.ParallelWriter ECB;
        
        [BurstCompile]
        private void Execute(GoblinWalkAspect goblin, [EntityIndexInQuery] int SortKey)
        {
            goblin.Walk(DeltaTime);
            // Vérifie si le gobelin est assez proche de sa cible (Heading)
            if (goblin.IsInStoppingRange(goblin.Heading, StopDistanceSq))
            {
                ECB.SetComponentEnabled<GoblinWalkProperties>(SortKey, goblin.Entity, false);
            }
        }
    }
}