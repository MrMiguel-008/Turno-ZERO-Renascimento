using UnityEngine;

public enum QuestType
{
    Interacao,
    IrParaArea,
    FalarComSupervisor,
    ColetarItens,
    Minigame            // <-- Adicionado
}

[CreateAssetMenu(fileName = "QuestDatabase", menuName = "Quest/Quest Database")]
public class QuestData : ScriptableObject
{
    [System.Serializable]
    public struct ItemRequirement
    {
        public string itemID;
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

        [Header("Critérios de Inventário (Opcional)")]
        public ItemRequirement[] requiredItems;

        [Header("Configuração de Minigame (Apenas para tipo Minigame)")]
        [Tooltip("Deve corresponder exatamente ao 'idObjetivo' configurado no script InteracaoMinigame do objeto no mapa.")]
        public string targetMinigameID;
    }

    [Header("Lista de Todas as Missões do Jogo")]
    public QuestInfo[] quests;
}