using UnityEngine;
using TMPro;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class InteracaoAnimacaoDireta : MonoBehaviour
{
    [Header("Identificação")]
    [SerializeField] private string idObjetivo = "objetivo_animado";

    [Header("UI e Mensagem")]
    [SerializeField] private string mensagemInteracao = "E - Interagir";
    [SerializeField] private TMP_Text mensagemUI;
    [SerializeField] private Vector3 deslocamentoMensagem = new Vector3(0f, 1f, 0f);

    [Header("Configuração da Animação")]
    [SerializeField] private string triggerAnimacao = "TocarAnimacao";
    [SerializeField] private float duracaoAnimacao = 2f;

    private Animator animator;
    private bool jogadorNaArea = false;
    private bool emAndamento = false;
    private bool concluido = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Diagnóstico inicial para ajudar a detetar erros de configuração no Inspector
        if (mensagemUI == null)
        {
            Debug.LogWarning($"[InteracaoAnimacaoDireta] O campo 'Mensagem UI' não está atribuído no objeto '{gameObject.name}'!", this);
        }

        EsconderMensagem();

        if (InteracaoMinigame.ObjetivoFoiConcluido(idObjetivo))
        {
            concluido = true;
            enabled = false;
        }
    }

    void Update()
    {
        if (concluido || emAndamento) return;

        // Atualiza a posição da mensagem continuamente enquanto o jogador estiver na área
        if (jogadorNaArea && mensagemUI != null && mensagemUI.gameObject.activeSelf)
        {
            AtualizarPosicaoMensagem();
        }

        if (jogadorNaArea && Input.GetKeyDown(KeyCode.E))
        {
            IniciarAnimacaoEMissao();
        }
    }

    void IniciarAnimacaoEMissao()
    {
        emAndamento = true;
        EsconderMensagem();

        if (animator != null)
        {
            animator.SetTrigger(triggerAnimacao);
        }

        StartCoroutine(EsperarFimDaAnimacao());
    }

    System.Collections.IEnumerator EsperarFimDaAnimacao()
    {
        yield return new WaitForSeconds(duracaoAnimacao);

        concluido = true;
        Debug.Log($"[InteracaoAnimacaoDireta] Animação concluída para: {idObjetivo}");

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.CheckMinigameQuest(idObjetivo);
        }

        enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (concluido || emAndamento) return;
        if (other.CompareTag("Player"))
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
        if (mensagemUI == null)
        {
            Debug.LogError($"[InteracaoAnimacaoDireta] Tentativa de mostrar mensagem, mas 'Mensagem UI' é nula em '{gameObject.name}'!");
            return;
        }

        mensagemUI.text = mensagemInteracao;
        mensagemUI.gameObject.SetActive(true);
        AtualizarPosicaoMensagem();
    }

    void EsconderMensagem()
    {
        if (mensagemUI == null) return;
        mensagemUI.gameObject.SetActive(false);
    }

    void AtualizarPosicaoMensagem()
    {
        if (mensagemUI == null) return;

        Camera cameraPrincipal = Camera.main;
        if (cameraPrincipal == null) return;

        // Converte a posição do mundo 2D para coordenadas de tela do UI
        Vector3 posicaoMundo = transform.position + deslocamentoMensagem;
        Vector3 posicaoTela = cameraPrincipal.WorldToScreenPoint(posicaoMundo);

        mensagemUI.transform.position = posicaoTela;
    }

    // Desenha uma esfera amarela no editor Unity para veres onde a mensagem vai aparecer
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + deslocamentoMensagem, 0.3f);
    }
}