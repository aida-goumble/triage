using UnityEngine;

/// <summary>
/// Données complètes d'un patient pour un scénario Triage.
/// Créer via : clic droit dans Assets > Create > Triage > PatientData
/// </summary>
[CreateAssetMenu(fileName = "NewPatient", menuName = "Triage/PatientData")]
public class PatientData : ScriptableObject
{
    [Header("Identité")]
    public string patientName;
    public int age;
    public Sprite portrait;
    public Sprite characterSprite;
    
    [Header("État médical")]
    [Range(0, 10)]
    public int severityLevel;         // 0 = stable, 10 = urgence vitale
    public string diagnosis;          // Ex : "Infarctus massif"
    public string arrivalOrder;       // Ex : "1er arrivé"

    [Header("Ressources nécessaires")]
    public bool needsOperatingRoom;
    public int surgeonsRequired;      // 0, 1, 2 ou 3
    public int bedCost;               // Lits occupés
    public int budgetCost;            // Coût en euros
    public bool needsRareMedication;  // Antibiotiques / anesthésiants rares

    [Header("Survie (pour dilemmes éthiques)")]
    [Range(0, 100)]
    public int survivalChance;        // En %, 0 si non applicable

    [Header("Contexte narratif")]
    [TextArea(3, 6)]
    public string backgroundContext;  // Résumé humain du patient

    [Header("Pression extérieure (dilemmes moraux)")]
    public bool hasPressure;
    [TextArea(2, 4)]
    public string pressureDescription; // Ex : "Menace de plainte, contact avec la direction"
    public PressureType pressureType;
}

public enum PressureType
{
    None,
    Political,    // Pouvoir politique
    Financial,    // Argent / donateur
    Legal,        // Menace juridique
    Family,       // Pression familiale
    Staff         // Pression interne du personnel
}
