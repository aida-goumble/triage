using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gère l'écran de résultat émotionnel après un choix.
/// Ce script est sur un GameObject dans la ResultScene.
/// Il lit la dernière conséquence depuis GameManager et l'affiche.
/// </summary>
public class ResultScreenUI : MonoBehaviour
{
    // =========================================================
    // RÉFÉRENCES UI (à assigner dans l'Inspector)
    // =========================================================

    [Header("Textes")]
    [SerializeField] private TextMeshProUGUI immediateText;
    [SerializeField] private TextMeshProUGUI laterText;
    [SerializeField] private TextMeshProUGUI scenarioTitleText;

    [Header("Fond coloré selon le ton émotionnel")]
    [SerializeField] private Image backgroundImage;

    [Header("Icône de ton (optionnel)")]
    [SerializeField] private TextMeshProUGUI toneIconText; // ex: "😔" ou "✓"

    [Header("Bouton Continuer")]
    [SerializeField] private Button continueButton;

    [Header("Délai avant d'activer le bouton Continuer (secondes)")]
    [SerializeField] private float continueDelay = 2f;

    // =========================================================
    // INITIALISATION
    // =========================================================

    private void Start()
    {
        // Désactive le bouton au départ (force le joueur à lire)
        if (continueButton)
        {
            continueButton.interactable = false;
            Invoke(nameof(EnableContinueButton), continueDelay);
        }

        DisplayResult();
    }

    // =========================================================
    // AFFICHAGE DU RÉSULTAT
    // =========================================================

    private void DisplayResult()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[ResultScreenUI] GameManager introuvable !");
            return;
        }

        // Récupère la conséquence du dernier choix
        bool choseA = ScenarioManager.LastChoiceWasPatientA;
        ConsequenceData consequence = GameManager.Instance.GetLastConsequence(choseA);
        ScenarioData scenario = GameManager.Instance.CurrentScenario;

        if (consequence == null)
        {
            Debug.LogError("[ResultScreenUI] Conséquence introuvable !");
            return;
        }

        // --- Titre du scénario ---
        if (scenarioTitleText && scenario != null)
            scenarioTitleText.text = scenario.scenarioTitle;

        // --- Textes narratifs ---
        if (immediateText) immediateText.text = consequence.immediateText;
        if (laterText)     laterText.text     = consequence.laterText;

        // --- Couleur de fond selon le ton ---
        if (backgroundImage)
            backgroundImage.color = consequence.resultScreenColor;

        // --- Icône selon le ton ---
        if (toneIconText)
            toneIconText.text = GetToneIcon(consequence.tone);
    }

    // =========================================================
    // NAVIGATION
    // =========================================================

    private void EnableContinueButton()
    {
        if (continueButton)
            continueButton.interactable = true;
    }

    /// <summary>
    /// Appelé par le bouton "Continuer" dans l'Inspector Unity :
    /// OnClick() → ResultScreenUI.OnContinueClicked()
    /// </summary>
    public void OnContinueClicked()
    {
        GameManager.Instance.ContinueToNextScenario();
    }

    // =========================================================
    // UTILITAIRES
    // =========================================================

    private string GetToneIcon(ResultTone tone)
    {
        return tone switch
        {
            ResultTone.Guilt     => "✗",
            ResultTone.Regret    => "~",
            ResultTone.Relief    => "✓",
            ResultTone.Ambiguous => "?",
            ResultTone.Shock     => "!",
            _                    => " "
        };
    }
}
