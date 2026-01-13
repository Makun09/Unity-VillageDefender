using ECS.Authoring;
using ECS.Components;
using Unity.Burst;
using Unity.Entities;

namespace ECS.Systems
{
    [BurstCompile]
    [UpdateAfter(typeof(SpawnGoblinSystem))]
    public partial struct GoblinRiseSystem : ISystem
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
            var deltaTime = SystemAPI.Time.DeltaTime;
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            new GoblinRiseSystemJob
            {
                DeltaTime = deltaTime,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct GoblinRiseSystemJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;
        [BurstCompile]
        private void Execute(GoblinRiseAspect goblin,[EntityIndexInQuery]int SortKey)
        {
            goblin.Rise(DeltaTime);
            if(!goblin.IsAboveGround) return;
            
            goblin.SetAtGroundLevel();
            ECB.RemoveComponent<GoblinRiseRate>(SortKey, goblin.Entity);
            
        }
    }
}