﻿using ECS.Components;
using ECS.Components.Goblin;
using UnityEngine;

namespace ECS.Authoring
{
    public class Goblin : MonoBehaviour
    {
        public float riseRate;
        public float walkSpeed;
        public int hasTarget;
        public int canWalk;
        public float targetHeight;
    }
    
    public class GoblinBaker : Unity.Entities.Baker<Goblin>
    {
        public override void Bake(Goblin authoring)
        {
            var entity = GetEntity(Unity.Entities.TransformUsageFlags.Dynamic);
            AddComponent(entity, new ECS.Components.GoblinRiseRate
            {
                Value = authoring.riseRate,
                TargetHeight = authoring.targetHeight
            });
            if (authoring.hasTarget == 1 || authoring.canWalk == 1)
            {
                AddComponent(entity, new GoblinWalkProperties
                {
                    WalkSpeed = authoring.walkSpeed
                });
                SetComponentEnabled<GoblinWalkProperties>(entity, false);
                AddComponent(entity, new GoblinHeading());
                SetComponentEnabled<GoblinHeading>(entity, false);
            }
        }
    }
}