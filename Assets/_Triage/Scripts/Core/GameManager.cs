using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Chef d'orchestre du jeu. Une seule instance existe pendant toute la partie.
/// Il gère : l'état de la session, les transitions entre scènes, et l'accès au HospitalState.
///
/// COMMENT L'UTILISER DEPUIS UN AUTRE SCRIPT :
///   GameManager.Instance.StartSession();
///   GameManager.Instance.HospitalState.MedicalScore  → lire une jauge
/// </summary>
public class GameManager : MonoBehaviour
{
    // =========================================================
    // SINGLETON
    // =========================================================

    /// <summary>Point d'accès global au GameManager depuis n'importe quel script.</summary>
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // Si une instance existe déjà, on détruit ce doublon
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Cet objet survit au changement de scène
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Lance automatiquement le scénario si on démarre
        // directement depuis la TriageScene (mode test)
        if (scenarios != null && scenarios.Length > 0)
        {
            hospitalState.Reset();
            EventBus.OnSessionStarted?.Invoke();
            EventBus.OnScenarioStarted?.Invoke(CurrentScenario);
        }
    }

    // =========================================================
    // RÉFÉRENCES (à assigner dans l'Inspector)
    // =========================================================

    [Header("État de l'hôpital (glisse HospitalState.asset ici)")]
    [SerializeField] private HospitalState hospitalState;

    [Header("Liste des scénarios dans l'ordre de jeu")]
    [SerializeField] private ScenarioData[] scenarios;

    [Header("Noms des scènes Unity")]
    [SerializeField] private string mainMenuScene    = "MainMenu";
    [SerializeField] private string triageScene      = "TriageScene";
    [SerializeField] private string penseeScene      = "PenseeScene";
    [SerializeField] private string collegueScene    = "CollegueScene";
    [SerializeField] private string endSessionScene  = "EndSession";

    // =========================================================
    // PROPRIÉTÉS PUBLIQUES
    // =========================================================

    /// <summary>Accès au HospitalState depuis n'importe quel script.</summary>
    public HospitalState HospitalState => hospitalState;

    /// <summary>Index du scénario en cours (0 = premier scénario).</summary>
    public int CurrentScenarioIndex { get; private set; }

    /// <summary>Le scénario actuellement affiché.</summary>
    public ScenarioData CurrentScenario => scenarios[CurrentScenarioIndex];

    /// <summary>True si tous les scénarios ont été joués.</summary>
    public bool IsSessionOver => CurrentScenarioIndex >= scenarios.Length;

    // =========================================================
    // DÉMARRAGE DE SESSION
    // =========================================================

    /// <summary>
    /// Appelé depuis le bouton "Nouvelle Partie" du MainMenu.
    /// Remet tout à zéro et charge le premier scénario.
    /// </summary>
    public void StartSession()
    {
        CurrentScenarioIndex = 0;
        hospitalState.Reset();

        EventBus.OnSessionStarted?.Invoke();

        LoadTriageScene();
    }

    // =========================================================
    // FLUX DE JEU
    // =========================================================

    /// <summary>
    /// Appelé par le bouton de choix dans la TriageScene.
    /// chosePatientA = true  → le joueur a choisi Patient A
    /// chosePatientA = false → le joueur a choisi Patient B
    /// </summary>
    public void MakeChoice(bool chosePatientA)
    {
        ScenarioData scenario = CurrentScenario;

        // 1. Déterminer quelle conséquence s'applique
        ConsequenceData consequence = chosePatientA
            ? scenario.consequenceIfA
            : scenario.consequenceIfB;

        // 2. Consommer les ressources du patient traité
        PatientData treatedPatient = chosePatientA ? scenario.patientA : scenario.patientB;
        hospitalState.ConsumeResources(treatedPatient);

        // 3. Appliquer les deltas de jauges
        hospitalState.ApplyConsequence(consequence);

        // 4. Enregistrer la décision (pour le profil moral)
        hospitalState.RegisterScenarioPlayed(scenario, chosePatientA);

        // 5. Notifier l'EventBus
        EventBus.OnChoiceMade?.Invoke(chosePatientA);
        EventBus.OnConsequenceApplied?.Invoke(consequence);
        EventBus.OnReputationChanged?.Invoke();
        EventBus.OnResourcesChanged?.Invoke();

        // 6. Vérifier les jauges critiques
        CheckCriticalThresholds();

        // 7. Passer à l'écran de résultat
        if (consequence.publicReputationDelta < 0)
            LoadCollegueScene();
        else
            LoadPenseeScene();
        //ContinueToNextScenario(); 
    }

    /// <summary>
    /// Appelé depuis le bouton "Continuer" de la PenseeScene ou CollegueScene.
    /// Passe au scénario suivant, ou termine la session.
    /// </summary>
    public void ContinueToNextScenario()
    {
        CurrentScenarioIndex++;

        if (IsSessionOver)
        {
            EventBus.OnSessionEnded?.Invoke();
            SceneManager.LoadScene(endSessionScene);
        }
        else
        {
            LoadTriageScene();
        }
    }

    /// <summary>
    /// Appelé par CollegueSceneManager quand le joueur clique pour continuer.
    /// Charge le scénario suivant dans la TriageScene.
    /// </summary>
    public void OnCollegueSceneComplete()
    {
        ContinueToNextScenario();
    }

    // =========================================================
    // NAVIGATION ENTRE SCÈNES
    // =========================================================

    public void LoadMainMenu()      => SceneManager.LoadScene(mainMenuScene);
    public void LoadTriageScene()   => SceneManager.LoadScene(triageScene);
    public void LoadPenseeScene()   => SceneManager.LoadScene(penseeScene);
    public void LoadCollegueScene() => SceneManager.LoadScene(collegueScene);

    // =========================================================
    // VÉRIFICATION DES SEUILS CRITIQUES
    // =========================================================

    private void CheckCriticalThresholds()
    {
        if (hospitalState.IsMedicalScoreCritical())
            EventBus.OnCriticalEvent?.Invoke("Enquête interne déclenchée. Votre bilan médical est alarmant.");

        if (hospitalState.IsTeamTrustCritical())
            EventBus.OnCriticalEvent?.Invoke("Grève du personnel infirmier. La confiance de l'équipe est au plus bas.");

        if (hospitalState.IsPublicReputationCritical())
            EventBus.OnCriticalEvent?.Invoke("Manifestation devant l'hôpital. Votre réputation publique s'effondre.");

        if (hospitalState.IsBudgetBalanceCritical())
            EventBus.OnCriticalEvent?.Invoke("Fermeture d'un service. L'équilibre budgétaire est critique.");
    }

    // =========================================================
    // UTILITAIRE - ACCÈS AUX CONSÉQUENCES DU SCÉNARIO EN COURS
    // =========================================================

    /// <summary>Retourne la conséquence correspondant au dernier choix effectué.</summary>
    public ConsequenceData GetLastConsequence(bool chosePatientA)
    {
        return chosePatientA
            ? CurrentScenario.consequenceIfA
            : CurrentScenario.consequenceIfB;
    }
}