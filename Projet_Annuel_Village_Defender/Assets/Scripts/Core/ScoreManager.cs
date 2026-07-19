using UnityEngine;

namespace Core
{
    public class ScoreManager : MonoBehaviour
    {
        
        //classe qui enregistre le meilleur score du joueur : son temps max = son meilleure score
        public static ScoreManager Instance { get; private set; }

        private const string BestTimeKey = "BestSurvivalTime";

        public float CurrentTime { get; private set; }
        public float BestTime { get; private set; }

        private bool _isTracking;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            BestTime = PlayerPrefs.GetFloat(BestTimeKey, 0f);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void StartRun()
        {
            CurrentTime = 0f;
            _isTracking = true;
        }

        private void Update()
        {
            if (!_isTracking) return;
            CurrentTime += Time.deltaTime;
            
        }

        public void StopRun()
        {
            if (!_isTracking) return;
            _isTracking = false;

            if (CurrentTime > BestTime)
            {
                BestTime = CurrentTime;
                PlayerPrefs.SetFloat(BestTimeKey, BestTime);
                PlayerPrefs.Save();
            }
        }
    }
}
