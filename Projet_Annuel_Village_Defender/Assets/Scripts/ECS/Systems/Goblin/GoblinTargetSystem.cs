using ECS.Components;
using ECS.Components.Tower;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Systems
{
    [BurstCompile]
    [UpdateAfter(typeof(GoblinRiseSystem))]
    [UpdateBefore(typeof(GoblinWalkSystem))]
    public partial struct GoblinTargetSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            // Require the village to exist before running
            state.RequireForUpdate<TowerTag>();
            state.RequireForUpdate<VillageTag>();
        }


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            
            foreach (var (heading, transform) in SystemAPI.Query<RefRW<GoblinHeading>, RefRO<LocalTransform>>())
            {
                var goblinPosition = transform.ValueRO.Position;
                float3 closestTower = float3.zero;
                float distMin = float.MaxValue;

                foreach (var towerTransform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<TowerTag>())
                {
                    var towerPosition = towerTransform.ValueRO.Position;
                    float distanceSq = math.distancesq(goblinPosition, towerPosition); // on calcul au carre pour optimiser car ba chiffre au carre si il est inferieur a autre truc au carre c inferieur aussi a autre truc en tant normal ca nous evite de faire une racine carree

                    if (distanceSq < distMin)
                    {
                        distMin = distanceSq;
                        closestTower = towerPosition;
                    }
                }

                heading.ValueRW.Value = closestTower;
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}
