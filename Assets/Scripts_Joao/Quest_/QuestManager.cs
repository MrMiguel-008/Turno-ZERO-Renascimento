using UnityEngine;
using TMPro; // Necessário para o TextMeshPro

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Base de Dados de Missões")]
    public QuestData questDatabase;

    [Header("Referências da UI de Missões")]
    public TextMeshProUGUI questTitleText; // Arraste o texto do Título aqui
    public TextMeshProUGUI questDescText;  // Arraste o texto da Descrição aqui
    public GameObject questUIContainer;    // (Opcional) O painel inteiro das missões para ligar/desligar

    [Header("Estado Atual")]
    public string activeQuestID = "";
    public bool isQuestCompleted = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Se houver missões na base de dados, inicia automaticamente a primeira (índice 0)
        if (questDatabase != null && questDatabase.quests != null && questDatabase.quests.Length > 0)
        {
            // Pega o ID da primeira missão cadastrada no banco de dados e ativa-a
            string primeiraQuestID = questDatabase.quests[0].questID;
            SetActiveQuest(primeiraQuestID);
        }
        else
        {
            UpdateQuestUI();
        }
    }

    // Método para definir uma missão ativa passando o ID
    public void SetActiveQuest(string questID)
    {
        var questInfo = GetQuestInfo(questID);
        if (!string.IsNullOrEmpty(questInfo.questID))
        {
            activeQuestID = questID;
            isQuestCompleted = false;
            Debug.Log($"Nova missão aceite: {questInfo.questTitle} (ID: {questInfo.questID})");

            UpdateQuestUI();
        }
        else
        {
            Debug.LogWarning($"A missão com o ID '{questID}' não foi encontrada na base de dados!");
        }
    }

    public QuestData.QuestInfo GetQuestInfo(string questID)
    {
        if (questDatabase == null || questDatabase.quests == null) return new QuestData.QuestInfo();

        foreach (var q in questDatabase.quests)
        {
            if (q.questID == questID)
            {
                return q;
            }
        }
        return new QuestData.QuestInfo();
    }

    // Atualiza os textos na tela com base na missão ativa
    void UpdateQuestUI()
    {
        if (string.IsNullOrEmpty(activeQuestID))
        {
            // Se não houver missão ativa, limpa os textos ou oculta o painel
            if (questTitleText != null) questTitleText.text = "";
            if (questDescText != null) questDescText.text = "Nenhuma missão ativa.";

            if (questUIContainer != null) questUIContainer.SetActive(false);
            return;
        }

        // Se houver missão ativa, busca os dados e exibe
        var currentQuest = GetQuestInfo(activeQuestID);

        if (questTitleText != null) questTitleText.text = currentQuest.questTitle;
        if (questDescText != null) questDescText.text = currentQuest.questDescription;

        if (questUIContainer != null) questUIContainer.SetActive(true);
    }

    public void CheckAreaQuest(string currentArea)
    {
        if (!string.IsNullOrEmpty(activeQuestID) && !isQuestCompleted)
        {
            var currentQuest = GetQuestInfo(activeQuestID);

            if (currentQuest.questType == QuestType.IrParaArea)
            {
                if (currentQuest.targetAreaName == currentArea)
                {
                    CompleteQuest();
                }
            }
        }
    }

    public void CheckSupervisorDialogueQuest()
    {
        if (!string.IsNullOrEmpty(activeQuestID) && !isQuestCompleted)
        {
            var currentQuest = GetQuestInfo(activeQuestID);

            if (currentQuest.questType == QuestType.FalarComSupervisor)
            {
                CompleteQuest();
            }
        }
    }

    public void CompleteQuest()
    {
        if (!string.IsNullOrEmpty(activeQuestID))
        {
            var currentQuest = GetQuestInfo(activeQuestID);
            isQuestCompleted = true;
            Debug.Log($"Missão Concluída: {currentQuest.questTitle}!");

            // Atualiza o ID da missão no Supervisor para desbloquear o diálogo correspondente
            NPCSupervisorController supervisor = FindObjectOfType<NPCSupervisorController>();
            if (supervisor != null)
            {
                supervisor.currentQuestID = activeQuestID;
            }

            // --- PROCURA E INICIA A PRÓXIMA MISSÃO AUTOMATICAMENTE ---
            int currentIndex = -1;
            for (int i = 0; i < questDatabase.quests.Length; i++)
            {
                if (questDatabase.quests[i].questID == activeQuestID)
                {
                    currentIndex = i;
                    break;
                }
            }

            // Se encontrou a missão e existe uma próxima na lista do Inspector
            int nextIndex = currentIndex + 1;
            if (currentIndex != -1 && nextIndex < questDatabase.quests.Length)
            {
                string nextQuestID = questDatabase.quests[nextIndex].questID;
                SetActiveQuest(nextQuestID); // Inicia a próxima missão instantaneamente
            }
            else
            {
                // Se era a última missão do jogo
                activeQuestID = "";
                UpdateQuestUI();
                Debug.Log("Todas as missões foram concluídas!");
            }
        }
    }
}