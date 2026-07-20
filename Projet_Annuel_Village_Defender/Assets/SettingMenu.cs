using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;

public class SettingMenu : MonoBehaviour
{
    // ── Audio ──────────────────────────────────────────────────────────────
    public AudioMixer audioMixer;

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }

    // ── Pseudo / Leaderboard ───────────────────────────────────────────────
    private const string PseudoKey    = "PlayerPseudo";
    private const string PlayerIdKey  = "PlayerId";

    [Header("Pseudo")]
    [Tooltip("Champ de saisie du pseudo dans le menu Paramètres.")]
    [SerializeField] private TMP_InputField pseudoInputField;

    [Tooltip("URL de l'API leaderboard. Ex: https://mon-serveur.com/api/leaderboard")]
    [SerializeField] private string leaderboardApiUrl = "";

    private void Start()
    {
        // Charge le pseudo sauvegardé et l'affiche dans le champ
        if (pseudoInputField != null)
        {
            pseudoInputField.text = PlayerPrefs.GetString(PseudoKey, "");
            pseudoInputField.onEndEdit.AddListener(OnPseudoEndEdit);
        }
    }

    /// <summary>Appelé quand l'utilisateur valide son pseudo (Enter / perd le focus).</summary>
    private void OnPseudoEndEdit(string value)
    {
        var pseudo = value.Trim();
        if (string.IsNullOrEmpty(pseudo)) return;

        // Sauvegarde locale persistante
        PlayerPrefs.SetString(PseudoKey, pseudo);
        PlayerPrefs.Save();

        // Envoi au serveur leaderboard
        if (!string.IsNullOrEmpty(leaderboardApiUrl))
            StartCoroutine(PostPseudoToServer(pseudo));
    }

    /// <summary>
    /// Retourne le pseudo sauvegardé (accessible depuis d'autres scripts).
    /// </summary>
    public static string GetSavedPseudo()
        => PlayerPrefs.GetString(PseudoKey, "");

    /// <summary>
    /// Retourne l'identifiant unique du joueur (créé à la première session).
    /// </summary>
    public static string GetPlayerId()
    {
        var id = PlayerPrefs.GetString(PlayerIdKey, "");
        if (!string.IsNullOrEmpty(id)) return id;

        id = Guid.NewGuid().ToString();
        PlayerPrefs.SetString(PlayerIdKey, id);
        PlayerPrefs.Save();
        return id;
    }

    private IEnumerator PostPseudoToServer(string pseudo)
    {
        var payload = $"{{\"playerId\":\"{GetPlayerId()}\",\"pseudo\":\"{pseudo}\"}}";
        var bodyRaw = Encoding.UTF8.GetBytes(payload);

        using var request = new UnityWebRequest(leaderboardApiUrl + "/player", "POST");
        request.uploadHandler   = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogWarning($"[Leaderboard] Erreur envoi pseudo : {request.error}");
        else
            Debug.Log($"[Leaderboard] Pseudo '{pseudo}' envoyé.");
    }
}

