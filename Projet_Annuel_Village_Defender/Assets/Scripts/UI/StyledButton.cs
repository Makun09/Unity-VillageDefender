using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script utilitaire pour créer rapidement des boutons stylisés dans le code
/// Ajoutez ce script à un bouton existant pour le personnaliser facilement
/// </summary>
[RequireComponent(typeof(Button))]
public class StyledButton : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] private Color normalColor = new Color(0.2f, 0.3f, 0.8f, 1f);
    [SerializeField] private Color highlightedColor = new Color(0.3f, 0.4f, 1f, 1f);
    [SerializeField] private Color pressedColor = new Color(0.1f, 0.2f, 0.6f, 1f);
    [SerializeField] private Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    [Header("Text Style")]
    [SerializeField] private float fontSize = 36f;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private bool autoUppercase = true;

    [Header("Optional Sound")]
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip hoverSound;

    private Button button;
    private TextMeshProUGUI buttonText;
    private AudioSource audioSource;

    private void Awake()
    {
        ApplyStyle();
    }

    private void Start()
    {
        // Ajouter les sons si spécifiés
        if (clickSound != null || hoverSound != null)
        {
            SetupAudio();
        }
    }

    /// <summary>
    /// Applique le style au bouton
    /// </summary>
    public void ApplyStyle()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        // Appliquer les couleurs
        ColorBlock colors = button.colors;
        colors.normalColor = normalColor;
        colors.highlightedColor = highlightedColor;
        colors.pressedColor = pressedColor;
        colors.disabledColor = disabledColor;
        colors.fadeDuration = 0.1f;
        button.colors = colors;

        // Appliquer le style de texte
        if (buttonText != null)
        {
            buttonText.fontSize = fontSize;
            buttonText.color = textColor;
            buttonText.alignment = TextAlignmentOptions.Center;

            if (autoUppercase && !string.IsNullOrEmpty(buttonText.text))
            {
                buttonText.text = buttonText.text.ToUpper();
            }
        }
    }

    /// <summary>
    /// Configure l'audio pour le bouton
    /// </summary>
    private void SetupAudio()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Ajouter le son de clic au bouton
        if (clickSound != null && button != null)
        {
            button.onClick.AddListener(() => PlaySound(clickSound));
        }
    }

    /// <summary>
    /// Joue un son
    /// </summary>
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// Change le texte du bouton
    /// </summary>
    public void SetText(string text)
    {
        if (buttonText != null)
        {
            buttonText.text = autoUppercase ? text.ToUpper() : text;
        }
    }

    /// <summary>
    /// Active ou désactive le bouton
    /// </summary>
    public void SetInteractable(bool interactable)
    {
        if (button != null)
        {
            button.interactable = interactable;
        }
    }

    /// <summary>
    /// Change la couleur normale du bouton
    /// </summary>
    public void SetNormalColor(Color color)
    {
        normalColor = color;
        ApplyStyle();
    }
}

