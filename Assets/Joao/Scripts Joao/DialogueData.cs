using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueData", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public struct DialogueCondition
    {
        public string requiredLocation; // Ex: "Recepcao", "Cozinha"
        public string requiredQuestID;  // Ex: "Quest_01", "" (vazio se não precisar de missão)
    }

    [System.Serializable]
    public struct DialogueConditionalBlock
    {
        [Header("Condições para este diálogo")]
        public DialogueCondition condition;

        [Header("Falas")]
        [TextArea(2, 5)]
        public string[] dialogueLines;
    }

    [Header("Lista de Diálogos Condicionais")]
    [Tooltip("O NPC vai checar de cima para baixo. O primeiro que atender às condições será o escolhido.")]
    public DialogueConditionalBlock[] conditionalDialogues;

    [Header("Diálogo Padrão (Fallback)")]
    [Tooltip("Caso nenhuma condição acima seja atendida, o NPC dirá isso.")]
    [TextArea(2, 5)]
    public string[] defaultDialogueLines;
}