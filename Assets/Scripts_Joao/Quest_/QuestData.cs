using UnityEngine;

public enum QuestType
{
    Interacao,
    IrParaArea,
    FalarComSupervisor,
    ColetarItens
}

[CreateAssetMenu(fileName = "QuestDatabase", menuName = "Quest/Quest Database")]
public class QuestData : ScriptableObject
{
    [System.Serializable]
    public struct ItemRequirement
    {
        [Tooltip("Nome exato do item exigido no inventário (Ex: Chave, Engrenagem).")]
        public string itemID;
        [Tooltip("Quantidade necessária deste item específico.")]
        public int amount;
    }

    [System.Serializable]
    public struct QuestInfo
    {
        [Header("Identificação")]
        public string questID;
        public string questTitle;
        [TextArea(2, 4)]
        public string questDescription;

        [Header("Regras da Missão")]
        public QuestType questType;

        [Tooltip("Preencher apenas se o tipo for 'IrParaArea'. Ex: Cozinha")]
        public string targetAreaName;

        [Header("Critérios de Inventário (Múltiplos Itens)")]
        [Tooltip("Adicione aqui todos os itens e quantidades que o player precisa de ter no inventário para concluir esta missão.")]
        public ItemRequirement[] requiredItems;
    }

    [Header("Lista de Todas as Missões do Jogo")]
    public QuestInfo[] quests;
}