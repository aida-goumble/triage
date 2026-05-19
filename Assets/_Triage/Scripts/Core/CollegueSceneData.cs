using UnityEngine;

[CreateAssetMenu(
    fileName = "CollegueScene_XX",
    menuName  = "Triage/Transitions/Collegue Scene Data")]
public class CollegueSceneData : ScriptableObject
{
    [System.Serializable]
    public class Dialogue
    {
        public string nomPersonnage;   // ex: "SOPHIE M. — INFIRMIÈRE"
        public string texte;           // ex: "T'as vu ce qu'il a fait ?"
        [Range(0, 2)]
        public int personnageIndex;    // 0=gauche, 1=centre, 2=droite
    }

    [Header("Scénario lié")]
    [TextArea(3, 6)]
    public string nomScenario;         // ex: "Le maire et le vieux monsieur"

    [Header("Dialogues (2 à 3 max)")]
    public Dialogue[] dialogues;
}