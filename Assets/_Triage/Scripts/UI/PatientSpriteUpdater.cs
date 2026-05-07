using UnityEngine;

public class PatientSpriteUpdater : MonoBehaviour
{
    [Header("Sprites dans la scène")]
    public SpriteRenderer spritePatientA;
    public SpriteRenderer spritePatientB;

    private void OnEnable()
    {
        EventBus.OnScenarioStarted += MettreAJourSprites;
    }

    private void OnDisable()
    {
        EventBus.OnScenarioStarted -= MettreAJourSprites;
    }

    private void MettreAJourSprites(ScenarioData scenario)
{
    if (spritePatientA && scenario.patientA.characterSprite != null)
        spritePatientA.sprite = scenario.patientA.characterSprite;

    if (spritePatientB && scenario.patientB.characterSprite != null)
        spritePatientB.sprite = scenario.patientB.characterSprite;
}
}