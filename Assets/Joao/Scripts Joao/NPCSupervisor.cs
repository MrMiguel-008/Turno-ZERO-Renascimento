using UnityEngine;

public class NPCSupervisor : MonoBehaviour
{
    [Header("Configuração de Movimento (Seguir Player)")]
    public Transform player;          // Arraste o Transform do Player aqui
    public float moveSpeed = 3f;      // Velocidade de movimento
    public float stoppingDistance = 2f; // Distância que ele para perto do player
    private Rigidbody2D rb;

    [Header("Sistema de Diálogo e Localização")]
    public DialogueData dialogueData;
    [Tooltip("Área atual onde o supervisor está (atualizada pelos triggers ou proximidade).")]
    public string currentLocation = "Desconhecido";
    public string currentQuestID = ""; // Pode ser alterado pelo seu sistema de missões

    private bool playerIsClose = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Se o player não foi atribuído no Inspector, tenta achar pela Tag
        if (player == null)
        {
            GameObject pObj = GameObject.FindGameObjectWithTag("Player");
            if (pObj != null) player = pObj.transform;
        }
    }

    void Update()
    {
        FollowPlayer();

        // Tecla 'E' para interagir quando estiver perto do NPC
        if (playerIsClose && Input.GetKeyDown(KeyCode.E))
        {
            TriggerDialogue();
        }
    }

    void FollowPlayer()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Se estiver longe do que o limite de parada, o NPC se move em direção ao player
        if (distanceToPlayer > stoppingDistance)
        {
            Vector2 targetPosition = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            rb.MovePosition(targetPosition);

            // Opcional: Virar o sprite para o lado que está andando
            if (player.position.x > transform.position.x)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void TriggerDialogue()
    {
        if (dialogueData == null)
        {
            Debug.LogWarning("Nenhum DialogueData atribuído ao Supervisor!");
            return;
        }

        string[] linesToSay = GetMatchingDialogue();

        Debug.Log($"--- Diálogo com Supervisor (Local: {currentLocation}) ---");
        foreach (string line in linesToSay)
        {
            Debug.Log(line);
        }
    }

    string[] GetMatchingDialogue()
    {
        // Varre as regras condicionais do seu ScriptableObject
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsClose = true;
            Debug.Log("Pressione 'E' para falar com o Supervisor.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsClose = false;
        }
    }
}