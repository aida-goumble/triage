using UnityEngine;

/// <summary>
/// Gère le déroulement d'un scénario dans la TriageScene.
/// Ce script est placé sur un GameObject dans la scène TriageScene.
/// 
/// Il récupère le scénario en cours depuis le GameManager,
/// et notifie l'UI (via EventBus) pour qu'elle affiche les bonnes données.
///
/// COMMENT ÇA FONCTIONNE :
///   1. La scène TriageScene se charge
///   2. Start() appelle LoadCurrentScenario()
///   3. LoadCurrentScenario() diffuse les données via EventBus
///   4. Les scripts UI écoutent l'EventBus et s'affichent
///   5. Le joueur clique → OnChoiceButtonClicked() → GameManager.MakeChoice()
/// </summary>
public class ScenarioManager : MonoBehaviour
{
    // =========================================================
    // ÉTAT INTERNE
    // =========================================================

    private ScenarioData currentScenario;

    /// <summary>Mémorise le dernier choix du joueur pour la ResultScene.</summary>
    public static bool LastChoiceWasPatientA { get; private set; }

    // =========================================================
    // INITIALISATION
    // =========================================================

    private void Start()
    {
        LoadCurrentScenario();
    }

    private void LoadCurrentScenario()
    {
        // Sécurité : vérifie que le GameManager existe
        if (GameManager.Instance == null)
        {
            Debug.LogError("[ScenarioManager] GameManager introuvable ! " +
                           "Assure-toi que le GameManager est présent dans la scène.");
            return;
        }

        currentScenario = GameManager.Instance.CurrentScenario;

        if (currentScenario == null)
        {
            Debug.LogError("[ScenarioManager] Aucun scénario en cours !");
            return;
        }

        // Diffuse le scénario à tous les scripts qui écoutent (UI, sons, etc.)
        EventBus.OnScenarioStarted?.Invoke(currentScenario);

        Debug.Log($"[ScenarioManager] Scénario chargé : #{currentScenario.scenarioId} " +
                  $"— {currentScenario.scenarioTitle} " +
                  $"[{(currentScenario.IsMoral ? "MORAL" : "ÉTHIQUE")}]");
    }

    // =========================================================
    // CHOIX DU JOUEUR
    // =========================================================

    /// <summary>
    /// À appeler depuis le bouton "Choisir Patient A" dans l'UI.
    /// Dans l'Inspector du bouton Unity : OnClick() → ScenarioManager.ChoosePatientA()
    /// </summary>
    public void ChoosePatientA()
    {
        if (!ValidateChoice()) return;
        LastChoiceWasPatientA = true;
        GameManager.Instance.MakeChoice(true);
    }

    /// <summary>
    /// À appeler depuis le bouton "Choisir Patient B" dans l'UI.
    /// Dans l'Inspector du bouton Unity : OnClick() → ScenarioManager.ChoosePatientB()
    /// </summary>
    public void ChoosePatientB()
    {
        if (!ValidateChoice()) return;
        LastChoiceWasPatientA = false;
        GameManager.Instance.MakeChoice(false);
    }

    // =========================================================
    // UTILITAIRES
    // =========================================================

    /// <summary>Vérifie qu'un choix est possible avant de l'accepter.</summary>
    private bool ValidateChoice()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[ScenarioManager] Impossible de faire un choix : GameManager manquant.");
            return false;
        }

        if (currentScenario == null)
        {
            Debug.LogError("[ScenarioManager] Impossible de faire un choix : scénario null.");
            return false;
        }

        return true;
    }

    /// <summary>Retourne les données du Patient A du scénario en cours.</summary>
    public PatientData GetPatientA() => currentScenario?.patientA;

    /// <summary>Retourne les données du Patient B du scénario en cours.</summary>
    public PatientData GetPatientB() => currentScenario?.patientB;

    /// <summary>Retourne le texte d'introduction du scénario en cours.</summary>
    public string GetIntroText() => currentScenario?.introductionText ?? "";

    /// <summary>True si le scénario est un dilemme moral.</summary>
    public bool CurrentScenarioIsMoral() => currentScenario?.IsMoral ?? false;
}
