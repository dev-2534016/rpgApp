using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;       // si tu utilises InputField classique
using TMPro;                // si tu utilises TMP_InputField

[Serializable]
public class LoginUser
{
    public string Email;
    public string MotDePasse;
}

[Serializable]
public class Utilisateur
{
    public int id;
    public string email;
    public string pseudo;
    // ajoute d'autres champs si tu en as (xp, niveau, etc.)
}

public class LoginManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField pseudoInput;      
    public TMP_InputField passwordInput;    
    public TextMeshProUGUI errorText;       

    [Header("Ecrans")]
    public GameObject loginScreen;          
    public GameObject mainScreen;           
    public GameObject bottomBar;

    [Header("API")]
    public string loginUrl = "https://localhost:7061/api/Utilisateurs/auth/login";
    public string personnageUrl = "https://localhost:7061/api/Personnages"; // base
    public PokedexManager pokedexManager;


    private Utilisateur currentUser;


    void Start()
    {
        if (bottomBar != null)
            bottomBar.SetActive(false);   // on cache la BottomBar pendant le login

        if (mainScreen != null)
            mainScreen.SetActive(false);  // on laisse juste le LoginScreen au début
    }

    public void OnClickLogin()
    {
        var email = pseudoInput.text;       
        var password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowError("Veuillez remplir tous les champs.");
            return;
        }

        LoginUser loginData = new LoginUser
        {
            Email = email,
            MotDePasse = password
        };

        StartCoroutine(SendLoginRequest(loginData));
    }

    private IEnumerator SendLoginRequest(LoginUser loginData)
    {
        ShowError("");

        string json = JsonUtility.ToJson(loginData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest req = new UnityWebRequest(loginUrl, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("accept", "application/json"); 

            yield return req.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (req.result != UnityWebRequest.Result.Success)
#else
            if (req.isHttpError || req.isNetworkError)
#endif
            {
                Debug.LogError("Erreur réseau login : " + req.error);
                ShowError("Erreur de connexion au serveur.");
                yield break;
            }

            long statusCode = req.responseCode;

            if (statusCode == 200)
            {
                string responseJson = req.downloadHandler.text;
                currentUser = JsonUtility.FromJson<Utilisateur>(responseJson);

                Debug.Log("Connecté en tant que " + currentUser.email);

                GameSession.Instance.SetUser(currentUser);

                yield return StartCoroutine(FetchPersonnageForUser(currentUser.id));
                if (pokedexManager != null)
                {
                    pokedexManager.InitForCurrentPlayer();
                }

                if (loginScreen != null) loginScreen.SetActive(false);
                if (mainScreen != null) mainScreen.SetActive(true);
                if (bottomBar != null) bottomBar.SetActive(true);
                if (pokedexManager != null)
{
    pokedexManager.InitForCurrentPlayer();
}
            }
            else if (statusCode == 400)
            {
                ShowError("Email ou mot de passe invalide.");
            }
            else if (statusCode == 401)
            {
                Debug.Log("401 : " + req.downloadHandler.text);
                ShowError("Email ou mot de passe incorrect.");
            }
            else
            {
                Debug.LogWarning("Code HTTP inattendu : " + statusCode + " - " + req.downloadHandler.text);
                ShowError("Erreur serveur (" + statusCode + ").");
            }
        }
    }

    private IEnumerator FetchPersonnageForUser(int userId)
    {
        string url = $"{personnageUrl}/User/{userId}";

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
                Debug.LogError("Erreur récupération personnage : " + req.error);
                ShowError("Impossible de récupérer le personnage.");
                yield break;
            }

            if (req.responseCode == 200)
            {
                string json = req.downloadHandler.text;
                Personnage perso = JsonUtility.FromJson<Personnage>(json);

                GameSession.Instance.SetPersonnage(perso);

                Debug.Log("Personnage chargé, id = " + perso.id);
            }
            else
            {
                Debug.LogWarning("Erreur HTTP personnage : " + req.responseCode + " - " + req.downloadHandler.text);
                ShowError("Aucun personnage trouvé pour cet utilisateur.");
            }
        }
    }


    private void ShowError(string msg)
    {
        if (errorText != null)
        {
            errorText.text = msg;
        }
    }
}
