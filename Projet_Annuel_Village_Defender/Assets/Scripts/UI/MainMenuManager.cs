using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Scene Settings")]
    [SerializeField] private string gameSceneName = "Game";
    [SerializeField] private bool useFadeTransition = true;

    private void Start()
    {
        // Assurez-vous que le panneau principal est actif au démarrage
        ShowMainMenu();
    }

    /// <summary>
    /// Lance la partie en chargeant la scène de jeu
    /// </summary>
    public void PlayGame()
    {
        if (useFadeTransition && SceneFadeTransition.GetInstance() != null)
        {
            // Utilise la transition avec fade si disponible
            SceneFadeTransition.GetInstance().LoadSceneWithFade(gameSceneName);
        }
        else
        {
            // Chargement direct de la scène
            SceneManager.LoadScene(gameSceneName);
        }
    }

    /// <summary>
    /// Affiche le panneau des paramètres
    /// </summary>
    public void OpenSettings()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    /// <summary>
    /// Retourne au menu principal
    /// </summary>
    public void ShowMainMenu()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
        
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    /// <summary>
    /// Quitte l'application
    /// </summary>
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}


