using ECS.Components.Enemy.AgressiveGoblin;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring.Enemy.Goblin
{
    public class GoblinTargetAuthoring : MonoBehaviour
    {
    }

    public class GoblinTargetBaker : Baker<GoblinTargetAuthoring>
    {
        public override void Bake(GoblinTargetAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<GoblinTargetTag>(entity);
        }
    }
}