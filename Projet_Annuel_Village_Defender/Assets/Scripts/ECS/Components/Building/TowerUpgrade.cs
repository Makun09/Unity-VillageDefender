using Unity.Entities;

namespace ECS.Components.Building
{
    /// <summary>
    /// Tracks the current upgrade level of a tower entity.
    /// Level 1 = base, Level 2 = upgraded once, Level 3 = max level.
    /// </summary>
    public struct TowerUpgrade : IComponentData
    {
        public int Level;
    }
}
