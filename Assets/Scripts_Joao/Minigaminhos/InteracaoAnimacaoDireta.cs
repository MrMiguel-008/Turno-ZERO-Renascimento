using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class InteracaoAnimacaoDireta : MonoBehaviour
{
    [Header("Identificação da Missão")]
    [Tooltip("ID da missão que precisa estar ativa para permitir a interação.")]
    [SerializeField] private string idMissaoRequerida;

    [Header("Configuração de Animação")]
    [SerializeField] private Animator animator;
    [SerializeField] private string nomeAnimacao;

    [Header("Interação UI (Opcional)")]
    [SerializeField] private string mensagemInteracao = "E - Interagir";
    [SerializeField] private TMP_Text mensagemUI;

    private bool jogadorNaArea = false;
    private bool jaInteragiu = false;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        EsconderMensagem();
    }

    // Valida se o jogador está na missão correta
    private bool PodeInteragir()
    {
        if (jaInteragiu) return false;

        if (QuestManager.Instance != null)
        {
            string questAtivaID = QuestManager.Instance.activeQuestID;
            var questInfo = QuestManager.Instance.GetQuestInfo(questAtivaID);

            // Só permite interagir se a missão ativa for exatamente a requerida
            return questInfo.questID == idMissaoRequerida;
        }

        return true; // Fallback caso não haja QuestManager
    }

    void Update()
    {
        // Se o jogador estiver na área, pressionar 'E' e a missão estiver correta
        if (jogadorNaArea && !jaInteragiu && Input.GetKeyDown(KeyCode.E))
        {
            if (PodeInteragir())
            {
                ExecutarAcao();
            }
            else
            {
                Debug.Log("[InteracaoAnimacaoDireta] A missão atual não permite esta interação!");
            }
        }
    }

    void ExecutarAcao()
    {
        jaInteragiu = true;
        EsconderMensagem();

        if (animator != null && !string.IsNullOrEmpty(nomeAnimacao))
        {
            animator.Play(nomeAnimacao);
            Debug.Log("[InteracaoAnimacaoDireta] Animação executada com sucesso!");
        }

        // Se quiseres avançar a missão automaticamente ao interagir:
        if (QuestManager.Instance != null)
        {
            // Opcional: Podes chamar uma verificação de minigame ou evento aqui, se aplicável
            // QuestManager.Instance.CheckMinigameQuest(idMissaoRequerida);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && PodeInteragir())
        {
            jogadorNaArea = true;
            MostrarMensagem();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorNaArea = false;
            EsconderMensagem();
        }
    }

    void MostrarMensagem()
    {
        if (mensagemUI != null)
        {
            mensagemUI.text = mensagemInteracao;
            mensagemUI.gameObject.SetActive(true);
        }
    }

    void EsconderMensagem()
    {
        if (mensagemUI != null)
        {
            mensagemUI.gameObject.SetActive(false);
        }
    }
}