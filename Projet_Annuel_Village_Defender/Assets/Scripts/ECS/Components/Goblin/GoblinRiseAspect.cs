using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Components
{
    public readonly partial struct GoblinRiseAspect : IAspect
    {
        public readonly Entity Entity;
        
        private readonly RefRW<LocalTransform> _transform;
        private readonly RefRO<GoblinRiseRate> _goblinRiseRate;
        
        public void Rise(float deltaTime)
        {
            _transform.ValueRW.Position += math.up() * _goblinRiseRate.ValueRO.Value * deltaTime;
        }
        
        public bool IsAboveGround => _transform.ValueRO.Position.y >= _goblinRiseRate.ValueRO.TargetHeight;

        public void SetAtGroundLevel()
        {
            var position = _transform.ValueRW.Position;
            position.y = _goblinRiseRate.ValueRO.TargetHeight;
            _transform.ValueRW.Position = position;
        }
    }
}