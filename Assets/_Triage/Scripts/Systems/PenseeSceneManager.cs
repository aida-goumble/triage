using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PenseeSceneManager : MonoBehaviour
{
    [Header("Éléments dans l'ordre d'apparition")]
    [SerializeField] private CanvasGroup guillemetsOuvrant;  // guillemets_0
    [SerializeField] private CanvasGroup guillemetsOuvrant1; // guillemets_1
    [Header("Données de la scène (fallback si pas de GameManager)")]
    [SerializeField] private PenseeSceneData sceneData;
    [SerializeField] private CanvasGroup textePensee;        // TextePensee
    [Header("Typewriter")]
    [SerializeField] private TMP_Text texteTypewriter;   // le composant TMP dans textePensee
    [SerializeField] private float vitesseTypewriter = 0.04f;
    [SerializeField] private CanvasGroup nomScenario;        // Nom_scenario

    [Header("Timing")]
    [SerializeField] private float dureeFadeIn       = 1f;
    [SerializeField] private float delaiApresGuillemets = 0.8f; // pause avant le texte
    [SerializeField] private float delaiApresTexte   = 1.2f;    // pause avant nom scénario
    [SerializeField] private float delaiAvantContinuer = 5f;    // attente après tout

    [Header("UI Continue")]
    [SerializeField] private CanvasGroup texteContinuer;

    [Header("Conséquence du choix")]
    [SerializeField] private CanvasGroup consequence;
    [SerializeField] private TMP_Text texteConsequence;
    
    [Header("Bouton Skip")]
    [SerializeField] private UnityEngine.UI.Button btnSkip;

    private bool _peutContinuer = false;

    void Start()
    {
        sceneData = GameManager.Instance?.CurrentPenseeSceneData ?? sceneData;

        // Tout cacher au départ
        guillemetsOuvrant.alpha  = 0f;
        guillemetsOuvrant1.alpha = 0f;
        textePensee.alpha        = 0f;
        nomScenario.alpha        = 0f;
        texteContinuer.alpha     = 0f;
        consequence.alpha        = 0f;

        var tmp = textePensee.GetComponent<TMPro.TMP_Text>();
        if (tmp != null) tmp.text = sceneData.textePensee;

        var tmpNom = nomScenario.GetComponent<TMPro.TMP_Text>();
        if (tmpNom != null) tmpNom.text = sceneData.nomScenario;

        if (texteConsequence != null && GameManager.Instance?.LastConsequence != null)
        {
            texteConsequence.text = GameManager.Instance.LastConsequence.immediateText + "\n" + GameManager.Instance.LastConsequence.laterText;
            texteConsequence.maxVisibleCharacters = 0;
        }
        
        if (btnSkip != null)
            btnSkip.onClick.AddListener(SkipScene);

        StartCoroutine(JouerSequence());
    }

    void Update()
    {
        if (_peutContinuer && Mouse.current.leftButton.wasPressedThisFrame)
            StartCoroutine(PasserSceneSuivante());
    }

    private IEnumerator JouerSequence()
    {
        yield return new WaitForSeconds(0.4f);

        // Étape 1 — Les deux guillemets ensemble (simultané)
        StartCoroutine(FadeIn(guillemetsOuvrant,  dureeFadeIn));
        yield return StartCoroutine(FadeIn(guillemetsOuvrant1, dureeFadeIn));

        yield return new WaitForSeconds(delaiApresGuillemets);

        // Étape 2 — Texte de la pensée
        yield return StartCoroutine(TypewriterEffect(texteTypewriter, vitesseTypewriter));

        yield return new WaitForSeconds(delaiApresTexte);

        // Étape 4 — "Cliquer pour continuer"
        yield return StartCoroutine(FadeIn(texteContinuer, 0.6f));

        _peutContinuer = true;
    }

    private IEnumerator PasserSceneSuivante()
    {
        _peutContinuer = false;

        yield return StartCoroutine(FadeOut(texteContinuer, 0.3f));

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeIn(consequence, dureeFadeIn));
        yield return new WaitForSeconds(0.6f);
        yield return StartCoroutine(TypewriterEffect(texteConsequence, 0.05f));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeIn(nomScenario, 0.3f));
        yield return new WaitForSeconds(delaiAvantContinuer);
        if (GameManager.Instance != null)
            GameManager.Instance.OnCollegueSceneComplete();
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("TriageScene");
    }

    private IEnumerator TypewriterEffect(TMP_Text tmp, float vitesse)
    {
        tmp.maxVisibleCharacters = 0;
        textePensee.alpha = 1f; // rendre le CanvasGroup visible d'emblée

        int total = tmp.textInfo.characterCount;

        for (int i = 0; i <= total; i++)
        {
            tmp.maxVisibleCharacters = i;
            yield return new WaitForSeconds(vitesse);
        }
    }

    private IEnumerator FadeIn(CanvasGroup cg, float duree)
    {
        float elapsed = 0f;
        while (elapsed < duree)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(elapsed / duree);
            yield return null;
        }
        cg.alpha = 1f;
    }

    private IEnumerator FadeOut(CanvasGroup cg, float duree)
    {
        float elapsed = 0f;
        float depart  = cg.alpha;
        while (elapsed < duree)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(depart - (elapsed / duree));
            yield return null;
        }
        cg.alpha = 0f;
    }
    
    public void SkipScene()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ContinueToNextScenario();
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("TriageScene");
    }
}