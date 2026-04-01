using Player;
using TMPro;
using UnityEngine;

namespace UI
{
    public class PlayerMoneyUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text moneyText;
        [SerializeField] private string prefix = "Argent : ";

        private bool _isSubscribed;

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
        }

        private void Update()
        {
            if (!_isSubscribed)
                TryBind();
        }

        private void OnDisable()
        {
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
            if (!moneyText) return;
            moneyText.text = $"{prefix}{amount}";
        }
    }
}