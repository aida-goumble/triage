using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Scène de transition qui s'affiche AVANT chaque TriageScene.
/// Apparaît en fondu depuis la scène précédente (CollegueScene ou PenseeScene),
/// affiche le titre et le numéro du scénario suivant,
/// puis disparaît en fondu vers la TriageScene.
///
/// SETUP DANS UNITY :
///   - Une Image "FondNoir" plein écran (noire, alpha 255, en dernier dans la hiérarchie)
///   - Un TMP "NumeroScenario"  ex: "SCÉNARIO 02"
///   - Un TMP "TitreScenario"   ex: "L'ENFANT ET LE PÈRE DE FAMILLE"
///   - Un TMP "TagType"         ex: "[ DILEMME ÉTHIQUE ]" ou "[ DILEMME MORAL ]"
///   Ce script se place sur le Canvas principal de la scène.
/// </summary>
public class TransitionSceneManager : MonoBehaviour
{
    [Header("Fondu global")]
    [SerializeField] private Image fondNoir;

    [Header("Éléments UI")]
    [SerializeField] private CanvasGroup numeroScenario;  // "SCÉNARIO 02"
    [SerializeField] private CanvasGroup titreScenario;   // "L'enfant et le père de famille"
    [SerializeField] private CanvasGroup tagType;         // "[ DILEMME ÉTHIQUE ]"

    [Header("Son clavier")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip   keySound;

    [Header("Timing")]
    [SerializeField] private float dureeFonduEntrant  = 1.0f;
    [SerializeField] private float delaiAvantTitre    = 0.5f;
    [SerializeField] private float dureeFadeInNumero  = 0.6f;
    [SerializeField] private float dureeFadeInTitre   = 0.8f;
    [SerializeField] private float dureeFadeInTag     = 0.5f;
    [SerializeField] private float tempsAffichage     = 2.5f;  // durée totale visible
    [SerializeField] private float dureeFonduSortant  = 1.0f;

    // =========================================================
    void Start()
    {
        // Cache tous les éléments
        SetAlpha(numeroScenario, 0f);
        SetAlpha(titreScenario,  0f);
        SetAlpha(tagType,        0f);

        // Fond noir opaque au départ (fondu entrant depuis scène précédente)
        SetImageAlpha(fondNoir, 1f);

        // Injecte les données du scénario suivant
        InjecterDonnees();

        StartCoroutine(JouerSequence());
    }

    // =========================================================
    private void InjecterDonnees()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("[TransitionSceneManager] GameManager introuvable.");
            // Numéro du scénario
            var tNumero = numeroScenario.GetComponent<TMP_Text>();
            if (tNumero != null)
                tNumero.text = "SCÉNARIO 01";

            // Titre du scénario
            var tTitre = titreScenario.GetComponent<TMP_Text>();
            if (tTitre != null)
                tTitre.text = "Le maire et le vieux monsieur";

            // Tag moral ou éthique
            var tTag = tagType.GetComponent<TMP_Text>();
            if (tTag != null)
                tTag.text = "[ DILEMME MORAL ]";

            return;
        }

        ScenarioData scenario = GameManager.Instance.CurrentScenario;
        if (scenario == null) return;

        // Numéro du scénario
        var tmpNumero = numeroScenario.GetComponent<TMP_Text>();
        if (tmpNumero != null)
            tmpNumero.text = $"SCÉNARIO {(GameManager.Instance.CurrentScenarioIndex + 1):D2}";

        // Titre du scénario
        var tmpTitre = titreScenario.GetComponent<TMP_Text>();
        if (tmpTitre != null)
            tmpTitre.text = scenario.scenarioTitle.ToUpper();

        // Tag moral ou éthique
        var tmpTag = tagType.GetComponent<TMP_Text>();
        if (tmpTag != null)
            tmpTag.text = scenario.IsMoral ? "[ DILEMME MORAL ]" : "[ DILEMME ÉTHIQUE ]";
    }

    // =========================================================
    private IEnumerator JouerSequence()
    {
        // 1 — Fondu entrant : noir → transparent
        // yield return StartCoroutine(FadeImage(fondNoir, 1f, 0f, dureeFonduEntrant));

        yield return new WaitForSeconds(delaiAvantTitre);

        // 2 — Numéro du scénario
        yield return StartCoroutine(TypewriterEffect(numeroScenario, 0.05f));

        // 3 — Titre
        yield return StartCoroutine(TypewriterEffect(titreScenario, 0.08f));

        // 4 — Tag type (moral / éthique)
        yield return StartCoroutine(TypewriterEffect(tagType, 0.05f));

        // 5 — Laisser la scène visible
        yield return new WaitForSeconds(tempsAffichage);

        // 6 — Fondu sortant : transparent → noir
        // yield return StartCoroutine(FadeImage(fondNoir, 0f, 1f, dureeFonduSortant));

        // 7 — Charger la TriageScene
        if (GameManager.Instance != null)
            GameManager.Instance.LoadTriageScene();
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("TriageScene");
    }

    // =========================================================
    // HELPERS
    // =========================================================

    private IEnumerator TypewriterEffect(CanvasGroup cg, float vitesse)
    {
        var tmp = cg.GetComponent<TMP_Text>();
        if (tmp == null)
            yield break;

        SetAlpha(cg, 1f);
        tmp.ForceMeshUpdate();
        tmp.maxVisibleCharacters = 0;

        string texte = tmp.text;

        for (int i = 0; i < texte.Length; i++)
        {
            tmp.maxVisibleCharacters = i + 1;
            char c = texte[i];

            if (c != ' ' && c != '\n' && audioSource != null && keySound != null)
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(keySound, 0.4f);
            }

            if (c == '.' || c == '!')
                yield return new WaitForSeconds(vitesse * 8f);
            else if (c == ',')
                yield return new WaitForSeconds(vitesse * 4f);
            else if (c == '\n')
                yield return new WaitForSeconds(vitesse * 3f);
            else
                yield return new WaitForSeconds(vitesse);
        }
    }

    private void SetAlpha(CanvasGroup cg, float a) => cg.alpha = a;

    private void SetImageAlpha(Image img, float a)
    {
        Color c = img.color;
        c.a = a;
        img.color = c;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float duree)
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

    private IEnumerator FadeImage(Image img, float de, float a, float duree)
    {
        float elapsed = 0f;
        while (elapsed < duree)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duree);
            SetImageAlpha(img, Mathf.Lerp(de, a, t));
            yield return null;
        }
        SetImageAlpha(img, a);
    }
}
