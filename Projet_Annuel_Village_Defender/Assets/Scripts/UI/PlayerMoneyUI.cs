using Core;
using Player;
using TMPro;
using UnityEngine;

namespace UI
{
    public class PlayerMoneyUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text moneyText;

        private bool _isSubscribed;
        private int _lastAmount;

        private void Awake()
        {
            if (moneyText == null)
                moneyText = GetComponent<TMP_Text>();

            if (moneyText == null)
                Debug.LogError("[PlayerMoneyUI] TMP_Text manquant (assigne moneyText dans l'Inspector).");
        }

        private void OnEnable()
        {
            TryBind();
            LocalizationManager.LanguageChanged += Refresh;
        }

        private void Update()
        {
            if (!_isSubscribed)
                TryBind();
        }

        private void OnDisable()
        {
            LocalizationManager.LanguageChanged -= Refresh;

            if (_isSubscribed && PlayerMoneyManager.Instance != null)
            {
                PlayerMoneyManager.Instance.MoneyChanged -= HandleMoneyChanged;
                _isSubscribed = false;
            }
        }

        private void TryBind()
        {
            if (!PlayerMoneyManager.Instance) return;

            PlayerMoneyManager.Instance.MoneyChanged += HandleMoneyChanged;
            HandleMoneyChanged(PlayerMoneyManager.Instance.CurrentMoney);
            _isSubscribed = true;
        }

        private void HandleMoneyChanged(int amount)
        {
            _lastAmount = amount;
            Refresh();
        }

        private void Refresh()
        {
            if (!moneyText) return;
            moneyText.text = $"{LocalizationManager.Get("player.money_prefix")}{_lastAmount}";
        }
    }
}