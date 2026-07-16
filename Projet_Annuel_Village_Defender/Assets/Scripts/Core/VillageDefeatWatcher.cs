using Core;
using ECS.Components.Building;
using Unity.Entities;
using UnityEngine;

public class VillageDefeatWatcher : MonoBehaviour
{
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private GameObject gamePanel;

    
    private EntityQuery _villageQuery;
    private bool _hadVillage;
    private bool _triggered;

    private void Start()
    {
        Instantiate(gamePanel);
        gamePanel.SetActive(true);
    }
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
            Debug.Log("Défaite !");
            _triggered = true;
            gamePanel.SetActive(false);
            Instantiate(defeatPanel);
            defeatPanel.SetActive(true);
            Time.timeScale = 0f; // Optionnel : met le jeu en pause
        }
    }
}