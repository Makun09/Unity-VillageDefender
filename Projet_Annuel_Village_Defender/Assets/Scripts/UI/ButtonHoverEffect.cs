using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Ajoute des effets visuels aux boutons du menu (scale au hover, etc.)
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scale Effect")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float animationSpeed = 10f;

    [Header("Optional Audio")]
    [SerializeField] private AudioClip hoverSound;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private RectTransform rectTransform;
    private AudioSource audioSource;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        targetScale = originalScale;

        // Optionnel : ajouter un AudioSource si un son est spécifié
        if (hoverSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = hoverSound;
            audioSource.playOnAwake = false;
        }
    }

    private void Update()
    {
        // Animation smooth du scale
        if (rectTransform.localScale != targetScale)
        {
            rectTransform.localScale = Vector3.Lerp(
                rectTransform.localScale, 
                targetScale, 
                Time.deltaTime * animationSpeed
            );
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * hoverScale;

        if (audioSource != null && hoverSound != null)
        {
            audioSource.Play();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
    }
}

