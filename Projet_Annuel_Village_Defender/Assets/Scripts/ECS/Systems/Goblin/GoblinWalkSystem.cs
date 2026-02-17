using ECS.Components;
using Unity.Burst;
using Unity.Entities;

namespace ECS.Systems
{
    /// <summary>
    /// System that makes goblins walk towards their target (heading).
    /// Only processes goblins with enabled GoblinWalkProperties.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(GoblinRiseSystem))]
    public partial struct GoblinWalkSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GoblinWalkProperties>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
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
            
            new GoblinWalkJob
            {
                DeltaTime = deltaTime,
                StopDistanceSq = 0.5f * 0.5f, // Distance d'arrêt au carré (0.5 unités)
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    [WithAll(typeof(GoblinWalkProperties))] // Only process entities with enabled GoblinWalkProperties
    public partial struct GoblinWalkJob : IJobEntity
    {
        public float DeltaTime;
        public float StopDistanceSq;
        public EntityCommandBuffer.ParallelWriter ECB;
        
        [BurstCompile]
        private void Execute(GoblinWalkAspect goblin, [EntityIndexInQuery] int sortKey)
        {
            goblin.Walk(DeltaTime);
            // Vérifie si le gobelin est assez proche de sa cible (Heading)
            if (goblin.IsInStoppingRange(goblin.Heading, StopDistanceSq))
            {
                ECB.SetComponentEnabled<GoblinWalkProperties>(sortKey, goblin.Entity, false);
            }
        }
    }
}