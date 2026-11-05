using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum EtapaHigiene
{
    ColocarMaosNaPia,
    LigarAgua,
    Lavar,
    IniciarEnxague,
    Enxaguar,
    LevarMaosParaSecagem,
    SecarMaos,
    DescartarPapel,
    Concluida
}

// Controla a ordem da fase. Os outros scripts sempre consultam EtapaAtual antes de agir,
// por isso nenhuma etapa pode acontecer antes da anterior.
public class SistemaHigiene : MonoBehaviour
{
    [Header("Lavagem")]
    public Slider barraHigiene;
    public GameObject agua;
    public float intervalo = 0.08f;
    [Tooltip("Quanto a barra sobe a cada toque em ESPAÇO. A barra vai de 0 a 100.")]
    public float ganhoPorToque = 4f;
    public Color corAguaComSabao = new Color(0.95f, 0.94f, 0.9f, 0.95f);
    public Color corAguaLimpa = new Color(0.25f, 0.62f, 0.95f, 0.85f);
    public AnimacaoTorneira animacaoTorneira;

    [Header("Outras etapas")]
    public EtapaEnxague etapaEnxague;
    public AreaSecagemMaos areaSecagem;
    public SecarMaos secarMaos;
    public ArrastarMao[] maos;
    public AnimacaoMaos animacaoMaos;
    [Tooltip("Borda da área de secagem. Só aparece depois do enxágue.")]
    public GameObject bordaAreaSecagem;

    [Header("Legendas e seta")]
    public LegendaHigiene legendaHigiene;
    public SetaIndicadora seta;
    public Transform alvoPia;
    public Transform alvoPedal;
    public Transform alvoSecagem;
    public Transform alvoLixeira;

    [Header("Fim da fase")]
    public float tempoAntesDeVoltar = 2f;
    [Tooltip("Cena aberta no fim quando a fase foi iniciada direto, sem passar por um InteracaoMinigame.")]
    public string cenaFabrica = "JogoPrincipal";

    public EtapaHigiene EtapaAtual { get; private set; }
    public bool AguaLigada { get; private set; }

    private readonly HashSet<ArrastarMao> maosNaPia = new HashSet<ArrastarMao>();
    private float contador = 0f;

    private void Start()
    {
        ValidarReferencias();

        barraHigiene.value = 0f;
        barraHigiene.gameObject.SetActive(true);
        DefinirAgua(false);

        IrParaEtapa(EtapaHigiene.ColocarMaosNaPia);
    }

    private void ValidarReferencias()
    {
        Exigir(barraHigiene, nameof(barraHigiene));
        Exigir(agua, nameof(agua));
        Exigir(etapaEnxague, nameof(etapaEnxague));
        Exigir(areaSecagem, nameof(areaSecagem));
        Exigir(secarMaos, nameof(secarMaos));
        Exigir(animacaoMaos, nameof(animacaoMaos));
        Exigir(animacaoTorneira, nameof(animacaoTorneira));
        Exigir(bordaAreaSecagem, nameof(bordaAreaSecagem));
        Exigir(legendaHigiene, nameof(legendaHigiene));
        Exigir(seta, nameof(seta));
        Exigir(alvoPia, nameof(alvoPia));
        Exigir(alvoPedal, nameof(alvoPedal));
        Exigir(alvoSecagem, nameof(alvoSecagem));
        Exigir(alvoLixeira, nameof(alvoLixeira));

        if (maos == null || maos.Length != 2)
        {
            Debug.LogError("[SistemaHigiene] O campo 'maos' precisa ter as duas mãos.", this);
        }
    }

    private void Exigir(Object referencia, string campo)
    {
        if (referencia == null)
        {
            Debug.LogError($"[SistemaHigiene] O campo '{campo}' não foi preenchido no Inspector.", this);
        }
    }

