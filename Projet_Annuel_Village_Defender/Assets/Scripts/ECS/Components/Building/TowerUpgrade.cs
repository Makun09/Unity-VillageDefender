using Unity.Entities;

namespace ECS.Components.Building
{
    /// <summary>
    /// Tracks the current upgrade level of a tower entity.
    /// Level 1 = base, Level 2 = upgraded once, Level 3 = max level.
    /// </summary>
    public struct TowerUpgrade : IComponentData
    {
        public int Level;   // 1 = base, 2, 3 = max upgrade
        public bool Fused;  // true once this tower has been fused with another
    }
}
