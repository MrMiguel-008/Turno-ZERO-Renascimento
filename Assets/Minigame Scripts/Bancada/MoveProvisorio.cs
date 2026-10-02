using UnityEngine;

public class MoveProvisorio : MonoBehaviour
{
    public float velocidade = 5f;

    private Rigidbody2D rb;
    private Vector2 inputMovimento;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        inputMovimento.x = Input.GetAxisRaw("Horizontal");
        inputMovimento.y = Input.GetAxisRaw("Vertical");

        inputMovimento = inputMovimento.normalized;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = inputMovimento * velocidade;
    }
}
