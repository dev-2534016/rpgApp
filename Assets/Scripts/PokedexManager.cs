
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

#region JSON CLASSES  ------------------------------------

[Serializable]
public class Personnage
{
    public int id;
    public int niveau;
    public int xp;
    public int pv;
    public int pvMax;
    public int force;
    public int defense;
    public int x;
    public int y;
}

[Serializable]
public class Monstre
{
    public int id;
    public int pokemonId;
    public string nom;
    public int pointsVieBase;
    public int forceBase;
    public int defenseBase;
    public int experienceBase;
    public string spriteUrl;
    public string type1;
    public string type2;
}

[Serializable]
public class PokedexEntry
{
    public Monstre monstre;
    public bool estVaincu;
}

[Serializable]
public class PokedexResponse
{
    public Personnage personnage;
    public List<PokedexEntry> pokedex;
}

#endregion

public class PokedexManager : MonoBehaviour
{
    [Header("API")]
    [SerializeField] private string baseUrl = "https://localhost:7061/api/Monsters/pokedex";

    [Header("UI")]
    public Transform gridParent;           // Content de ton ScrollView
    public MonsterCardUI cardPrefab;       // Prefab de la carte
    public TextMeshProUGUI huntedCounter;  // (optionnel) texte "x / y chassés"

    // Appelé par exemple dans Start ou depuis un bouton
    private void Start()
    {
        // Exemple avec les paramètres de ta requête
        StartCoroutine(LoadPokedex(
            personnageId: 57,
            page: 2,
            pageSize: 20,
            type: "",
            nom: ""));
    }

    public IEnumerator LoadPokedex(int personnageId, int page, int pageSize, string type, string nom)
    {
        // Construction de l’URL
        string url = $"{baseUrl}/{personnageId}?page={page}&pageSize={pageSize}&type={type}&nom={nom}";

        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            // Tu peux garder ou enlever ce header, selon ce que ton API attend
            req.SetRequestHeader("accept", "text/plain");

            yield return req.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (req.result != UnityWebRequest.Result.Success)
#else
            if (req.isNetworkError || req.isHttpError)
#endif
            {
                Debug.LogError("Erreur Pokedex : " + req.error + "\n" + url);
                yield break;
            }

            string json = req.downloadHandler.text;
            // Debug.Log(json); // pour vérifier

            // Désérialisation
            PokedexResponse data = JsonUtility.FromJson<PokedexResponse>(json);
            if (data == null)
            {
                Debug.LogError("Impossible de parser le JSON du pokedex.");
                yield break;
            }

            // Mise à jour de l’UI
            yield return StartCoroutine(RefreshUI(data));
        }
    }

    private IEnumerator RefreshUI(PokedexResponse data)
    {
        // Effacer les cartes existantes
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        int chasses = 0;

        foreach (var entry in data.pokedex)
        {
            // Instancier la carte
            MonsterCardUI card = Instantiate(cardPrefab, gridParent);
            bool vaincu = entry.estVaincu;
            if (vaincu) chasses++;

            // On commence la config de base (texte + couleurs)
            card.SetData(entry.monstre.nom, vaincu);

            // Charger le sprite depuis l'URL
            if (!string.IsNullOrEmpty(entry.monstre.spriteUrl))
            {
                yield return StartCoroutine(LoadSpriteFromUrl(entry.monstre.spriteUrl, card.icon));
            }
        }

        if (huntedCounter != null)
        {
            huntedCounter.text = $"{chasses} / {data.pokedex.Count} chassé(s)";
        }
    }

    private IEnumerator LoadSpriteFromUrl(string url, Image targetImage)
    {
        using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(url))
        {
            yield return req.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (req.result != UnityWebRequest.Result.Success)
#else
            if (req.isNetworkError || req.isHttpError)
#endif
            {
                Debug.LogWarning("Erreur chargement sprite : " + url + " -> " + req.error);
                yield break;
            }

            Texture2D tex = DownloadHandlerTexture.GetContent(req);
            Sprite sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f),
                100f
            );
            targetImage.sprite = sprite;
        }
    }
}
