using ECS.Components.Enemy.AgressiveGoblin;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Systems.Enemy.AgressiveGoblin
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(GoblinWalkSystem))]
    public partial struct GoblinGroundSnapSystem : ISystem
    {
        private static readonly string GroundTag = "Ground";
        private const float RaycastStartOffset = 2f;
        private const float RaycastDistance = 20f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GoblinWalkProperties>();
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (transform, walkProperties) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<GoblinWalkProperties>>()
                         .WithAll<GoblinWalkProperties>()
                         .WithNone<GoblinReachedTarget>())
            {
                var position = transform.ValueRO.Position;
                var origin = new Vector3(position.x, position.y + RaycastStartOffset, position.z);

                if (!Physics.Raycast(origin, Vector3.down, out var hit, RaycastDistance, Physics.DefaultRaycastLayers,
                        QueryTriggerInteraction.Ignore))
                {
                    continue;
                }

                if (hit.collider == null || !hit.collider.CompareTag(GroundTag))
                {
                    continue;
                }

                position.y = hit.point.y + walkProperties.ValueRO.GroundSnapOffset;
                transform.ValueRW.Position = position;
            }
        }
    }
}


