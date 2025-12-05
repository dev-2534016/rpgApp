using UnityEngine;

public class UIScreenManager : MonoBehaviour
{
    public GameObject questScreen;
    public GameObject pokedexScreen;

    void Start()
    {
        //ShowQuestScreen();
        ShowPokedexScreen();
    }

    public void ShowQuestScreen()
    {
        questScreen.SetActive(true);
        pokedexScreen.SetActive(false);
    }

    public void ShowPokedexScreen()
    {
        questScreen.SetActive(false);
        pokedexScreen.SetActive(true);
    }
}
