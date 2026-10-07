using UnityEngine;

public enum QuestType
{
    Interacao,          // Ex: Interagir com um objeto
    IrParaArea,         // Ex: Ir até à Cozinha ou Recepção
    FalarComSupervisor  // Ex: Falar com o NPC Supervisor
}

[CreateAssetMenu(fileName = "QuestDatabase", menuName = "Quest/Quest Database")]
public class QuestData : ScriptableObject
{
    [System.Serializable]
    public struct QuestInfo
    {
        [Header("Identificação")]
        public string questID;          // Ex: "Quest_01" (deve coincidir com o DialogueData)
        public string questTitle;       // Ex: "Ir para a Cozinha"
        [TextArea(2, 4)]
        public string questDescription; // Ex: "O supervisor mandou-te ir verificar a cozinha."

        [Header("Regras da Missão")]
        public QuestType questType;
        [Tooltip("Preencher apenas se o tipo for 'IrParaArea'. Ex: Cozinha")]
        public string targetAreaName;
    }

    [Header("Lista de Todas as Missões do Jogo")]
    [Tooltip("Adicione quantas missões quiser aqui.")]
    public QuestInfo[] quests;
}