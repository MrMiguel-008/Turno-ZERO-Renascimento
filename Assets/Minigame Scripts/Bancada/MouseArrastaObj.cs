using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseArrastaObj : MonoBehaviour
{
    // Pontuação total acumulada na cena atual
    private static float totalPontos = 0f;

    [Header("Configuração de Valor do Objeto")]
    [Tooltip("Quanto vale este objeto específico ao ser depositado (ex: 1, 0.5, 2, 10).")]
    [SerializeField] private float valorDoObjeto = 1f;

    [Header("Configuração de Metas por Cena")]
    [Tooltip("Pontuação necessária para concluir a Cena 1")]
    [SerializeField] private float metaCena1 = 7f;
    [SerializeField] private string nomeCena1 = "CenaBancadaDesarrumada";

    [Tooltip("Pontuação necessária para concluir a Cena 2")]
    [SerializeField] private float metaCena2 = 6f;
    [SerializeField] private string nomeCena2 = "CenaOutroMinigame";

    private float pontosNecessariosDestaCena = 7f;

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

    // Reseta a pontuação sempre que uma nova cena é carregada
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InicializarOuvinteDeCenas()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        totalPontos = 0f;
        Debug.Log($"[MouseArrastaObj] Cena carregada: {scene.name}. Pontuação resetada para 0.");
    }

    void Start()
    {
        cameraPrincipal = Camera.main;
        posicaoOriginal = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        ordemInicial = spriteRenderer.sortingOrder;

        if (canvas != null)
            canvas.gameObject.SetActive(false);

        // Define a meta de pontos com base na cena ativa atual
        string nomeCenaAtual = SceneManager.GetActiveScene().name;

        if (nomeCenaAtual == nomeCena1)
        {
            pontosNecessariosDestaCena = metaCena1;
        }
        else if (nomeCenaAtual == nomeCena2)
        {
            pontosNecessariosDestaCena = metaCena2;
        }
        else
        {
            pontosNecessariosDestaCena = metaCena1; // Fallback
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

        if (zonaDeColisaoAtual != null && zonaDeColisaoAtual.CompareTag(tagDoAlvoCorreto))
        {
            SucessoNoDeposito();
        }
        else
        {
            transform.position = posicaoOriginal;
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

    private void SucessoNoDeposito()
    {
        if (travado) return;

        travado = true;

        // Adiciona o valor personalizado deste objeto específico à pontuação total
        totalPontos += valorDoObjeto;

        Debug.Log($"Objeto depositado (+{valorDoObjeto} pts). Total acumulado: {totalPontos}/{pontosNecessariosDestaCena}");

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

        // Verifica se a pontuação total atingiu a meta da cena
        if (totalPontos >= pontosNecessariosDestaCena)
        {
            Debug.Log("Meta de pontos atingida! A finalizar minigame...");
            InteracaoMinigame.FinalizarMinigame(true);
        }
    }
}