using UnityEngine;
using TMPro;

namespace UI
{
    public class HealthBarUI : MonoBehaviour
    {
        public static HealthBarUI Instance { get; private set; }

        [SerializeField] private TextMeshProUGUI healthText;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void SetHealth(float current, float max)
        {
            if (healthText == null) return;
            healthText.text = "Village HP: "+$"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
        }
    }
}