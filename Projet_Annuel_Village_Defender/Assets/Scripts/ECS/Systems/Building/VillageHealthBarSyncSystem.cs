using ECS.Components.Building;
using UI;
using Unity.Entities;

namespace ECS.Systems.Building
{
    public partial class VillageHealthBarSyncSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            if (HealthBarUI.Instance == null)
                return; // pas de barre affichée pour l'instant (menu, etc.)

            foreach (var (health, def) in SystemAPI.Query<RefRO<BuildingHealth>, RefRO<BuildingDefinition>>()
                         .WithAll<VillageTag>())
            {
                HealthBarUI.Instance.SetHealth(health.ValueRO.Value, def.ValueRO.MaxHealth);
                return; // un seul village, pas besoin de continuer
            }
        }
    }
}