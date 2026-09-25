using UnityEngine;

public class ObjetoMovimento : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform playerTransform;

    [Header("Configurações")]
    [SerializeField] private float distanciaInteracao = 3f;
    [SerializeField] private KeyCode teclaInteracao = KeyCode.F;
    [SerializeField] private Vector3 posicaoRelativa = new Vector3(0f, 1f, 0f);

    private bool estaSegurando = false;
    private Rigidbody2D rbCaixa;
    private Collider2D colCaixa;

    // Controla se já existe ALGUM item sendo segurado no jogo para evitar pegar dois
    public static bool playerEstaSegurandoAlgo = false;

    void Start()
    {
        rbCaixa = GetComponent<Rigidbody2D>();
        colCaixa = GetComponent<Collider2D>();

        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }

        if (rbCaixa != null)
        {
            rbCaixa.bodyType = RigidbodyType2D.Kinematic;
            rbCaixa.useFullKinematicContacts = true;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        if (Input.GetKeyDown(teclaInteracao))
        {
            float distancia = Vector2.Distance(transform.position, playerTransform.position);

            // Só pode pegar se estiver perto, se NÃO estiver segurando este item, 
            // e se o player NÃO estiver a segurar nenhum outro item no momento
            if (!estaSegurando && distancia <= distanciaInteracao && !playerEstaSegurandoAlgo)
            {
                PegarCaixa();
            }
            else if (estaSegurando)
            {
                SoltarCaixa();
            }
        }
    }

    void PegarCaixa()
    {
        estaSegurando = true;
        playerEstaSegurandoAlgo = true; // Informa que o player agora está ocupado segurando algo

        if (colCaixa != null) colCaixa.enabled = false;

        transform.SetParent(playerTransform);
        transform.localPosition = posicaoRelativa;
    }

    void SoltarCaixa()
    {
        estaSegurando = false;
        playerEstaSegurandoAlgo = false; // Libera o player para poder pegar itens novamente

        Vector3 posicaoSoltura = transform.position;
        transform.SetParent(null);
        transform.position = posicaoSoltura;

        if (rbCaixa != null)
        {
            rbCaixa.linearVelocity = Vector2.zero;
            rbCaixa.angularVelocity = 0f;
        }

        if (colCaixa != null) colCaixa.enabled = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaInteracao);
    }
}