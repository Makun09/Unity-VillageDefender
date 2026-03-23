using ECS.Authorings.Enemy.Goblin;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authorings.Enemy.Goblin
{
    public class GoblinTargetAuthoring : MonoBehaviour
    {
        private void Start()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new GoblinTargetPosition
            {
                Value = transform.position
            });
        }
    }
}