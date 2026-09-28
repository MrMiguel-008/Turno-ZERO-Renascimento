using UnityEngine;

public class LancarObjeto : MonoBehaviour
{
    [Header("Configurações de Lançamento")]
    [SerializeField] private KeyCode teclaLancar = KeyCode.G;
    [SerializeField] private float forcaLancamento = 15f;

    private Rigidbody2D rbCaixa;
    private Collider2D colCaixa;
    private Transform playerTransform;
    private Collider2D colisorPlayer;

    private Vector2 ultimaDirecao = Vector2.right;

    void Start()
    {
        rbCaixa = GetComponent<Rigidbody2D>();
        colCaixa = GetComponent<Collider2D>();

        if (rbCaixa != null)
        {
            rbCaixa.freezeRotation = true;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            colisorPlayer = playerObj.GetComponent<Collider2D>();
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Atualiza a última direção com base nas teclas W, A, S, D
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            ultimaDirecao = Vector2.up;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            ultimaDirecao = Vector2.down;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            ultimaDirecao = Vector2.left; // Esquerda
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            ultimaDirecao = Vector2.right; // Direita
        }

        bool estaSendoSegurado = transform.parent != null && transform.parent == playerTransform;

        if (estaSendoSegurado && Input.GetKeyDown(teclaLancar))
        {
            ExecutarLancamento();
        }
    }

    void ExecutarLancamento()
    {
        // 1. Libera o player no script de movimento
        ObjetoMovimento.playerEstaSegurandoAlgo = false;

        // 2. Desanexa do player primeiro para podermos alterar a posição livremente
        transform.SetParent(null);

        // 3. Posiciona a caixa diretamente FORA do player na direção do tiro (resolve o travamento para a esquerda)
        // Usamos uma distância maior (0.8f) para garantir que ela nasça totalmente livre do colisor
        transform.position = playerTransform.position + (Vector3)(ultimaDirecao * 0.8f);

        // 4. Configura a colisão: ativa a caixa, mas ignora o player temporariamente para evitar empurrões indesejados
        if (colCaixa != null)
        {
            colCaixa.enabled = true;

            if (colisorPlayer != null)
            {
                Physics2D.IgnoreCollision(colCaixa, colisorPlayer, true);
                // Aumentei o tempo para 0.4s para dar tempo do item se afastar bem antes de poder colidir com o player de novo
                Invoke("ReativarColisaoPlayer", 0.4f);
            }
        }

        // 5. Aplica a física do lançamento
        if (rbCaixa != null)
        {
            rbCaixa.bodyType = RigidbodyType2D.Dynamic;
            rbCaixa.simulated = true;
            rbCaixa.gravityScale = 0f;

            rbCaixa.linearVelocity = ultimaDirecao * forcaLancamento;
        }
    }

    void ReativarColisaoPlayer()
    {
        if (colCaixa != null && colisorPlayer != null)
        {
            Physics2D.IgnoreCollision(colCaixa, colisorPlayer, false);
        }
    }
}