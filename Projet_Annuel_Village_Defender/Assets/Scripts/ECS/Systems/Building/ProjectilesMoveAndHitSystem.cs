using ECS.Components.Building;
using ECS.Components.Enemy.SimpleGoblin;
using ECS.Systems.Enemy.AgressiveGoblin;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Systems.Building
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(GoblinWalkSystem))]
    [UpdateAfter(typeof(TowerFireSystem))]
    [UpdateBefore(typeof(ECS.Systems.Enemy.SimpleGoblin.GoblinDeathSystem))]
    public partial struct ProjectileMoveAndHitSystem : ISystem
    {
        private ComponentLookup<GoblinHealth> _healthLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Projectile>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();

            _healthLookup = state.GetComponentLookup<GoblinHealth>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _healthLookup.Update(ref state);

            var targetCount = SystemAPI.QueryBuilder()
                .WithAll<LocalTransform, GoblinHealth>()
                .WithNone<Prefab, Disabled>()
                .Build()
                .CalculateEntityCount();

            var targetPositions = new NativeParallelHashMap<Entity, float3>(math.max(1, targetCount), Allocator.TempJob);
            foreach (var (transform, health, entity) in
                     SystemAPI.Query<RefRO<LocalTransform>, RefRO<GoblinHealth>>()
                         .WithNone<Prefab, Disabled>()
                         .WithEntityAccess())
            {
                if (health.ValueRO.Value <= 0f) continue;
                targetPositions.TryAdd(entity, transform.ValueRO.Position);
            }

            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

            state.Dependency = new ProjectileMoveAndHitJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                TargetPositions = targetPositions,
                HealthLookup = _healthLookup,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged)
            }.Schedule(state.Dependency); // non-parallèle pour éviter les conflits d'écriture sur GoblinHealth

            state.Dependency = targetPositions.Dispose(state.Dependency);
        }
    }

    [BurstCompile]
    public partial struct ProjectileMoveAndHitJob : IJobEntity
    {
        public float DeltaTime;
        [ReadOnly] public NativeParallelHashMap<Entity, float3> TargetPositions;
        public ComponentLookup<GoblinHealth> HealthLookup;
        public EntityCommandBuffer ECB;

        private void Execute(Entity projectileEntity, ref LocalTransform projectileTransform, ref Projectile projectile)
        {
            projectile.RemainingLifetime -= DeltaTime;
            if (projectile.RemainingLifetime <= 0f)
            {
                ECB.DestroyEntity(projectileEntity);
                return;
            }

            var target = projectile.Target;

            if (!TargetPositions.TryGetValue(target, out var targetPos) || !HealthLookup.HasComponent(target))
            {
                ECB.DestroyEntity(projectileEntity);
                return;
            }

            var health = HealthLookup[target];
            if (health.Value <= 0f)
            {
                ECB.DestroyEntity(projectileEntity);
                return;
            }

            var hitRadius = math.max(0.001f, projectile.HitRadius);
            var startPos = projectileTransform.Position;
            var speed = math.max(0.01f, projectile.Speed);
            var step = speed * DeltaTime;

            if (math.distancesq(startPos, targetPos) <= hitRadius * hitRadius)
            {
                health.Value -= projectile.Damage;
                HealthLookup[target] = health;
                ECB.DestroyEntity(projectileEntity);
                return;
            }

            var moveDirection = projectile.Straight
                ? math.normalizesafe(projectile.Direction, new float3(0f, 0f, 1f))
                : math.normalizesafe(targetPos - startPos, new float3(0f, 0f, 1f));

            var endPos = startPos + moveDirection * step;

            if (SegmentHitsSphere(startPos, endPos, targetPos, hitRadius))
            {
                health.Value -= projectile.Damage;
                HealthLookup[target] = health;
                ECB.DestroyEntity(projectileEntity);
                return;
            }

            projectileTransform.Position = endPos;
        }

        private static bool SegmentHitsSphere(float3 a, float3 b, float3 center, float radius)
        {
            var ab = b - a;
            var abLenSq = math.lengthsq(ab);
            if (abLenSq < 0.000001f)
            {
                return math.distancesq(a, center) <= radius * radius;
            }

            var t = math.clamp(math.dot(center - a, ab) / abLenSq, 0f, 1f);
            var closest = a + ab * t;
            return math.distancesq(closest, center) <= radius * radius;
        }
    }

}
