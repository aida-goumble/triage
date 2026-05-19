using UnityEngine;

public class DossierClickable : MonoBehaviour
{
    public bool estPatientA = true;

    void OnMouseDown()
    {
          // Vérifier si en pause
    if (PauseManager.Instance != null && PauseManager.Instance.EnPause)
        return;
        Debug.Log("Dossier cliqué : " + (estPatientA ? "A" : "B"));
        UIManager.Instance.AfficherDossierPatient(estPatientA);
    }
}