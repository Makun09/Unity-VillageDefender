using ECS.Components.Enemy.AgressiveGoblin;
using ECS.Components.Enemy.SimpleGoblin;
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
        private ComponentLookup<GoblinGravityState> _gravityStateLookup;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            _canWalkLookup = state.GetComponentLookup<GoblinWalkProperties>(true);
            _gravityStateLookup = state.GetComponentLookup<GoblinGravityState>(true);
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            _canWalkLookup.Update(ref state);
            _gravityStateLookup.Update(ref state);
            new GoblinRiseSystemJob
            {
                DeltaTime = deltaTime,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                CanWalk = _canWalkLookup,
                GravityStateLookup = _gravityStateLookup,
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct GoblinRiseSystemJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;
        [ReadOnly] public ComponentLookup<GoblinWalkProperties> CanWalk;
        [ReadOnly] public ComponentLookup<GoblinGravityState> GravityStateLookup;

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

                    if (GravityStateLookup.HasComponent(goblin.Entity))
                    {
                        var gravityState = GravityStateLookup[goblin.Entity];
                        gravityState.VerticalSpeed = 0f;
                        ECB.SetComponent(sortKey, goblin.Entity, gravityState);
                        ECB.SetComponentEnabled<GoblinGravityState>(sortKey, goblin.Entity, true);
                    }
                }
                else
                {
                    ECB.DestroyEntity(sortKey, goblin.Entity);
                }
            }
        }
    }
}
