using ECS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECS.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct InitializeGoblinSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var goblin in SystemAPI.Query<GoblinWalkAspect>().WithAll<NewGoblinTag>())
            {
                ecb.RemoveComponent<NewGoblinTag>(goblin.Entity);
                ecb.SetComponentEnabled<GoblinWalkProperties>(goblin.Entity, false);
            }
            ecb.Playback(state.EntityManager);
        }
    }
}