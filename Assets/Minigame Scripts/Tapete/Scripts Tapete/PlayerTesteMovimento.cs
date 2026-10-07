using UnityEngine;

public class PlayerTesteMovimento : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float velocidade = 3f;

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 movimento = new Vector2(horizontal, vertical).normalized;

        transform.position += (Vector3)(movimento * velocidade * Time.deltaTime);
    }
}
