using UnityEngine;
using System.Collections;

public class TamponAnimator : MonoBehaviour
{
    public GameObject tamponApprouve;
    public GameObject tamponRefuse;

    public void JouerApprouve(System.Action callback)
    {
        StartCoroutine(AnimerTampon(tamponApprouve, callback));
    }

    public void JouerRefuse(System.Action callback)
    {
        StartCoroutine(AnimerTampon(tamponRefuse, callback));
    }

    private IEnumerator AnimerTampon(GameObject tampon, System.Action callback)
    {
        tampon.SetActive(true);
        RectTransform rt = tampon.GetComponent<RectTransform>();
        CanvasGroup cg = tampon.GetComponent<CanvasGroup>();

        // Ajoute CanvasGroup si absent
        if (cg == null) cg = tampon.AddComponent<CanvasGroup>();

        // État initial — grand et transparent
        rt.localScale = Vector3.one * 2f;
        cg.alpha = 0f;

        float duree = 0.2f;
        float t = 0f;

        // Animation entrée — s'écrase sur le dossier
        while (t < duree)
        {
            t += Time.deltaTime;
            float ratio = t / duree;
            rt.localScale = Vector3.one * Mathf.Lerp(2f, 1f, ratio);
            cg.alpha = Mathf.Lerp(0f, 1f, ratio * 2f);
            yield return null;
        }

        rt.localScale = Vector3.one;
        cg.alpha = 1f;

        // Attendre
        yield return new WaitForSeconds(1f);

        // Disparaît
        t = 0f;
        while (t < 0.3f)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, t / 0.3f);
            yield return null;
        }

        tampon.SetActive(false);
        callback?.Invoke();
    }
}