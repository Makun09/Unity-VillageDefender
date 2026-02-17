using ECS.Components;
using ECS.Components.Tower;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring
{
    public class VillageAuthoring : MonoBehaviour
    {
    }
    
    public class VillageBaker : Baker<VillageAuthoring>
    {
        public override void Bake(VillageAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<VillageTag>(entity);
            AddComponent<TowerTag>(entity);
        }
    }
}
