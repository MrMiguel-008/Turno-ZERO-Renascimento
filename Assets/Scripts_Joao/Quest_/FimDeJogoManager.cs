using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class FimDeJogoManager : MonoBehaviour
{
    [Header("Configuração de Cenas")]
    [Tooltip("Nome exato da cena do Menu Principal")]
    [SerializeField] private string nomeCenaMenu = "MenuPrincipal";

    [Header("Elementos de UI (Atribuir no Inspector)")]
    [Tooltip("O GameObject do painel/HUD de fim de jogo que vai aparecer no fim")]
    [SerializeField] private GameObject painelFimDeJogo;

    [Tooltip("Texto onde vai aparecer a pontuação obtida pelo jogador")]
    [SerializeField] private TMP_Text textoPontuacaoPlayer;

    [Tooltip("Texto onde vai aparecer a pontuação máxima possível (ex: 400 pts)")]
    [SerializeField] private TMP_Text textoPontuacaoMaxima;

    [Tooltip("Botão para reiniciar o jogo")]
    [SerializeField] private Button botaoReiniciar;

    [Tooltip("Botão para voltar ao menu principal")]
    [SerializeField] private Button botaoMenuPrincipal;

    [Header("Configuração de Pontuação")]
    [Tooltip("Soma total máxima que o jogador pode conseguir em todos os minigames")]
    [SerializeField] private float pontuacaoMaximaTotal = 400f; // Ajusta conforme o total dos teus minigames

    void Start()
    {
        // Garante que o painel começa escondido e o tempo a correr normalmente
        if (painelFimDeJogo != null)
            painelFimDeJogo.SetActive(false);

        Time.timeScale = 1f;

        // Configura os ouvintes dos botões por código para garantir que funcionam sempre
        if (botaoReiniciar != null)
            botaoReiniciar.onClick.AddListener(ReiniciarJogo);

        if (botaoMenuPrincipal != null)
            botaoMenuPrincipal.onClick.AddListener(VoltarAoMenu);
    }

    // Podes chamar este método quando a última missão for concluída
    public void DispararFimDeJogo()
    {
        // Congela completamente o tempo do jogo
        Time.timeScale = 0f;

        // Ativa o HUD de fim de jogo
        if (painelFimDeJogo != null)
            painelFimDeJogo.SetActive(true);

        // Puxa a pontuação acumulada no GameScoreManager
        float pontosPlayer = GameScoreManager.GetPontuacaoTotal();

        // Atualiza os textos na tela
        if (textoPontuacaoPlayer != null)
            textoPontuacaoPlayer.text = $"Sua Pontuação: {pontosPlayer} pts";

        if (textoPontuacaoMaxima != null)
            textoPontuacaoMaxima.text = $"Pontuação Máxima: {pontuacaoMaximaTotal} pts";

        Debug.Log("[FimDeJogo] Jogo concluído congelado. Pontuação exibida com sucesso.");
    }

    public void ReiniciarJogo()
    {
        // Restaura o tempo antes de mudar de cena para evitar bugs de congelamento
        Time.timeScale = 1f;

        // Reseta a pontuação global guardada
        GameScoreManager.ResetarPontuacao();

        // Recarrega a cena atual (ou a primeira fase do jogo)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void VoltarAoMenu()
    {
        // Restaura o tempo antes de mudar de cena
        Time.timeScale = 1f;

        // Reseta a pontuação global guardada
        GameScoreManager.ResetarPontuacao();

        // Vai diretamente para a cena do Menu Principal
        SceneManager.LoadScene(nomeCenaMenu);
    }
}