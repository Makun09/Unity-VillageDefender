using Unity.Entities;

namespace ECS.Components.Enemy.AgressiveGoblin
{
    public struct GoblinTowerAttack : IComponentData
    {
        public float Damage;
        public float Range;
        public float Interval;
    }

    public struct GoblinTowerAttackCooldown : IComponentData
    {
        public float TimeLeft;
    }
}

