using ECS.Components.Building;
using ECS.Components.Enemy.AgressiveGoblin;
using ECS.Link;
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
        public struct BuildingPlacementOption
        {
            public string label;
            public GameObject prefab;
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

            [Header("Niveau 2")]
            public float level2MaxHealth;
            public float level2Damage;
            public float level2FireRate;
            public int   level2UpgradeCost;
            public GameObject level2Prefab;

            [Header("Niveau 3")]
            public float level3MaxHealth;
            public float level3Damage;
            public float level3FireRate;
            public int   level3UpgradeCost;
            public GameObject level3Prefab;
        }

        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private List<BuildingPlacementOption> buildingOptions = new List<BuildingPlacementOption>();
        [SerializeField] private int defaultBuildingIndex;
        [SerializeField] private float placementCheckRadius = 1.5f;

        private bool _placementMode;
        private int _selectedBuildingIndex = -1;

        public bool IsInPlacementMode => _placementMode;

        public bool TryGetOptionByTypeId(int typeId, out BuildingPlacementOption option)
        {
            foreach (var opt in buildingOptions)
            {
                if (opt.typeId == typeId) { option = opt; return true; }
            }
            option = default;
            return false;
        }

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
            if (IsPositionOccupied(worldPos)) return;

            var paid = PlayerMoneyManager.Instance.TrySpend(selectedOption.buildCost);
            if (!paid) return;

            CreateBuildingTargetEntity(worldPos, hitNormal, selectedOption);
            _placementMode = false;
        }

        private bool IsPositionOccupied(float3 position)
        {
            var center = new Vector3(position.x, position.y + 0.5f, position.z);
            var hits = Physics.OverlapSphere(center, placementCheckRadius);
            foreach (var col in hits)
            {
                if (col.GetComponentInParent<BuildingEntityLink>() != null)
                    return true;
            }
            return false;
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

            em.AddComponentData(building, new BuildingHealth
            {
                Value = selectedOption.maxHealth
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

                em.AddComponentData(building, new ECS.Components.Building.TowerUpgrade
                {
                    Level = 1
                });
            }

            if (selectedOption.prefab)
            {
                var spawnPosition = new Vector3(position.x, position.y, position.z);
                var visualInstance = Instantiate(selectedOption.prefab, spawnPosition, selectedOption.prefab.transform.rotation);

                var link = visualInstance.GetComponent<BuildingEntityLink>();
                if (link == null)
                {
                    link = visualInstance.AddComponent<BuildingEntityLink>();
                }

                link.Bind(building, selectedOption.typeId);
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
