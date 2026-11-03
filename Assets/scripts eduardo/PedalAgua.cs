using System.Collections;
using UnityEngine;

// Antes da lavagem o pedal liga e desliga a água. Depois da lavagem ele inicia o enxágue.
// Em qualquer outra etapa ele não faz nada. O efeito só acontece quando o pé encosta no pedal.
public class PedalAgua : MonoBehaviour
{
    public SistemaHigiene sistemaHigiene;
    public EtapaEnxague etapaEnxague;

    [Header("Animação do pé")]
    public Transform pe;
    [Tooltip("Onde o pé fica parado. Fora da tela, abaixo do pedal.")]
    public Vector3 posicaoPeDescanso;
    [Tooltip("Onde o pé fica quando está pisando.")]
    public Vector3 posicaoPePisando;
    public float duracaoDescida = 0.15f;
    public float duracaoPisada = 0.12f;
    public float duracaoSubida = 0.2f;
    [Tooltip("Altura do pedal enquanto está pisado, em relação à altura normal.")]
    public float achatamento = 0.7f;
    [Tooltip("Opcional. Sprite do pedal afundado, para quando a arte chegar.")]
    public Sprite spritePisado;

    public bool Pisando { get; private set; }

    private Vector3 escalaNormal;
    private SpriteRenderer visual;
    private Sprite spriteNormal;

    private bool PodeAcionar
    {
        get
        {
            EtapaHigiene etapa = sistemaHigiene.EtapaAtual;
            return etapa == EtapaHigiene.LigarAgua || etapa == EtapaHigiene.Lavar || etapa == EtapaHigiene.IniciarEnxague;
        }
    }

    private void Start()
    {
        escalaNormal = transform.localScale;
        visual = GetComponent<SpriteRenderer>();
        spriteNormal = visual != null ? visual.sprite : null;

        if (pe == null)
        {
            Debug.LogError("[PedalAgua] O campo 'pe' não foi preenchido no Inspector.", this);
            return;
        }

        pe.position = posicaoPeDescanso;
    }

    private void OnMouseDown()
    {
        Acionar();
    }

    public void Acionar()
    {
        // Ignorar cliques durante a pisada evita ligar e desligar a água duas vezes seguidas.
        if (Pisando || !PodeAcionar) return;

        StartCoroutine(Pisar());
    }

    private IEnumerator Pisar()
    {
        Pisando = true;

        yield return MoverPe(posicaoPeDescanso, posicaoPePisando, duracaoDescida);

        AfundarPedal(true);
        SoundManager.Tocar(Som.InteracaoItem);
        AplicarEfeito();

        yield return new WaitForSeconds(duracaoPisada);

        AfundarPedal(false);

        yield return MoverPe(posicaoPePisando, posicaoPeDescanso, duracaoSubida);

        Pisando = false;
    }

    private IEnumerator MoverPe(Vector3 de, Vector3 para, float duracao)
    {
        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.deltaTime;
            pe.position = Vector3.Lerp(de, para, Mathf.SmoothStep(0f, 1f, tempo / duracao));
            yield return null;
        }

        pe.position = para;
    }

    private void AfundarPedal(bool afundado)
    {
        Vector3 escala = escalaNormal;
        if (afundado) escala.y *= achatamento;
        transform.localScale = escala;

        if (visual != null && spritePisado != null)
        {
            visual.sprite = afundado ? spritePisado : spriteNormal;
        }
    }

    private void AplicarEfeito()
    {
        switch (sistemaHigiene.EtapaAtual)
        {
            case EtapaHigiene.LigarAgua:
            case EtapaHigiene.Lavar:
                sistemaHigiene.AlternarAgua();
                break;
            case EtapaHigiene.IniciarEnxague:
                etapaEnxague.IniciarEnxague();
                break;
        }
    }
}
