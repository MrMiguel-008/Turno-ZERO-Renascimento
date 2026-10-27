using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseArrastaObj : MonoBehaviour
{
    // =========================================================
    // VARIÁVEIS ESTÁTICAS (GLOBAIS POR CENA)
    // =========================================================
    private static int totalSucessos = 0;
    private static float pontuacaoAtualMinigame = 100f;
    private static float penalizacaoAtual = 10f;
    private static string cenaAtualContada = "";

    // =========================================================
    // CONFIGURAÇÕES DE CENAS E METAS (PRIVATIZADAS)
    // =========================================================
    [Header("Configuração da Cena 1 (Minigame EPI)")]
    [SerializeField] private string nomeCena1 = "MinigameEPI";
    [SerializeField] private int metaCena1 = 6;
    [SerializeField] private float pontosIniciais1 = 6f;
    [SerializeField] private float penalizacaoErro1 = 1f;

    [Header("Configuração da Cena 2 (Bancada)")]
    [SerializeField] private string nomeCena2 = "BancadaMinigame";
    [SerializeField] private int metaCena2 = 7;
    [SerializeField] private float pontosIniciais2 = 100f;
    [SerializeField] private float penalizacaoErro2 = 10f;

    private int objetivosNecessariosDestaCena = 6;

    // =========================================================
    // VARIÁVEIS DE COMPONENTES E ESTADO DO OBJETO
    // =========================================================
    [SerializeField] private Canvas canvas;
    [SerializeField] private string tagDoAlvoCorreto;
    private Vector3 posicaoOriginal;
    private bool segurandoObjeto = false;
    private Collider2D zonaDeColisaoAtual;
    private Camera cameraPrincipal;

    private SpriteRenderer spriteRenderer;
    private int ordemInicial;
    private int ordemArrastando = 100;
    private bool travado = false;

    // =========================================================
    // UNITY INITIALIZATION & SCENE MANAGEMENT
    // =========================================================
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

        // Debug para verificar se o Unity está a ler corretamente a cena certa
        Debug.Log($"[MouseArrastaObj] Cena detetada pelo Unity: '{nomeCenaAtual}'");

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
        }

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

        Debug.Log($"[MouseArrastaObj] Meta definida para esta cena: {objetivosNecessariosDestaCena} itens.");
    }

    // =========================================================
    // INPUT DO RATO (DRAG AND DROP)
    // =========================================================
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

    // =========================================================
    // COLISÕES COM AS ZONAS DE DEPÓSITO
    // =========================================================
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

    // =========================================================
    // LÓGICA DE ERRO E PENALIZAÇÃO
    // =========================================================
    private void RegistrarErro(string motivo)
    {
        transform.position = posicaoOriginal;
        pontuacaoAtualMinigame = Mathf.Max(0f, pontuacaoAtualMinigame - penalizacaoAtual);
        Debug.LogWarning($"[ERRO] {motivo} -{penalizacaoAtual} pts. Pontuação atual: {pontuacaoAtualMinigame} pts");
    }

    // =========================================================
    // LÓGICA DE SUCESSO E CONCLUSÃO
    // =========================================================
    private void SucessoNoDeposito()
    {
        if (travado) return;

        travado = true;
        totalSucessos++;

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

        if (totalSucessos >= objetivosNecessariosDestaCena)
        {
            Debug.Log($"Minigame concluído! Pontuação final enviada: {pontuacaoAtualMinigame} pontos.");
            GameScoreManager.AdicionarPontosMinigame(SceneManager.GetActiveScene().name, pontuacaoAtualMinigame);
            InteracaoMinigame.FinalizarMinigame(true);
        }
    }
}