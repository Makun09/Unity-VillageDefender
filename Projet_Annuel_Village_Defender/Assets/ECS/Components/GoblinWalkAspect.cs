using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Components
{
    public readonly partial struct GoblinWalkAspect : IAspect
    {
        public  readonly Entity Entity;
        private readonly RefRW<LocalTransform> _transform;
        private readonly RefRO<GoblinWalkProperties> _goblinWalkSpeed;
        private readonly RefRO<GoblinHeading> _goblinHeading;
        
        private float WalkSpeed => _goblinWalkSpeed.ValueRO.WalkSpeed;
        private float3 Heading => _goblinHeading.ValueRO.Value;
        
        public void Walk(float deltaTime)
        {
            _transform.ValueRW.Position += _transform.ValueRO.Forward() * WalkSpeed * deltaTime; // Move forward in the direction the goblin is facing
        }
        
        public bool IsInStoppingRange(float3 targetPosition, float stoppingDistance)
        {
            return math.distancesq(targetPosition, _transform.ValueRO.Position) <= stoppingDistance ; 
        }
    }
}