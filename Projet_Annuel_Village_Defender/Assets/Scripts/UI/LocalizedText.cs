using Core;
using TMPro;
using UnityEngine;

namespace UI
{
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string key;

        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            Refresh();
            LocalizationManager.LanguageChanged += Refresh;
        }

        private void OnDisable()
        {
            LocalizationManager.LanguageChanged -= Refresh;
        }

        private void Refresh()
        {
            if (_text != null)
                _text.text = LocalizationManager.Get(key);
        }
    }
}
