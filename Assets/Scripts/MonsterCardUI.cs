using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterCardUI : MonoBehaviour
{
    public Image background;
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statusText;

    public Color huntedColor = Color.green;
    public Color notHuntedColor = Color.gray;

    public void SetData(string nom, bool estVaincu)
    {
        nameText.text = nom;

        if (estVaincu)
        {
            statusText.text = "✔ chassé";
            background.color = huntedColor;
        }
        else
        {
            statusText.text = "Non chassé";
            background.color = notHuntedColor;
        }
    }
}
