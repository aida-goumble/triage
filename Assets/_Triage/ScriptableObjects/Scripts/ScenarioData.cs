using UnityEngine;

/// <summary>
/// Définit un scénario complet du jeu Triage.
/// Contient les deux patients, le type de dilemme, et les conséquences des deux choix.
/// Créer via : clic droit dans Assets > Create > Triage > ScenarioData
/// </summary>
[CreateAssetMenu(fileName = "NewScenario", menuName = "Triage/ScenarioData")]
public class ScenarioData : ScriptableObject
{
    [Header("Identification")]
    public int scenarioId;            // 1 à 10
    public string scenarioTitle;      // Ex : "Le maire et le vieux monsieur"
    public ScenarioDilemmaType dilemmaType;

    [Header("Les deux patients")]
    public PatientData patientA;
    public PatientData patientB;

    [Header("Conséquences selon le choix")]
    public ConsequenceData consequenceIfA;  // Si joueur choisit Patient A
    public ConsequenceData consequenceIfB;  // Si joueur choisit Patient B

    [Header("Texte d'introduction du scénario")]
    [TextArea(3, 6)]
    public string introductionText;

    [Header("Thème narratif")]
    public ScenarioTheme theme;

    [Header("Conditions de déblocage (optionnel)")]
    public bool isAlwaysAvailable = true;
    public int minimumScenarioIndex;  // Ce scénario ne peut apparaître qu'après N scénarios joués

    // --- Propriété calculée ---
    /// <summary>Retourne true si c'est un dilemme moral (pression extérieure)</summary>
    public bool IsMoral => dilemmaType == ScenarioDilemmaType.Moral;
}

public enum ScenarioDilemmaType
{
    /// <summary>
    /// La bonne réponse médicale existe.
    /// Une pression extérieure tente de faire dévier le joueur.
    /// </summary>
    Moral,

    /// <summary>
    /// Deux valeurs légitimes s'affrontent.
    /// Aucune réponse n'est objectivement juste.
    /// </summary>
    Ethical
}

public enum ScenarioTheme
{
    PoliticalPressure,   // Pression politique
    FinancialPressure,   // Argent / donateur
    FamilyLinks,         // Liens familiaux
    StaffPressure,       // Pression interne du personnel
    LegalThreat,         // Menace juridique
    AgeAndValue,         // Dilemme d'âge / valeur de la vie
    SurvivalChance,      // Probabilités de survie opposées
    LimitedResources,    // Ressources critiques uniques
    Moral_vs_Emotion     // Devoir médical vs compassion
}
