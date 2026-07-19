using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

// Nécessaire pour les Coroutines

namespace Core
{
    public class GameLoopManager : MonoBehaviour
    {
        public static GameLoopManager Instance { get; private set; }
        [SerializeField] private GameStateSO gameState;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
        
        private void Start()
        {
            gameState.ChangeState(GameState.MainMenu);
        }

        public void StartPlaying()
        {
            StartCoroutine(LoadLevelRoutine(1));
        }

        // Coroutine générique pour charger un niveau
        private IEnumerator LoadLevelRoutine(int sceneIndex, bool reload = false)
        {
            yield return null;
    
            Time.timeScale = 1f;
            
    
            if (reload)
            {
                ResetEcsWorld(); // <-- AJOUT : reset aussi au restart, pas seulement au retour menu
            }
            else
            {
                gameState.ChangeState(GameState.Playing);
            }
            ScoreManager.Instance?.StartRun();
            SceneManager.LoadScene(sceneIndex);
        }
        
        public void ReturnToMenu()
        {
            ScoreManager.Instance?.StopRun();
            StartCoroutine(ReturnToMenuRoutine());
        }

        public void RestartGame()
        {
            ScoreManager.Instance?.StopRun();
            StartCoroutine(LoadLevelRoutine(1, true));
            
        }
        
        private IEnumerator ReturnToMenuRoutine()
        {
            yield return null;

            Time.timeScale = 1f;
            gameState.ChangeState(GameState.MainMenu);

            ResetEcsWorld();

            SceneManager.LoadScene(0);
        }
        
        
        private void ResetEcsWorld()
        {
            var world = Unity.Entities.World.DefaultGameObjectInjectionWorld;
            if (world != null && world.IsCreated)
            {
                world.Dispose();
                Unity.Entities.DefaultWorldInitialization.Initialize("Default World");
            }
        }


        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Escape))
            //{
               // if (gameState.CurrentState == GameState.Playing)
                   //
                   // Pause();
               // else if (gameState.CurrentState == GameState.Paused)
             //       Resume();
            //}
            
            
        }
        

        public void Pause()
        {
            Time.timeScale = 0f;
            gameState.ChangeState(GameState.Paused);
        }

        public void Resume()
        {
            Time.timeScale = 1f;
            gameState.ChangeState(GameState.Playing);
        }
        
        
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
