using UnityEngine;

public class Embalagem : MonoBehaviour
{
    public float velocidadeEsteira = 2f;
    public bool estaDanificada = false;

    private bool sendoArrastada = false;
    private bool esteiraDeveMover = false;
    private Rigidbody2D rb;

    // ALTERADO: Mudamos de Start para Awake para antecipar a física
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Força o objeto a ignorar a gravidade imediatamente ao nascer
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f; // Segurança extra
        }
    }

    void Start()
    {
        // Pode deixar o Start vazio ou apagá-lo se não tiver mais nada nele
    }


    void Update()
    {
        if (esteiraDeveMover && !sendoArrastada)
        {
            transform.Translate(Vector2.right * velocidadeEsteira * Time.deltaTime);
        }
    }

    public void DefinirArrasto(bool status)
    {
        sendoArrastada = status;
        if (status && rb != null)
        {
            rb.Sleep();
        }
    }

    public void DefinirMovimento(bool mover)
    {
        esteiraDeveMover = mover;
    }

    private void OnDestroy()
    {
        if (QualidadeGerenciador.Instancia != null)
        {
            QualidadeGerenciador.Instancia.RemoverDaLista(this);
        }
    }
}
