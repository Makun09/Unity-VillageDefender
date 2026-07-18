using ECS.Components.Building;
using ECS.Link;
using Player;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Handles tower selection on click and displays an upgrade panel.
    /// Shows current stats, next-level stats with deltas, upgrade cost and button.
    /// Depends on BuildingPlacementController to know when placement mode is active.
    /// </summary>
    public class TowerUpgradeUI : MonoBehaviour
    {
        [Header("Références scène")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private BuildingPlacementController placementController;

        [Header("Panel d'amélioration")]
        [SerializeField] private GameObject upgradePanel;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private TMP_Text damageText;
        [SerializeField] private TMP_Text fireRateText;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private TMP_Text upgradeButtonText;

        [Header("Bouton Fusion (dans le panel upgrade)")]
        [SerializeField] private Button fusionButton;
        [SerializeField] private TMP_Text fusionCostText;

        [Header("Panel sélection cible fusion")]
        [SerializeField] private GameObject fusionSelectionPanel;
        [SerializeField] private TMP_Text fusionInstructionText;

        private BuildingEntityLink _selectedLink;
        private BuildingPlacementController.BuildingPlacementOption _selectedOption;

        private bool _fusionMode;
        private BuildingEntityLink _fusionSourceLink;
        private BuildingPlacementController.BuildingPlacementOption _fusionSourceOption;
        private bool _wasInPlacementMode;

        private void Awake()
        {
            if (mainCamera == null) mainCamera = Camera.main;
            if (upgradePanel != null) upgradePanel.SetActive(false);
            if (upgradeButton != null) upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
            if (fusionButton != null) { fusionButton.onClick.AddListener(OnFusionButtonClicked); fusionButton.gameObject.SetActive(false); }
            if (fusionSelectionPanel != null) fusionSelectionPanel.SetActive(false);
        }

        private void Update()
        {
            if (Mouse.current == null) return;

            // Close panel and skip while placing a building
            if (placementController != null && placementController.IsInPlacementMode)
            {
                if (upgradePanel != null && upgradePanel.activeSelf) HidePanel();
                if (_fusionMode) CancelFusion();
                _wasInPlacementMode = true;
                return;
            }

            // Skip the click on the frame placement just ended (same click that placed the building)
            if (_wasInPlacementMode)
            {
                _wasInPlacementMode = false;
                return;
            }

            // Right-click cancels fusion or closes panel
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                if (_fusionMode) CancelFusion();
                else HidePanel();
                return;
            }

            // ── Fusion selection mode ─────────────────────────────────────────
            if (_fusionMode)
            {
                if (!Mouse.current.leftButton.wasPressedThisFrame) return;
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
                TrySelectFusionTarget();
                return;
            }

            // Auto-close if the selected tower was destroyed
            if (_selectedLink == null && upgradePanel != null && upgradePanel.activeSelf)
            {
                HidePanel();
                return;
            }

            // Keep upgrade button state in sync with current money
            if (upgradePanel != null && upgradePanel.activeSelf)
                RefreshUpgradeButton();

            if (!Mouse.current.leftButton.wasPressedThisFrame) return;

            // Don't intercept clicks on UI elements
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            TrySelectTower();
        }

        // ─── Selection ────────────────────────────────────────────────────────────

        private void TrySelectTower()
        {
            var mousePos = Mouse.current.position.ReadValue();
            var ray = mainCamera.ScreenPointToRay(mousePos);

            // Use RaycastAll so we don't miss the tower when the ground is hit first
            var hits = Physics.RaycastAll(ray, 1000f);

            BuildingEntityLink foundLink = null;
            foreach (var hit in hits)
            {
                var link = hit.collider.GetComponentInParent<BuildingEntityLink>();
                if (link != null) { foundLink = link; break; }
            }

            if (foundLink == null)
            {
                HidePanel();
                return;
            }

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated) { HidePanel(); return; }
            var em = world.EntityManager;

            if (!em.Exists(foundLink.LinkedEntity) || !em.HasComponent<TowerAttack>(foundLink.LinkedEntity))
            {
                HidePanel();
                return;
            }

            if (placementController == null ||
                !placementController.TryGetOptionByTypeId(foundLink.TypeId, out var option))
            {
                HidePanel();
                return;
            }

            _selectedLink  = foundLink;
            _selectedOption = option;
            ShowPanel(em);
        }

        // ─── Panel display ────────────────────────────────────────────────────────

        private void ShowPanel(EntityManager em)
        {
            if (!em.Exists(_selectedLink.LinkedEntity)) { HidePanel(); return; }

            var attack  = em.GetComponentData<TowerAttack>(_selectedLink.LinkedEntity);
            var def     = em.GetComponentData<BuildingDefinition>(_selectedLink.LinkedEntity);

            // Towers placed before the upgrade system existed won't have TowerUpgrade — treat as level 1
            int level = 1;
            if (em.HasComponent<TowerUpgrade>(_selectedLink.LinkedEntity))
                level = em.GetComponentData<TowerUpgrade>(_selectedLink.LinkedEntity).Level;

            titleText.text = $"Tour — Niveau {level}";

            GetNextLevelStats(level, out float nextHp, out float nextDmg, out float nextFr, out int cost, out bool isMax);

            if (!isMax)
            {
                float dHp = nextHp  - def.MaxHealth;
                float dDmg = nextDmg - attack.Damage;
                float dFr  = nextFr  - attack.FireRate;

                healthText.text   = $"PV : {def.MaxHealth:0}  →  {nextHp:0}  <color=#00cc44>(+{dHp:0})</color>";
                damageText.text   = $"Dégâts : {attack.Damage:0.##}  →  {nextDmg:0.##}  <color=#00cc44>(+{dDmg:0.##})</color>";
                fireRateText.text = $"Tirs/s : {attack.FireRate:0.##}  →  {nextFr:0.##}  <color=#00cc44>(+{dFr:0.##})</color>";
                costText.text     = $"Coût amélioration : {cost}";
                if (fusionButton != null) fusionButton.gameObject.SetActive(false);
            }
            else
            {
                bool isFused = em.HasComponent<TowerUpgrade>(_selectedLink.LinkedEntity) &&
                               em.GetComponentData<TowerUpgrade>(_selectedLink.LinkedEntity).Fused;

                if (!isFused)
                {
                    float dHp  = _selectedOption.fusionMaxHealth - def.MaxHealth;
                    float dDmg = _selectedOption.fusionDamage    - attack.Damage;
                    float dFr  = _selectedOption.fusionFireRate  - attack.FireRate;

                    healthText.text   = $"PV : {def.MaxHealth:0}  \u2192  {_selectedOption.fusionMaxHealth:0}  <color=#00cc44>(+{dHp:0})</color>";
                    damageText.text   = $"D\u00e9g\u00e2ts : {attack.Damage:0.##}  \u2192  {_selectedOption.fusionDamage:0.##}  <color=#00cc44>(+{dDmg:0.##})</color>";
                    fireRateText.text = $"Tirs/s : {attack.FireRate:0.##}  \u2192  {_selectedOption.fusionFireRate:0.##}  <color=#00cc44>(+{dFr:0.##})</color>";
                    costText.text     = $"Co\u00fbt fusion : {_selectedOption.fusionCost}";
                }
                else
                {
                    healthText.text   = $"PV : {def.MaxHealth:0}";
                    damageText.text   = $"D\u00e9g\u00e2ts : {attack.Damage:0.##}";
                    fireRateText.text = $"Tirs/s : {attack.FireRate:0.##}";
                    costText.text     = "Tour fusionn\u00e9e";
                }

                if (fusionButton != null)
                {
                    fusionButton.gameObject.SetActive(!isFused);
                    if (!isFused)
                        fusionButton.interactable = PlayerMoneyManager.Instance != null &&
                                                    PlayerMoneyManager.Instance.CurrentMoney >= _selectedOption.fusionCost;
                }
            }

            RefreshUpgradeButton(isMax, cost);
            upgradePanel.SetActive(true);
        }

        private void RefreshUpgradeButton()
        {
            if (_selectedLink == null || !_selectedLink) return;

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated) return;
            var em = world.EntityManager;
            if (!em.Exists(_selectedLink.LinkedEntity)) return;

            int level = em.HasComponent<TowerUpgrade>(_selectedLink.LinkedEntity)
                ? em.GetComponentData<TowerUpgrade>(_selectedLink.LinkedEntity).Level : 1;
            GetNextLevelStats(level, out _, out _, out _, out int cost, out bool isMax);
            RefreshUpgradeButton(isMax, cost);

            if (isMax && fusionButton != null && fusionButton.gameObject.activeSelf)
                fusionButton.interactable = PlayerMoneyManager.Instance != null &&
                                            PlayerMoneyManager.Instance.CurrentMoney >= _selectedOption.fusionCost;
        }

        private void RefreshUpgradeButton(bool isMax, int cost)
        {
            if (upgradeButton == null) return;
            if (isMax)
            {
                upgradeButton.interactable = false;
                if (upgradeButtonText != null) upgradeButtonText.text = "Niveau max";
            }
            else
            {
                bool canAfford = PlayerMoneyManager.Instance != null &&
                                 PlayerMoneyManager.Instance.CurrentMoney >= cost;
                upgradeButton.interactable = canAfford;
                if (upgradeButtonText != null) upgradeButtonText.text = "Améliorer";
            }
        }

        private void HidePanel()
        {
            _selectedLink = null;
            if (upgradePanel != null) upgradePanel.SetActive(false);
            if (fusionButton != null) fusionButton.gameObject.SetActive(false);
        }

        // ─── Upgrade ──────────────────────────────────────────────────────────────

        private void OnUpgradeButtonClicked()
        {
            if (_selectedLink == null || !_selectedLink) return;

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated) return;
            var em = world.EntityManager;
            if (!em.Exists(_selectedLink.LinkedEntity)) { HidePanel(); return; }

            var upgrade = em.HasComponent<TowerUpgrade>(_selectedLink.LinkedEntity)
                ? em.GetComponentData<TowerUpgrade>(_selectedLink.LinkedEntity)
                : new TowerUpgrade { Level = 1 };
            int currentLevel = upgrade.Level;
            if (currentLevel >= 3) return;

            GetNextLevelStats(currentLevel, out float newHp, out float newDmg, out float newFr,
                              out int cost, out bool isMax);
            if (isMax) return;

            GameObject newPrefab = currentLevel == 1 ? _selectedOption.level2Prefab
                                                     : _selectedOption.level3Prefab;

            if (!PlayerMoneyManager.Instance.TrySpend(cost)) return;

            // ── Update ECS components ──────────────────────────────────────────
            var attack = em.GetComponentData<TowerAttack>(_selectedLink.LinkedEntity);
            attack.Damage   = newDmg;
            attack.FireRate = newFr;
            em.SetComponentData(_selectedLink.LinkedEntity, attack);

            em.SetComponentData(_selectedLink.LinkedEntity, new BuildingDefinition
            {
                TypeId    = _selectedOption.typeId,
                MaxHealth = newHp
            });
            em.SetComponentData(_selectedLink.LinkedEntity, new BuildingHealth { Value = newHp });
            em.SetComponentData(_selectedLink.LinkedEntity, new TowerUpgrade    { Level = currentLevel + 1 });

            // ── Swap visual prefab ─────────────────────────────────────────────
            if (newPrefab != null)
            {
                var entity   = _selectedLink.LinkedEntity;
                var pos      = _selectedLink.transform.position;
                var rot      = newPrefab.transform.rotation;
                var oldGO    = _selectedLink.gameObject;
                int typeId   = _selectedLink.TypeId;

                _selectedLink = null;   // clear before Destroy to avoid dangling refs
                Destroy(oldGO);

                var newVisual = Instantiate(newPrefab, pos, rot);
                var newLink   = newVisual.GetComponent<BuildingEntityLink>()
                             ?? newVisual.AddComponent<BuildingEntityLink>();
                newLink.Bind(entity, typeId);
            }

            HidePanel();
        }

        // ─── Helpers ──────────────────────────────────────────────────────────────

        private void GetNextLevelStats(int currentLevel,
                                       out float hp, out float dmg, out float fr,
                                       out int cost, out bool isMax)
        {
            if (currentLevel == 1)
            {
                hp    = _selectedOption.level2MaxHealth;
                dmg   = _selectedOption.level2Damage;
                fr    = _selectedOption.level2FireRate;
                cost  = _selectedOption.level2UpgradeCost;
                isMax = false;
            }
            else if (currentLevel == 2)
            {
                hp    = _selectedOption.level3MaxHealth;
                dmg   = _selectedOption.level3Damage;
                fr    = _selectedOption.level3FireRate;
                cost  = _selectedOption.level3UpgradeCost;
                isMax = false;
            }
            else
            {
                hp = dmg = fr = 0f;
                cost  = 0;
                isMax = true;
            }
        }

        // ─── Fusion ───────────────────────────────────────────────────────────────

        private void OnFusionButtonClicked()
        {
            if (_selectedLink == null || !_selectedLink) return;
            if (!PlayerMoneyManager.Instance.TrySpend(_selectedOption.fusionCost)) return;

            _fusionMode = true;
            _fusionSourceLink   = _selectedLink;
            _fusionSourceOption = _selectedOption;

            HidePanel();
            if (fusionSelectionPanel != null) fusionSelectionPanel.SetActive(true);
            if (fusionInstructionText != null)
                fusionInstructionText.text = "Cliquez sur une tour niveau 3 d'un autre type pour fusionner";
        }

        private void TrySelectFusionTarget()
        {
            var mousePos = Mouse.current.position.ReadValue();
            var ray  = mainCamera.ScreenPointToRay(mousePos);
            var hits = Physics.RaycastAll(ray, 1000f);

            BuildingEntityLink foundLink = null;
            foreach (var hit in hits)
            {
                var link = hit.collider.GetComponentInParent<BuildingEntityLink>();
                if (link != null && link != _fusionSourceLink) { foundLink = link; break; }
            }

            // Clicked on nothing or the source itself → cancel fusion
            if (foundLink == null)
            {
                CancelFusion();
                return;
            }

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated) return;
            var em = world.EntityManager;
            if (!em.Exists(foundLink.LinkedEntity)) return;

            if (foundLink.TypeId == _fusionSourceLink.TypeId)
            {
                if (fusionInstructionText != null)
                    fusionInstructionText.text = "M\u00eame type ! Choisissez une tour d'un type diff\u00e9rent.";
                return;
            }

            // Target must be level 3
            var targetUpgrade = em.HasComponent<TowerUpgrade>(foundLink.LinkedEntity)
                ? em.GetComponentData<TowerUpgrade>(foundLink.LinkedEntity)
                : new TowerUpgrade { Level = 1 };
            if (targetUpgrade.Level < 3)
            {
                if (fusionInstructionText != null)
                    fusionInstructionText.text = "Cette tour n'est pas niveau 3 !";
                return;
            }

            ExecuteFusion(em, foundLink);
        }

        private void ExecuteFusion(EntityManager em, BuildingEntityLink targetLink)
        {
            if (!em.Exists(_fusionSourceLink.LinkedEntity)) { CancelFusion(); return; }

            // ── Mettre à jour les stats ECS de la tour source ──────────────────
            var attack = em.GetComponentData<TowerAttack>(_fusionSourceLink.LinkedEntity);
            attack.Damage   = _fusionSourceOption.fusionDamage;
            attack.FireRate = _fusionSourceOption.fusionFireRate;
            em.SetComponentData(_fusionSourceLink.LinkedEntity, attack);

            em.SetComponentData(_fusionSourceLink.LinkedEntity, new BuildingDefinition
            {
                TypeId    = _fusionSourceOption.typeId,
                MaxHealth = _fusionSourceOption.fusionMaxHealth
            });
            em.SetComponentData(_fusionSourceLink.LinkedEntity, new BuildingHealth
            {
                Value = _fusionSourceOption.fusionMaxHealth
            });
            em.SetComponentData(_fusionSourceLink.LinkedEntity, new TowerUpgrade
            {
                Level = 4,
                Fused = true
            });

            // ── Détruire la tour sacrifiée (ECS → le GO sera détruit au prochain LateUpdate) ──
            if (em.Exists(targetLink.LinkedEntity))
                em.DestroyEntity(targetLink.LinkedEntity);

            // ── Remplacer le prefab visuel de la tour source ───────────────────
            if (_fusionSourceOption.fusionPrefab != null)
            {
                var entity = _fusionSourceLink.LinkedEntity;
                var pos    = _fusionSourceLink.transform.position;
                var rot    = _fusionSourceOption.fusionPrefab.transform.rotation;
                var oldGO  = _fusionSourceLink.gameObject;
                int typeId = _fusionSourceLink.TypeId;

                _fusionSourceLink = null;
                Destroy(oldGO);

                var newVisual = Instantiate(_fusionSourceOption.fusionPrefab, pos, rot);
                var newLink   = newVisual.GetComponent<BuildingEntityLink>()
                             ?? newVisual.AddComponent<BuildingEntityLink>();
                newLink.Bind(entity, typeId);
            }

            CancelFusion();
        }

        private void CancelFusion()
        {
            _fusionMode       = false;
            _fusionSourceLink = null;
            if (fusionSelectionPanel != null) fusionSelectionPanel.SetActive(false);
        }
    }
}
