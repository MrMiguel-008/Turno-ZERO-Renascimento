using UnityEngine;

// O papel só pode ser arrastado nas etapas de secar e de descartar, depois de sair do suporte.
public class ArrastarPapel : MonoBehaviour
{
    public SistemaHigiene sistemaHigiene;
    public SecarMaos secarMaos;

    public bool Arrastando { get; private set; }

    private Camera cam;
    private Vector3 distanciaDaPegada;

    private bool PodeArrastar
    {
        get
        {
            EtapaHigiene etapa = sistemaHigiene.EtapaAtual;
            return secarMaos.PapelDisponivel && (etapa == EtapaHigiene.SecarMaos || etapa == EtapaHigiene.DescartarPapel);
        }
    }

    private void Start()
    {
        cam = Camera.main;
    }

    private void OnMouseDown()
    {
        if (!PodeArrastar) return;

        Arrastando = true;
        distanciaDaPegada = transform.position - MouseNoMundo();
        SoundManager.Tocar(Som.InteracaoItem);
    }

    private void OnMouseDrag()
    {
        if (Arrastando)
        {
            MoverPara(MouseNoMundo() + distanciaDaPegada);
        }
    }

    private void OnMouseUp()
    {
        if (Arrastando)
        {
            SoundManager.Tocar(Som.SoltarItem);
        }

        Arrastando = false;
    }

    private Vector3 MouseNoMundo()
    {
        return cam.ScreenToWorldPoint(Input.mousePosition);
    }

    public void MoverPara(Vector3 posicao)
    {
        posicao.z = transform.position.z;
        transform.position = posicao;
    }
}
