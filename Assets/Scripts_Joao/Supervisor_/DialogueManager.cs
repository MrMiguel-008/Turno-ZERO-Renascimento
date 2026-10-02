using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Referências da UI")]
    public GameObject dialogueBoxPanel;
    public TextMeshProUGUI dialogueText;

    private string[] currentLines;
    private int currentLineIndex;
    private bool isDialogueActive = false;

    // Variável pública para o NPC saber se pode interagir novamente
    [HideInInspector]
    public bool canInteractAgain = true;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (dialogueBoxPanel != null)
            dialogueBoxPanel.SetActive(false);
    }

    void Update()
    {
        // Avança ou fecha o diálogo ao apertar Espaço ou E
        if (isDialogueActive && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E)))
        {
            DisplayNextLine();
        }
    }

    public void StartDialogue(string[] lines)
    {
        // Se estiver no período de segurança após fechar, não faz nada
        if (!canInteractAgain) return;

        if (lines == null || lines.Length == 0) return;

        currentLines = lines;
        currentLineIndex = 0;
        isDialogueActive = true;

        if (dialogueBoxPanel != null)
            dialogueBoxPanel.SetActive(true);

        dialogueText.text = currentLines[currentLineIndex];
    }

    void DisplayNextLine()
    {
        currentLineIndex++;

        if (currentLineIndex < currentLines.Length)
        {
            dialogueText.text = currentLines[currentLineIndex];
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        currentLines = null;

        if (dialogueBoxPanel != null)
            dialogueBoxPanel.SetActive(false);

        // Dispara o bloqueio temporário para ignorar o 'E' do fecho
        StartCoroutine(CooldownRoutine());
    }

    IEnumerator CooldownRoutine()
    {
        canInteractAgain = false;
        yield return new WaitForSeconds(0.4f); // 0.4 segundos de pausa de segurança
        canInteractAgain = true;
    }

    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }
}