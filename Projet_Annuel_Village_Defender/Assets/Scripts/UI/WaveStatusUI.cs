using ECS.Components.Enemy.SimpleGoblin;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace UI
{
    public class WaveStatusUI : MonoBehaviour
    {
        [Header("Texts")]
        [SerializeField] private TMP_Text currentWaveText;
        [SerializeField] private TMP_Text interWaveTimerText;
        [SerializeField] private TMP_Text remainingGoblinsText;

        [Header("Labels")]
        [SerializeField] private string currentWavePrefix = "Vague : ";
        [SerializeField] private string interWavePrefix = "Prochaine vague dans : ";
        [SerializeField] private string remainingPrefix = "Gobelins restants : ";

        [Header("Display")]
        [SerializeField] private string waitingText = "En cours";
        [SerializeField] private int timerDecimals = 1;

        private World _boundWorld;
        private EntityManager _entityManager;
        private EntityQuery _waveStateQuery;
        private EntityQuery _spawnZonePropsQuery;
        private EntityQuery _aliveGoblinsQuery;
        private bool _isBound;

        private void Awake()
        {
            if (currentWaveText == null || interWaveTimerText == null || remainingGoblinsText == null)
            {
                Debug.LogWarning("[WaveStatusUI] Assigne currentWaveText, interWaveTimerText et remainingGoblinsText dans l'Inspector.");
            }
        }

        private void Update()
        {
            if (!TryBindWorldAndQueries() || _waveStateQuery.IsEmptyIgnoreFilter || _spawnZonePropsQuery.IsEmptyIgnoreFilter)
            {
                SetUnavailable();
                return;
            }

            var waveState = _waveStateQuery.GetSingleton<GoblinWaveState>();
            var aliveGoblins = _aliveGoblinsQuery.CalculateEntityCount();

            var remainingNotSpawned = math.max(0, waveState.TargetThisWave - waveState.SpawnedThisWave);
            var remainingToKill = waveState.WaitingNextWave == 1 ? 0 : aliveGoblins + remainingNotSpawned;
            var clampedTimer = waveState.WaitingNextWave == 1 ? math.max(0f, waveState.InterWaveCooldown) : 0f;

            SetText(currentWaveText, $"{currentWavePrefix}{math.max(1, waveState.WaveIndex)}");
            SetText(interWaveTimerText, waveState.WaitingNextWave == 1
                ? $"{interWavePrefix}{clampedTimer.ToString($"F{math.max(0, timerDecimals)}")}s"
                : $"{interWavePrefix}{waitingText}");
            SetText(remainingGoblinsText, $"{remainingPrefix}{math.max(0, remainingToKill)}");
        }

        private bool TryBindWorldAndQueries()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                _isBound = false;
                return false;
            }

            if (_isBound && _boundWorld == world)
            {
                return true;
            }

            _boundWorld = world;
            _entityManager = world.EntityManager;

            _waveStateQuery = _entityManager.CreateEntityQuery(ComponentType.ReadOnly<GoblinWaveState>());
            _spawnZonePropsQuery = _entityManager.CreateEntityQuery(ComponentType.ReadOnly<SpawnZoneProperties>());
            _aliveGoblinsQuery = _entityManager.CreateEntityQuery(new EntityQueryDesc
            {
                All = new[] { ComponentType.ReadOnly<GoblinHealth>() },
                None = new[] { ComponentType.ReadOnly<Prefab>(), ComponentType.ReadOnly<Disabled>() }
            });

            _isBound = true;
            return true;
        }

        private void SetUnavailable()
        {
            SetText(currentWaveText, $"{currentWavePrefix}-");
            SetText(interWaveTimerText, $"{interWavePrefix}-");
            SetText(remainingGoblinsText, $"{remainingPrefix}-");
        }

        private static void SetText(TMP_Text label, string value)
        {
            if (!label) return;
            label.text = value;
        }
    }
}

