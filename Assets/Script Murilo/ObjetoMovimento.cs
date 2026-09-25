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

    void Start()
    {
        rbCaixa = GetComponent<Rigidbody2D>();
        colCaixa = GetComponent<Collider2D>();

        // Acha o player automaticamente pela Tag se não foi arrastado no Inspector
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }

        // Garante que a caixa nunca seja empurrada fisicamente ao esbarrar inicialmente
        if (rbCaixa != null)
        {
            rbCaixa.bodyType = RigidbodyType2D.Kinematic;
            rbCaixa.useFullKinematicContacts = true;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Se apertar a tecla F
        if (Input.GetKeyDown(teclaInteracao))
        {
            float distancia = Vector2.Distance(transform.position, playerTransform.position);

            // Se não está segurando e chegou perto o suficiente, pega
            if (!estaSegurando && distancia <= distanciaInteracao)
            {
                PegarCaixa();
            }
            // Se já está segurando, solta onde o player estiver
            else if (estaSegurando)
            {
                SoltarCaixa();
            }
        }
    }

    void PegarCaixa()
    {
        estaSegurando = true;

        // Desativa a colisão para não enroscar no player enquanto carrega
        if (colCaixa != null) colCaixa.enabled = false;

        // Gruda no player
        transform.SetParent(playerTransform);
        transform.localPosition = posicaoRelativa;
    }

    void SoltarCaixa()
    {
        estaSegurando = false;

        // 1. Desanexa do Player primeiro
        transform.SetParent(null);

        // 2. Reseta a velocidade física para evitar bugs de movimento
        if (rbCaixa != null)
        {
            rbCaixa.linearVelocity = Vector2.zero;
            rbCaixa.angularVelocity = 0f;
        }

        // 3. Reativa a colisão
        if (colCaixa != null) colCaixa.enabled = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaInteracao);
    }
}