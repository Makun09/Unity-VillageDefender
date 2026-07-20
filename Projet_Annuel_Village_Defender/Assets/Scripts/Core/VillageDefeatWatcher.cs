using Core;
using ECS.Components.Building;
using System.Collections;
using UnityEngine.Networking;
using Unity.Entities;
using UnityEngine;

public class VillageDefeatWatcher : MonoBehaviour
{
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private GameObject gamePanel;
    [Header("Leaderboard")]
    [SerializeField] private string scoreApiUrl = "http://localhost/Unity-VillageDefender/leaderbord_server/api/scores.php";
    [SerializeField] private bool postScoreOnDefeat = true;

    
    private EntityQuery _villageQuery;
    private bool _hadVillage;
    private bool _triggered;

    [System.Serializable]
    private class ScorePayload
    {
        public string player_name;
        public float time_seconds;
    }

    private void Start()
    {
        Instantiate(gamePanel);
        gamePanel.SetActive(true);
    }
    private void Update()
    {
        if (_triggered) return;

        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null || !world.IsCreated) return;

        var em = world.EntityManager;
        if (_villageQuery == default)
        {
            _villageQuery = em.CreateEntityQuery(ComponentType.ReadOnly<VillageTag>());
        }

        var hasVillage = !_villageQuery.IsEmptyIgnoreFilter;

        if (!_hadVillage)
        {
            _hadVillage = hasVillage; // évite déclenchement au démarrage
            return;
        }

        if (!hasVillage)
        {
            Debug.Log("Défaite !");
            _triggered = true;

            var finalTime = 0f;
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.StopRun();
                finalTime = ScoreManager.Instance.CurrentTime;
            }

            if (postScoreOnDefeat && !string.IsNullOrWhiteSpace(scoreApiUrl))
            {
                StartCoroutine(PostScoreCoroutine(finalTime));
            }

            gamePanel.SetActive(false);
            Instantiate(defeatPanel);
            defeatPanel.SetActive(true);
            Time.timeScale = 0f; // Optionnel : met le jeu en pause
        }
    }

    private IEnumerator PostScoreCoroutine(float timeSeconds)
    {
        var payload = new ScorePayload
        {
            player_name = string.IsNullOrWhiteSpace(SettingMenu.GetSavedPseudo()) ? "Joueur" : SettingMenu.GetSavedPseudo(),
            time_seconds = Mathf.Max(0f, timeSeconds)
        };

        var requestBody = JsonUtility.ToJson(payload);
        using var request = new UnityWebRequest(scoreApiUrl, UnityWebRequest.kHttpVerbPOST);
        request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(requestBody));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning($"[Leaderboard] Echec envoi score: {request.error} | URL: {scoreApiUrl}");
            yield break;
        }

        Debug.Log($"[Leaderboard] Score envoye ({payload.player_name}: {payload.time_seconds:F2}s)");
    }
}