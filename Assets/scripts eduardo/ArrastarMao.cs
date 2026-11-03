using UnityEngine;

// Fica em cada mão. Cada uma é arrastada e identificada de forma independente.
public class ArrastarMao : MonoBehaviour
{
    public SistemaHigiene sistemaHigiene;

    [Tooltip("Onde esta mão fica em relação ao centro da pia e da área de secagem. Esquerda: x negativo. Direita: x positivo.")]
    public Vector2 deslocamentoNoEncaixe = new Vector2(-1f, 0f);

    [Header("Aparência")]
    public SpriteRenderer visual;
    [Tooltip("Vazios enquanto a arte não chega. Nesse caso a mão só muda de cor.")]
    public Sprite spriteAberta;
    [Tooltip("Mão de lado, enquanto esfrega.")]
    public Sprite spriteEsfregando;
    public Color corAberta = Color.white;
    public Color corEsfregando = Color.cyan;

    [Tooltip("Desenhos da espuma por cima da mão. Eles aparecem e somem pela transparência.")]
    public SpriteRenderer[] camadasEspuma;
    [Tooltip("Gotas por cima da mão. Aparecem no enxágue e somem quando o papel passa.")]
    public SpriteRenderer[] camadasMolhada;

    public bool Encaixada { get; private set; }
    public float Umidade { get; private set; }
    public Bounds Limites => colisor.bounds;

    private Collider2D colisor;
    private Camera cam;
    private Vector3 distanciaDaPegada;
    private Vector3 posicaoDeEncaixe;
    private bool segurando = false;

    private bool PodeArrastar
    {
        get
        {
            if (Encaixada) return false;

            EtapaHigiene etapa = sistemaHigiene.EtapaAtual;
            return etapa == EtapaHigiene.ColocarMaosNaPia || etapa == EtapaHigiene.LevarMaosParaSecagem;
        }
    }

    private void Awake()
    {
        colisor = GetComponent<Collider2D>();
    }

    private void Start()
    {
        cam = Camera.main;
        DefinirEsfregando(false);
        DefinirEspuma(0f);
        DefinirUmidade(0f);
    }

    private void OnMouseDown()
    {
        if (!PodeArrastar) return;

        segurando = true;
        distanciaDaPegada = transform.position - MouseNoMundo();
        SoundManager.Tocar(Som.InteracaoItem);
    }

    private void OnMouseDrag()
    {
        if (segurando && PodeArrastar)
        {
            MoverPara(MouseNoMundo() + distanciaDaPegada);
        }
    }

    private void OnMouseUp()
    {
        if (segurando && !Encaixada)
        {
            SoundManager.Tocar(Som.SoltarItem);
        }

        segurando = false;
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

    private void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.CompareTag("AreaMaos") && sistemaHigiene.EtapaAtual == EtapaHigiene.ColocarMaosNaPia && !Encaixada)
        {
            Encaixar(outro.transform.position);
            sistemaHigiene.MaoChegouNaPia(this);
        }
    }

    public void Encaixar(Vector3 centroDaArea)
    {
        Encaixada = true;
        segurando = false;
        posicaoDeEncaixe = centroDaArea + (Vector3)deslocamentoNoEncaixe;
        MoverPara(posicaoDeEncaixe);
    }

    public void LiberarParaSecagem()
    {
        Encaixada = false;
    }

    // Usado pela animação. A mão só se mexe em volta do encaixe enquanto está presa nele.
    public void DefinirDeslocamento(Vector2 deslocamento)
    {
        if (Encaixada)
        {
            MoverPara(posicaoDeEncaixe + (Vector3)deslocamento);
        }
    }

    public void DefinirEsfregando(bool esfregando)
    {
        if (visual == null) return;

        Sprite sprite = esfregando ? spriteEsfregando : spriteAberta;

        if (sprite != null)
        {
            visual.sprite = sprite;
            visual.color = Color.white;
        }
        else
        {
            visual.color = esfregando ? corEsfregando : corAberta;
        }

        // A arte da espuma é da mão aberta, então ela some enquanto a mão está de lado.
        if (camadasEspuma != null)
        {
            foreach (SpriteRenderer camada in camadasEspuma)
            {
                camada.enabled = !esfregando;
            }
        }
    }

    // 0 = mão limpa, 1 = mão cheia de espuma.
    public void DefinirEspuma(float quantidade)
    {
        DefinirTransparencia(camadasEspuma, quantidade);
    }

    // 0 = mão seca, 1 = mão molhada.
    public void DefinirUmidade(float quantidade)
    {
        Umidade = Mathf.Clamp01(quantidade);
        DefinirTransparencia(camadasMolhada, Umidade);
    }

    private void DefinirTransparencia(SpriteRenderer[] camadas, float transparencia)
    {
        if (camadas == null) return;

        foreach (SpriteRenderer camada in camadas)
        {
            Color cor = camada.color;
            cor.a = transparencia;
            camada.color = cor;
        }
    }
}
