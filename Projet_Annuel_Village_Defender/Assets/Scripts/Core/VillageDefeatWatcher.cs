using Core;
using ECS.Components.Building;
using Unity.Entities;
using UnityEngine;

public class VillageDefeatWatcher : MonoBehaviour
{
    private EntityQuery _villageQuery;
    private bool _hadVillage;
    private bool _triggered;

    private void Update()
    {
        if (_triggered) return;

        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null || !world.IsCreated) return;

        var em = world.EntityManager;
        if (_villageQuery == default)
        {
            _villageQuery = em.CreateEntityQuery(ComponentType.ReadOnly<VillageTag>());
        }

        var hasVillage = !_villageQuery.IsEmptyIgnoreFilter;

        if (!_hadVillage)
        {
            _hadVillage = hasVillage; // évite déclenchement au démarrage
            return;
        }

        if (!hasVillage)
        {
            _triggered = true;
            GameLoopManager.Instance?.ReturnToMenu();
        }
    }
}