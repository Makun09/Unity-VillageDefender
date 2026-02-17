using ECS.Components;
using UnityEngine;

namespace ECS.Authoring
{
    public class Goblin : MonoBehaviour
    {
        public float riseRate;
        public float walkSpeed;
        public Transform target;
    }
    
    public class GoblinBaker : Unity.Entities.Baker<Goblin>
    {
        public override void Bake(Goblin authoring)
        {
            var entity = GetEntity(Unity.Entities.TransformUsageFlags.Dynamic);
            AddComponent(entity, new ECS.Components.GoblinRiseRate
            {
                Value = authoring.riseRate
            });
            AddComponent(entity, new ECS.Components.GoblinWalkProperties
            {
                WalkSpeed = authoring.walkSpeed
            });
            AddComponent<GoblinHeading>();
            AddComponent<NewGoblinTag>();
        }
    }
}