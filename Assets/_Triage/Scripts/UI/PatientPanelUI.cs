using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Affiche les données d'un patient (nom, âge, diagnostic, etc.) dans un panneau UI.
/// Ce script est attaché à UN panneau patient. Tu en auras deux dans la scène :
/// un pour le Patient A (gauche) et un pour le Patient B (droite).
///
/// DANS L'INSPECTOR : assigne chaque champ TextMeshPro au bon texte de ton Canvas.
/// </summary>
public class PatientPanelUI : MonoBehaviour
{
    // =========================================================
    // RÉFÉRENCES UI (à assigner dans l'Inspector)
    // =========================================================

    [Header("Textes du panneau")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI ageText;
    [SerializeField] private TextMeshProUGUI diagnosisText;
    [SerializeField] private TextMeshProUGUI arrivalOrderText;
    [SerializeField] private TextMeshProUGUI backgroundText;
    [SerializeField] private TextMeshProUGUI pressureText;       // Texte de pression (dilemmes moraux)
    [SerializeField] private TextMeshProUGUI severityText;       // "Gravité : 8/10"
    [SerializeField] private TextMeshProUGUI survivalChanceText; // "Survie estimée : 65%" (dilemmes éthiques)

    [Header("Portrait du patient")]
    [SerializeField] private Image portraitImage;

    [Header("Indicateurs visuels")]
    [SerializeField] private Image severityBar;        // Barre de gravité (remplie selon severityLevel)
    [SerializeField] private GameObject pressurePanel; // Panneau de pression (affiché seulement si hasPressure)

    [Header("Bouton de choix")]
    [SerializeField] private Button choiceButton;

    [Header("Ce panneau est-il le Patient A ou B ?")]
    [SerializeField] private bool isPatientA = true;

    // =========================================================
    // COULEURS DE GRAVITÉ
    // =========================================================

    [Header("Couleurs de la barre de gravité")]
    [SerializeField] private Color lowSeverityColor    = new Color(0.2f, 0.8f, 0.2f);  // Vert
    [SerializeField] private Color medSeverityColor    = new Color(1f,   0.6f, 0f);    // Orange
    [SerializeField] private Color highSeverityColor   = new Color(0.9f, 0.1f, 0.1f); // Rouge

    // =========================================================
    // INITIALISATION
    // =========================================================

    private void OnEnable()
    {
        // S'abonne à l'EventBus pour recevoir le scénario quand il se charge
        EventBus.OnScenarioStarted += OnScenarioStarted;
    }

    private void OnDisable()
    {
        // Se désabonne pour éviter les fuites mémoire
        EventBus.OnScenarioStarted -= OnScenarioStarted;
    }

    // =========================================================
    // AFFICHAGE DES DONNÉES
    // =========================================================

    private void OnScenarioStarted(ScenarioData scenario)
    {
        // Récupère le bon patient selon le côté du panneau
        PatientData patient = isPatientA ? scenario.patientA : scenario.patientB;

        if (patient == null)
        {
            Debug.LogWarning($"[PatientPanelUI] Patient {(isPatientA ? "A" : "B")} est null !");
            return;
        }

        DisplayPatient(patient);
    }

    //private void DisplayPatient(PatientData patient)
    public void DisplayPatient(PatientData patient)
    {
        // --- Textes de base ---
        if (nameText)         nameText.text         = patient.patientName;
        if (ageText)          ageText.text          = $"{patient.age} ans";
        if (diagnosisText)    diagnosisText.text    = patient.diagnosis;
        if (arrivalOrderText) arrivalOrderText.text = patient.arrivalOrder;
        if (backgroundText)   backgroundText.text  = patient.backgroundContext;
        if (severityText)     severityText.text     = $"Gravité : {patient.severityLevel}/10";

        // --- Survie (uniquement pour les dilemmes éthiques) ---
        if (survivalChanceText)
        {
            if (patient.survivalChance > 0)
            {
                survivalChanceText.gameObject.SetActive(true);
                survivalChanceText.text = $"Survie estimée : {patient.survivalChance}%";
            }
            else
            {
                survivalChanceText.gameObject.SetActive(false);
            }
        }

        // --- Portrait ---
        if (portraitImage && patient.portrait != null)
        {
            portraitImage.sprite = patient.portrait;
            portraitImage.gameObject.SetActive(true);
        }

        // --- Barre de gravité ---
        if (severityBar)
        {
            // Remplie proportionnellement (0 à 10)
            severityBar.fillAmount = patient.severityLevel / 10f;
            severityBar.color = GetSeverityColor(patient.severityLevel);
        }

        // --- Panneau de pression ---
        if (pressurePanel)
            pressurePanel.SetActive(patient.hasPressure);

        if (pressureText && patient.hasPressure)
            pressureText.text = patient.pressureDescription;
    }

    // =========================================================
    // UTILITAIRES
    // =========================================================

    private Color GetSeverityColor(int severity)
    {
        if (severity <= 3) return lowSeverityColor;
        if (severity <= 6) return medSeverityColor;
        return highSeverityColor;
    }

    /// <summary>Active ou désactive le bouton de choix (ex: pendant l'écran de résultat).</summary>
    public void SetButtonInteractable(bool interactable)
    {
        if (choiceButton) choiceButton.interactable = interactable;
    }
}
