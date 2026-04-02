using ECS.Components.Building;
using ECS.Components.Enemy.AgressiveGoblin;
using Player;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class BuildingPlacementController : MonoBehaviour
    {
        [System.Serializable]
        private struct BuildingPlacementOption
        {
            public string label;
            public GameObject prefab;
            public Vector3 rotationOffsetEuler;
            public int typeId;
            public float maxHealth;
            public int buildCost;

            [Header("Tower")]
            public bool isTower;
            public float range;
            public float damage;
            public float fireRate;
            public int fireCount;
            public float projectileSpeed;
            public float projectileHitRadius;
            public bool projectileStraight;
        }

        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private List<BuildingPlacementOption> buildingOptions = new List<BuildingPlacementOption>();
        [SerializeField] private int defaultBuildingIndex;

        private bool _placementMode;
        private int _selectedBuildingIndex = -1;

        private void Awake()
        {
            if (buildingOptions.Count == 0) return;
            _selectedBuildingIndex = Mathf.Clamp(defaultBuildingIndex, 0, buildingOptions.Count - 1);
        }

        public bool SelectBuildingType(int optionIndex)
        {
            if (optionIndex < 0 || optionIndex >= buildingOptions.Count)
            {
                return false;
            }

            _selectedBuildingIndex = optionIndex;
            return true;
        }

        public void SelectBuildingTypeAndActivate(int optionIndex)
        {
            if (!SelectBuildingType(optionIndex)) return;
            ActivateBuildingPlacement();
        }

        public void ActivateBuildingPlacement()
        {
            if (!TryGetSelectedOption(out _))
            {
                return;
            }

            _placementMode = true;
        }

        private void Update()
        {
            if (Mouse.current == null) return;
            if (Mouse.current.rightButton.wasPressedThisFrame) _placementMode = false;
            if (!_placementMode) return;
            if (!TryGetSelectedOption(out var selectedOption)) return;

            if (!Mouse.current.leftButton.wasPressedThisFrame) return;
            if (!TryGetMouseWorldPosition(out var worldPos, out var hitNormal)) return;

            var paid = PlayerMoneyManager.Instance.TrySpend(selectedOption.buildCost);
            if (!paid) return;

            CreateBuildingTargetEntity(worldPos, hitNormal, selectedOption);
        }

        private bool TryGetMouseWorldPosition(out float3 worldPos, out Vector3 hitNormal)
        {
            var mousePos = Mouse.current.position.ReadValue();
            var ray = mainCamera.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out var hit, 1000f, groundMask, QueryTriggerInteraction.Ignore))
            {
                worldPos = hit.point;
                hitNormal = hit.normal;
                return true;
            }

            worldPos = default;
            hitNormal = Vector3.up;
            return false;
        }

        private void CreateBuildingTargetEntity(float3 position, Vector3 hitNormal, BuildingPlacementOption selectedOption)
        {
            if (selectedOption.prefab)
            {
                var spawnRotation = ComputePlacementRotation(hitNormal, selectedOption.rotationOffsetEuler);
                var spawnPosition = new Vector3(position.x, position.y, position.z);
                Instantiate(selectedOption.prefab, spawnPosition, spawnRotation);
            }

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated) return;

            var em = world.EntityManager;
            var building = em.CreateEntity();

            em.AddComponentData(building, LocalTransform.FromPosition(position));
            em.AddComponentData(building, new BuildingDefinition
            {
                TypeId = selectedOption.typeId,
                MaxHealth = selectedOption.maxHealth
            });

            em.AddComponent<GoblinTargetTag>(building);
            em.AddComponent<BuildingTag>(building);

            if (selectedOption.isTower)
            {
                em.AddComponentData(building, new TowerAttack
                {
                    Range = math.max(0.01f, selectedOption.range),
                    Damage = math.max(0f, selectedOption.damage),
                    FireRate = math.max(0.01f, selectedOption.fireRate),
                    FireCount = math.max(1, selectedOption.fireCount),
                    ProjectileSpeed = math.max(0.01f, selectedOption.projectileSpeed),
                    ProjectileHitRadius = math.max(0.01f, selectedOption.projectileHitRadius),
                    ProjectileStraight = selectedOption.projectileStraight,
                    ProjectilePrefab = ResolveProjectilePrefab(em, selectedOption.typeId)
                });

                em.AddComponentData(building, new TowerCoolDown
                {
                    TimeLeft = 0f
                });
            }
        }

        private bool TryGetSelectedOption(out BuildingPlacementOption selectedOption)
        {
            if (_selectedBuildingIndex < 0 || _selectedBuildingIndex >= buildingOptions.Count)
            {
                selectedOption = default;
                return false;
            }

            selectedOption = buildingOptions[_selectedBuildingIndex];
            return true;
        }

        private Quaternion ComputePlacementRotation(Vector3 hitNormal, Vector3 rotationOffsetEuler)
        {
            var up = hitNormal.normalized;

            // Keep a stable forward axis on the hit surface using camera heading.
            var forward = Vector3.ProjectOnPlane(mainCamera.transform.forward, up).normalized;
            if (forward.sqrMagnitude < 0.0001f)
            {
                forward = Vector3.Cross(up, Vector3.right).normalized;
                if (forward.sqrMagnitude < 0.0001f)
                {
                    forward = Vector3.Cross(up, Vector3.forward).normalized;
                }
            }

            var surfaceRotation = Quaternion.LookRotation(forward, up);
            var offsetRotation = Quaternion.Euler(rotationOffsetEuler);
            return surfaceRotation * offsetRotation;
        }

        private Entity ResolveProjectilePrefab(EntityManager em, int typeId)
        {
            using var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<ECS.Authoring.Building.TowerProjectileRegistryTag>(),
                ComponentType.ReadOnly<ECS.Authoring.Building.TowerProjectilePrefabByType>());

            if (query.IsEmptyIgnoreFilter) return Entity.Null;

            var registryEntity = query.GetSingletonEntity();
            var buffer = em.GetBuffer<ECS.Authoring.Building.TowerProjectilePrefabByType>(registryEntity);

            for (var i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].TypeId == typeId)
                    return buffer[i].ProjectilePrefab;
            }

            return Entity.Null;
        }
    }
}
