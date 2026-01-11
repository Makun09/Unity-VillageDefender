using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LP.SpawnTower
{
    public class SpawnTower : MonoBehaviour
    {
        [SerializeField] GameObject towerPrefab = null;
        [SerializeField] float minSurfaceAngle = 0.8f; // Ajuste cette valeur entre 0 et 1

        private Camera cam = null;

        private void Start()
        {
            cam = Camera.main;
        }

        private void Update()
        {
            SpawnAtMousePos();
        }

        private void SpawnAtMousePos()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    // Vérifier si on peut placer une tour à cet endroit
                    if (hit.collider.CompareTag("CanPlace") && !hit.collider.CompareTag("CantPlace"))
                    {
                        // Vérifier que la surface est horizontale (normale proche de Y)
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