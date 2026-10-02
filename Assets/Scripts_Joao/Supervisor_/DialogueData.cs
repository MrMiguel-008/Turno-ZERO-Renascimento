using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueData", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public struct DialogueCondition
    {
        public string requiredLocation; // Ex: "Recepcao", "Cozinha"
        public string requiredQuestID;  // Ex: "Quest_01" ou vazio
    }

    [System.Serializable]
    public struct DialogueBlock
    {
        public DialogueCondition condition;
        [TextArea(2, 5)]
        public string[] dialogueLines;
    }

    public DialogueBlock[] conditionalDialogues;

    [Header("Diálogo Padrão (Fallback)")]
    [TextArea(2, 5)]
    public string[] defaultDialogueLines;
}