using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject optionsPanel;

    [Header("Volume")]
    public Slider            sliderVolume;
    public TextMeshProUGUI   volumeValText;

    [Header("Boutons")]
    public Button btnOptions;
    public Button btnRetour;
    public Button btnReset;

    private const string SAVE_KEY        = "HasSave";
    private const string SAVE_VOLUME     = "Volume";
    private const string SAVE_PATIENTS   = "StatPatients";
    private const string SAVE_DECISIONS  = "StatDecisions";
    private const string SAVE_REPUTATION = "StatReputation";

    void Start()
    {
        // Charge le volume sauvegardé (0.8 par défaut)
        float savedVolume = PlayerPrefs.GetFloat(SAVE_VOLUME, 0.8f);
        sliderVolume.value = savedVolume;
        AudioListener.volume = savedVolume;
        UpdateVolumeText(savedVolume);

        // Branche les événements
        sliderVolume.onValueChanged.AddListener(OnVolumeChanged);
        btnOptions.onClick.AddListener(OuvrirOptions);
        btnRetour.onClick.AddListener(FermerOptions);
        btnReset.onClick.AddListener(OnReset);

        // Cache le panel au départ
        optionsPanel.SetActive(false);
    }

    void OnVolumeChanged(float val)
    {
        AudioListener.volume = val;
        UpdateVolumeText(val);
        PlayerPrefs.SetFloat(SAVE_VOLUME, val);
        PlayerPrefs.Save();
    }

    void UpdateVolumeText(float val)
    {
        if (volumeValText)
            volumeValText.text = Mathf.RoundToInt(val * 100) + "%";
    }

    void OuvrirOptions()
    {
        optionsPanel.SetActive(true);
    }

    void FermerOptions()
    {
        optionsPanel.SetActive(false);
    }

    void OnReset()
    {
        // Supprime toutes les sauvegardes
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.DeleteKey(SAVE_PATIENTS);
        PlayerPrefs.DeleteKey(SAVE_DECISIONS);
        PlayerPrefs.DeleteKey(SAVE_REPUTATION);
        PlayerPrefs.Save();

        // Recharge la scène pour tout réinitialiser
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}