    private void Update()
    {
        if (contador > 0f)
        {
            contador -= Time.deltaTime;
        }

        if (EtapaAtual != EtapaHigiene.Lavar || !AguaLigada || contador > 0f)
        {
            return;
        }

        // wasPressedThisFrame só é verdadeiro no frame do toque: segurar a tecla não conta.
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            contador = intervalo;
            barraHigiene.value += ganhoPorToque;
            SoundManager.Tocar(Som.Bolhas);
            animacaoMaos.AoToqueLavagem();

            if (barraHigiene.value >= 100f)
            {
                ConcluirLavagem();
            }
        }
    }

    private void IrParaEtapa(EtapaHigiene etapa)
    {
        EtapaAtual = etapa;

        bordaAreaSecagem.SetActive(etapa == EtapaHigiene.LevarMaosParaSecagem || etapa == EtapaHigiene.SecarMaos);

        switch (etapa)
        {
            case EtapaHigiene.ColocarMaosNaPia:
                Orientar("Coloque as duas mãos na pia.", alvoPia);
                break;
            case EtapaHigiene.LigarAgua:
                Orientar("Pise no pedal para abrir a torneira.", alvoPedal);
                break;
            case EtapaHigiene.Lavar:
                Orientar("Pressione ESPAÇO repetidamente para ensaboar e lavar as mãos.", null);
                break;
            case EtapaHigiene.IniciarEnxague:
                Orientar("Pise no pedal para iniciar o enxágue.", alvoPedal);
                break;
            case EtapaHigiene.Enxaguar:
                Orientar("Pressione ESPAÇO repetidamente para enxaguar as mãos.", null);
                break;
            case EtapaHigiene.LevarMaosParaSecagem:
                Orientar("Leve as duas mãos para a área de secagem.", alvoSecagem);
                break;
            case EtapaHigiene.SecarMaos:
                // Nesta etapa quem move a seta é o SecarMaos, porque ela depende de onde está o papel.
                legendaHigiene.MudarLegenda("Passe o papel-toalha nas duas mãos para secá-las.");
                break;
            case EtapaHigiene.DescartarPapel:
                Orientar("Leve o papel usado até a lixeira.", alvoLixeira);
                break;
            case EtapaHigiene.Concluida:
                Orientar("Higienização concluída!", null);
                break;
        }
    }

    private void Orientar(string mensagem, Transform alvoDaSeta)
    {
        legendaHigiene.MudarLegenda(mensagem);
        seta.ApontarPara(alvoDaSeta);
    }

    public void DefinirAgua(bool ligada, bool comSabao = false)
    {
        AguaLigada = ligada;
        agua.SetActive(ligada);

        SpriteRenderer visualAgua = agua.GetComponent<SpriteRenderer>();
        if (visualAgua != null)
        {
            visualAgua.color = comSabao ? corAguaComSabao : corAguaLimpa;
        }

        if (ligada)
        {
            animacaoTorneira.Ligar(comSabao);
        }
        else
        {
            animacaoTorneira.Desligar();
        }
    }

    // =========================================================
    // CHAMADAS DOS OUTROS SCRIPTS, NA ORDEM DA FASE
    // =========================================================

    public void MaoChegouNaPia(ArrastarMao mao)
    {
        if (EtapaAtual != EtapaHigiene.ColocarMaosNaPia || !maosNaPia.Add(mao))
        {
            return;
        }

        SoundManager.Tocar(Som.EncaixeBancada);

        if (maosNaPia.Count >= 2)
        {
            IrParaEtapa(EtapaHigiene.LigarAgua);
        }
    }

    public void AlternarAgua()
    {
        if (EtapaAtual != EtapaHigiene.LigarAgua && EtapaAtual != EtapaHigiene.Lavar)
        {
            return;
        }

        DefinirAgua(!AguaLigada, true);
        IrParaEtapa(AguaLigada ? EtapaHigiene.Lavar : EtapaHigiene.LigarAgua);
    }

    private void ConcluirLavagem()
    {
        barraHigiene.value = 100f;
        barraHigiene.gameObject.SetActive(false);
        DefinirAgua(false);
        SoundManager.Tocar(Som.Acerto);

        etapaEnxague.PrepararBarra();
        IrParaEtapa(EtapaHigiene.IniciarEnxague);
    }

    public void EnxagueIniciado()
    {
        DefinirAgua(true);
        IrParaEtapa(EtapaHigiene.Enxaguar);
    }

    public void EnxagueConcluido()
    {
        DefinirAgua(false);

        foreach (ArrastarMao mao in maos)
        {
            mao.DefinirUmidade(1f);
            mao.LiberarParaSecagem();
        }

        IrParaEtapa(EtapaHigiene.LevarMaosParaSecagem);
        areaSecagem.VerificarMaos();
    }

    public void MaosChegaramNaSecagem()
    {
        if (EtapaAtual != EtapaHigiene.LevarMaosParaSecagem)
        {
            return;
        }

        foreach (ArrastarMao mao in maos)
        {
            mao.Encaixar(areaSecagem.transform.position);
        }

        SoundManager.Tocar(Som.EncaixeBancada);
        secarMaos.IniciarSecagem();
        IrParaEtapa(EtapaHigiene.SecarMaos);
    }

    public void SecagemConcluida()
    {
        SoundManager.Tocar(Som.Acerto);
        IrParaEtapa(EtapaHigiene.DescartarPapel);
    }

    public void PapelDescartado()
    {
        SoundManager.Tocar(Som.Acerto);
        IrParaEtapa(EtapaHigiene.Concluida);
        StartCoroutine(VoltarParaFabrica());
    }

    private IEnumerator VoltarParaFabrica()
    {
        yield return new WaitForSeconds(tempoAntesDeVoltar);

        // Informa o InteracaoMinigame para concluir o objetivo e retornar à cena principal
        InteracaoMinigame.FinalizarMinigame(true);
    }
}