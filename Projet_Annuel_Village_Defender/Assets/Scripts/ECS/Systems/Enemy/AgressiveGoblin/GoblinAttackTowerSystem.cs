using ECS.Components.Building;
using ECS.Components.Enemy.AgressiveGoblin;
using ECS.Systems.Building;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Systems.Enemy.AgressiveGoblin
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(GoblinWalkSystem))]
    [UpdateBefore(typeof(TowerFireSystem))]
    public partial struct GoblinAttackTowerSystem : ISystem
    {
        private ComponentLookup<BuildingHealth> _buildingHealthLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GoblinTowerAttack>();
            state.RequireForUpdate<BuildingHealth>();

            _buildingHealthLookup = state.GetComponentLookup<BuildingHealth>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _buildingHealthLookup.Update(ref state);

            var towerEntities = new NativeList<Entity>(Allocator.TempJob);
            var towerPositions = new NativeList<float3>(Allocator.TempJob);

            foreach (var (transform, health, entity) in SystemAPI
                         .Query<RefRO<LocalTransform>, RefRO<BuildingHealth>>()
                         .WithAll<BuildingTag, GoblinTargetTag>()
                         .WithNone<Prefab, Disabled>()
                         .WithEntityAccess())
            {
                if (health.ValueRO.Value <= 0f) continue;
                towerEntities.Add(entity);
                towerPositions.Add(transform.ValueRO.Position);
            }

            if (towerEntities.Length == 0)
            {
                towerEntities.Dispose();
                towerPositions.Dispose();
                return;
            }

            state.Dependency = new GoblinAttackTowerJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                TowerEntities = towerEntities.AsArray(),
                TowerPositions = towerPositions.AsArray(),
                BuildingHealthLookup = _buildingHealthLookup
            }.Schedule(state.Dependency); // non-parallele pour eviter les ecritures concurrentes sur BuildingHealth

            state.Dependency = towerEntities.Dispose(state.Dependency);
            state.Dependency = towerPositions.Dispose(state.Dependency);
        }
    }

    [BurstCompile]
    [WithNone(typeof(Prefab), typeof(Disabled))]
    public partial struct GoblinAttackTowerJob : IJobEntity
    {
        public float DeltaTime;
        [ReadOnly] public NativeArray<Entity> TowerEntities;
        [ReadOnly] public NativeArray<float3> TowerPositions;
        public ComponentLookup<BuildingHealth> BuildingHealthLookup;

        private static float HorizontalDistanceSq(float3 from, float3 to)
        {
            var delta = to - from;
            delta.y = 0f;
            return math.lengthsq(delta);
        }

        private void Execute(in LocalTransform goblinTransform, in GoblinTowerAttack attack, ref GoblinTowerAttackCooldown cooldown)
        {
            cooldown.TimeLeft = math.max(0f, cooldown.TimeLeft - DeltaTime);

            var rangeSq = attack.Range * attack.Range;
            var nearestIndex = -1;
            var nearestDistSq = float.MaxValue;

            for (var i = 0; i < TowerPositions.Length; i++)
            {
                var distSq = HorizontalDistanceSq(goblinTransform.Position, TowerPositions[i]);
                if (distSq < nearestDistSq)
                {
                    nearestDistSq = distSq;
                    nearestIndex = i;
                }
            }

            if (nearestIndex < 0 || nearestDistSq > rangeSq || cooldown.TimeLeft > 0f)
            {
                return;
            }

            var towerEntity = TowerEntities[nearestIndex];
            if (!BuildingHealthLookup.HasComponent(towerEntity))
            {
                return;
            }

            var health = BuildingHealthLookup[towerEntity];
            if (health.Value <= 0f)
            {
                return;
            }

            health.Value -= attack.Damage;
            BuildingHealthLookup[towerEntity] = health;

            cooldown.TimeLeft = math.max(0.05f, attack.Interval);
        }
    }
}

