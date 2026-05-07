using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// État global de l'hôpital pour toute la session de jeu.
/// Singleton ScriptableObject : une seule instance partagée entre toutes les scènes.
/// Créer via : clic droit dans Assets > Create > Triage > HospitalState
/// IMPORTANT : Reset() doit être appelé au début de chaque nouvelle session.
/// </summary>
[CreateAssetMenu(fileName = "HospitalState", menuName = "Triage/HospitalState")]
public class HospitalState : ScriptableObject
{
    // =========================================================
    // RESSOURCES HOSPITALIÈRES
    // =========================================================

    [Header("Ressources — valeurs de départ")]
    public int startingBeds = 8;
    public int startingOperatingRooms = 2;
    public int startingSurgeons = 4;
    public int startingBudget = 50000;
    public bool startingRareMedication = true; // Stock disponible ou non

    [Header("Ressources — état courant (runtime)")]
    [SerializeField] private int currentBeds;
    [SerializeField] private int currentOperatingRooms;
    [SerializeField] private int currentSurgeons;
    [SerializeField] private int currentBudget;
    [SerializeField] private bool rareMedicationAvailable;

    [Header("Moral du chef de service")]
    public int startingMoralScore = 100;
    [SerializeField] private int moralScore;
    public int MoralScore => moralScore;

    // Propriétés publiques en lecture
    public int CurrentBeds => currentBeds;
    public int CurrentOperatingRooms => currentOperatingRooms;
    public int CurrentSurgeons => currentSurgeons;
    public int CurrentBudget => currentBudget;
    public bool RareMedicationAvailable => rareMedicationAvailable;

    // =========================================================
    // JAUGES DE RÉPUTATION (0 à 100)
    // =========================================================

    [Header("Jauges de réputation — valeurs initiales")]
    public int startingMedicalScore = 100;
    public int startingTeamTrust = 100;
    public int startingPublicReputation = 100;
    public int startingBudgetBalance = 100;

    [Header("Jauges — état courant (runtime)")]
    [SerializeField] private int medicalScore;
    [SerializeField] private int teamTrust;
    [SerializeField] private int publicReputation;
    [SerializeField] private int budgetBalance;

    public int MedicalScore => medicalScore;
    public int TeamTrust => teamTrust;
    public int PublicReputation => publicReputation;
    public int BudgetBalance => budgetBalance;

    // Seuil en dessous duquel un événement négatif se déclenche
    [Header("Seuils d'alerte")]
    public int criticalThreshold = 30;

    // =========================================================
    // PROGRESSION DE SESSION
    // =========================================================

    [Header("Progression (runtime)")]
    [SerializeField] private int scenariosPlayed;
    [SerializeField] private List<int> playedScenarioIds = new List<int>();

    // Suivi des décisions morales : le joueur a-t-il cédé à la pression ?
    [SerializeField] private int moralFailCount;   // Nombre de fois où le joueur a cédé (dilemmes moraux)
    [SerializeField] private int moralSuccessCount;

    public int ScenariosPlayed => scenariosPlayed;
    public int MoralFailCount => moralFailCount;
    public int MoralSuccessCount => moralSuccessCount;

    // =========================================================
    // MÉTHODES DE GESTION
    // =========================================================

    /// <summary>Appelé au début d'une nouvelle session. Remet tout à zéro.</summary>
    public void Reset()
    {
        currentBeds = startingBeds;
        currentOperatingRooms = startingOperatingRooms;
        currentSurgeons = startingSurgeons;
        currentBudget = startingBudget;
        rareMedicationAvailable = startingRareMedication;

       // medicalScore = startingMedicalScore;
        teamTrust = startingTeamTrust;
        publicReputation = startingPublicReputation;
        budgetBalance = startingBudgetBalance;

        moralScore = startingMoralScore;
        scenariosPlayed = 0;
        moralFailCount = 0;
        moralSuccessCount = 0;
        playedScenarioIds.Clear();
    }

    /// <summary>Consomme les ressources d'un patient traité.</summary>
    public void ConsumeResources(PatientData patient)
    {
        currentBeds -= patient.bedCost;
        currentBudget -= patient.budgetCost;

        if (patient.needsOperatingRoom)
            currentOperatingRooms -= 1;

        currentSurgeons -= patient.surgeonsRequired;

        if (patient.needsRareMedication)
            rareMedicationAvailable = false;

        // Sécurité : jamais en dessous de 0
        currentBeds = Mathf.Max(currentBeds, 0);
        currentOperatingRooms = Mathf.Max(currentOperatingRooms, 0);
        currentSurgeons = Mathf.Max(currentSurgeons, 0);
        currentBudget = Mathf.Max(currentBudget, 0);
    }

    /// <summary>Applique les deltas de jauges d'une ConsequenceData.</summary>
    public void ApplyConsequence(ConsequenceData consequence)
    {
        //medicalScore     = Mathf.Clamp(medicalScore     + consequence.medicalScoreDelta,     0, 100);
        moralScore = Mathf.Clamp(moralScore + consequence.moralScoreDelta, 0, 100);
        teamTrust        = Mathf.Clamp(teamTrust        + consequence.teamTrustDelta,        0, 100);
        publicReputation = Mathf.Clamp(publicReputation + consequence.publicReputationDelta, 0, 100);
        budgetBalance    = Mathf.Clamp(budgetBalance    + consequence.budgetBalanceDelta,    0, 100);
    }

    /// <summary>Enregistre qu'un scénario a été joué et met à jour les compteurs moraux.</summary>
    public void RegisterScenarioPlayed(ScenarioData scenario, bool chosePatientA)
    {
        scenariosPlayed++;
        playedScenarioIds.Add(scenario.scenarioId);

        if (scenario.IsMoral)
        {
            // Dans un dilemme moral, Patient B est toujours le "bon" choix médical
            // (le Patient A étant celui avec la pression extérieure)
            bool choseCorrectly = !chosePatientA;
            if (choseCorrectly) moralSuccessCount++;
            else moralFailCount++;
        }
    }

    /// <summary>Vérifie si une jauge est en dessous du seuil critique.</summary>
    public bool IsMedicalScoreCritical()     => medicalScore     <= criticalThreshold;
    public bool IsTeamTrustCritical()        => teamTrust        <= criticalThreshold;
    public bool IsPublicReputationCritical() => publicReputation <= criticalThreshold;
    public bool IsBudgetBalanceCritical()    => budgetBalance    <= criticalThreshold;

    /// <summary>Calcule le profil moral de fin de session (pour l'écran de bilan).</summary>
    public MoralProfile GetMoralProfile()
    {
        int moralScenarios = moralFailCount + moralSuccessCount;
        if (moralScenarios == 0) return MoralProfile.Undetermined;

        float failRate = (float)moralFailCount / moralScenarios;

        if (failRate == 0f)   return MoralProfile.Principled;    // N'a jamais cédé
        if (failRate < 0.34f) return MoralProfile.Pragmatic;     // A cédé rarement
        if (failRate < 0.67f) return MoralProfile.Torn;          // Hésitant
        return MoralProfile.Compromised;                          // A souvent cédé
    }
}

public enum MoralProfile
{
    Undetermined,   // Seulement des dilemmes éthiques joués
    Principled,     // Jamais cédé à la pression
    Pragmatic,      // Quelques compromis calculés
    Torn,           // Partagé entre devoir et pression
    Compromised     // A sacrifié l'éthique médicale régulièrement
}
