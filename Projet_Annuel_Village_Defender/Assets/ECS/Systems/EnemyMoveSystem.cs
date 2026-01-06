using ECS.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Systems
{
    [BurstCompile]
    public partial struct EnemyMoveSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            UnityEngine.Debug.Log("EnemyMoveSystem running");

            float dt = SystemAPI.Time.DeltaTime;

            foreach (var (transform, speed)
                     in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveSpeed>>()
                         .WithAll<EnemyTag>())
            {
                transform.ValueRW.Position +=
                    new float3(0f, 0f, 1f) * speed.ValueRO.Value * dt;
            }
        }
    }
}