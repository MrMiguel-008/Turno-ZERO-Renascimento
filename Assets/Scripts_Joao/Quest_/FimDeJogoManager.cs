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

    void Awake()
    {
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
        Time.timeScale = 1f;

        if (painelFimDeJogo != null)
            painelFimDeJogo.SetActive(false);

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

        Time.timeScale = 0f;
        SoundManager.Tocar(Som.Vitoria);

        if (painelFimDeJogo != null)
        {
            painelFimDeJogo.SetActive(true);
        }
        else
        {
            Debug.LogError("[FimDeJogoManager] O Painel de Fim de Jogo não está atribuído no Inspector!");
        }

        int pontosPlayer = 0;
        int pontosMaximos = 0;

        // Puxa dinamicamente a pontuação acumulada e o máximo real do QuestManager
        if (QuestManager.Instance != null)
        {
            pontosPlayer = QuestManager.Instance.pontuacaoTotalAcumulada;
            pontosMaximos = QuestManager.Instance.pontuacaoMaximaAcumulada;
        }

        if (textoPontuacaoPlayer != null)
            textoPontuacaoPlayer.text = $"Sua Pontuação: {pontosPlayer} pts";

        if (textoPontuacaoMaxima != null)
            textoPontuacaoMaxima.text = $"Pontuação Máxima: {pontosMaximos} pts";
    }

    public void ReiniciarJogo()
    {
        SoundManager.Tocar(Som.CliqueBotao);
        Time.timeScale = 1f;

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.ReiniciarProgressoMissoes();
        }

        InteracaoMinigame.LimparEstadoDaPartida();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void VoltarAoMenu()
    {
        SoundManager.Tocar(Som.CliqueBotao);
        Time.timeScale = 1f;

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.ReiniciarProgressoMissoes();
        }

        InteracaoMinigame.LimparEstadoDaPartida();
        SceneManager.LoadScene(nomeCenaMenu);
    }
}