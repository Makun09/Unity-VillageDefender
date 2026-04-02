using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components.Building
{
    public struct TowerAttack : IComponentData
    {
        public float Range;
        public float Damage;
        public float FireRate;
        public float ProjectileSpeed;
        public float ProjectileHitRadius;
        public int FireCount;
        public bool ProjectileStraight;
        public Entity ProjectilePrefab;
    }
    
    public struct TowerCoolDown : IComponentData
    {
        public float TimeLeft;
    }
    
    public struct Projectile : IComponentData
    {
        public Entity Target;
        public float Speed;
        public float HitRadius;
        public float Damage;
        public bool Straight;
        public float3 Direction;
        public float RemainingLifetime;
    }
}