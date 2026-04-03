using ECS.Components.Enemy.AgressiveGoblin;
using ECS.Components.Enemy.SimpleGoblin;
using UnityEngine;

namespace ECS.Authoring.Enemy.Goblin
{
    public class Goblin : MonoBehaviour
    {
        public float riseRate;
        public float walkSpeed;
        public float groundSnapOffset;
        public int hasTarget;
        public int canWalk;
        public float targetHeight;
        public float maxHealth;
        public int deathReward = 10;
        public float towerDamage = 5f;
        public float towerAttackRange = 1.2f;
        public float towerAttackInterval = 1f;
    }
    
    public class GoblinBaker : Unity.Entities.Baker<Goblin>
    {
        public override void Bake(Goblin authoring)
        {
            var entity = GetEntity(Unity.Entities.TransformUsageFlags.Dynamic);
            AddComponent(entity, new GoblinRiseRate
            {
                Value = authoring.riseRate,
                TargetHeight = authoring.targetHeight
            });
            AddComponent(entity, new GoblinHealth
            {
                Value = authoring.maxHealth
            });
            AddComponent(entity, new GoblinBounty
            {
                Value = Mathf.Max(0, authoring.deathReward)
            });
            if (authoring.hasTarget == 1 || authoring.canWalk == 1)
            {
                AddComponent(entity, new GoblinWalkProperties
                {
                    WalkSpeed = authoring.walkSpeed,
                    GroundSnapOffset = authoring.groundSnapOffset
                });
                SetComponentEnabled<GoblinWalkProperties>(entity, false);
                AddComponent(entity, new GoblinHeading());
                SetComponentEnabled<GoblinHeading>(entity, false);

                AddComponent(entity, new GoblinTowerAttack
                {
                    Damage = authoring.towerDamage,
                    Range = authoring.towerAttackRange,
                    Interval = authoring.towerAttackInterval
                });
                AddComponent(entity, new GoblinTowerAttackCooldown
                {
                    TimeLeft = 0f
                });
            }
        }
    }
}