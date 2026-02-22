using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

/// <summary>
/// Gère un écran de chargement avec barre de progression
/// </summary>
public class LoadingScreen : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject loadingScreenPanel;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private TextMeshProUGUI percentageText;

    [Header("Settings")]
    [SerializeField] private float minimumLoadTime = 1f;
    [SerializeField] private bool showPercentage = true;
    [SerializeField] private bool animateLoadingText = true;

    [Header("Loading Messages")]
    [SerializeField] private string[] loadingMessages = new string[]
    {
        "Chargement en cours...",
        "Préparation de la défense...",
        "Génération du terrain...",
        "Placement des ennemis...",
        "Initialisation des tours..."
    };

    private static LoadingScreen instance;
    private int dotCount = 0;
    private float dotTimer = 0f;
    private float dotInterval = 0.5f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Cacher l'écran de chargement au démarrage
        if (loadingScreenPanel != null)
        {
            loadingScreenPanel.SetActive(false);
        }
    }

    private void Update()
    {
        // Animer les points de suspension
        if (animateLoadingText && loadingScreenPanel != null && loadingScreenPanel.activeSelf)
        {
            dotTimer += Time.deltaTime;
            if (dotTimer >= dotInterval)
            {
                dotTimer = 0f;
                dotCount = (dotCount + 1) % 4;
                UpdateLoadingText();
            }
        }
    }

    /// <summary>
    /// Charge une scène de manière asynchrone avec écran de chargement
    /// </summary>
    public static void LoadSceneAsync(string sceneName)
    {
        if (instance != null)
        {
            instance.StartCoroutine(instance.LoadSceneCoroutine(sceneName));
        }
    }

    /// <summary>
    /// Charge une scène par index de manière asynchrone
    /// </summary>
    public static void LoadSceneAsync(int sceneIndex)
    {
        if (instance != null)
        {
            instance.StartCoroutine(instance.LoadSceneCoroutine(sceneIndex));
        }
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // Afficher l'écran de chargement
        if (loadingScreenPanel != null)
        {
            loadingScreenPanel.SetActive(true);
        }

        // Réinitialiser la barre de progression
        if (progressBar != null)
        {
            progressBar.value = 0f;
        }

        // Message de chargement aléatoire
        if (loadingText != null && loadingMessages.Length > 0)
        {
            loadingText.text = loadingMessages[Random.Range(0, loadingMessages.Length)];
        }

        float startTime = Time.time;

        // Commencer le chargement asynchrone
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // Attendre que le chargement soit terminé
        while (!asyncLoad.isDone)
        {
            // La progression va de 0 à 0.9 pendant le chargement
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            // Mettre à jour la barre de progression
            if (progressBar != null)
            {
                progressBar.value = progress;
            }

            // Mettre à jour le texte de pourcentage
            if (showPercentage && percentageText != null)
            {
                percentageText.text = Mathf.RoundToInt(progress * 100f) + "%";
            }

            // Attendre le temps minimum
            float elapsedTime = Time.time - startTime;
            if (asyncLoad.progress >= 0.9f && elapsedTime >= minimumLoadTime)
            {
                // Compléter le chargement
                if (progressBar != null)
                {
                    progressBar.value = 1f;
                }

                if (showPercentage && percentageText != null)
                {
                    percentageText.text = "100%";
                }

                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        // Cacher l'écran de chargement
        if (loadingScreenPanel != null)
        {
            loadingScreenPanel.SetActive(false);
        }
    }

    private IEnumerator LoadSceneCoroutine(int sceneIndex)
    {
        // Même logique que pour le chargement par nom
        if (loadingScreenPanel != null)
        {
            loadingScreenPanel.SetActive(true);
        }

        if (progressBar != null)
        {
            progressBar.value = 0f;
        }

        if (loadingText != null && loadingMessages.Length > 0)
        {
            loadingText.text = loadingMessages[Random.Range(0, loadingMessages.Length)];
        }

        float startTime = Time.time;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            if (progressBar != null)
            {
                progressBar.value = progress;
            }

            if (showPercentage && percentageText != null)
            {
                percentageText.text = Mathf.RoundToInt(progress * 100f) + "%";
            }

            float elapsedTime = Time.time - startTime;
            if (asyncLoad.progress >= 0.9f && elapsedTime >= minimumLoadTime)
            {
                if (progressBar != null)
                {
                    progressBar.value = 1f;
                }

                if (showPercentage && percentageText != null)
                {
                    percentageText.text = "100%";
                }

                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        if (loadingScreenPanel != null)
        {
            loadingScreenPanel.SetActive(false);
        }
    }

    private void UpdateLoadingText()
    {
        if (loadingText != null && loadingMessages.Length > 0)
        {
            string baseMessage = loadingMessages[Random.Range(0, loadingMessages.Length)];
            string dots = new string('.', dotCount);
            loadingText.text = baseMessage + dots;
        }
    }

    public static LoadingScreen GetInstance()
    {
        return instance;
    }
}

