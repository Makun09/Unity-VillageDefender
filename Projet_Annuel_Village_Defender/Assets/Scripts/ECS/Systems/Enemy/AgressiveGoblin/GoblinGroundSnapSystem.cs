using ECS.Components.Enemy.AgressiveGoblin;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Systems.Enemy.AgressiveGoblin
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(GoblinWalkSystem))]
    public partial struct GoblinGroundSnapSystem : ISystem
    {
        private static readonly string GroundTag = "Ground";
        private const float RaycastStartOffset = 4f;
        private const float RaycastDistance = 50f;
        private const float GroundSnapTolerance = 0.25f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GoblinWalkProperties>();
            state.RequireForUpdate<GoblinGravityState>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (transform, walkProperties, gravityState) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<GoblinWalkProperties>, RefRW<GoblinGravityState>>())
            {
                var position = transform.ValueRO.Position;
                var origin = new Vector3(position.x, position.y + RaycastStartOffset, position.z);
                var verticalSpeed = gravityState.ValueRO.VerticalSpeed - gravityState.ValueRO.GravityAcceleration * deltaTime;

                if (gravityState.ValueRO.MaxFallSpeed > 0f)
                {
                    verticalSpeed = math.max(verticalSpeed, -gravityState.ValueRO.MaxFallSpeed);
                }

                var nextY = position.y + verticalSpeed * deltaTime;

                if (!Physics.Raycast(origin, Vector3.down, out var hit, RaycastDistance, Physics.DefaultRaycastLayers,
                        QueryTriggerInteraction.Ignore))
                {
                    position.y = nextY;
                    transform.ValueRW.Position = position;
                    var stateValue = gravityState.ValueRO;
                    stateValue.VerticalSpeed = verticalSpeed;
                    gravityState.ValueRW = stateValue;
                    continue;
                }

                if (hit.collider == null || !hit.collider.CompareTag(GroundTag))
                {
                    position.y = nextY;
                    transform.ValueRW.Position = position;
                    var stateValue = gravityState.ValueRO;
                    stateValue.VerticalSpeed = verticalSpeed;
                    gravityState.ValueRW = stateValue;
                    continue;
                }

                var groundY = hit.point.y + walkProperties.ValueRO.GroundSnapOffset;

                if (nextY <= groundY || position.y <= groundY + GroundSnapTolerance)
                {
                    position.y = groundY;
                    verticalSpeed = 0f;
                }
                else
                {
                    position.y = nextY;
                }

                transform.ValueRW.Position = position;
                var gravityStateValue = gravityState.ValueRO;
                gravityStateValue.VerticalSpeed = verticalSpeed;
                gravityState.ValueRW = gravityStateValue;
            }
        }
    }
}


