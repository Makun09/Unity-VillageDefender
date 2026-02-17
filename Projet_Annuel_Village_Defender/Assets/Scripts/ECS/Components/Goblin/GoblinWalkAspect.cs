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
        public float3 Heading => _goblinHeading.ValueRO.Value;
        public float3 Position => _transform.ValueRO.Position;
        
        public void Walk(float deltaTime)
        {
            // Calcule la direction vers le heading (cible)
            float3 direction = math.normalizesafe(Heading - _transform.ValueRO.Position);
            
            // Si la direction est valide, on avance vers elle
            if (math.lengthsq(direction) > 0.001f)
            {
                // Déplace le gobelin vers la direction cible
                _transform.ValueRW.Position += direction * WalkSpeed * deltaTime;
                
                // Fait tourner le gobelin pour qu'il regarde vers la direction de déplacement
                _transform.ValueRW.Rotation = quaternion.LookRotationSafe(direction, math.up());
            }
        }
        
        public bool IsInStoppingRange(float3 targetPosition, float stoppingDistance)
        {
            return math.distancesq(targetPosition, _transform.ValueRO.Position) <= stoppingDistance ; 
        }
    }
}