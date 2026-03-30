using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Components.Enemy.AgressiveGoblin
{
    public readonly partial struct GoblinWalkAspect : IAspect
    {
        public  readonly Entity Entity;
        private readonly RefRW<LocalTransform> _transform;
        private readonly RefRO<GoblinWalkProperties> _goblinWalkSpeed;
        private readonly RefRW<GoblinHeading> _goblinHeading;
        
        private float WalkSpeed => _goblinWalkSpeed.ValueRO.WalkSpeed;
        public float3 Heading => _goblinHeading.ValueRO.Value;
        public float3 Position => _transform.ValueRO.Position;
        
        public void SetHeading(float3 target)
        {
            _goblinHeading.ValueRW.Value = target;
        }
        
        public void Walk(float deltaTime)
        {
        float3 direction = math.normalizesafe(Heading - _transform.ValueRO.Position);
            
            if (math.lengthsq(direction) > 0.001f)
            {
                _transform.ValueRW.Position += direction * WalkSpeed * deltaTime;
                
            }
        }
        
        public bool IsInStoppingRange(float3 targetPosition, float stoppingDistance)
        {
            return math.distancesq(targetPosition, _transform.ValueRO.Position) <= stoppingDistance ; 
        }
    }
}