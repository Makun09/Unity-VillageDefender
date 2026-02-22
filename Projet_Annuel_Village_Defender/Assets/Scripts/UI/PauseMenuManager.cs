using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/// <summary>
/// Gère le menu de pause pendant le jeu
/// </summary>
public class PauseMenuManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Settings")]
    [SerializeField] private bool pauseTimeScale = true;

    private bool isPaused = false;
    private float previousTimeScale = 1f;

    private void Start()
    {
        // Assurez-vous que le menu est caché au démarrage
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        isPaused = false;
    }

    private void Update()
    {
        // Vérifier si le joueur appuie sur la touche de pause (Escape)
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    /// <summary>
    /// Met le jeu en pause
    /// </summary>
    public void Pause()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        isPaused = true;

        // Mettre le jeu en pause
        if (pauseTimeScale)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }

        // Optionnel : déverrouiller le curseur
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Reprend le jeu
    /// </summary>
    public void Resume()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        isPaused = false;

        // Reprendre le temps
        if (pauseTimeScale)
        {
            Time.timeScale = previousTimeScale;
        }

        // Optionnel : reverrouiller le curseur si nécessaire
        // Décommentez si votre jeu utilise un curseur verrouillé
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    /// <summary>
    /// Ouvre le panneau des paramètres
    /// </summary>
    public void OpenSettings()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    /// <summary>
    /// Retourne au menu pause
    /// </summary>
    public void BackToPauseMenu()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    /// <summary>
    /// Redémarre le niveau actuel
    /// </summary>
    public void RestartLevel()
    {
        // Remettre le temps à la normale avant de recharger
        Time.timeScale = 1f;

        // Recharger la scène actuelle
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    /// <summary>
    /// Retourne au menu principal
    /// </summary>
    public void ReturnToMainMenu()
    {
        // Remettre le temps à la normale avant de changer de scène
        Time.timeScale = 1f;

        // Charger le menu principal (modifiez le nom si nécessaire)
        if (SceneFadeTransition.GetInstance() != null)
        {
            SceneFadeTransition.GetInstance().LoadSceneWithFade("MainMenu");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    /// <summary>
    /// Quitte l'application
    /// </summary>
    public void QuitGame()
    {
        // Remettre le temps à la normale
        Time.timeScale = 1f;

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    /// <summary>
    /// Vérifie si le jeu est en pause
    /// </summary>
    public bool IsPaused()
    {
        return isPaused;
    }
}

