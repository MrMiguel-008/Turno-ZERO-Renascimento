using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class InteracaoAnimacaoDireta : MonoBehaviour
{
    [Header("Identificação da Missão")]
    [Tooltip("ID da missão que precisa estar ativa para permitir a interação.")]
    [SerializeField] private string idMissaoRequerida = "producao";

    [Header("Configuração de Animação")]
    [SerializeField] private Animator animator;
    [SerializeField] private string nomeAnimacao = "Produção";

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

    private bool PodeInteragir()
    {
        if (jaInteragiu) return false;

        if (QuestManager.Instance != null)
        {
            string questAtivaID = QuestManager.Instance.activeQuestID;
            var questInfo = QuestManager.Instance.GetQuestInfo(questAtivaID);

            return questInfo.questType == QuestType.Minigame && questInfo.targetMinigameID == idMissaoRequerida;
        }

        return true;
    }

    void Update()
    {
        if (jogadorNaArea && !jaInteragiu && Input.GetKeyDown(KeyCode.E))
        {
            if (PodeInteragir())
            {
                StartCoroutine(ExecutarAcaoEAguardarAnimacao());
            }
            else
            {
                Debug.Log("[InteracaoAnimacaoDireta] A missão atual não permite esta interação!");
            }
        }
    }

    IEnumerator ExecutarAcaoEAguardarAnimacao()
    {
        jaInteragiu = true;
        EsconderMensagem();

        if (animator != null && !string.IsNullOrEmpty(nomeAnimacao))
        {
            animator.Play(nomeAnimacao);
            Debug.Log("[InteracaoAnimacaoDireta] Animação iniciada...");

            // Aguarda 1 frame para garantir que o Animator mudou de estado e obteve o comprimento correto do clip
            yield return null;

            // Descobre quanto tempo dura a animação atual em segundos
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float duracaoAnimacao = stateInfo.length;

            // Se por algum motivo o Unity não conseguir ler a duração, define um valor de segurança (ex: 2 segundos)
            if (duracaoAnimacao <= 0f) duracaoAnimacao = 2f;

            // Espera exatamente o tempo que a animação demora a terminar
            yield return new WaitForSeconds(duracaoAnimacao);

            Debug.Log("[InteracaoAnimacaoDireta] Animação concluída com sucesso!");
        }
        else
        {
            // Fallback caso não haja animator, espera 1 segundo
            yield return new WaitForSeconds(1f);
        }

        // --- SÓ AVANÇA A MISSÃO DEPOIS DA ANIMAÇÃO ACABAR ---
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.CheckMinigameQuest(idMissaoRequerida);
            Debug.Log("[InteracaoAnimacaoDireta] Missão avançada no QuestManager!");
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