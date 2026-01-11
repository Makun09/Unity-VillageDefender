using ECS.Helpers;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Components
{
    public readonly partial struct SpawnZoneAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRW<LocalTransform> _transform;

        private readonly RefRO<SpawnZoneProperties> _spawnZoneProperties;
        private readonly RefRW<SpawnZoneRandom> _spawnZoneRandom;
        private readonly RefRW<GoblinSpawnPoint> _goblinSpawnPoints;
        private readonly RefRW<GoblinSpawnTimer> _goblinSpawnTimer;
        
        private const float SAFETY_RADIUS = 100f;
        public int NumberSpawnPointToSpawn => _spawnZoneProperties.ValueRO.NumberSpawnPoints;
        public Entity SpawnPointPrefab => _spawnZoneProperties.ValueRO.EnemySpawnPrefab;

        public NativeArray<float3> GoblinSpawnPoints
        {
            get => _goblinSpawnPoints.ValueRW.Value;
            set => _goblinSpawnPoints.ValueRW.Value = value;
        }
        public LocalTransform GetRandomSpawnPointTransform()
        {
            return new LocalTransform
            {
                Position = GetRandomPosition(),
                Rotation = GetRandomRotation(),
                Scale = GetRandomScale(0.5f)
            };
        }

        private float3 GetRandomPosition()
        {
            float3 randomPosition;
            do
            {
                randomPosition = _spawnZoneRandom.ValueRW.Value.NextFloat3(MinCorner, MaxCorner);
            }while (math.distancesq(_transform.ValueRO.Position,randomPosition) < SAFETY_RADIUS); // Ensure spawn point is not too close to center/village
            return randomPosition;
        }

        private float3 HalfDimensions => new ()
        {
            x =_spawnZoneProperties.ValueRO.FieldDimensions.x * 0.5f,
            y = 0f,
            z = _spawnZoneProperties.ValueRO.FieldDimensions.y * 0.5f
        };
        private float3 MinCorner => _transform.ValueRO.Position - HalfDimensions;
        private float3 MaxCorner => _transform.ValueRO.Position + HalfDimensions;
        public float3 Position => _transform.ValueRO.Position;
        
        private quaternion GetRandomRotation() => quaternion.RotateY(_spawnZoneRandom.ValueRW.Value.NextFloat(-0.25f,0.25f));
        private float GetRandomScale(float min) => _spawnZoneRandom.ValueRW.Value.NextFloat(min, 1f);
        
        public float GoblinSpawnTimer
        {
            get => _goblinSpawnTimer.ValueRO.Value;
            set => _goblinSpawnTimer.ValueRW.Value = value;
        }
        
        public bool TimeToSpawnGoblin => GoblinSpawnTimer <= 0f;
        
        public float GoblinSpawnRate => _spawnZoneProperties.ValueRO.GoblinSpawnRate;
        
        public Entity BasicGoblinPrefab => _spawnZoneProperties.ValueRO.BasicGoblinPrefab;
        
        public LocalTransform GetGoblinSpawnPoint()
        {
            var position = GetRandomGoblinSpawnPoint();
            return new LocalTransform
            {
                Position = position,
                Rotation = quaternion.RotateY(MathHelpers.GetHeading(position, _transform.ValueRO.Position)),
                Scale = 1f
            };
        }

        private float3 GetRandomGoblinSpawnPoint()
        {
            return GoblinSpawnPoints[_spawnZoneRandom.ValueRW.Value.NextInt(0, GoblinSpawnPoints.Length)];
        }
    }
}