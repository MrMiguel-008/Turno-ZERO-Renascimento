using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Base de Dados de Missões")]
    public QuestData questDatabase;

    [Header("Referências da UI de Missões")]
    public TextMeshProUGUI questTitleText;
    public TextMeshProUGUI questDescText;
    public GameObject questUIContainer;

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
        if (questDatabase != null && questDatabase.quests != null && questDatabase.quests.Length > 0)
        {
            string primeiraQuestID = questDatabase.quests[0].questID;
            SetActiveQuest(primeiraQuestID);
        }
        else
        {
            UpdateQuestUI();
        }
    }

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
            if (q.questID == questID) return q;
        }
        return new QuestData.QuestInfo();
    }

    void UpdateQuestUI()
    {
        if (string.IsNullOrEmpty(activeQuestID))
        {
            if (questTitleText != null) questTitleText.text = "";
            if (questDescText != null) questDescText.text = "Missões concluídas.";
            if (questUIContainer != null) questUIContainer.SetActive(false);
            return;
        }

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

            if (currentQuest.questType == QuestType.IrParaArea && currentQuest.targetAreaName == currentArea)
            {
                ValidarEConcluir(currentQuest);
            }
        }
    }

    // Chamado pelo ItemColetavel quando um item é apanhado
    public void CheckItemCollectionQuest(string itemColetadoID)
    {
        if (!string.IsNullOrEmpty(activeQuestID) && !isQuestCompleted)
        {
            var currentQuest = GetQuestInfo(activeQuestID);

            if (currentQuest.questType == QuestType.ColetarItens)
            {
                if (currentQuest.requiredItems != null)
                {
                    foreach (var req in currentQuest.requiredItems)
                    {
                        if (req.itemID == itemColetadoID)
                        {
                            ValidarEConcluir(currentQuest);
                            break;
                        }
                    }
                }
            }
        }
    }

    // Chamado pelo DialogueManager quando o diálogo termina
    public void OnDialogueEnded()
    {
        if (!string.IsNullOrEmpty(activeQuestID) && !isQuestCompleted)
        {
            var currentQuest = GetQuestInfo(activeQuestID);

            if (currentQuest.questType == QuestType.FalarComSupervisor)
            {
                ValidarEConcluir(currentQuest);
            }
        }
    }

    void ValidarEConcluir(QuestData.QuestInfo quest)
    {
        if (quest.requiredItems != null && quest.requiredItems.Length > 0)
        {
            InventarioHUD inventario = InventarioHUD.Instance;
            if (inventario != null)
            {
                foreach (var req in quest.requiredItems)
                {
                    if (!inventario.PossuiItem(req.itemID))
                    {
                        Debug.Log($"Falta o item necessário: '{req.itemID}'.");
                        return;
                    }
                }
            }
            else
            {
                return;
            }
        }

        CompleteQuest();
    }

    public void CompleteQuest()
    {
        if (!string.IsNullOrEmpty(activeQuestID))
        {
            var currentQuest = GetQuestInfo(activeQuestID);
            isQuestCompleted = true;
            Debug.Log($"Missão Concluída: {currentQuest.questTitle}!");

            // Dispara a verificação e lógica de fim de jogo se esta for a última missão
            ConcluirMissao(activeQuestID);

            NPCSupervisorController supervisor = FindObjectOfType<NPCSupervisorController>();
            if (supervisor != null)
            {
                supervisor.currentQuestID = activeQuestID;
            }

            int currentIndex = -1;
            for (int i = 0; i < questDatabase.quests.Length; i++)
            {
                if (questDatabase.quests[i].questID == activeQuestID)
                {
                    currentIndex = i;
                    break;
                }
            }

            int nextIndex = currentIndex + 1;
            if (currentIndex != -1 && nextIndex < questDatabase.quests.Length)
            {
                string nextQuestID = questDatabase.quests[nextIndex].questID;
                SetActiveQuest(nextQuestID);
            }
            else
            {
                activeQuestID = "";
                UpdateQuestUI();
                Debug.Log("Todas as missões foram concluídas!");
            }
        }
    }

    // Chamado quando um minigame termina com sucesso
    public void CheckMinigameQuest(string idMinigameConcluido)
    {
        if (!string.IsNullOrEmpty(activeQuestID) && !isQuestCompleted)
        {
            var currentQuest = GetQuestInfo(activeQuestID);

            if (currentQuest.questType == QuestType.Minigame)
            {
                if (currentQuest.targetMinigameID == idMinigameConcluido)
                {
                    ValidarEConcluir(currentQuest);
                }
            }
        }
    }

    public void ConcluirMissao(string idDaMissao)
    {
        Debug.Log($"Missão concluída: {idDaMissao}");

        // Substitui "NomeDaUltimaMissao" pelo ID exato da última missão do teu QuestData
        if (idDaMissao == "NomeDaUltimaMissao")
        {
            Debug.Log("Última missão concluída! Disparando tela de fim de jogo...");

            FimDeJogoManager fimDeJogo = FindObjectOfType<FimDeJogoManager>();
            if (fimDeJogo != null)
            {
                fimDeJogo.DispararFimDeJogo();
            }
            else
            {
                Debug.LogWarning("[QuestManager] FimDeJogoManager não foi encontrado na cena!");
            }
        }
    }
}