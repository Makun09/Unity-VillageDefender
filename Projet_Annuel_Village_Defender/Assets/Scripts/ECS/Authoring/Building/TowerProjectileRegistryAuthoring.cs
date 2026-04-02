using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring.Building
{
    public class TowerProjectileRegistryAuthoring : MonoBehaviour
    {
        [System.Serializable]
        public struct Entry
        {
            public int typeId;
            public GameObject projectilePrefab;
        }

        public List<Entry> entries = new();
    }

    public struct TowerProjectileRegistryTag : IComponentData {}

    public struct TowerProjectilePrefabByType : IBufferElementData
    {
        public int TypeId;
        public Entity ProjectilePrefab;
    }

    public class TowerProjectileRegistryBaker : Baker<TowerProjectileRegistryAuthoring>
    {
        public override void Bake(TowerProjectileRegistryAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent<TowerProjectileRegistryTag>(entity);

            var buffer = AddBuffer<TowerProjectilePrefabByType>(entity);

            foreach (var entry in authoring.entries)
            {
                if (entry.projectilePrefab == null) continue;

                buffer.Add(new TowerProjectilePrefabByType
                {
                    TypeId = entry.typeId,
                    ProjectilePrefab = GetEntity(entry.projectilePrefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}