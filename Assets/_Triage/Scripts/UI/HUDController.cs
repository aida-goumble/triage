using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Affiche les 4 jauges de réputation en temps réel dans le HUD.
/// Place ce script sur un GameObject "HUD" dans ta scène.
/// </summary>
public class HUDController : MonoBehaviour
{
    // =========================================================
    // RÉFÉRENCES UI (à assigner dans l'Inspector)
    // =========================================================

    [Header("Barres de jauges (Image type Filled)")]
    [SerializeField] private Image medicalScoreBar;
    [SerializeField] private Image teamTrustBar;
    [SerializeField] private Image publicReputationBar;
    [SerializeField] private Image budgetBalanceBar;

    [Header("Textes des valeurs (optionnel)")]
    [SerializeField] private TextMeshProUGUI medicalScoreText;
    [SerializeField] private TextMeshProUGUI teamTrustText;
    [SerializeField] private TextMeshProUGUI publicReputationText;
    [SerializeField] private TextMeshProUGUI budgetBalanceText;

    [Header("Texte d'événement critique")]
    [SerializeField] private TextMeshProUGUI criticalEventText;
    [SerializeField] private float criticalMessageDuration = 4f;

    // Couleurs des jauges
    [Header("Couleurs")]
    [SerializeField] private Color normalColor   = new Color(0.2f, 0.7f, 0.3f);  // Vert
    [SerializeField] private Color warningColor  = new Color(1f,   0.6f, 0f);    // Orange
    [SerializeField] private Color criticalColor = new Color(0.9f, 0.1f, 0.1f); // Rouge

    private float criticalTimer = 0f;

    // =========================================================
    // ABONNEMENTS AUX ÉVÉNEMENTS
    // =========================================================

    private void OnEnable()
    {
        EventBus.OnReputationChanged += RefreshHUD;
        EventBus.OnSessionStarted    += RefreshHUD;
        EventBus.OnCriticalEvent     += ShowCriticalMessage;
    }

    private void OnDisable()
    {
        EventBus.OnReputationChanged -= RefreshHUD;
        EventBus.OnSessionStarted    -= RefreshHUD;
        EventBus.OnCriticalEvent     -= ShowCriticalMessage;
    }

    private void Start()
    {
        // Affichage initial
        RefreshHUD();

        // Cache le message critique au départ
        if (criticalEventText)
            criticalEventText.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Fait disparaître le message critique après quelques secondes
        if (criticalTimer > 0f)
        {
            criticalTimer -= Time.deltaTime;
            if (criticalTimer <= 0f && criticalEventText)
                criticalEventText.gameObject.SetActive(false);
        }
    }

    // =========================================================
    // RAFRAÎCHISSEMENT DU HUD
    // =========================================================

    private void RefreshHUD()
    {
        if (GameManager.Instance == null) return;

        HospitalState state = GameManager.Instance.HospitalState;

        UpdateBar(medicalScoreBar,      medicalScoreText,      state.MedicalScore);
        UpdateBar(teamTrustBar,         teamTrustText,         state.TeamTrust);
        UpdateBar(publicReputationBar,  publicReputationText,  state.PublicReputation);
        UpdateBar(budgetBalanceBar,     budgetBalanceText,     state.BudgetBalance);
    }

    private void UpdateBar(Image bar, TextMeshProUGUI label, int value)
    {
        if (bar == null) return;

        // Remplie de 0 à 1 (valeur sur 100)
        bar.fillAmount = value / 100f;

        // Couleur selon le niveau
        if      (value <= 30) bar.color = criticalColor;
        else if (value <= 50) bar.color = warningColor;
        else                  bar.color = normalColor;

        if (label) label.text = value.ToString();
    }

    // =========================================================
    // MESSAGE D'ÉVÉNEMENT CRITIQUE
    // =========================================================

    private void ShowCriticalMessage(string message)
    {
        if (criticalEventText == null) return;

        criticalEventText.text = message;
        criticalEventText.gameObject.SetActive(true);
        criticalTimer = criticalMessageDuration;
    }
}
