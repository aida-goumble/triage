using UnityEngine;

[CreateAssetMenu(
    fileName = "PenseeScene_XX",
    menuName  = "Triage/Transitions/Pensee Scene Data")]
public class PenseeSceneData : ScriptableObject
{
    [Header("Scénario lié")]
    [TextArea(3, 6)]
    public string nomScenario;   // ex: "— SCÉNARIO 01 · LE MAIRE ET LE VIEUX MONSIEUR"

    [Header("Monologue intérieur")]
    [TextArea(3, 6)]
    public string textePensee;   // le texte qui s'écrit ligne par ligne
}