using ECS.Components.Building;
using ECS.Link;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECS.Visual
{
    /// <summary>
    /// Marque la bouche du canon dans le prefab.
    /// La position world de ce GameObject est écrite dans CannonMuzzlePosition
    /// sur l'entité ECS de la tour. TowerFireSystem l'utilise comme point de
    /// départ des boulets à la place de la position de base de la tour.
    ///
    /// Aucune rotation ici — voir CannonBarrelRotator pour ça.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CannonOrigin : MonoBehaviour
    {
        private BuildingEntityLink _link;
        private bool               _registered;

        private void Update()
        {
            if (_registered) return;

            if (_link == null) _link = GetComponentInParent<BuildingEntityLink>();
            if (_link == null) return;

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated) return;
            var em = world.EntityManager;

            if (!em.Exists(_link.LinkedEntity)) return;

            // Ajoute le composant s'il n'existe pas encore
            if (!em.HasComponent<CannonMuzzlePosition>(_link.LinkedEntity))
                em.AddComponent<CannonMuzzlePosition>(_link.LinkedEntity);

            em.SetComponentData(_link.LinkedEntity, new CannonMuzzlePosition
            {
                WorldPosition = new float3(
                    transform.position.x,
                    transform.position.y,
                    transform.position.z)
            });

            _registered = true; // le bâtiment ne bouge pas, une seule écriture suffit
        }
    }
}
