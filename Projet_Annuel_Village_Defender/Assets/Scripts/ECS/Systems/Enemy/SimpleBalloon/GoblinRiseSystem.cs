using ECS.Components;
using ECS.Components.Goblin;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECS.Systems.Enemy.SimpleGoblin
{
    [BurstCompile]
    [UpdateAfter(typeof(SpawnGoblinSystem))]
    public partial struct GoblinRiseSystem : ISystem
    {
        private ComponentLookup<GoblinWalkProperties> _canWalkLookup;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            _canWalkLookup = state.GetComponentLookup<GoblinWalkProperties>(true);
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            _canWalkLookup.Update(ref state);
            new GoblinRiseSystemJob
            {
                DeltaTime = deltaTime,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                CanWalk = _canWalkLookup,
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct GoblinRiseSystemJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;
        [ReadOnly] public ComponentLookup<GoblinWalkProperties> CanWalk;

        private void Execute(GoblinRiseAspect goblin, [EntityIndexInQuery] int sortKey)
        {
            goblin.Rise(DeltaTime);

            if (goblin.IsAboveLimit)
            {
                ECB.RemoveComponent<GoblinRiseRate>(sortKey, goblin.Entity);
                if (CanWalk.HasComponent(goblin.Entity))
                {
                    ECB.SetComponentEnabled<GoblinWalkProperties>(sortKey, goblin.Entity, true);
                    ECB.SetComponentEnabled<GoblinHeading>(sortKey, goblin.Entity, true);
                }
                else
                {
                    ECB.DestroyEntity(sortKey, goblin.Entity);
                }
            }
        }
    }
}
