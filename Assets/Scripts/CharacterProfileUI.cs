using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterProfileUI : MonoBehaviour
{
    [Header("Textes principaux")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelClassText;

    [Header("Santé")]
    public TextMeshProUGUI healthText;
    public Slider healthSlider;

    [Header("XP")]
    public TextMeshProUGUI xpText;
    public Slider xpSlider;

    [Header("Stats")]
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;

    private void OnEnable()
    {
        RefreshFromGameSession();
    }

    public void RefreshFromGameSession()
    {
        if (GameSession.Instance == null ||
            GameSession.Instance.CurrentPersonnage == null)
        {
            Debug.LogWarning("CharacterProfileUI : pas de personnage dans GameSession.");
            return;
        }

        var perso = GameSession.Instance.CurrentPersonnage;
        var user = GameSession.Instance.CurrentUser;

        // Nom : si tu veux le pseudo de l'utilisateur
        if (nameText != null)
            nameText.text = string.IsNullOrEmpty(user?.pseudo) ? user?.email : user.pseudo;

        if (levelClassText != null)
            levelClassText.text = $"Niveau {perso.niveau} - Guerrier"; // ou autre classe si tu as

        // Santé
        if (healthText != null)
            healthText.text = $"{perso.pv} / {perso.pvMax}";

        if (healthSlider != null)
        {
            healthSlider.maxValue = perso.pvMax;
            healthSlider.value = perso.pv;
        }

        // XP (si tu as un xpMax côté serveur, sinon tu mets une valeur fixe)
        int xpMax = 5000; // TODO : remplacer par ton vrai champ si tu l'ajoutes au backend
        if (xpText != null)
            xpText.text = $"{perso.xp} / {xpMax}";

        if (xpSlider != null)
        {
            xpSlider.maxValue = xpMax;
            xpSlider.value = perso.xp;
        }

        // Stats
        if (attackText != null)
            attackText.text = perso.force.ToString();

        if (defenseText != null)
            defenseText.text = perso.defense.ToString();
    }
}
