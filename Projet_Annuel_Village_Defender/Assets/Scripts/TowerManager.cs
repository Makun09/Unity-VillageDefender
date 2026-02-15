using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance { get; private set; }
    
    private List<Transform> _towers = new List<Transform>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterTower(Transform tower)
    {
        if (!_towers.Contains(tower))
        {
            _towers.Add(tower);
            Debug.Log($"Tour enregistrée. Total: {_towers.Count}");
        }
    }

    public void UnregisterTower(Transform tower)
    {
        if (_towers.Contains(tower))
        {
            _towers.Remove(tower);
            Debug.Log($"Tour retirée. Total: {_towers.Count}");
        }
    }

    public List<Transform> GetAllTowers()
    {
        return _towers;
    }

    /// <summary>
    /// Trouve la position de la tour la plus proche d'une position donnée
    /// </summary>
    public bool TryGetClosestTowerPosition(float3 fromPosition, out float3 closestPosition)
    {
        closestPosition = float3.zero;
        
        if (_towers.Count == 0)
            return false;

        float closestDistanceSq = float.MaxValue;
        Transform closestTower = null;

        // Nettoyer les tours détruites
        _towers.RemoveAll(t => t == null);

        foreach (var tower in _towers)
        {
            if (tower == null) continue;
            
            float3 towerPos = tower.position;
            float distSq = math.distancesq(fromPosition, towerPos);
            
            if (distSq < closestDistanceSq)
            {
                closestDistanceSq = distSq;
                closestTower = tower;
            }
        }

        if (closestTower != null)
        {
            closestPosition = closestTower.position;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Retourne toutes les positions des tours sous forme de NativeArray (pour ECS)
    /// </summary>
    public float3[] GetAllTowerPositions()
    {
        _towers.RemoveAll(t => t == null);
        
        float3[] positions = new float3[_towers.Count];
        for (int i = 0; i < _towers.Count; i++)
        {
            positions[i] = _towers[i].position;
        }
        return positions;
    }

    public int TowerCount => _towers.Count;
}

