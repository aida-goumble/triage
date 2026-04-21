using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panneaux")]
    public GameObject dossierPanel;

    [Header("Overlay")]
    public GameObject overlay;

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

        PatientData data = estA
            ? GameManager.Instance.CurrentScenario.patientA
            : GameManager.Instance.CurrentScenario.patientB;

        dossierPanel.GetComponent<PatientPanelUI>().DisplayPatient(data);
    }

    public void FermerDossier()
    {
        overlay.SetActive(false);
        dossierPanel.SetActive(false);
    }

    public void OnBtnIgnorer()
    {
        overlay.SetActive(false);
        dossierPanel.SetActive(false);
        GameManager.Instance.MakeChoice(!_dossierEstA);
    }

    public void OnBtnPrioriser()
    {
        overlay.SetActive(false);
        dossierPanel.SetActive(false);
        GameManager.Instance.MakeChoice(_dossierEstA);
    }
}