using UnityEngine;

/// <summary>
/// Décrit les conséquences narratives et mécaniques d'un choix dans un scénario.
/// Une instance par choix possible (ex : ConsequenceA_Scenario1, ConsequenceB_Scenario1)
/// Créer via : clic droit dans Assets > Create > Triage > ConsequenceData
/// </summary>
[CreateAssetMenu(fileName = "NewConsequence", menuName = "Triage/ConsequenceData")]
public class ConsequenceData : ScriptableObject
{
    [Header("Narration")]
    [TextArea(3, 8)]
    public string immediateText;      // Ce qui arrive juste après le choix
    [TextArea(3, 8)]
    public string laterText;          // Conséquences différées (scénarios suivants)

    [Header("Impact sur les 4 jauges de réputation")]
    [Range(-30, 30)]
    public int medicalScoreDelta;     // Bilan médical
    [Range(-30, 30)]
    public int teamTrustDelta;        // Confiance équipe
    [Range(-30, 30)]
    public int publicReputationDelta; // Réputation publique
    [Range(-30, 30)]
    public int budgetBalanceDelta;    // Équilibre budgétaire
    [Range(-30, 30)]
    public int moralScoreDelta;
    [Header("Événements spéciaux déclenchés")]
    public bool triggersInternalInquiry;   // Enquête interne
    public bool triggersStrike;            // Grève infirmières
    public bool triggersMediaCoverage;     // Médias / presse
    public bool triggersLegalAction;       // Plainte / procédure juridique
    public bool triggersServiceClosure;    // Fermeture d'un service

    [Header("Ton émotionnel de l'écran de résultat")]
    public ResultTone tone;
    public Color resultScreenColor;   // Couleur dominante de l'écran de résultat
}

public enum ResultTone
{
    Neutral,
    Regret,       // Bon choix médical mais coût humain visible
    Relief,       // Tension résolue positivement
    Guilt,        // Le joueur a cédé à la pression
    Ambiguous,    // Dilemme éthique — aucune "bonne" réponse
    Shock         // Conséquence dramatique inattendue
}
