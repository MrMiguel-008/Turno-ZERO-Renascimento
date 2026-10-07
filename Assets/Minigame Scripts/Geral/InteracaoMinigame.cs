using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteracaoMinigame : MonoBehaviour
{
    [Header("Interação")]
    [Tooltip("Texto que aparecerá quando o jogador estiver perto.")]
    [SerializeField] private string mensagemInteracao = "E - Interagir";

    [Tooltip("Texto da interface que mostrará a mensagem.")]
    [SerializeField] private TMP_Text mensagemUI;

    [Header("Minigame")]
    [Tooltip("Nome exato da cena do minigame.")]
    [SerializeField] private string cenaMinigame;

    [SerializeField] private Vector3 deslocamentoMensagem = new Vector3(0f, 1f, 0f);

    private Camera cameraPrincipal;
    private bool jogadorNaArea = false;

    // Informações da cena anterior
    public static string CenaAnterior { get; private set; }
    public static Vector3 PosicaoPlayer { get; private set; }

    private void Start()
    {
        EsconderMensagem();

        RestaurarPosicaoPlayer();

        cameraPrincipal = Camera.main;
        EsconderMensagem();
    }

    private void Update()
    {
        if (jogadorNaArea)
        {
            AtualizarPosicaoMensagem();
        }


        if (!jogadorNaArea)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            IniciarMinigame();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        jogadorNaArea = true;

        MostrarMensagem();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        jogadorNaArea = false;

        EsconderMensagem();
    }

    private void MostrarMensagem()
    {
        if (mensagemUI == null)
            return;

        mensagemUI.text = mensagemInteracao;
        mensagemUI.gameObject.SetActive(true);
    }

    private void EsconderMensagem()
    {
        if (mensagemUI == null)
            return;

        mensagemUI.gameObject.SetActive(false);
    }

    private void IniciarMinigame()
    {
        EsconderMensagem();

        if (string.IsNullOrEmpty(cenaMinigame))
        {
            Debug.LogWarning(
                "InteracaoMinigame: nenhuma cena de minigame foi configurada."
            );

            return;
        }

        // Guarda a cena atual
        CenaAnterior = SceneManager.GetActiveScene().name;

        // Procura o Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Guarda a posição exata do Player
            PosicaoPlayer = player.transform.position;
        }
        else
        {
            Debug.LogWarning(
                "InteracaoMinigame: nenhum objeto com a Tag 'Player' foi encontrado."
            );
        }

        // Vai para o minigame
        SceneManager.LoadScene(cenaMinigame);
    }

    private void RestaurarPosicaoPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning(
                "InteracaoMinigame: não foi encontrado um Player para restaurar."
            );

            return;
        }

        player.transform.position = PosicaoPlayer;
    }
    private void AtualizarPosicaoMensagem()
    {
        if (mensagemUI == null || cameraPrincipal == null)
            return;

        Vector3 posicaoMundo = transform.position + deslocamentoMensagem;

        Vector3 posicaoTela = cameraPrincipal.WorldToScreenPoint(posicaoMundo);

        mensagemUI.transform.position = posicaoTela;
    }
}