using Unity.Entities;

namespace ECS.Components.Building
{
	public struct BuildingDefinition : IComponentData
	{
		public int TypeId;
		public float MaxHealth;
	}
}

