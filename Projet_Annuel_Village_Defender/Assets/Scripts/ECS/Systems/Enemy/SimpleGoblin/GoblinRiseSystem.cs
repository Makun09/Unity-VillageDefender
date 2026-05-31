using ECS.Components.Enemy.AgressiveGoblin;
using ECS.Components.Enemy.SimpleGoblin;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

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

        private void Execute(Entity entity, ref LocalTransform transform, in GoblinRiseRate riseRate, [EntityIndexInQuery] int sortKey)
        {
            transform.Position += math.up() * riseRate.Value * DeltaTime;

            if (transform.Position.y >= riseRate.TargetHeight)
            {
                ECB.RemoveComponent<GoblinRiseRate>(sortKey, entity);
                if (CanWalk.HasComponent(entity))
                {
                    ECB.SetComponentEnabled<GoblinWalkProperties>(sortKey, entity, true);
                    ECB.SetComponentEnabled<GoblinHeading>(sortKey, entity, true);

                    if (GravityStateLookup.HasComponent(entity))
                    {
                        var gravityState = GravityStateLookup[entity];
                        gravityState.VerticalSpeed = 0f;
                        ECB.SetComponent(sortKey, entity, gravityState);
                        ECB.SetComponentEnabled<GoblinGravityState>(sortKey, entity, true);
                    }
                }
                else
                {
                    ECB.DestroyEntity(sortKey, entity);
                }
            }
        }
    }
}
