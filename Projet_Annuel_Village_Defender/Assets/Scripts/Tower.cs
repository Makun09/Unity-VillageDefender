using UnityEngine;

public class Tower : MonoBehaviour
{
    private void Start()
    {
        if (TowerManager.Instance != null)
        {
            TowerManager.Instance.RegisterTower(transform);
        }
    }

    private void OnDestroy()
    {
        if (TowerManager.Instance != null)
        {
            TowerManager.Instance.UnregisterTower(transform);
        }
    }
}

