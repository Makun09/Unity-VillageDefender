using ECS.Components;
using UnityEngine;
using Unity.Entities;

namespace ECS.Authoring
{
    public class EnemyAuthoring : MonoBehaviour
    {
        public float speed = 3f;

        class Baker : Baker<EnemyAuthoring>
        {
            public override void Bake(EnemyAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                
                AddComponent(entity, new MoveSpeed
                {
                    Value = authoring.speed
                });
                AddComponent<EnemyTag>(entity);
            }
        }
    }
}