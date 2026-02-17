using Unity.Entities;
using Unity.Transforms;

namespace ECS.Components.Tower
{
    public readonly partial struct TowerSpawnAspect : IAspect
    {
        public readonly Entity Entity;
        
        private readonly RefRW<LocalTransform> _transform;
        
    }
}