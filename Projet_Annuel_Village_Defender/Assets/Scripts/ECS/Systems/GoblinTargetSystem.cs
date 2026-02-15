using ECS.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace ECS.Systems
{
    /// <summary>
    /// System that updates the heading of all goblins to target the village position
    /// This runs after GoblinRiseSystem and before GoblinWalkSystem
    /// </summary>
    [BurstCompile]
    [UpdateAfter(typeof(GoblinRiseSystem))]
    [UpdateBefore(typeof(GoblinWalkSystem))]
    public partial struct GoblinTargetSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            // Require the village to exist before running
            state.RequireForUpdate<VillageTag>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Get the village entity and its position
            var villageEntity = SystemAPI.GetSingletonEntity<VillageTag>();
            var villageTransform = SystemAPI.GetComponent<LocalTransform>(villageEntity);
            var villagePosition = villageTransform.Position;

            // Update all goblins' heading to point towards the village
            foreach (var heading in SystemAPI.Query<RefRW<GoblinHeading>>())
            {
                heading.ValueRW.Value = villagePosition;
            }
        }
    }
}
