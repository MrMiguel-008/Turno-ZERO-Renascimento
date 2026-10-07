using UnityEngine;

public class NPCSupervisorController : MonoBehaviour
{
    [Header("Configuração de Movimento")]
    public Transform player;
    public float moveSpeed = 3f;
    public float stoppingDistance = 2.0f;
    private Rigidbody2D rb;

    [Header("Sistema de Diálogo e Localização")]
    public DialogueData dialogueData;
    public float interactionDistance = 2.5f;

    [HideInInspector]
    public string currentLocation = "Desconhecido";
    public string currentQuestID = "";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (!gameObject.CompareTag("Supervisor"))
        {
            gameObject.tag = "Supervisor";
        }

        if (player == null)
        {
            GameObject pObj = GameObject.FindGameObjectWithTag("Player");
            if (pObj != null) player = pObj.transform;
        }
    }

    void Update()
    {
        FollowPlayer();
        CheckPlayerInteraction();
    }

    void FollowPlayer()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > stoppingDistance)
        {
            Vector2 targetPosition = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            rb.MovePosition(targetPosition);

            if (player.position.x > transform.position.x)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void CheckPlayerInteraction()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Condições: Perto o suficiente, tecla E premida, diálogo não ativo E gestor permite nova interação
        if (distanceToPlayer <= interactionDistance && Input.GetKeyDown(KeyCode.E))
        {
            if (DialogueManager.Instance != null &&
                !DialogueManager.Instance.IsDialogueActive() &&
                DialogueManager.Instance.canInteractAgain)
            {
                TriggerDialogue();
            }
        }
    }

    void TriggerDialogue()
    {
        if (dialogueData == null)
        {
            Debug.LogWarning("Nenhum DialogueData atribuído ao Supervisor!");
            return;
        }

        // **NOVIDADE:** Verifica se há uma missão ativa do tipo "Falar com o Supervisor"
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.CheckSupervisorDialogueQuest();
        }

        string[] linesToSay = GetMatchingDialogue();
        DialogueManager.Instance.StartDialogue(linesToSay);
    }

    string[] GetMatchingDialogue()
    {
        foreach (var block in dialogueData.conditionalDialogues)
        {
            bool locationMatches = string.IsNullOrEmpty(block.condition.requiredLocation) ||
                                   block.condition.requiredLocation == currentLocation;

            bool questMatches = string.IsNullOrEmpty(block.condition.requiredQuestID) ||
                                block.condition.requiredQuestID == currentQuestID;

            if (locationMatches && questMatches)
            {
                return block.dialogueLines;
            }
        }

        return dialogueData.defaultDialogueLines;
    }
}