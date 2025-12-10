
using UnityEngine;
using UnityEngine.UI;  // nécessaire pour VerticalLayoutGroup

public class UIScreenManager : MonoBehaviour
{
    public GameObject questScreen;
    public GameObject pokedexScreen;
    public GameObject characterScreen;    // NEW
    public GameObject leaderboardScreen;  // NEW

    public void ShowQuestScreen()
    {
        questScreen.SetActive(true);
        pokedexScreen.SetActive(false);
        characterScreen.SetActive(false);
        leaderboardScreen.SetActive(false);
    }

    public void ShowPokedexScreen()
    {
        questScreen.SetActive(false);
        pokedexScreen.SetActive(true);
        characterScreen.SetActive(false);
        leaderboardScreen.SetActive(false);
    }

    public void ShowCharacterScreen()
    {
        questScreen.SetActive(false);
        pokedexScreen.SetActive(false);
        characterScreen.SetActive(true);
        leaderboardScreen.SetActive(false);
    }

    public void ShowLeaderboardScreen()
    {
        questScreen.SetActive(false);
        pokedexScreen.SetActive(false);
        characterScreen.SetActive(false);
        leaderboardScreen.SetActive(true);
    }
}

