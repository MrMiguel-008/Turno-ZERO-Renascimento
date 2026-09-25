using UnityEngine;

public class LancarObjeto : MonoBehaviour
{
    [Header("Configurações de Lançamento")]
    [SerializeField] private KeyCode teclaLancar = KeyCode.G;
    [SerializeField][Tooltip("Ajuste aqui o quão longe o item vai ao ser lançado")] private float distanciaLancamento = 4f;

    private Rigidbody2D rbCaixa;
    private Collider2D colCaixa;
    private Transform playerTransform;
    private bool estaSendoSegurado = false;

    void Start()
    {
        rbCaixa = GetComponent<Rigidbody2D>();
        colCaixa = GetComponent<Collider2D>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Verifica se a caixa está sendo segurada (se é filha do player)
        bool segurandoAgora = transform.parent != null && transform.parent == playerTransform;
        estaSendoSegurado = segurandoAgora;

        // Se estiver sendo segurado e apertar G, lança
        if (estaSendoSegurado && Input.GetKeyDown(teclaLancar))
        {
            Lancar();
        }
    }

    void Lancar()
    {
        // 1. Desanexa do player
        transform.SetParent(null);
        estaSendoSegurado = false;

        // 2. Ativa a física do Rigidbody
        if (rbCaixa != null)
        {
            rbCaixa.bodyType = RigidbodyType2D.Dynamic;
            rbCaixa.simulated = true;

            // Descobre para qual lado o player está virado (direita ou esquerda)
            float direcaoOlhar = playerTransform.localScale.x >= 0 ? 1f : -1f;

            // Mantém o eixo Y em 0 para ir perfeitamente em linha reta horizontal
            Vector2 direcaoLancamento = new Vector2(direcaoOlhar, 0f);

            // Aplica a velocidade em linha reta
            rbCaixa.linearVelocity = direcaoLancamento * distanciaLancamento;
        }

        // 3. Reativa a colisão
        if (colCaixa != null)
        {
            colCaixa.enabled = true;
        }
    }
}