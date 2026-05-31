using Unity.Entities;

namespace ECS.Components.Enemy.SimpleGoblin
{
    public struct GoblinDeathRewardEvent : IComponentData
    {
        public int Value;
    }
}

