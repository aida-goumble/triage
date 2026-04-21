using UnityEngine;
using TMPro;

public class VitalSignsAnimator : MonoBehaviour
{
    [Header("Textes header")]
    public TextMeshProUGUI fcText;
    public TextMeshProUGUI spo2Text;

    [Header("FC")]
    public int fcBase      = 72;
    public int fcVariation = 4;
    public float fcInterval = 1.2f;

    [Header("SpO2")]
    public int spo2Base      = 98;
    public int spo2Variation = 1;
    public float spo2Interval = 3.0f;

    [Header("Couleurs")]
    public Color colorNormal  = new Color(0.77f, 0.42f, 0.23f);
    public Color colorOk      = new Color(0.55f, 0.77f, 0.55f);
    public Color colorWarning = new Color(0.90f, 0.20f, 0.20f);

    private float fcTimer;
    private float spo2Timer;

    void Update()
    {
        // Anime la FC
        fcTimer += Time.deltaTime;
        if (fcTimer >= fcInterval)
        {
            fcTimer = 0f;
            int fc = fcBase + Random.Range(-fcVariation, fcVariation + 1);
            if (fcText)
            {
                fcText.text  = "FC  " + fc + " bpm";
                fcText.color = (fc > 100 || fc < 55)
                    ? colorWarning
                    : colorNormal;
            }
        }

        // Anime le SpO2
        spo2Timer += Time.deltaTime;
        if (spo2Timer >= spo2Interval)
        {
            spo2Timer = 0f;
            int spo2 = spo2Base + Random.Range(-spo2Variation, spo2Variation + 1);
            if (spo2Text)
            {
                spo2Text.text  = "SpO2  " + spo2 + "%";
                spo2Text.color = spo2 < 95
                    ? colorWarning
                    : colorOk;
            }
        }
    }
}