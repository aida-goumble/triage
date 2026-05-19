using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panneaux")]
    public GameObject dossierPanel;

    [Header("Overlay")]
    public GameObject overlay;

    [Header("Tampon")]
    public TamponAnimator tamponAnimator;

    [Header("Textes dossiers")]
    public GameObject textDossierA;
    public GameObject textDossierB;

    private bool _dossierEstA;

    void Awake()
    {
        Instance = this;
    }

    public void AfficherDossierPatient(bool estA)
    {
        _dossierEstA = estA;
        overlay.SetActive(true);
        dossierPanel.SetActive(true);

         // Cacher les textes des dossiers
        if (textDossierA) textDossierA.SetActive(false);
        if (textDossierB) textDossierB.SetActive(false);

        PatientData data = estA
            ? GameManager.Instance.CurrentScenario.patientA
            : GameManager.Instance.CurrentScenario.patientB;

        dossierPanel.GetComponent<PatientPanelUI>().DisplayPatient(data, estA);
    }

    public void FermerDossier()
    {
        overlay.SetActive(false);
        dossierPanel.SetActive(false);
         // Réafficher les textes des dossiers
        if (textDossierA) textDossierA.SetActive(true);
        if (textDossierB) textDossierB.SetActive(true);
    }

    public void OnBtnIgnorer()
    {
        tamponAnimator.JouerRefuse(() =>
        {
        overlay.SetActive(false);
        dossierPanel.SetActive(false);
         // Réafficher les textes des dossiers
        if (textDossierA) textDossierA.SetActive(true);
        if (textDossierB) textDossierB.SetActive(true);
        GameManager.Instance.MakeChoice(!_dossierEstA);
         });
    }

    public void OnBtnPrioriser()
    {
          tamponAnimator.JouerApprouve(() =>
         {
        overlay.SetActive(false);
        dossierPanel.SetActive(false);
         // Réafficher les textes des dossiers
        if (textDossierA) textDossierA.SetActive(true);
        if (textDossierB) textDossierB.SetActive(true);
        GameManager.Instance.MakeChoice(_dossierEstA);
        });
    }
}