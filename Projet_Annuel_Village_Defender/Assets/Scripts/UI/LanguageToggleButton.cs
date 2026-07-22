using Core;
using TMPro;
using UnityEngine;

namespace UI
{
    public class LanguageToggleButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;

        private void OnEnable()
        {
            Refresh();
            LocalizationManager.LanguageChanged += Refresh;
        }

        private void OnDisable()
        {
            LocalizationManager.LanguageChanged -= Refresh;
        }

        public void ToggleLanguage()
        {
            LocalizationManager.ToggleLanguage();
        }

        private void Refresh()
        {
            if (label != null)
                label.text = LocalizationManager.Get("ui.language_button");
        }
    }
}
