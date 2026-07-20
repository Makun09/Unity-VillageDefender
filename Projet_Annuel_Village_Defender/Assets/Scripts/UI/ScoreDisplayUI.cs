using UnityEngine;
using TMPro;
using Core;

namespace UI
{
    public class ScoreDisplayUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI currentTimeText;
        [SerializeField] private TextMeshProUGUI bestTimeText;

        private void Update()
        {
            if (ScoreManager.Instance == null) return;
            if (currentTimeText != null)
                currentTimeText.text = FormatTime(ScoreManager.Instance.CurrentTime);
            if (bestTimeText != null)
                bestTimeText.text = $"Best: {FormatTime(ScoreManager.Instance.BestTime)}";
        }

        private string FormatTime(float seconds)
        {
            int minutes = Mathf.FloorToInt(seconds / 60f);
            int secs = Mathf.FloorToInt(seconds % 60f);
            return $"{minutes:00}:{secs:00}";
        }
    }
}