using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseArrastaObj : MonoBehaviour
{
    private static int totalSucessos = 0;
    private static float pontuacaoAtualMinigame = 100f;
    private static float penalizacaoAtual = 10f;
    private static string cenaAtualContada = "";

    [Header("Configuração da Cena 1 (Bancada)")]
    [SerializeField] private string nomeCena1 = "CenaBancadaDesarrumada";
    [SerializeField] private int metaCena1 = 7;
    [SerializeField] private float pontosIniciais1 = 100f;
    [SerializeField] private float penalizacaoErro1 = 10f;

    [Header("Configuração da Cena 2 (Tapete)")]
    [SerializeField] private string nomeCena2 = "CenaOutroMinigame";
    [SerializeField] private int metaCena2 = 6;
    [SerializeField] private float pontosIniciais2 = 150f;
    [SerializeField] private float penalizacaoErro2 = 20f;

    private int objetivosNecessariosDestaCena = 7;

    public Canvas canvas;
    public string tagDoAlvoCorreto;
    private Vector3 posicaoOriginal;
    private bool segurandoObjeto = false;
    private Collider2D zonaDeColisaoAtual;
    private Camera cameraPrincipal;

    private SpriteRenderer spriteRenderer;
    private int ordemInicial;
    private int ordemArrastando = 100;
    private bool travado = false;

    // Reseta os dados sempre que uma nova cena carrega
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InicializarOuvinteDeCenas()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        totalSucessos = 0;
        cenaAtualContada = "";
    }

    void Start()
    {
        cameraPrincipal = Camera.main;
        posicaoOriginal = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        ordemInicial = spriteRenderer.sortingOrder;

        if (canvas != null)
            canvas.gameObject.SetActive(false);

        string nomeCenaAtual = SceneManager.GetActiveScene().name;

        // Configura a pontuação inicial baseada na cena apenas na primeira execução da cena
        if (cenaAtualContada != nomeCenaAtual)
        {
            totalSucessos = 0;
            cenaAtualContada = nomeCenaAtual;

            if (nomeCenaAtual == nomeCena1)
            {
                pontuacaoAtualMinigame = pontosIniciais1;
                penalizacaoAtual = penalizacaoErro1;
            }
            else if (nomeCenaAtual == nomeCena2)
            {
                pontuacaoAtualMinigame = pontosIniciais2;
                penalizacaoAtual = penalizacaoErro2;
            }
            else
            {
                pontuacaoAtualMinigame = pontosIniciais1;
                penalizacaoAtual = penalizacaoErro1;
            }

            Debug.Log($"[Minigame] Cena: {nomeCenaAtual} | Pontos Iniciais: {pontuacaoAtualMinigame} | Penalização por Erro: {penalizacaoAtual}");
        }

        // Define a quantidade de itens necessários para a cena atual
        if (nomeCenaAtual == nomeCena1)
        {
            objetivosNecessariosDestaCena = metaCena1;
        }
        else if (nomeCenaAtual == nomeCena2)
        {
            objetivosNecessariosDestaCena = metaCena2;
        }
        else
        {
            objetivosNecessariosDestaCena = metaCena1;
        }
    }

    void OnMouseDown()
    {
        if (travado) return;

        segurandoObjeto = true;
        spriteRenderer.sortingOrder = ordemArrastando;
    }

    void OnMouseDrag()
    {
        if (segurandoObjeto && !travado)
        {
            Vector3 posicaoMouse = cameraPrincipal.ScreenToWorldPoint(Input.mousePosition);
            posicaoMouse.z = 0f;
            transform.position = posicaoMouse;
        }
    }

    void OnMouseUp()
    {
        if (travado) return;

        segurandoObjeto = false;
        spriteRenderer.sortingOrder = ordemInicial;

        if (zonaDeColisaoAtual != null)
        {
            if (zonaDeColisaoAtual.CompareTag(tagDoAlvoCorreto))
            {
                SucessoNoDeposito();
            }
            else
            {
                RegistrarErro("Colocou o item na zona incorreta!");
            }
        }
        else
        {
            RegistrarErro("Soltou o item fora de qualquer zona!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (travado) return;
        zonaDeColisaoAtual = collision;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (zonaDeColisaoAtual == collision)
        {
            zonaDeColisaoAtual = null;
        }
    }

    private void RegistrarErro(string motivo)
    {
        transform.position = posicaoOriginal;

        // Desconta a penalização da cena, garantindo que o limite mínimo seja exatamente 0
        pontuacaoAtualMinigame = Mathf.Max(0f, pontuacaoAtualMinigame - penalizacaoAtual);

        Debug.LogWarning($"[ERRO] {motivo} -{penalizacaoAtual} pts. Pontuação atual: {pontuacaoAtualMinigame} pts");
    }

    private void SucessoNoDeposito()
    {
        if (travado) return;

        travado = true;
        totalSucessos++;

        Debug.Log($"Objeto depositado! Total itens: {totalSucessos}/{objetivosNecessariosDestaCena} | Pontuação atual: {pontuacaoAtualMinigame} pts");

        if (tagDoAlvoCorreto == "Lixeira")
        {
            Destroy(gameObject);
        }
        else
        {
            transform.position = zonaDeColisaoAtual.transform.position;

            Collider2D meuCollider = GetComponent<Collider2D>();
            if (meuCollider != null) meuCollider.enabled = false;
        }

        // Verifica se completou todos os itens da cena atual
        if (totalSucessos >= objetivosNecessariosDestaCena)
        {
            Debug.Log($"Minigame concluído com sucesso! Pontuação final enviada: {pontuacaoAtualMinigame} pontos.");

            // Envia a pontuação obtida para o gestor global do jogo
            GameScoreManager.AdicionarPontosMinigame(SceneManager.GetActiveScene().name, pontuacaoAtualMinigame);

            InteracaoMinigame.FinalizarMinigame(true);
        }
    }
}