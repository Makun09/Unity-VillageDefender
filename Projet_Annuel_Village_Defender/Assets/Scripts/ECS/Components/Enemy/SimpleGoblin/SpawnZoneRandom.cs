using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components.Enemy.SimpleGoblin
{
    public struct SpawnZoneRandom : IComponentData
    {
        public Random Value;
    }
}