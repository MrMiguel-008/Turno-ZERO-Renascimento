using UnityEngine;

public class QualidadeArrasta : MonoBehaviour
{
    private Camera cam;
    private Embalagem embalagemScript;
    private Vector3 posicaoOriginal;

    void Start()
    {
        cam = Camera.main;
        embalagemScript = GetComponent<Embalagem>();
    }

    void OnMouseDown()
    {
        posicaoOriginal = transform.position;
        SoundManager.Tocar(Som.ColetaBiscoito);
        if (embalagemScript != null)
        {
            embalagemScript.DefinirArrasto(true);
        }
    }

    void OnMouseDrag()
    {
        if (cam != null)
        {
            Vector3 posicaoMouse = cam.ScreenToWorldPoint(Input.mousePosition);
            posicaoMouse.z = 0f;
            transform.position = posicaoMouse;
        }
    }

    void OnMouseUp()
    {
        SoundManager.Tocar(Som.SoltarItem);
        transform.position = new Vector3(transform.position.x, posicaoOriginal.y, transform.position.z);

        if (embalagemScript != null)
        {
            embalagemScript.DefinirArrasto(false);

            if (QualidadeGerenciador.Instancia != null && !QualidadeGerenciador.Instancia.IsEsteiraAtiva())
            {
                embalagemScript.DefinirMovimento(false);
            }
        }
    }
}
