using ECS.Components;
using ECS.Components.Goblin;
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
            AddComponent(entity, new GoblinWalkProperties
            {
                WalkSpeed = authoring.walkSpeed
            });
            SetComponentEnabled<GoblinWalkProperties>(entity, false); // Désactivé par défaut, sera activé après le Rise
            AddComponent<GoblinHeading>(entity);
        }
    }
}