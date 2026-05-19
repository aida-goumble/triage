using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class EndSystemScene : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI acteText;
    public Image           fadeOverlay;

    [Header("Timing")]
    public float fadeInDuration  = 2f;
    public float charDelay       = 0.04f;
    public float pauseEntreActes = 2f;
    public float pauseFinale     = 5f;
    public float fadeOutDuration = 3f;

    [Header("Couleurs")]
    public Color colorEloge    = new Color(0.90f, 0.85f, 0.65f); // beige clair festif
    public Color colorPensee   = new Color(0.40f, 0.45f, 0.38f); // gris vert sombre

    [Header("Scène suivante")]
    public string nextScene = "MainMenu";

    private readonly string acte1 =
        "SALLE DE DIRECTION — CÉRÉMONIE ANNUELLE\n\n" +
        "\"Félicitations, Docteur.\n" +
        "Vos résultats sont exemplaires.\n" +
        "Votre service est un modèle\n" +
        "pour l'établissement.\"\n\n" +
        "Bravo.\n" +
        "Bravo.\n" +
        "Bravo.";

    private readonly string acte2 =
        "Je ne me souviens plus\n" +
        "pourquoi j'ai choisi ce métier.";

    void Start()
    {
        acteText.text = "";
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        // Acte 1 — Éloge
        acteText.color    = colorEloge;
        acteText.fontSize = 18;
        acteText.fontStyle = FontStyles.Normal;
        acteText.alignment = TextAlignmentOptions.Center;

        yield return StartCoroutine(Fade(1f, 0f, fadeInDuration));
        yield return StartCoroutine(Typewriter(acte1, 0.04f));
        yield return new WaitForSeconds(pauseEntreActes);

        // Fondu entre actes
        yield return StartCoroutine(Fade(0f, 1f, 1.2f));
        acteText.text = "";
        yield return new WaitForSeconds(0.8f);

        // Acte 2 — Pensée intérieure
        acteText.color     = colorPensee;
        acteText.fontSize  = 22;
        acteText.fontStyle = FontStyles.Italic;
        acteText.alignment = TextAlignmentOptions.Center;

        yield return StartCoroutine(Fade(1f, 0f, fadeInDuration));
        yield return StartCoroutine(Typewriter(acte2, 0.08f));
        yield return new WaitForSeconds(pauseFinale);

        // Fondu final
        yield return StartCoroutine(Fade(0f, 1f, fadeOutDuration));
        SceneManager.LoadScene(nextScene);
    }

    IEnumerator Typewriter(string texte, float delay)
    {
        acteText.text = "";
        foreach (char c in texte)
        {
            acteText.text += c;
            if (c == '.' || c == '!')
                yield return new WaitForSeconds(delay * 8f);
            else if (c == ',')
                yield return new WaitForSeconds(delay * 4f);
            else if (c == '\n')
                yield return new WaitForSeconds(delay * 3f);
            else
                yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        Color c = fadeOverlay.color;
        while (t < duration)
        {
            t   += Time.deltaTime;
            c.a  = Mathf.Lerp(from, to, t / duration);
            fadeOverlay.color = c;
            yield return null;
        }
        c.a = to;
        fadeOverlay.color = c;
    }
}