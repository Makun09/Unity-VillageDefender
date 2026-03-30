using ECS.Components.Enemy.AgressiveGoblin;
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
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GoblinWalkProperties>();
            state.RequireForUpdate<GoblinTargetTag>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var deltaTime = SystemAPI.Time.DeltaTime;

            var targetPositions = new NativeList<float3>(Allocator.TempJob);
            foreach (var targetTransform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<GoblinTargetTag>())
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
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            }.ScheduleParallel(state.Dependency);

            state.Dependency = targetPositions.Dispose(state.Dependency);
        }
    }

    [BurstCompile]
    [WithAll(typeof(GoblinWalkProperties))]
    [WithNone(typeof(GoblinReachedTarget))]
    public partial struct GoblinWalkJob : IJobEntity
    {
        public float DeltaTime;
        public float StopDistanceSq;
        [ReadOnly] public NativeArray<float3> TargetPositions;
        public EntityCommandBuffer.ParallelWriter ECB;
        
        private void Execute(GoblinWalkAspect goblin, [EntityIndexInQuery] int sortKey)
        {
            var nearestTarget = TargetPositions[0];
            var nearestDistanceSq = math.distancesq(goblin.Position, nearestTarget);

            for (var i = 1; i < TargetPositions.Length; i++)
            {
                var candidateTarget = TargetPositions[i];
                var candidateDistanceSq = math.distancesq(goblin.Position, candidateTarget);
                if (candidateDistanceSq < nearestDistanceSq)
                {
                    nearestDistanceSq = candidateDistanceSq;
                    nearestTarget = candidateTarget;
                }
            }

            goblin.SetHeading(nearestTarget);
            
            goblin.Walk(DeltaTime);
            
            if (nearestDistanceSq <= StopDistanceSq)
            {
                ECB.AddComponent<GoblinReachedTarget>(sortKey, goblin.Entity);
            }
        }
    }
}