using TMPro;
using UnityEngine;

public class QuestCardUI : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public TMP_Text progressText;

    public void SetQuest(string title, string description, string progress)
    {
        if (titleText != null) titleText.text = title;
        if (descriptionText != null) descriptionText.text = description;
        if (progressText != null) progressText.text = progress;
    }
}
