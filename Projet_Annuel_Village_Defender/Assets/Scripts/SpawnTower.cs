using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LP.SpawnTower
{
    public class SpawnTower : MonoBehaviour
    {
        [SerializeField] GameObject towerPrefab = null;

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
                        Instantiate(towerPrefab, hit.point, Quaternion.identity);
                    }
                }
            }
        }

    }
}