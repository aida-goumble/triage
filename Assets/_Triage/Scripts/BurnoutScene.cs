using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class BurnoutScene : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI letterText;
    public Image           fadeOverlay;

    [Header("Timing")]
    public float fadeInDuration    = 2f;
    public float charDelay         = 0.045f;
    public float pauseAfterLetter  = 3f;
    public float fadeOutDuration   = 3f;

    [Header("Scène suivante")]
    public string nextScene = "MainMenu";

    private readonly string letter =
        "À l'attention de la Direction,\n\n" +
        "Je vous informe de ma décision de quitter\n" +
        "mes fonctions à compter de ce jour.\n\n" +
        "Ces derniers mois ont eu raison de moi.\n" +
        "Chaque décision pesait des vies.\n" +
        "Chaque nuit, les visages revenaient.\n\n" +
        "Je ne suis plus en mesure d'exercer.\n\n" +
        "Je ne suis plus en mesure.\n\n" +
        "Je ne suis plus.";
    void Start()
    {
        letterText.text = "";
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        // 1. Fondu depuis le noir
        yield return StartCoroutine(Fade(1f, 0f, fadeInDuration));

        // 2. Effet typewriter
        yield return StartCoroutine(Typewriter());

        // 3. Pause — laisser le joueur lire
        yield return new WaitForSeconds(pauseAfterLetter);

        // 4. Fondu au noir
        yield return StartCoroutine(Fade(0f, 1f, fadeOutDuration));

        // 5. Scène suivante
        SceneManager.LoadScene(nextScene);
    }

    IEnumerator Typewriter()
    {
        letterText.text = "";
        foreach (char c in letter)
        {
            letterText.text += c;

            // Pause plus longue sur la ponctuation
            if (c == '.' || c == ',') 
                yield return new WaitForSeconds(charDelay * 6f);
            else if (c == '\n')       
                yield return new WaitForSeconds(charDelay * 3f);
            else                      
                yield return new WaitForSeconds(charDelay);
        }
    }

    IEnumerator Fade(float fromAlpha, float toAlpha, float duration)
    {
        float t = 0f;
        Color c = fadeOverlay.color;
        while (t < duration)
        {
            t   += Time.deltaTime;
            c.a  = Mathf.Lerp(fromAlpha, toAlpha, t / duration);
            fadeOverlay.color = c;
            yield return null;
        }
        c.a = toAlpha;
        fadeOverlay.color = c;
    }
}