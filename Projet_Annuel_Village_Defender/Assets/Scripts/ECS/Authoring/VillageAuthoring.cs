using ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring
{
    /// <summary>
    /// Authoring component to mark a GameObject as the village (target for enemies)
    /// Add this component to your village GameObject in the Unity editor
    /// </summary>
    public class VillageAuthoring : MonoBehaviour
    {
    }
    
    public class VillageBaker : Baker<VillageAuthoring>
    {
        public override void Bake(VillageAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<VillageTag>(entity);
        }
    }
}
