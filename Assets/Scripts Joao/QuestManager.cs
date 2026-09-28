using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum QuestType
{
    EnterArea,
    TalkToNPC,
    InteractWithItem,
    ChangeScene
}

// Classe que define os dados da missão direto no Inspector
[System.Serializable]
public class QuestStep
{
    public string questTitle;
    [TextArea] public string questDescription;
    public QuestType questType;
    [Tooltip("Nome exato da Área, do NPC ou da Cena para este objetivo")]
    public string targetIdentifier;
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Linha de Progressão (Configurada no Inspector)")]
    [Tooltip("Adicione e ordene as missões diretamente aqui.")]
    public List<QuestStep> allQuests = new List<QuestStep>();

    [Header("Estado Atual (Apenas Leitura)")]
    [SerializeField] private int currentQuestIndex = 0;

    public QuestStep CurrentQuest
    {
        get
        {
            if (allQuests != null && currentQuestIndex >= 0 && currentQuestIndex < allQuests.Count)
                return allQuests[currentQuestIndex];
            return null;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PrintCurrentQuestInfo();
    }

    // Método chamado pelos gatilhos do mundo
    public void CheckObjective(QuestType typeReported, string targetReported)
    {
        QuestStep activeQuest = CurrentQuest;

        if (activeQuest == null)
        {
            Debug.Log("Parabéns! Todas as missões do jogo já foram concluídas.");
            return;
        }

        // Valida se o tipo e o identificador batem com a missão atual da lista
        if (activeQuest.questType == typeReported && activeQuest.targetIdentifier == targetReported)
        {
            Debug.Log($"<color=green>Missão Concluída:</color> {activeQuest.questTitle}");
            AdvanceToNextQuest();
        }
        else
        {
            Debug.Log($"Ação ignorada. O objetivo atual requer: [{activeQuest.questType}] com alvo [{activeQuest.targetIdentifier}], mas recebeu [{typeReported}] com alvo [{targetReported}].");
        }
    }

    private void AdvanceToNextQuest()
    {
        currentQuestIndex++;

        if (currentQuestIndex < allQuests.Count)
        {
            PrintCurrentQuestInfo();

            // Se a nova missão for mudança de cena automática ao iniciar
            if (CurrentQuest.questType == QuestType.ChangeScene)
            {
                LoadSceneForCurrentQuest();
            }
        }
        else
        {
            Debug.Log("<color=cyan>Fim da Linha de Missões!</color> Jogo concluído.");
        }
    }

    private void PrintCurrentQuestInfo()
    {
        if (CurrentQuest != null)
        {
            Debug.Log($"---> Missão Atual [{currentQuestIndex}]: {CurrentQuest.questTitle} | Objetivo: {CurrentQuest.questType} -> {CurrentQuest.targetIdentifier}");
        }
    }

    public void LoadSceneForCurrentQuest()
    {
        if (CurrentQuest != null && !string.IsNullOrEmpty(CurrentQuest.targetIdentifier))
        {
            PlayerPrefs.SetString("PreviousScene", SceneManager.GetActiveScene().name);
            SceneManager.LoadScene(CurrentQuest.targetIdentifier);
        }
    }
}