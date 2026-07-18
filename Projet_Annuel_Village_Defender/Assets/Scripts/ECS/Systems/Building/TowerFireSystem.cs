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
    public partial struct TowerFireSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TowerAttack>();
            state.RequireForUpdate<GoblinHealth>();
            state.RequireForUpdate<BuildingHealth>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var deltaTime = SystemAPI.Time.DeltaTime;

            var goblinEntities = new NativeList<Entity>(Allocator.TempJob);
            var goblinPositions = new NativeList<float3>(Allocator.TempJob);

            foreach (var (transform, health, entity) in
                     SystemAPI.Query<RefRO<LocalTransform>, RefRO<GoblinHealth>>().WithNone<Prefab, Disabled>().WithEntityAccess())
            {
                if (health.ValueRO.Value <= 0f) continue;
                goblinEntities.Add(entity);
                goblinPositions.Add(transform.ValueRO.Position);
            }


            if (goblinEntities.Length == 0)
            {
                goblinEntities.Dispose();
                goblinPositions.Dispose();
                return;
            }

            state.Dependency = new TowerFireJob
            {
                DeltaTime = deltaTime,
                GoblinEntities = goblinEntities.AsArray(),
                GoblinPositions = goblinPositions.AsArray(),
                MuzzleLookup = SystemAPI.GetComponentLookup<CannonMuzzlePosition>(true),
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            }.ScheduleParallel(state.Dependency);

            state.Dependency = goblinEntities.Dispose(state.Dependency);
            state.Dependency = goblinPositions.Dispose(state.Dependency);
        }
    }

    [BurstCompile]
    public partial struct TowerFireJob : IJobEntity
    {
        public float DeltaTime;

        [ReadOnly] public NativeArray<Entity> GoblinEntities;
        [ReadOnly] public NativeArray<float3> GoblinPositions;
        [ReadOnly] public ComponentLookup<CannonMuzzlePosition> MuzzleLookup;

        public EntityCommandBuffer.ParallelWriter ECB;

        private void Execute(
            [EntityIndexInQuery] int sortKey,
            Entity entity,
            in LocalTransform towerTransform,
            in TowerAttack towerAttack,
            in BuildingHealth towerHealth,
            ref TowerCoolDown towerCoolDown)
        {
            if (towerHealth.Value <= 0f) return;

            towerCoolDown.TimeLeft -= DeltaTime;
            if (towerCoolDown.TimeLeft > 0f) return;

            var rangeSq = towerAttack.Range * towerAttack.Range;
            var projectileCount = math.max(1, towerAttack.FireCount);
            var projectileSpeed = math.max(0.01f, towerAttack.ProjectileSpeed);
            var hitRadius = math.max(0.05f, towerAttack.ProjectileHitRadius);
            var usedTargetIndices = new FixedList128Bytes<int>();
            var shotsFired = 0;

            for (var shot = 0; shot < projectileCount; shot++)
            {
                var nearestIndex = -1;
                var nearestDistSq = float.MaxValue;

                for (var i = 0; i < GoblinPositions.Length; i++)
                {
                    var alreadyUsed = false;
                    for (var used = 0; used < usedTargetIndices.Length; used++)
                    {
                        if (usedTargetIndices[used] == i)
                        {
                            alreadyUsed = true;
                            break;
                        }
                    }

                    if (alreadyUsed) continue;

                    var distSq = math.distancesq(towerTransform.Position, GoblinPositions[i]);
                    if (distSq > rangeSq) continue;

                    if (distSq < nearestDistSq)
                    {
                        nearestDistSq = distSq;
                        nearestIndex = i;
                    }
                }

                if (nearestIndex < 0) break;

                usedTargetIndices.Add(nearestIndex);
                shotsFired++;

                var targetPos      = GoblinPositions[nearestIndex];
                // Si un CannonOrigin a enregistré la position de la bouche, on l'utilise
                var spawnOrigin    = MuzzleLookup.TryGetComponent(entity, out var muzzle)
                    ? muzzle.WorldPosition
                    : towerTransform.Position;
                var muzzleDirection = math.normalizesafe(targetPos - spawnOrigin, new float3(0f, 0f, 1f));
                var spawnPosition  = spawnOrigin + muzzleDirection * 0.25f;
                var projectileLifetime = math.max(0.25f, (towerAttack.Range + hitRadius) / projectileSpeed);

                Entity projectile;

                if (towerAttack.ProjectilePrefab != Entity.Null)
                {
                    projectile = ECB.Instantiate(sortKey, towerAttack.ProjectilePrefab);
                    ECB.SetComponent(sortKey, projectile, LocalTransform.FromPosition(spawnPosition));
                }
                else
                {
                    projectile = ECB.CreateEntity(sortKey);
                    ECB.AddComponent(sortKey, projectile, LocalTransform.FromPosition(spawnPosition));
                }

                ECB.AddComponent(sortKey, projectile, new Projectile
                {
                    Target = GoblinEntities[nearestIndex],
                    Speed = projectileSpeed,
                    HitRadius = hitRadius,
                    Damage = towerAttack.Damage,
                    Straight = towerAttack.ProjectileStraight,
                    Direction = muzzleDirection,
                    RemainingLifetime = projectileLifetime
                });
            }

            if (shotsFired == 0)
            {
                towerCoolDown.TimeLeft = 0f;
                return;
            }

            towerCoolDown.TimeLeft = 1f / math.max(0.01f, towerAttack.FireRate);
        }
    }
}
