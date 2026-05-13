using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PenseeSceneManager : MonoBehaviour
{
    [Header("Éléments dans l'ordre d'apparition")]
    [SerializeField] private CanvasGroup guillemetsOuvrant;  // guillemets_0
    [SerializeField] private CanvasGroup guillemetsOuvrant1; // guillemets_1
    [SerializeField] private CanvasGroup textePensee;        // TextePensee
    [SerializeField] private CanvasGroup nomScenario;        // Nom_scenario

    [Header("Timing")]
    [SerializeField] private float dureeFadeIn       = 1f;
    [SerializeField] private float delaiApresGuillemets = 0.8f; // pause avant le texte
    [SerializeField] private float delaiApresTexte   = 1.2f;    // pause avant nom scénario
    [SerializeField] private float delaiAvantContinuer = 5f;    // attente après tout

    [Header("UI Continue")]
    [SerializeField] private CanvasGroup texteContinuer;

    private bool _peutContinuer = false;

    void Start()
    {
        // Tout cacher au départ
        guillemetsOuvrant.alpha  = 0f;
        guillemetsOuvrant1.alpha = 0f;
        textePensee.alpha        = 0f;
        nomScenario.alpha        = 0f;
        texteContinuer.alpha     = 0f;

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
        yield return StartCoroutine(FadeIn(textePensee, dureeFadeIn * 1.5f));

        yield return new WaitForSeconds(delaiApresTexte);

        // Étape 3 — Nom du scénario
        yield return StartCoroutine(FadeIn(nomScenario, dureeFadeIn));

        yield return new WaitForSeconds(delaiAvantContinuer);

        // Étape 4 — "Cliquer pour continuer"
        yield return StartCoroutine(FadeIn(texteContinuer, 0.6f));

        _peutContinuer = true;
    }

    private IEnumerator PasserSceneSuivante()
    {
        _peutContinuer = false;

        yield return StartCoroutine(FadeOut(texteContinuer, 0.3f));

        if (GameManager.Instance != null)
            GameManager.Instance.OnCollegueSceneComplete();
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("TriageScene");
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
}