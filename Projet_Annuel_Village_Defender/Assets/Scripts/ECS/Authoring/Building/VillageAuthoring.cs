using ECS.Components.Building;
using ECS.Components.Enemy.AgressiveGoblin;
using ECS.Components.Enemy.SimpleGoblin;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Authoring.Building
{
    public class VillageAuthoring : MonoBehaviour
    {
        public int typeId = 999;
        public float maxHealth = 1000f;
    }

    public class VillageBaker : Baker<VillageAuthoring>
    {
        public override void Bake(VillageAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent<BuildingTag>(entity);
            AddComponent<GoblinTargetTag>(entity);
            AddComponent(entity, new BuildingHealth
            {
                Value = Mathf.Max(1f, authoring.maxHealth)
            });
            AddComponent(entity, new BuildingDefinition
            {
                TypeId = authoring.typeId,
                MaxHealth = Mathf.Max(1f, authoring.maxHealth)
            });
        }
    }
}