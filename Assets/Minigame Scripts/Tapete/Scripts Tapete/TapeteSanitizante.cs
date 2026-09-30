using UnityEngine;
using UnityEngine.SceneManagement;

public class TapeteSanitizante : MonoBehaviour
{
    // =========================================================
    // CONFIGURAÇÃO
    // =========================================================

    [Header("Interação")]
    [SerializeField] private GameObject mensagemInteracao;

    [Tooltip("Nome da cena do minigame.")]
    [SerializeField] private string cenaMinigame = "TapeteSanitizanteMinigame";

    // =========================================================
    // ESTADO
    // =========================================================

    private bool jogadorNaArea = false;
    private bool concluido = false;

    // =========================================================
    // UNITY
    // =========================================================

    private void Start()
    {
        // Verifica se o tapete já foi concluído anteriormente.
        concluido = PlayerPrefs.GetInt("TapeteConcluido", 0) == 1;

        // A mensagem começa escondida.
        if (mensagemInteracao != null)
        {
            mensagemInteracao.SetActive(false);
        }
    }

    private void Update()
    {
        // Se já foi concluído, não faz mais nada.
        if (concluido)
            return;

        // Se o jogador está perto e apertou E...
        if (jogadorNaArea && Input.GetKeyDown(KeyCode.E))
        {
            IniciarMinigame();
        }
    }

    // =========================================================
    // ENTRADA NO TAPETE
    // =========================================================

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Se já concluiu, não mostra interação.
        if (concluido)
            return;

        jogadorNaArea = true;

        if (mensagemInteracao != null)
        {
            mensagemInteracao.SetActive(true);
        }
    }

    // =========================================================
    // SAÍDA DO TAPETE
    // =========================================================

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        jogadorNaArea = false;

        if (mensagemInteracao != null)
        {
            mensagemInteracao.SetActive(false);
        }
    }

    // =========================================================
    // INICIAR MINIGAME
    // =========================================================

    private void IniciarMinigame()
    {
        // Segurança extra:
        // se por algum motivo já estiver concluído, não inicia.
        if (concluido)
            return;

        // Esconde a mensagem antes de trocar de cena.
        if (mensagemInteracao != null)
        {
            mensagemInteracao.SetActive(false);
        }

        SceneManager.LoadScene(cenaMinigame);
    }
}