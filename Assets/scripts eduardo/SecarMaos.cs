using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Fica no papel-toalha, junto com o ArrastarPapel. Cada mão seca só quando o papel passa por cima dela.
public class SecarMaos : MonoBehaviour
{
    public Slider barraSecagem;
    public SistemaHigiene sistemaHigiene;
    public ArrastarPapel arrastarPapel;
    [Tooltip("Ponto em cima do papel para onde a seta aponta.")]
    public Transform alvoDoPapel;

    [Tooltip("Quanto a barra sobe por segundo enquanto o papel passa em uma mão.")]
    public float velocidadeSecagem = 10f;
    [Tooltip("Velocidade mínima do papel para contar como passar na mão. Papel parado não seca.")]
    public float velocidadeMinimaDoPapel = 0.5f;

    [Header("Saída do suporte")]
    [Tooltip("Onde o papel fica no começo: escondido atrás do suporte.")]
    public Vector3 posicaoNoSuporte;
    [Tooltip("Onde o papel para depois de sair, pronto para ser pego.")]
    public Vector3 posicaoForaDoSuporte;
    public float duracaoSaida = 0.5f;

    [Header("Papel amassado")]
    public Sprite spriteAmassado;
    [Tooltip("Tamanho do papel amassado em relação ao papel aberto.")]
    public float tamanhoAmassado = 0.7f;
    public float duracaoAmassar = 0.45f;

    public bool PapelDisponivel { get; private set; }

    private Collider2D colisor;
    private Vector3 posicaoAnterior;
    private bool papelNaArea = false;

    private void Awake()
    {
        colisor = GetComponent<Collider2D>();
    }

    private void Start()
    {
        transform.position = posicaoNoSuporte;
        posicaoAnterior = transform.position;

        if (barraSecagem == null)
        {
            Debug.LogError("[SecarMaos] O campo 'barraSecagem' não foi preenchido no Inspector.", this);
            return;
        }

        barraSecagem.value = 0f;
        barraSecagem.gameObject.SetActive(false);
    }

    public void IniciarSecagem()
    {
        barraSecagem.value = 0f;
        barraSecagem.gameObject.SetActive(true);
        StartCoroutine(SairDoSuporte());
    }

    // O papel desce de dentro do suporte. Só pode ser pego depois de sair por inteiro.
    private IEnumerator SairDoSuporte()
    {
        Vector3 inicio = posicaoNoSuporte;
        float tempo = 0f;

        while (tempo < duracaoSaida)
        {
            tempo += Time.deltaTime;
            transform.position = Vector3.Lerp(inicio, posicaoForaDoSuporte, Mathf.SmoothStep(0f, 1f, tempo / duracaoSaida));
            yield return null;
        }

        transform.position = posicaoForaDoSuporte;
        PapelDisponivel = true;
    }

    private void Update()
    {
        float velocidade = Time.deltaTime > 0f ? (transform.position - posicaoAnterior).magnitude / Time.deltaTime : 0f;
        posicaoAnterior = transform.position;

        if (sistemaHigiene.EtapaAtual != EtapaHigiene.SecarMaos)
        {
            return;
        }

        AtualizarSeta();

        if (arrastarPapel.Arrastando && velocidade >= velocidadeMinimaDoPapel)
        {
            SecarMaosEmbaixoDoPapel();
        }
    }

    private void SecarMaosEmbaixoDoPapel()
    {
        ArrastarMao[] maos = sistemaHigiene.maos;

        // A barra soma as duas mãos, então cada mão precisa secar no dobro da velocidade da barra.
        float secagem = velocidadeSecagem * 2f / 100f * Time.deltaTime;
        float umidadeTotal = 0f;

        foreach (ArrastarMao mao in maos)
        {
            if (EstaEmCimaDa(mao))
            {
                mao.DefinirUmidade(mao.Umidade - secagem);
            }

            umidadeTotal += mao.Umidade;
        }

        barraSecagem.value = (1f - umidadeTotal / maos.Length) * 100f;

        if (umidadeTotal <= 0f)
        {
            barraSecagem.value = 100f;
            barraSecagem.gameObject.SetActive(false);
            StartCoroutine(Amassar());
            sistemaHigiene.SecagemConcluida();
        }
    }

    // Compara só X e Y. O Bounds.Intersects também compara o Z, e o papel fica em outra profundidade
    // de propósito, para receber o clique antes das mãos.
    private bool EstaEmCimaDa(ArrastarMao mao)
    {
        Bounds papel = colisor.bounds;
        Bounds limitesMao = mao.Limites;

        return papel.min.x <= limitesMao.max.x && papel.max.x >= limitesMao.min.x
            && papel.min.y <= limitesMao.max.y && papel.max.y >= limitesMao.min.y;
    }

    // O papel encolhe apertando e torcendo, até virar uma bolinha.
    private IEnumerator Amassar()
    {
        SpriteRenderer visual = GetComponent<SpriteRenderer>();
        Vector3 escalaInicial = transform.localScale;
        Vector3 escalaFinal = escalaInicial * tamanhoAmassado;
        bool trocouVisual = false;
        float tempo = 0f;

        while (tempo < duracaoAmassar)
        {
            tempo += Time.deltaTime;
            float progresso = Mathf.Clamp01(tempo / duracaoAmassar);

            float aperto = Mathf.Sin(progresso * Mathf.PI * 3f) * (1f - progresso) * 0.2f;
            Vector3 escala = Vector3.Lerp(escalaInicial, escalaFinal, Mathf.SmoothStep(0f, 1f, progresso));
            transform.localScale = new Vector3(escala.x * (1f + aperto), escala.y * (1f - aperto), escala.z);
            transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Sin(progresso * Mathf.PI * 4f) * 25f * (1f - progresso));

            if (!trocouVisual && progresso >= 0.5f)
            {
                trocouVisual = true;
                if (spriteAmassado != null) visual.sprite = spriteAmassado;
            }

            yield return null;
        }

        transform.localScale = escalaFinal;
        transform.rotation = Quaternion.identity;
    }

    // Primeiro a seta mostra o papel. Com o papel na mão, ela mostra para onde levá-lo.
    private void AtualizarSeta()
    {
        if (papelNaArea)
        {
            sistemaHigiene.seta.ApontarPara(null);
        }
        else if (arrastarPapel.Arrastando)
        {
            sistemaHigiene.seta.ApontarPara(sistemaHigiene.alvoSecagem);
        }
        else
        {
            sistemaHigiene.seta.ApontarPara(alvoDoPapel);
        }
    }

    private void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.CompareTag("AreaSecagem"))
        {
            papelNaArea = true;
        }

        // O papel só pode ir para o lixo depois de terminar a secagem.
        if (outro.CompareTag("AreaLixo") && sistemaHigiene.EtapaAtual == EtapaHigiene.DescartarPapel)
        {
            SoundManager.Tocar(Som.Lixo);
            gameObject.SetActive(false);
            sistemaHigiene.PapelDescartado();
        }
    }

    private void OnTriggerExit2D(Collider2D outro)
    {
        if (outro.CompareTag("AreaSecagem"))
        {
            papelNaArea = false;
        }
    }
}
