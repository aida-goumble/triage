using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class CollegueSceneManager : MonoBehaviour
{
    [Header("Bulles de dialogue (dans l'ordre d'apparition)")]
    [SerializeField] private CanvasGroup[] bulles;

    [Header("Données de la scène (fallback si pas de GameManager)")]
    [SerializeField] private CollegueSceneData sceneData;

    [Header("UI Continue")]
    [SerializeField] private CanvasGroup texteContinuer; // ton objet texte "Cliquer pour continuer"

    [Header("Timing")]
    [SerializeField] private float delaiEntreApparitions = 2.5f;
    [SerializeField] private float dureeFadeIn = 0.4f;
    [SerializeField] private float delaiAvantContinuer = 2f; // attente après dernière bulle

    [Header("Nom du scénario")]
    [SerializeField] private CanvasGroup nomScenario;

    [Header("Conséquence du choix")]
    [SerializeField] private CanvasGroup consequence;
    [SerializeField] private TMP_Text texteConsequence;
    
    [Header("Bouton Skip")]
    [SerializeField] private UnityEngine.UI.Button btnSkip;

    private bool _peutContinuer = false;

    void Start()
    {
        sceneData = GameManager.Instance?.CurrentCollegueSceneData ?? sceneData;

        // Injecter les textes depuis le ScriptableObject
        for (int i = 0; i < bulles.Length; i++)
        {
            if (sceneData == null || i >= sceneData.dialogues.Length) break;

            var dialogue = sceneData.dialogues[i];

            // Cherche les deux TMP enfants par nom
            var nom   = bulles[i].transform.Find("NomPersonnage")?.GetComponent<TMP_Text>();
            var texte = bulles[i].transform.Find("TexteDialogue")?.GetComponent<TMP_Text>();

            if (nom   != null) nom.text   = dialogue.nomPersonnage;
            if (texte != null) texte.text = dialogue.texte;
        }

        foreach (var b in bulles)
        {
            b.alpha = 0f;
            b.gameObject.SetActive(true);
        }

        // Cacher le texte continuer au départ
        texteContinuer.alpha = 0f;
        texteContinuer.gameObject.SetActive(true);
        consequence.alpha = 0f;
        nomScenario.alpha = 0f;

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
        // Le clic n'est valide QUE si le texte est apparu
        if (_peutContinuer && Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartCoroutine(PasserSceneSuivante());
        }
    }

    private IEnumerator JouerSequence()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (var bulle in bulles)
        {
            yield return StartCoroutine(FadeIn(bulle, dureeFadeIn));
            yield return new WaitForSeconds(delaiEntreApparitions);
        }

        // Attendre 2 secondes après la dernière bulle
        yield return new WaitForSeconds(delaiAvantContinuer);

        // Faire apparaître le texte "Cliquer pour continuer"
        yield return StartCoroutine(FadeIn(texteContinuer, 0.6f));

        // Maintenant seulement on autorise le clic
        _peutContinuer = true;
    }

    private IEnumerator PasserSceneSuivante()
    {
        _peutContinuer = false; // évite double clic

        // Fade out du texte avant de partir
        yield return StartCoroutine(FadeOut(texteContinuer, 0.3f));

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeIn(consequence, dureeFadeIn));
        yield return new WaitForSeconds(0.6f);
        yield return StartCoroutine(TypewriterEffect(texteConsequence, 0.05f));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeIn(nomScenario, 0.3f));
        yield return new WaitForSeconds(delaiAvantContinuer);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCollegueSceneComplete();
        }
        else
        {
            // Fallback pour tester la scène en isolation
            UnityEngine.SceneManagement.SceneManager.LoadScene("TriageScene");
        }
    }

    private IEnumerator TypewriterEffect(TMP_Text tmp, float vitesse)
    {
        tmp.maxVisibleCharacters = 0;
        consequence.alpha = 1f; // rendre le CanvasGroup visible d'emblée

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
        float depart = cg.alpha;
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