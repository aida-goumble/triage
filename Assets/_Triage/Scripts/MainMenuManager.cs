using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Boutons")]
    public UnityEngine.UI.Button btnJouer;
    public UnityEngine.UI.Button btnQuitter;

    [Header("Texte du bouton Jouer")]
    public TMPro.TextMeshProUGUI btnJouerText;

    [Header("Tampon")]
    public TMPro.TextMeshProUGUI stampText;

    [Header("Stats")]
    public TMPro.TextMeshProUGUI statPatientsVal;
    public TMPro.TextMeshProUGUI statDecisionsVal;
    public TMPro.TextMeshProUGUI statReputationVal;

    [Header("Nom de la scène de jeu")]
    public string sceneTriageScene = "TriageScene";

    private const string SAVE_KEY         = "HasSave";
    private const string SAVE_PATIENTS    = "StatsPatients";
    private const string SAVE_DECISIONS   = "StatsDecisions";
    private const string SAVE_REPUTATION  = "StatsReputation";

    void Start()
    {
        bool hasSave = PlayerPrefs.HasKey(SAVE_KEY);

        // Change le texte du bouton selon si une save existe
        if (btnJouerText != null)
            btnJouerText.text = hasSave
                ? "01  CONTINUER"
                : "01  NOUVELLE PARTIE";

        // Change le tampon
        if (stampText != null)
            stampText.text = hasSave ? "EN COURS" : "EN ATTENTE";

        // Affiche les stats si save existe
        if (hasSave)
        {
            if (statPatientsVal)
                statPatientsVal.text   = PlayerPrefs.GetInt(SAVE_PATIENTS,   0).ToString();
            if (statDecisionsVal)
                statDecisionsVal.text  = PlayerPrefs.GetInt(SAVE_DECISIONS,  0).ToString();
            if (statReputationVal)
                statReputationVal.text = PlayerPrefs.GetInt(SAVE_REPUTATION, 0).ToString();
        }

        // Branche les boutons
        btnJouer.onClick.AddListener(OnJouer);
        btnQuitter.onClick.AddListener(OnQuitter);
    }

    void OnJouer()
    {
        bool hasSave = PlayerPrefs.HasKey("HasSave");

        if (!hasSave)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            if (GameManager.Instance != null)
                GameManager.Instance.ResetSession();

            SceneManager.LoadScene("IntroScene");
        }
        else
        {
            // Restaure l'index du scénario sauvegardé
            int savedIndex = PlayerPrefs.GetInt("SavedScenarioIndex", 0);
            if (GameManager.Instance != null)
                GameManager.Instance.RestoreSession(savedIndex);

            SceneManager.LoadScene(sceneTriageScene);
        }
    }

    void OnQuitter()
    {
        Application.Quit();
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}