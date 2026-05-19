using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class EndHeroScene : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI acteText;
    public Image           fadeOverlay;

    [Header("Timing")]
    public float fadeInDuration   = 2f;
    public float charDelay        = 0.05f;
    public float pauseEntreActes  = 2f;
    public float pauseFinale      = 4f;
    public float fadeOutDuration  = 3f;

    [Header("Scène suivante")]
    public string nextScene = "MainMenu";

    private readonly string acte1 =
        "BUREAU DE LA DIRECTION\n\n" +
        "\"Votre bilan est catastrophique.\n" +
        "Votre réputation nous a coûté cher.\n" +
        "Nous ne pouvons pas vous garder.\"";

    private readonly string acte2 =
        "COULOIR — 18h32\n\n" +
        "Une silhouette s'arrête devant vous.\n\n" +
        "Marc T.\n" +
        "Hypothermie sévère.\n" +
        "Scénario 3.\n\n" +
        "Il vous tend la main.\n" +
        "Pas de mots.";

    private readonly string acte3 =
        "\"Vous avez fait ce qui était juste.\"";

    void Start()
    {
        acteText.text = "";
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        // Acte 1 — Bureau
        yield return StartCoroutine(Fade(1f, 0f, fadeInDuration));
        yield return StartCoroutine(Typewriter(acte1));
        yield return new WaitForSeconds(pauseEntreActes);

        // Fondu entre les actes
        yield return StartCoroutine(Fade(0f, 1f, 1f));
        acteText.text = "";
        yield return new WaitForSeconds(0.5f);

        // Acte 2 — Couloir
        yield return StartCoroutine(Fade(1f, 0f, fadeInDuration));
        yield return StartCoroutine(Typewriter(acte2));
        yield return new WaitForSeconds(pauseEntreActes);

        // Fondu avant texte final
        yield return StartCoroutine(Fade(0f, 1f, 1f));
        acteText.text = "";
        yield return new WaitForSeconds(0.5f);

        // Acte 3 — Texte final
        acteText.fontSize = 24;
        acteText.alignment = TextAlignmentOptions.Center;
        yield return StartCoroutine(Fade(1f, 0f, fadeInDuration));
        yield return StartCoroutine(Typewriter(acte3));
        yield return new WaitForSeconds(pauseFinale);

        // Fondu final
        yield return StartCoroutine(Fade(0f, 1f, fadeOutDuration));
        SceneManager.LoadScene(nextScene);
    }

    IEnumerator Typewriter(string texte)
    {
        acteText.text = "";
        foreach (char c in texte)
        {
            acteText.text += c;
            if (c == '.' || c == '!')
                yield return new WaitForSeconds(charDelay * 8f);
            else if (c == ',')
                yield return new WaitForSeconds(charDelay * 4f);
            else if (c == '\n')
                yield return new WaitForSeconds(charDelay * 3f);
            else
                yield return new WaitForSeconds(charDelay);
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