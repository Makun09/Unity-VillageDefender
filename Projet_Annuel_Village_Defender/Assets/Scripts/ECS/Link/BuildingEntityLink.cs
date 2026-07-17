using Unity.Entities;
using UnityEngine;

namespace ECS.Link
{
    [DisallowMultipleComponent]
    public sealed class BuildingEntityLink : MonoBehaviour
    {
        private Entity _entity;
        private bool _isBound;

        /// <summary>The ECS entity this visual object is linked to.</summary>
        public Entity LinkedEntity => _entity;

        /// <summary>The building type id used to look up upgrade data.</summary>
        public int TypeId { get; private set; }

        public void Bind(Entity entity, int typeId = 0)
        {
            _entity = entity;
            _isBound = true;
            TypeId = typeId;
        }

        private void LateUpdate()
        {
            if (!_isBound) return;
            if (!TryGetEntityManager(out var entityManager)) return;

            if (!entityManager.Exists(_entity))
            {
                Destroy(gameObject);
            }
        }

        private static bool TryGetEntityManager(out EntityManager entityManager)
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                entityManager = default;
                return false;
            }

            entityManager = world.EntityManager;
            return true;
        }
    }
}


