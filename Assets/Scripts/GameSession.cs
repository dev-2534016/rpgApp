using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    public Utilisateur CurrentUser { get; private set; }
    public Personnage CurrentPersonnage { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // reste entre les scènes si tu en utilises
    }

    public void SetUser(Utilisateur user)
    {
        CurrentUser = user;
    }

    public void SetPersonnage(Personnage perso)
    {
        CurrentPersonnage = perso;
    }
}

