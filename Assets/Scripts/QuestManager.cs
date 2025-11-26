using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class QuestManager : MonoBehaviour
{
    [Header("API")]
    // par ex : "https://localhost:7061/api/Quest/QuetesPerso/"
    public string baseUrl = "https://localhost:7061/api/Quest/QuetesPerso/";
    public int personnageId = 57;

    [Header("UI")]
    public RectTransform content;        // Scroll View -> Viewport -> Content
    public GameObject questCardPrefab;   // prefab QuestCard
    public GameObject emptySlotPrefab;   // optionnel
    public int maxCardsToShow = 3;

    private void Start()
    {
        StartCoroutine(LoadQuestsCoroutine());
    }

    private IEnumerator LoadQuestsCoroutine()
    {
        string url = baseUrl + personnageId;

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Erreur API Quetes : " + www.error);
                Debug.Log(www.downloadHandler.text);
                yield break;
            }

            string json = www.downloadHandler.text;
            Debug.Log("Réponse Quetes: " + json);

            QuestDTO dto = JsonUtility.FromJson<QuestDTO>(json);
            Debug.Log($"nbQuetes={dto.nbQuetes}, " +
                        $"niveau={dto.queteNiveauAtteints?.Count ?? 0}, " +
                        $"monstres={dto.quetesVaincreMonstre?.Count ?? 0}, " +
                        $"tuiles={dto.quetesVisiterTuile?.Count ?? 0}");


            if (dto == null)
            {
                Debug.LogError("Impossible de parser QuestDTO");
                yield break;
            }

            // On construit une liste de quêtes à afficher
            var displayQuests = new List<(string title, string desc, string progress)>();

            // 1) Quêtes niveau
            if (dto.queteNiveauAtteints != null)
            {
                foreach (var q in dto.queteNiveauAtteints)
                {
                    displayQuests.Add((
                        title: "Gain de Niveau",
                        desc: $"Monter au niveau {q.niveauAAtteindre}",
                        progress: $"Niveau actuel : {q.niveauPerso} / {q.niveauAAtteindre}"
                    ));
                }
            }

            // 2) Quêtes monstres
            if (dto.quetesVaincreMonstre != null)
            {
                foreach (var q in dto.quetesVaincreMonstre)
                {
                    displayQuests.Add((
                        title: "Vaincre des monstres",
                        desc: $"Vaincre {q.nbMonstresAVaincre} monstres de type {q.typeMonstre}",
                        progress: $"{q.nbMonstresVaincu} / {q.nbMonstresAVaincre}"
                    ));
                }
            }

            // 3) Quêtes tuiles
            if (dto.quetesVisiterTuile != null)
            {
                foreach (var q in dto.quetesVisiterTuile)
                {
                    displayQuests.Add((
                        title: "Explorer",
                        desc: $"Atteindre la case ({q.x}, {q.y})",
                        progress: ""
                    ));
                }
            }


            // On limite à maxCardsToShow
            int count = Mathf.Min(maxCardsToShow, displayQuests.Count);

            for (int i = 0; i < count; i++)
            {
                var data = displayQuests[i];

                GameObject cardObj = Instantiate(questCardPrefab, content);
                QuestCardUI ui = cardObj.GetComponent<QuestCardUI>();

                if (ui != null)
                {
                    ui.SetQuest(data.title, data.desc, data.progress);
                }
            }

            // Ajout de slots vides jusqu'à maxCardsToShow
            if (emptySlotPrefab != null)
            {
                for (int i = count; i < maxCardsToShow; i++)
                {
                    Instantiate(emptySlotPrefab, content);
                }
            }
        }
    }
}
