using Unity.Entities;

namespace ECS.Components.Enemy.SimpleGoblin
{
    public struct GoblinWaveState : IComponentData 
    {
        public int WaveIndex;             
        public int SpawnedThisWave;       // combien deja spawn
        public int TargetThisWave;        // combien a spawn pour cette vague
        public float SpawnCooldown;       // cooldown entre 2 goblins
        public float InterWaveCooldown;   // cooldown entre vagues
        public byte WaitingNextWave;      // 0 = vague active, 1 = attente
    }
}