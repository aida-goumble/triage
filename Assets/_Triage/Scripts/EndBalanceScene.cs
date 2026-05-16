using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class EndBalanceScene : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI acteText;
    public Image           fadeOverlay;

    [Header("Timing")]
    public float fadeInDuration  = 2f;
    public float charDelay       = 0.06f;
    public float pauseEntreActes = 2f;
    public float pauseFinale     = 5f;
    public float fadeOutDuration = 3f;

    [Header("Couleurs")]
    public Color colorNarration  = new Color(0.82f, 0.78f, 0.60f); // beige normal
    public Color colorNotif      = new Color(0.55f, 0.77f, 0.55f); // vert médical
    public Color colorReponse    = new Color(0.45f, 0.50f, 0.42f); // gris vert
    public Color colorFinal      = new Color(0.35f, 0.40f, 0.32f); // gris très sombre

    [Header("Scène suivante")]
    public string nextScene = "MainMenu";

    private readonly string acte1 =
        "Le trajet du retour.\n\n" +
        "Les couloirs vides.\n" +
        "L'ascenseur.\n" +
        "La porte.";

    private readonly string acte2 =
        "Une notification.\n\n" +
        "\"Présence requise demain — 6h00.\n" +
        "Service des urgences.\"";

    private readonly string acte3 =
        "Vous tapez :\n\n" +
        "\"Présent.\"";

    private readonly string acte4 =
        "La vie continue.";

    void Start()
    {
        acteText.text = "";
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        // Acte 1 — Retour
        acteText.color     = colorNarration;
        acteText.fontSize  = 18;
        acteText.fontStyle = FontStyles.Normal;
        acteText.alignment = TextAlignmentOptions.Center;

        yield return StartCoroutine(Fade(1f, 0f, fadeInDuration));
        yield return StartCoroutine(Typewriter(acte1));
        yield return new WaitForSeconds(pauseEntreActes);

        // Fondu
        yield return StartCoroutine(Fade(0f, 1f, 1f));
        acteText.text = "";
        yield return new WaitForSeconds(0.5f);

        // Acte 2 — Notification
        acteText.color     = colorNotif;
        acteText.fontSize  = 16;
        acteText.fontStyle = FontStyles.Normal;
        acteText.alignment = TextAlignmentOptions.Center;

        yield return StartCoroutine(Fade(1f, 0f, fadeInDuration));
        yield return StartCoroutine(Typewriter(acte2));
        yield return new WaitForSeconds(pauseEntreActes);

        // Fondu
        yield return StartCoroutine(Fade(0f, 1f, 1f));
        acteText.text = "";
        yield return new WaitForSeconds(0.5f);

        // Acte 3 — Réponse
        acteText.color     = colorReponse;
        acteText.fontSize  = 18;
        acteText.fontStyle = FontStyles.Italic;
        acteText.alignment = TextAlignmentOptions.Center;

        yield return StartCoroutine(Fade(1f, 0f, fadeInDuration));
        yield return StartCoroutine(Typewriter(acte3));
        yield return new WaitForSeconds(pauseEntreActes);

        // Fondu
        yield return StartCoroutine(Fade(0f, 1f, 1f));
        acteText.text = "";
        yield return new WaitForSeconds(0.8f);

        // Acte 4 — La vie continue
        acteText.color     = colorFinal;
        acteText.fontSize  = 22;
        acteText.fontStyle = FontStyles.Normal;
        acteText.alignment = TextAlignmentOptions.Center;

        yield return StartCoroutine(Fade(1f, 0f, fadeInDuration));
        yield return StartCoroutine(Typewriter(acte4));
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