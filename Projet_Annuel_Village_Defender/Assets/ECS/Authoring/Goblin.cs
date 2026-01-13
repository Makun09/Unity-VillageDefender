using ECS.Components;
using UnityEngine;

namespace ECS.Authoring
{
    public class Goblin : MonoBehaviour
    {
        public float RiseRate;
        public float WalkSpeed;
    }
    
    public class GoblinBaker : Unity.Entities.Baker<Goblin>
    {
        public override void Bake(Goblin authoring)
        {
            var entity = GetEntity(Unity.Entities.TransformUsageFlags.Dynamic);
            AddComponent(entity, new ECS.Components.GoblinRiseRate
            {
                Value = authoring.RiseRate
            });
            AddComponent(entity, new ECS.Components.GoblinWalkProperties
            {
                WalkSpeed = authoring.WalkSpeed
            });
            AddComponent<GoblinHeading>();
            AddComponent<NewGoblinTag>();
        }
    }
}