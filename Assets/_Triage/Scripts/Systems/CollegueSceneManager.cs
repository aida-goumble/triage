using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CollegueSceneManager : MonoBehaviour
{
    [Header("Bulles de dialogue (dans l'ordre d'apparition)")]
    [SerializeField] private CanvasGroup[] bulles;

    [Header("UI Continue")]
    [SerializeField] private CanvasGroup texteContinuer; // ton objet texte "Cliquer pour continuer"

    [Header("Timing")]
    [SerializeField] private float delaiEntreApparitions = 2.5f;
    [SerializeField] private float dureeFadeIn = 0.4f;
    [SerializeField] private float delaiAvantContinuer = 2f; // attente après dernière bulle

    private bool _peutContinuer = false;

    void Start()
    {
        foreach (var b in bulles)
        {
            b.alpha = 0f;
            b.gameObject.SetActive(true);
        }

        // Cacher le texte continuer au départ
        texteContinuer.alpha = 0f;
        texteContinuer.gameObject.SetActive(true);

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
}