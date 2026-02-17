using UnityEngine;
using ECS.Components;
using ECS.Components.Tower;

namespace ECS.Authoring.Tower
{
    public class Tower : MonoBehaviour
    {
    }
    
    public class TowerBaker : Unity.Entities.Baker<Tower>
    {
        public override void Bake(Tower authoring)
        {
            var entity = GetEntity(Unity.Entities.TransformUsageFlags.Dynamic);
            AddComponent<TowerTag>(entity);
        }
    } 
}