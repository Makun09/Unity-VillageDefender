using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace LP.SpawnTower
{
    public class SpawnTower : MonoBehaviour
    {
        [SerializeField] GameObject towerPrefab = null;
        [SerializeField] float minSurfaceAngle = 0.8f;

        private Camera cam = null;
        private static SpawnTower activeSpawner = null;

        private void Start()
        {
            cam = Camera.main;
        }

        private void Update()
        {
            SpawnAtMousePos();
        }

        public void SelectThis()
        {
            activeSpawner = this;
            Debug.Log($"Tour sélectionnée : {towerPrefab.name}");
        }

        private void SpawnAtMousePos()
        {
            // Vérifier si ce spawner est actif
            if (activeSpawner != this) return;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // Vérifier si le clic est sur un bouton UI
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }

                Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.CompareTag("CanPlace") && !hit.collider.CompareTag("CantPlace"))
                    {
                        if (Vector3.Dot(hit.normal, Vector3.up) > minSurfaceAngle)
                        {
                            Instantiate(towerPrefab, hit.point, Quaternion.identity);
                        }
                    }
                }
            }
        }
    }
}