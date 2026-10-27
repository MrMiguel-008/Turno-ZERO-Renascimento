using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class FimDeJogoManager : MonoBehaviour
{
    public static FimDeJogoManager Instance;

    [Header("Configuração de Cenas")]
    [Tooltip("Nome exato da cena do Menu Principal")]
    [SerializeField] private string nomeCenaMenu = "MenuPrincipal";

    [Header("Elementos de UI (Atribuir no Inspector)")]
    [Tooltip("O GameObject do painel/HUD de fim de jogo que vai aparecer no fim")]
    [SerializeField] private GameObject painelFimDeJogo;

    [Tooltip("Texto onde vai aparecer a pontuação obtida pelo jogador")]
    [SerializeField] private TMP_Text textoPontuacaoPlayer;

    [Tooltip("Texto onde vai aparecer a pontuação máxima possível")]
    [SerializeField] private TMP_Text textoPontuacaoMaxima;

    [Tooltip("Botão para reiniciar o jogo")]
    [SerializeField] private Button botaoReiniciar;

    [Tooltip("Botão para voltar ao menu principal")]
    [SerializeField] private Button botaoMenuPrincipal;

    [Header("Configuração de Pontuação")]
    [Tooltip("Soma total máxima que o jogador pode conseguir")]
    [SerializeField] private float pontuacaoMaximaTotal = 400f;

    void Awake()
    {
        // Garante que o Singleton não duplique ao reiniciar a cena
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Garante que o tempo roda normal e o painel começa escondido
        Time.timeScale = 1f;

        if (painelFimDeJogo != null)
            painelFimDeJogo.SetActive(false);

        // Configura os botões com segurança e evita cliques duplicados
        if (botaoReiniciar != null)
        {
            botaoReiniciar.onClick.RemoveAllListeners();
            botaoReiniciar.onClick.AddListener(ReiniciarJogo);
        }

        if (botaoMenuPrincipal != null)
        {
            botaoMenuPrincipal.onClick.RemoveAllListeners();
            botaoMenuPrincipal.onClick.AddListener(VoltarAoMenu);
        }
    }

    public void DispararFimDeJogo()
    {
        Debug.Log("[FimDeJogoManager] DispararFimDeJogo foi executado com sucesso!");

        // Congela completamente o tempo do jogo
        Time.timeScale = 0f;

        // Ativa o HUD de fim de jogo
        if (painelFimDeJogo != null)
        {
            painelFimDeJogo.SetActive(true);
        }
        else
        {
            Debug.LogError("[FimDeJogoManager] O Painel de Fim de Jogo não está atribuído no Inspector!");
        }

        // Puxa a pontuação acumulada no GameScoreManager
        float pontosPlayer = GameScoreManager.GetPontuacaoTotal();

        // Atualiza os textos na tela
        if (textoPontuacaoPlayer != null)
            textoPontuacaoPlayer.text = $"Sua Pontuação: {pontosPlayer} pts";

        if (textoPontuacaoMaxima != null)
            textoPontuacaoMaxima.text = $"Pontuação Máxima: {pontuacaoMaximaTotal} pts";
    }

    public void ReiniciarJogo()
    {
        Time.timeScale = 1f;

        // 1. Reseta a pontuação
        GameScoreManager.ResetarPontuacao();

        // 2. Reseta o QuestManager completamente (limpa estados e volta para a primeira missão)
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.ReiniciarProgressoMissoes();
        }

        // 3. Reseta o estado estático dos minigames (liberta as interações com a tecla E)
        InteracaoMinigame.LimparEstadoDaPartida();

        // 4. Recarrega a cena do zero
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void VoltarAoMenu()
    {
        Time.timeScale = 1f;
        GameScoreManager.ResetarPontuacao();
        InteracaoMinigame.LimparEstadoDaPartida();
        SceneManager.LoadScene(nomeCenaMenu);
    }
}