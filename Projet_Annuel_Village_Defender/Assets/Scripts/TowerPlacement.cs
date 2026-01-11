using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField] private Camera PlayerCamera;
    
    private GameObject CurrentPlacingTower;

    void Update()
    {
        if (CurrentPlacingTower != null)
        {
            Ray camray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
            
            if(Physics.Raycast(camray, out RaycastHit hitInfo, 100f))
            {
                CurrentPlacingTower.transform.position = hitInfo.point;
            }

            if (Input.GetMouseButtonDown(0))
            {
                CurrentPlacingTower = null;
            }
        }
    }

    public void SetTowerToPlace(GameObject tower)
    {
        Ray camray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 spawnPos = Vector3.zero;

        if(Physics.Raycast(camray, out RaycastHit hitInfo, 100f))
        {
            spawnPos = hitInfo.point;
        }

        CurrentPlacingTower = Instantiate(tower, spawnPos, Quaternion.identity);
    }
}