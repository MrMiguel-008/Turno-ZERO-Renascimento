using UnityEngine;

// Anima as mãos por código, mexendo na posição delas e na transparência da espuma.
// Como não depende de nenhum desenho, os sprites podem ser trocados sem refazer a animação.
public class AnimacaoMaos : MonoBehaviour
{
    public SistemaHigiene sistemaHigiene;
    public ArrastarMao maoEsquerda;
    public ArrastarMao maoDireita;
    public BolhasDeSabao bolhas;
    [Tooltip("Ponto em que a água encontra as mãos. Os respingos saem daqui.")]
    public Transform pontoDaAgua;

    [Header("Esfregar")]
    [Tooltip("Quanto cada mão anda em direção à outra enquanto esfrega.")]
    public float aproximacao = 0.55f;
    public float amplitudeEsfregar = 0.32f;
    public float velocidadeEsfregar = 16f;
    [Tooltip("Segundos que as mãos continuam esfregando depois de cada toque em ESPAÇO.")]
    public float tempoEsfregandoPorToque = 0.35f;
    public int bolhasPorToque = 4;
    public Color corBolha = new Color(0.92f, 0.97f, 1f, 0.85f);

    [Header("Enxaguar")]
    public float aproximacaoEnxague = 0.35f;
    [Tooltip("As mãos vão de um lado para o outro para a água passar pelas duas.")]
    public float amplitudeEnxague = 0.85f;
    public float amplitudeVerticalEnxague = 0.3f;
    public float velocidadeEnxague = 3f;
    public float intervaloRespingos = 0.1f;
    public Color corRespingo = new Color(0.55f, 0.82f, 1f, 0.9f);

    [Header("Mãos molhadas")]
    [Tooltip("Segundos entre os pingos que caem das mãos molhadas.")]
    public float intervaloPingos = 0.45f;

    [Header("Espuma")]
    [Tooltip("Velocidade da transição da espuma, em porcentagem da mão por segundo.")]
    public float velocidadeEspuma = 1.5f;

    private float ultimoToque = -10f;
    private float intensidadeEsfregar = 0f;
    private float intensidadeEnxague = 0f;
    private float tempoParaRespingo = 0f;
    private float tempoParaPingo = 0f;
    private float espuma = 0f;

    public void AoToqueLavagem()
    {
        ultimoToque = Time.time;

        Vector3 meioDasMaos = (maoEsquerda.transform.position + maoDireita.transform.position) / 2f;
        bolhas.Soltar(meioDasMaos, bolhasPorToque, 1.1f, corBolha, 0.3f, 1.6f, 0.9f);
    }

    private void Update()
    {
        AtualizarEsfregar();
        AtualizarEnxague();
        AtualizarEspuma();
        AtualizarPingos();
    }

    private void AtualizarEsfregar()
    {
        bool esfregando = sistemaHigiene.EtapaAtual == EtapaHigiene.Lavar && Time.time - ultimoToque < tempoEsfregandoPorToque;
        float anterior = intensidadeEsfregar;
        intensidadeEsfregar = Mathf.MoveTowards(intensidadeEsfregar, esfregando ? 1f : 0f, Time.deltaTime / 0.12f);

        if (intensidadeEsfregar <= 0f && anterior <= 0f) return;

        float vaiEVem = Mathf.Sin(Time.time * velocidadeEsfregar) * amplitudeEsfregar * intensidadeEsfregar;
        float aproximar = aproximacao * intensidadeEsfregar;

        maoEsquerda.DefinirDeslocamento(new Vector2(aproximar, vaiEVem));
        maoDireita.DefinirDeslocamento(new Vector2(-aproximar, -vaiEVem));

        maoEsquerda.DefinirEsfregando(intensidadeEsfregar > 0f);
        maoDireita.DefinirEsfregando(intensidadeEsfregar > 0f);
    }

    private void AtualizarEnxague()
    {
        if (sistemaHigiene.EtapaAtual != EtapaHigiene.Enxaguar)
        {
            intensidadeEnxague = 0f;
            return;
        }

        intensidadeEnxague = Mathf.MoveTowards(intensidadeEnxague, 1f, Time.deltaTime / 0.4f);

        float paraOsLados = Mathf.Sin(Time.time * velocidadeEnxague) * amplitudeEnxague * intensidadeEnxague;
        float paraCimaEBaixo = Mathf.Sin(Time.time * velocidadeEnxague * 1.7f) * amplitudeVerticalEnxague * intensidadeEnxague;
        float aproximar = aproximacaoEnxague * intensidadeEnxague;

        maoEsquerda.DefinirDeslocamento(new Vector2(aproximar + paraOsLados, paraCimaEBaixo));
        maoDireita.DefinirDeslocamento(new Vector2(-aproximar + paraOsLados, paraCimaEBaixo));

        // As mãos vão ficando molhadas conforme a água passa. Nunca secam durante o enxágue.
        float molhado = sistemaHigiene.etapaEnxague.barraEnxague.value / 100f;
        maoEsquerda.DefinirUmidade(Mathf.Max(maoEsquerda.Umidade, molhado));
        maoDireita.DefinirUmidade(Mathf.Max(maoDireita.Umidade, molhado));

        tempoParaRespingo -= Time.deltaTime;
        if (tempoParaRespingo <= 0f)
        {
            tempoParaRespingo = intervaloRespingos;
            bolhas.Soltar(pontoDaAgua.position, 2, 0.35f, corRespingo, 0.14f, -0.7f, 0.35f);
        }
    }

    // Depois do enxágue as mãos continuam pingando até serem secas, cada uma na sua proporção.
    private void AtualizarPingos()
    {
        EtapaHigiene etapa = sistemaHigiene.EtapaAtual;
        if (etapa != EtapaHigiene.LevarMaosParaSecagem && etapa != EtapaHigiene.SecarMaos)
        {
            return;
        }

        tempoParaPingo -= Time.deltaTime;
        if (tempoParaPingo > 0f) return;

        tempoParaPingo = intervaloPingos;
        Pingar(maoEsquerda);
        Pingar(maoDireita);
    }

    private void Pingar(ArrastarMao mao)
    {
        if (Random.value < mao.Umidade)
        {
            Vector3 baseDaMao = new Vector3(mao.Limites.center.x, mao.Limites.min.y, 0f);
            bolhas.Soltar(baseDaMao, 1, 0.3f, corRespingo, 0.12f, -0.9f, 0.45f);
        }
    }

    // A espuma acompanha as barras: aparece enquanto lava e vai embora enquanto enxágua.
    private void AtualizarEspuma()
    {
        float alvo = 0f;

        switch (sistemaHigiene.EtapaAtual)
        {
            case EtapaHigiene.Lavar:
            case EtapaHigiene.LigarAgua:
                alvo = sistemaHigiene.barraHigiene.value / 100f;
                break;
            case EtapaHigiene.IniciarEnxague:
                alvo = 1f;
                break;
            case EtapaHigiene.Enxaguar:
                alvo = 1f - sistemaHigiene.etapaEnxague.barraEnxague.value / 100f;
                break;
        }

        if (Mathf.Approximately(espuma, alvo)) return;

        espuma = Mathf.MoveTowards(espuma, alvo, Time.deltaTime * velocidadeEspuma);
        maoEsquerda.DefinirEspuma(espuma);
        maoDireita.DefinirEspuma(espuma);
    }
}
