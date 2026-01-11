using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moneyText;
    private RectTransform rectTransform;

    private void Start()
    {
        moneyText = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        moneyText.text = "Money: " + PlayerStats.GetMoney();
        
        // Adapter la largeur au contenu
        rectTransform.sizeDelta = new Vector2(moneyText.preferredWidth, moneyText.preferredHeight);
    }
}