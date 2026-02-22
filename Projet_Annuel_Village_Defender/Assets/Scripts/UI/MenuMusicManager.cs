using UnityEngine;

/// <summary>
/// Gère la musique de fond du menu. Ne se détruit pas lors du changement de scène.
/// </summary>
public class MenuMusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private bool playOnAwake = true;
    [SerializeField] private bool loopMusic = true;
    [SerializeField] [Range(0f, 1f)] private float defaultVolume = 0.5f;

    private AudioSource audioSource;
    private static MenuMusicManager instance;

    private void Awake()
    {
        // Pattern Singleton pour éviter la duplication de musique
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Configuration de l'AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = menuMusic;
        audioSource.loop = loopMusic;
        audioSource.volume = defaultVolume;
        audioSource.playOnAwake = playOnAwake;

        if (playOnAwake && menuMusic != null)
        {
            audioSource.Play();
        }
    }

    /// <summary>
    /// Change le volume de la musique
    /// </summary>
    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = Mathf.Clamp01(volume);
        }
    }

    /// <summary>
    /// Joue la musique
    /// </summary>
    public void Play()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    /// <summary>
    /// Met en pause la musique
    /// </summary>
    public void Pause()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    /// <summary>
    /// Arrête la musique
    /// </summary>
    public void Stop()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    /// <summary>
    /// Change la musique
    /// </summary>
    public void ChangeMusic(AudioClip newMusic)
    {
        if (audioSource != null && newMusic != null)
        {
            audioSource.clip = newMusic;
            audioSource.Play();
        }
    }

    /// <summary>
    /// Obtient l'instance du manager
    /// </summary>
    public static MenuMusicManager GetInstance()
    {
        return instance;
    }
}

