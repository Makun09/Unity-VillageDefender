using ECS.Components.Building;
using ECS.Components.Enemy.SimpleGoblin;
using ECS.Systems.Enemy.AgressiveGoblin;
using ECS.Systems.Enemy.SimpleGoblin;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Systems.Building
{
    /// <summary>
    /// Processes Tesla towers each frame:
    ///   1. Finds up to FireCount goblins in range.
    ///   2. Applies Damage * deltaTime (continuous DPS) to each.
    ///   3. Fills the TeslaCurrentTarget buffer so TeslaArcVisual can draw lines.
    ///
    /// Uses a single-threaded job (.Schedule) so writes to GoblinHealth are safe
    /// even when multiple towers share the same target.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(GoblinWalkSystem))]
    [UpdateBefore(typeof(GoblinDeathSystem))]
    public partial struct TeslaSystem : ISystem
    {
        private ComponentLookup<GoblinHealth> _goblinHealthLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TeslaTag>();
            state.RequireForUpdate<GoblinHealth>();
            _goblinHealthLookup = state.GetComponentLookup<GoblinHealth>(isReadOnly: false);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _goblinHealthLookup.Update(ref state);

            var deltaTime = SystemAPI.Time.DeltaTime;

            // Collect goblin positions — NO RefRO<GoblinHealth> on main thread.
            // Reading GoblinHealth here AND writing it in the scheduled job creates
            // a DOTS read-write conflict that silently prevents the job from running.
            // Dead-goblin filtering is done inside the job via GoblinHealthLookup.
            var goblinEntities  = new NativeList<Entity>(Allocator.TempJob);
            var goblinPositions = new NativeList<float3>(Allocator.TempJob);

            foreach (var (xform, entity) in
                     SystemAPI.Query<RefRO<LocalTransform>>()
                               .WithAll<GoblinHealth>()
                               .WithNone<Prefab, Disabled>()
                               .WithEntityAccess())
            {
                goblinEntities.Add(entity);
                goblinPositions.Add(xform.ValueRO.Position);
            }

            state.Dependency = new TeslaArcJob
            {
                DeltaTime        = deltaTime,
                GoblinEntities   = goblinEntities.AsArray(),
                GoblinPositions  = goblinPositions.AsArray(),
                GoblinHealthLookup = _goblinHealthLookup
            }.Schedule(state.Dependency);   // single-thread: safe for cross-entity writes

            state.Dependency = goblinEntities.Dispose(state.Dependency);
            state.Dependency = goblinPositions.Dispose(state.Dependency);
        }
    }

    [BurstCompile]
    public partial struct TeslaArcJob : IJobEntity
    {
        public float DeltaTime;

        [ReadOnly] public NativeArray<Entity> GoblinEntities;
        [ReadOnly] public NativeArray<float3> GoblinPositions;

        // Read + Write — single-threaded job (.Schedule), safe.
        // NativeDisableParallelForRestriction is required to satisfy the safety
        // system even though this job never runs in parallel.
        [NativeDisableParallelForRestriction]
        public ComponentLookup<GoblinHealth> GoblinHealthLookup;

        private void Execute(
            in LocalTransform towerTransform,
            in TowerAttack attack,
            in BuildingHealth towerHealth,
            DynamicBuffer<TeslaCurrentTarget> targets)
        {
            targets.Clear();

            if (towerHealth.Value <= 0f || GoblinPositions.Length == 0) return;

            var towerPos    = towerTransform.Position;
            var rangeSq     = attack.Range * attack.Range;
            var maxTargets  = math.max(1, attack.FireCount);
            var damage      = attack.Damage * DeltaTime; // DPS → damage this frame

            // ── Collect alive goblins in range ────────────────────────────────
            var inRange = new FixedList512Bytes<int>();

            for (var i = 0; i < GoblinPositions.Length; i++)
            {
                // Skip dead goblins (health checked here to avoid main-thread read)
                if (!GoblinHealthLookup.TryGetComponent(GoblinEntities[i], out var ghp) || ghp.Value <= 0f)
                    continue;

                if (math.distancesq(towerPos, GoblinPositions[i]) <= rangeSq)
                    inRange.Add(i);
            }

            if (inRange.Length == 0) return;

            // ── Insertion-sort by distance (small list, cheap) ───────────────
            for (var a = 1; a < inRange.Length; a++)
            {
                var key     = inRange[a];
                var keyDist = math.distancesq(towerPos, GoblinPositions[key]);
                var b       = a - 1;

                while (b >= 0 && math.distancesq(towerPos, GoblinPositions[inRange[b]]) > keyDist)
                {
                    inRange[b + 1] = inRange[b];
                    b--;
                }

                inRange[b + 1] = key;
            }

            // ── Apply damage to nearest N targets and fill visual buffer ──────
            var count = math.min(inRange.Length, maxTargets);
            for (var k = 0; k < count; k++)
            {
                var idx    = inRange[k];
                var entity = GoblinEntities[idx];

                if (GoblinHealthLookup.HasComponent(entity))
                {
                    var hp = GoblinHealthLookup.GetRefRW(entity);
                    hp.ValueRW.Value -= damage;
                }

                targets.Add(new TeslaCurrentTarget
                {
                    Position     = GoblinPositions[idx],
                    GoblinEntity = entity
                });
            }
        }
    }
}
