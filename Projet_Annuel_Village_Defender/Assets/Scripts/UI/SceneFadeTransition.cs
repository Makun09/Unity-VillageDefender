using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Gère les transitions en fondu entre les scènes
/// </summary>
public class SceneFadeTransition : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private bool fadeOnStart = true;

    private static SceneFadeTransition instance;

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

        // Assurez-vous que l'image couvre tout l'écran
        if (fadeImage != null)
        {
            fadeImage.raycastTarget = false;
        }
    }

    private void Start()
    {
        if (fadeOnStart)
        {
            FadeIn();
        }
    }

    /// <summary>
    /// Effectue un fondu entrant (de noir vers transparent)
    /// </summary>
    public void FadeIn()
    {
        StartCoroutine(FadeCoroutine(1f, 0f));
    }

    /// <summary>
    /// Effectue un fondu sortant (de transparent vers noir)
    /// </summary>
    public void FadeOut()
    {
        StartCoroutine(FadeCoroutine(0f, 1f));
    }

    /// <summary>
    /// Charge une scène avec transition en fondu
    /// </summary>
    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    /// <summary>
    /// Charge une scène par index avec transition en fondu
    /// </summary>
    public void LoadSceneWithFade(int sceneIndex)
    {
        StartCoroutine(LoadSceneCoroutine(sceneIndex));
    }

    private IEnumerator FadeCoroutine(float startAlpha, float endAlpha)
    {
        if (fadeImage == null) yield break;

        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            color.a = alpha;
            fadeImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // Fondu sortant
        yield return StartCoroutine(FadeCoroutine(0f, 1f));

        // Charger la scène
        SceneManager.LoadScene(sceneName);

        // Fondu entrant
        yield return StartCoroutine(FadeCoroutine(1f, 0f));
    }

    private IEnumerator LoadSceneCoroutine(int sceneIndex)
    {
        // Fondu sortant
        yield return StartCoroutine(FadeCoroutine(0f, 1f));

        // Charger la scène
        SceneManager.LoadScene(sceneIndex);

        // Fondu entrant
        yield return StartCoroutine(FadeCoroutine(1f, 0f));
    }

    /// <summary>
    /// Obtient l'instance du SceneFadeTransition
    /// </summary>
    public static SceneFadeTransition GetInstance()
    {
        return instance;
    }
}

