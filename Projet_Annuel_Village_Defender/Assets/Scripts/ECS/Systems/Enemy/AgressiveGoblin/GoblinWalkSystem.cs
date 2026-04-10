using ECS.Components.Enemy.AgressiveGoblin;
using ECS.Components.Building;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Systems.Enemy.AgressiveGoblin
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(SimpleGoblin.GoblinRiseSystem))]
    public partial struct GoblinWalkSystem : ISystem
    {
        private ComponentLookup<GoblinReachedTarget> _reachedTargetLookup;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GoblinWalkProperties>();
            state.RequireForUpdate<GoblinTargetTag>();
            state.RequireForUpdate<BuildingTag>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            _reachedTargetLookup = state.GetComponentLookup<GoblinReachedTarget>(true);
        }
        
        public void OnUpdate(ref SystemState state)
        {
            _reachedTargetLookup.Update(ref state);

            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var deltaTime = SystemAPI.Time.DeltaTime;

            var targetPositions = new NativeList<float3>(Allocator.TempJob);
            foreach (var targetTransform in SystemAPI.Query<RefRO<LocalTransform>>()
                         .WithAll<GoblinTargetTag, BuildingTag>()
                         .WithNone<Prefab, Disabled>())
            {
                targetPositions.Add(targetTransform.ValueRO.Position);
            }

            if (targetPositions.Length == 0)
            {
                targetPositions.Dispose();
                return;
            }
            
            state.Dependency = new GoblinWalkJob
            {
                DeltaTime = deltaTime,
                StopDistanceSq = 0.5f,
                TargetPositions = targetPositions.AsArray(),
                ReachedTargetLookup = _reachedTargetLookup,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            }.ScheduleParallel(state.Dependency);

            state.Dependency = targetPositions.Dispose(state.Dependency);
        }
    }

    [BurstCompile]
    [WithAll(typeof(GoblinWalkProperties))]
    public partial struct GoblinWalkJob : IJobEntity
    {
        public float DeltaTime;
        public float StopDistanceSq;
        [ReadOnly] public NativeArray<float3> TargetPositions;
        [ReadOnly] public ComponentLookup<GoblinReachedTarget> ReachedTargetLookup;
        public EntityCommandBuffer.ParallelWriter ECB;

        private static float HorizontalDistanceSq(float3 from, float3 to)
        {
            var delta = to - from;
            delta.y = 0f;
            return math.lengthsq(delta);
        }

        private static float3 ProjectToGoblinHeight(float3 target, float goblinY)
        {
            target.y = goblinY;
            return target;
        }
        
        private void Execute(Entity entity, ref LocalTransform transform, ref GoblinHeading heading, in GoblinWalkProperties walk, [EntityIndexInQuery] int sortKey)
        {
            var goblinPosition = transform.Position;
            var nearestTarget = ProjectToGoblinHeight(TargetPositions[0], goblinPosition.y);
            var nearestDistanceSq = HorizontalDistanceSq(goblinPosition, nearestTarget);

            for (var i = 1; i < TargetPositions.Length; i++)
            {
                var candidateTarget = ProjectToGoblinHeight(TargetPositions[i], goblinPosition.y);
                var candidateDistanceSq = HorizontalDistanceSq(goblinPosition, candidateTarget);

                if (candidateDistanceSq < nearestDistanceSq)
                {
                    nearestDistanceSq = candidateDistanceSq;
                    nearestTarget = candidateTarget;
                }
            }

            var hasReachedTargetTag = ReachedTargetLookup.HasComponent(entity);
            if (nearestDistanceSq <= StopDistanceSq)
            {
                if (!hasReachedTargetTag)
                {
                    ECB.AddComponent<GoblinReachedTarget>(sortKey, entity);
                }
                return;
            }

            if (hasReachedTargetTag)
            {
                ECB.RemoveComponent<GoblinReachedTarget>(sortKey, entity);
            }

            heading.Value = nearestTarget;

            var toTarget = heading.Value - transform.Position;
            toTarget.y = 0f;
            var direction = math.normalizesafe(toTarget);

            if (math.lengthsq(direction) > 0.001f)
            {
                transform.Position += direction * walk.WalkSpeed * DeltaTime;
            }
        }
    }
}