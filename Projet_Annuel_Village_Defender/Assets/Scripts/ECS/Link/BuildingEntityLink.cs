using Unity.Entities;
using UnityEngine;

namespace ECS.Link
{
    [DisallowMultipleComponent]
    public sealed class BuildingEntityLink : MonoBehaviour
    {
        private Entity _entity;
        private bool _isBound;

        public void Bind(Entity entity)
        {
            _entity = entity;
            _isBound = true;
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


