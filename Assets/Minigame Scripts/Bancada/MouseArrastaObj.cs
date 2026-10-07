using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseArrastaObj : MonoBehaviour
{
    private static int totalSucessos = 0;
    private const int OBJETIVOS_NECESSARIOS = 7;
    public Canvas canvas;

    public string tagDoAlvoCorreto;
    private Vector3 posicaoOriginal;
    private bool segurandoObjeto = false;
    private Collider2D zonaDeColisaoAtual;
    private Camera cameraPrincipal;

    private SpriteRenderer spriteRenderer;
    private int ordemInicial;
    private int ordemArrastando = 100;

    private bool travado = false;

    void Start()
    {
        PlayerPrefs.SetInt("MinigameIniciado_01", 1);
        PlayerPrefs.Save();

        cameraPrincipal = Camera.main;
        posicaoOriginal = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        ordemInicial = spriteRenderer.sortingOrder;

        totalSucessos = 0;

        canvas.gameObject.SetActive(false);
    }

    void OnMouseDown()
    {
        if (travado) return;

        segurandoObjeto = true;
        spriteRenderer.sortingOrder = ordemArrastando;
    }

    void OnMouseDrag()
    {
        if (segurandoObjeto && !travado)
        {
            Vector3 posicaoMouse = cameraPrincipal.ScreenToWorldPoint(Input.mousePosition);
            posicaoMouse.z = 0f;
            transform.position = posicaoMouse;
        }
    }

    void OnMouseUp()
    {
        if (travado) return;

        segurandoObjeto = false;
        spriteRenderer.sortingOrder = ordemInicial;

        if (zonaDeColisaoAtual != null && zonaDeColisaoAtual.CompareTag(tagDoAlvoCorreto))
        {
            SucessoNoDeposito();
        }
        else
        {
            transform.position = posicaoOriginal;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (travado) return;
        zonaDeColisaoAtual = collision;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (zonaDeColisaoAtual == collision)
        {
            zonaDeColisaoAtual = null;
        }
    }

    private void SucessoNoDeposito()
    {
        totalSucessos++;

        if (tagDoAlvoCorreto == "Lixeira")
        {
            Destroy(gameObject);
        }
        else
        {
            transform.position = zonaDeColisaoAtual.transform.position;

            travado = true;

            Collider2D meuCollider = GetComponent<Collider2D>();
            if (meuCollider != null) meuCollider.enabled = false;
        }

        if (totalSucessos >= OBJETIVOS_NECESSARIOS)
        {
            SceneManager.LoadScene(0);
        }
    }
}