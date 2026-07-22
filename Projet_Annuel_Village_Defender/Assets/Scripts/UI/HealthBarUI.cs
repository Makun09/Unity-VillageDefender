using Core;
using UnityEngine;
using TMPro;

namespace UI
{
    public class HealthBarUI : MonoBehaviour
    {
        public static HealthBarUI Instance { get; private set; }

        [SerializeField] private TextMeshProUGUI healthText;

        private float _current;
        private float _max;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            LocalizationManager.LanguageChanged += Refresh;
        }

        private void OnDisable()
        {
            LocalizationManager.LanguageChanged -= Refresh;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void SetHealth(float current, float max)
        {
            _current = current;
            _max = max;
            Refresh();
        }

        private void Refresh()
        {
            if (healthText == null) return;
            healthText.text = $"{LocalizationManager.Get("village.hp_prefix")}{Mathf.CeilToInt(_current)} / {Mathf.CeilToInt(_max)}";
        }
    }
}