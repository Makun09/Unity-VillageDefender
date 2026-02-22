using UnityEngine;
using TMPro;

/// <summary>
/// Anime le titre du menu avec différents effets visuels
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class MenuTitleAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private bool enablePulse = true;
    [SerializeField] private bool enableFloat = true;
    [SerializeField] private bool enableColorShift = false;

    [Header("Pulse Settings")]
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseAmount = 0.1f;

    [Header("Float Settings")]
    [SerializeField] private float floatSpeed = 1.5f;
    [SerializeField] private float floatAmount = 10f;

    [Header("Color Shift Settings")]
    [SerializeField] private Color color1 = Color.white;
    [SerializeField] private Color color2 = Color.yellow;
    [SerializeField] private float colorShiftSpeed = 1f;

    private TextMeshProUGUI titleText;
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private float timeOffset;

    private void Awake()
    {
        titleText = GetComponent<TextMeshProUGUI>();
        originalScale = transform.localScale;
        originalPosition = transform.localPosition;
        
        // Ajouter un offset aléatoire pour varier les animations
        timeOffset = Random.Range(0f, 100f);
    }

    private void Update()
    {
        float time = Time.time + timeOffset;

        // Animation de pulse (scale)
        if (enablePulse)
        {
            float pulse = 1f + Mathf.Sin(time * pulseSpeed) * pulseAmount;
            transform.localScale = originalScale * pulse;
        }

        // Animation de flottement (position Y)
        if (enableFloat)
        {
            float yOffset = Mathf.Sin(time * floatSpeed) * floatAmount;
            transform.localPosition = originalPosition + new Vector3(0, yOffset, 0);
        }

        // Animation de changement de couleur
        if (enableColorShift && titleText != null)
        {
            float lerp = (Mathf.Sin(time * colorShiftSpeed) + 1f) / 2f;
            titleText.color = Color.Lerp(color1, color2, lerp);
        }
    }

    /// <summary>
    /// Active ou désactive toutes les animations
    /// </summary>
    public void SetAnimationsEnabled(bool enabled)
    {
        enablePulse = enabled;
        enableFloat = enabled;
        enableColorShift = enabled;

        // Réinitialiser aux valeurs originales si désactivé
        if (!enabled)
        {
            transform.localScale = originalScale;
            transform.localPosition = originalPosition;
            if (titleText != null)
            {
                titleText.color = color1;
            }
        }
    }

    /// <summary>
    /// Active uniquement l'effet de pulse
    /// </summary>
    public void EnablePulseOnly()
    {
        enablePulse = true;
        enableFloat = false;
        enableColorShift = false;
    }

    /// <summary>
    /// Active uniquement l'effet de flottement
    /// </summary>
    public void EnableFloatOnly()
    {
        enablePulse = false;
        enableFloat = true;
        enableColorShift = false;
    }
}

