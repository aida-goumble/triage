using UnityEngine;

public class DossierClickable : MonoBehaviour
{
    public bool estPatientA = true;

    void OnMouseDown()
    {
        Debug.Log("Dossier cliqué : " + (estPatientA ? "A" : "B"));
        UIManager.Instance.AfficherDossierPatient(estPatientA);
    }
}