using UnityEngine;

public class TocaAnimacao : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float velocidadeReal = rb.linearVelocity.magnitude;

        animator.SetFloat("Speed", velocidadeReal);
    }
}