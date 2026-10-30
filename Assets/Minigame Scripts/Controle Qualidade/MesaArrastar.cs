using UnityEngine;

public class MesaArrastar : MonoBehaviour
{
    [Header("Configurações de Sprite")]
    public SpriteRenderer spriteRenderer;
    public Sprite spriteNormal;
    public Sprite spriteArrastando;

    [Header("Identificação")]
    public string tipoItem;

    private Vector3 posicaoOriginal;
    private bool arrastando = false;
    private bool estaSobreACaixa = false;
    private Camera cameraPrincipal;

    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        cameraPrincipal = Camera.main;
        posicaoOriginal = transform.position;
    }

    void OnMouseDown()
    {
        arrastando = true;
        if (spriteArrastando != null) spriteRenderer.sprite = spriteArrastando;
    }

    void OnMouseDrag()
    {
        if (!arrastando) return;

        // Segue o mouse livremente, mesmo passando por cima da caixa
        Vector3 posicaoMouse = cameraPrincipal.ScreenToWorldPoint(Input.mousePosition);
        posicaoMouse.z = 0f;
        transform.position = posicaoMouse;
    }

    void OnMouseUp()
    {
        arrastando = false;
        if (spriteNormal != null) spriteRenderer.sprite = spriteNormal;

        // SÓ ENTRA AQUI QUANDO O JOGADOR LARGAR O MOUSE
        if (estaSobreACaixa)
        {
            // Envia o comando para o gerenciador mudar o computador/caixa
            MesaManager.Instancia.ProcessarEntrega(tipoItem);
        }
        else
        {
            // Se largou fora da caixa, volta para o lugar original
            RetornarOrigem();
        }
    }

    // Executa continuamente enquanto o item estiver colidindo/em cima da Caixa
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("MesaCaixa"))
        {
            estaSobreACaixa = true;
        }
    }

    // Detecta quando o item saiu totalmente de cima da Caixa
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("MesaCaixa"))
        {
            estaSobreACaixa = false;
        }
    }

    public void RetornarOrigem()
    {
        transform.position = posicaoOriginal;
        estaSobreACaixa = false;
    }
}
