using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

[Serializable]
public class LeaderboardEntry
{
    public int rang;
    public int personnageId;
    public string nomPersonnage;
    public int niveau;
    public int score;
}

[Serializable]
public class LeaderboardResponse
{
    public string categorie;
    public List<LeaderboardEntry> entries;
}

public class LeaderboardManager : MonoBehaviour
{
    [Header("UI")]
    public Transform contentParent;         // Content du ScrollView
    public LeaderboardRowUI rowPrefab;      // prefab LeaderboardRowCard
    public TextMeshProUGUI titleText;       // "Par niveau" / "Monstres tués" (optionnel)

    [Header("API")]
    public string leaderboardUrl = "https://localhost:7061/api/Personnages/Leaderboard";

    private string currentCategory = "niveau";

    private void OnEnable()
    {
        // Quand l'écran s'affiche, on charge la catégorie courante
        StartCoroutine(LoadLeaderboard(currentCategory));
    }

    // Bouton "Par niveau"
    public void OnClickParNiveau()
    {
        currentCategory = "niveau";
        StartCoroutine(LoadLeaderboard(currentCategory));
    }

    // Bouton "Monstres tués"
    public void OnClickMonstresTues()
    {
        currentCategory = "monstrestues";
        StartCoroutine(LoadLeaderboard(currentCategory));
    }

    private IEnumerator LoadLeaderboard(string categorie)
    {
        // Nettoyer les anciennes lignes
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        if (titleText != null)
        {
            titleText.text = (categorie == "niveau")
                ? "Classement par niveau"
                : "Classement par monstres tués";
        }

        string url = $"{leaderboardUrl}?categorie={categorie}";
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.SetRequestHeader("accept", "application/json");
            yield return req.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (req.result != UnityWebRequest.Result.Success)
#else
            if (req.isHttpError || req.isNetworkError)
#endif
            {
                Debug.LogError("Erreur classement : " + req.error);
                yield break;
            }

            string json = req.downloadHandler.text;
            // Debug.Log("Leaderboard json: " + json);

            LeaderboardResponse data = JsonUtility.FromJson<LeaderboardResponse>(json);
            if (data == null || data.entries == null)
            {
                Debug.LogWarning("Leaderboard : données vides.");
                yield break;
            }

            foreach (var entry in data.entries)
            {
                var row = Instantiate(rowPrefab, contentParent);
                row.Setup(entry);
            }
        }
    }
}
