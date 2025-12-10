using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardRowUI : MonoBehaviour
{
    [Header("UI refs")]
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI playerText;
    public TextMeshProUGUI valueText;
    public Image background;          // ton fond violet

    [Header("Couleurs")]
    public Color normalColor = new Color(0.7f, 0.3f, 1f);   // violet
    public Color highlightColor = new Color(0.2f, 0.8f, 0.2f); // vert pour le joueur

    // Appelé depuis le manager
    public void Setup(LeaderboardEntry entry)
    {
        if (rankText != null) rankText.text = entry.rang.ToString();
        if (playerText != null) playerText.text = entry.nomPersonnage;
        if (valueText != null) valueText.text = entry.score.ToString("N0"); // 12 345

        // Surligner le perso courant si GameSession existe
        bool isCurrent = false;
        if (GameSession.Instance != null && GameSession.Instance.CurrentPersonnage != null)
        {
            isCurrent = (GameSession.Instance.CurrentPersonnage.id == entry.personnageId);
        }

        if (background != null)
            background.color = isCurrent ? highlightColor : normalColor;
    }
}
