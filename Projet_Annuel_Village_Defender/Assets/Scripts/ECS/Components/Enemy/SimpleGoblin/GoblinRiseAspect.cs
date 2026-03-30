using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Components.Enemy.SimpleGoblin
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
        
        public bool IsAboveLimit => _transform.ValueRO.Position.y >= _goblinRiseRate.ValueRO.TargetHeight;
        
    }
}