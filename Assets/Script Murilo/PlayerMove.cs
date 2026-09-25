using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        // Pega o componente Rigidbody2D anexado ao jogador
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Captura as entradas do teclado/controle (WASD ou Setas)
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Junta as entradas em um Vector2 e normaliza para não andar mais rápido na diagonal
        moveInput = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        // Aplica o movimento na física do Rigidbody2D
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}