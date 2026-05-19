using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class IntroScene : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI introText;
    public Image           fadeOverlay;

    [Header("Timing")]
    public float fadeInDuration  = 2f;
    public float charDelay       = 0.05f;
    public float pauseMillieu    = 2f;
    public float pauseFinale     = 3f;
    public float fadeOutDuration = 2f;
    
    [Header("Son clavier")]
    public AudioSource audioSource;
    public AudioClip   keySound;
    
    [Header("Scène suivante")]
    public string nextScene = "TriageScene";

    private readonly string partie1 =
        "HÔPITAL SONORA — SERVICE DES URGENCES\n" +
        "Nuit du 14 mars.\n\n" +
        "Vous êtes chef de service.\n" +
        "Chaque soir, des décisions impossibles.\n" +
        "Chaque soir, des vies dans la balance.";

    private readonly string partie2 =
        "Il n'y a pas de bonne réponse.\n" +
        "Il n'y a que votre choix.\n\n" +
        "Et ses conséquences.";

    private readonly string partieFinale =
        "Bonne chance, Docteur.";

    void Start()
    {
        introText.text = "";
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        // Fade depuis le noir
        yield return StartCoroutine(Fade(1f, 0f, fadeInDuration));

        // Partie 1
        introText.color    = new Color(0.82f, 0.78f, 0.60f);
        introText.fontSize = 30;
        introText.fontStyle = FontStyles.Normal;
        yield return StartCoroutine(Typewriter(partie1));
        yield return new WaitForSeconds(pauseMillieu);

        // Fondu
        yield return StartCoroutine(Fade(0f, 1f, 1f));
        introText.text = "";
        yield return new WaitForSeconds(0.5f);

        // Partie 2
        introText.color    = new Color(0.65f, 0.65f, 0.58f);
        introText.fontSize = 30;
        introText.fontStyle = FontStyles.Italic;
        yield return StartCoroutine(Fade(1f, 0f, fadeInDuration));
        yield return StartCoroutine(Typewriter(partie2));
        yield return new WaitForSeconds(pauseMillieu);

        // Fondu
        yield return StartCoroutine(Fade(0f, 1f, 1f));
        introText.text = "";
        yield return new WaitForSeconds(0.8f);

        // Partie finale
        introText.color     = new Color(0.55f, 0.77f, 0.55f);
        introText.fontSize  = 30;
        introText.fontStyle = FontStyles.Normal;
        introText.alignment = TextAlignmentOptions.Center;
        yield return StartCoroutine(Fade(1f, 0f, fadeInDuration));
        yield return StartCoroutine(Typewriter(partieFinale));
        yield return new WaitForSeconds(pauseFinale);

        // Fondu final vers TriageScene
        yield return StartCoroutine(Fade(0f, 1f, fadeOutDuration));
        SceneManager.LoadScene(nextScene);
    }

    IEnumerator Typewriter(string texte)
    {
        introText.text = "";
        foreach (char c in texte)
        {
            introText.text += c;

            // Joue le son uniquement sur les vrais caractères
            if (c != ' ' && c != '\n')
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f); // légère variation
                audioSource.PlayOneShot(keySound, 0.4f);
            }

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