using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TapeteSanitizanteMinigame : MonoBehaviour, IObjetivoMinigame
{
    private static float pontuacaoTapete = 150f;
    private const float PONTUACAO_MAXIMA = 150f;
    private const float PENALIZACAO_ERRO = 20f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetarPontuacaoTapete()
    {
        pontuacaoTapete = 150f;
    }

    [Header("UI")]
    [SerializeField] private TMP_Text teclaAtual;
    [SerializeField] private Slider barraProgresso;

    [Header("Bota")]
    [SerializeField] private RectTransform bota;
    [SerializeField] private Vector2 posicaoA = new Vector2(-60f, 0f);
    [SerializeField] private Vector2 posicaoD = new Vector2(60f, 0f);
    [SerializeField] private float duracaoMovimentoPe = 0.12f;

    [Header("Bolhas")]
    [SerializeField] private RectTransform bolhasContainer;
    [SerializeField] private GameObject bolhaPrefab;
    [SerializeField] private int quantidadeBolhas = 4;
    [SerializeField] private float espalhamentoHorizontal = 35f;
    [SerializeField] private float espalhamentoVertical = 10f;
    [SerializeField] private float duracaoBolha = 0.7f;
    [SerializeField] private float alturaBolha = 60f;

    [Header("Minigame")]
    [SerializeField] private int quantidadeAcertosNecessarios = 10;
    [SerializeField] private float tempoAntesDeVoltar = 0.8f;

    [Header("Feedback de erro")]
    [SerializeField] private float intensidadeSacudida = 8f;
    [SerializeField] private float duracaoSacudida = 0.12f;

    private bool esperandoA = true;
    private int acertos = 0;
    private bool minigameConcluido = false;
    private Vector2 posicaoInicialBota;
    private Coroutine movimentoBotaAtual;

    public bool Concluido { get; private set; }

    private void Start()
    {
        pontuacaoTapete = 150f;
        posicaoInicialBota = bota.anchoredPosition;
        ConfigurarBarra();
        AtualizarTeclaNaTela();
    }

    private void Update()
    {
        if (minigameConcluido)
            return;

        VerificarTeclas();
    }

    private void ConfigurarBarra()
    {
        barraProgresso.minValue = 0;
        barraProgresso.maxValue = quantidadeAcertosNecessarios;
        barraProgresso.value = 0;
    }

    private void VerificarTeclas()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            VerificarTeclaPressionada(KeyCode.A);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            VerificarTeclaPressionada(KeyCode.D);
        }
    }

    private void VerificarTeclaPressionada(KeyCode teclaPressionada)
    {
        KeyCode teclaEsperada = esperandoA ? KeyCode.A : KeyCode.D;

        if (teclaPressionada == teclaEsperada)
        {
            Acertou();
        }
        else
        {
            Errou();
        }
    }

    private void Acertou()
    {
        acertos++;
        barraProgresso.value = acertos;

        MovimentarBota();
        CriarBolhas();

        if (acertos >= quantidadeAcertosNecessarios)
        {
            ConcluirMinigame();
            return;
        }

        esperandoA = !esperandoA;
        AtualizarTeclaNaTela();
    }

    private void Errou()
    {
        pontuacaoTapete = Mathf.Max(0f, pontuacaoTapete - PENALIZACAO_ERRO);
        Debug.LogWarning($"Tecla errada! -20 pontos. Pontuação atual: {pontuacaoTapete}/{PONTUACAO_MAXIMA}");
        StartCoroutine(SacudirTecla());
    }

    private void AtualizarTeclaNaTela()
    {
        teclaAtual.text = esperandoA ? "A" : "D";
    }

    private void MovimentarBota()
    {
        Vector2 destino = esperandoA ? (posicaoInicialBota + posicaoA) : (posicaoInicialBota + posicaoD);

        if (movimentoBotaAtual != null)
        {
            StopCoroutine(movimentoBotaAtual);
        }

        movimentoBotaAtual = StartCoroutine(MoverBota(destino));
    }

    private IEnumerator MoverBota(Vector2 destino)
    {
        Vector2 inicio = bota.anchoredPosition;
        float tempo = 0f;
        Vector3 escalaOriginal = bota.localScale;
        Vector3 escalaComprimida = new Vector3(escalaOriginal.x * 0.95f, escalaOriginal.y * 0.90f, escalaOriginal.z);

        while (tempo < duracaoMovimentoPe)
        {
            tempo += Time.deltaTime;
            float progresso = tempo / duracaoMovimentoPe;
            float suavizado = Mathf.SmoothStep(0f, 1f, progresso);

            bota.anchoredPosition = Vector2.Lerp(inicio, destino, suavizado);

            if (progresso < 0.5f)
            {
                bota.localScale = Vector3.Lerp(escalaOriginal, escalaComprimida, progresso * 2f);
            }
            else
            {
                bota.localScale = Vector3.Lerp(escalaComprimida, escalaOriginal, (progresso - 0.5f) * 2f);
            }

            yield return null;
        }

        bota.anchoredPosition = destino;
        bota.localScale = escalaOriginal;
        movimentoBotaAtual = null;
    }

    private void CriarBolhas()
    {
        if (bolhaPrefab == null || bolhasContainer == null) return;

        for (int i = 0; i < quantidadeBolhas; i++)
        {
            GameObject novaBolha = Instantiate(bolhaPrefab, bolhasContainer);
            RectTransform rectBolha = novaBolha.GetComponent<RectTransform>();

            if (rectBolha == null)
            {
                Destroy(novaBolha);
                continue;
            }

            Vector2 posicaoBolha = bota.anchoredPosition;
            posicaoBolha.x += Random.Range(-espalhamentoHorizontal, espalhamentoHorizontal);
            posicaoBolha.y += Random.Range(-espalhamentoVertical, espalhamentoVertical);
            rectBolha.anchoredPosition = posicaoBolha;

            StartCoroutine(AnimarBolha(novaBolha, rectBolha));
        }
    }

    private IEnumerator AnimarBolha(GameObject bolha, RectTransform rectBolha)
    {
        CanvasGroup canvasGroup = bolha.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = bolha.AddComponent<CanvasGroup>();

        Vector2 posicaoInicial = rectBolha.anchoredPosition;
        Vector2 posicaoFinal = posicaoInicial + Vector2.up * alturaBolha;
        float tempo = 0f;

        while (tempo < duracaoBolha)
        {
            tempo += Time.deltaTime;
            float progresso = tempo / duracaoBolha;
            float suavizado = Mathf.SmoothStep(0f, 1f, progresso);

            rectBolha.anchoredPosition = Vector2.Lerp(posicaoInicial, posicaoFinal, suavizado);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, progresso);
            rectBolha.localScale = Vector3.one * Mathf.Lerp(0.7f, 1f, progresso);

            yield return null;
        }

        Destroy(bolha);
    }

    private IEnumerator SacudirTecla()
    {
        Vector3 posicaoOriginal = teclaAtual.transform.localPosition;
        float tempo = 0f;

        while (tempo < duracaoSacudida)
        {
            tempo += Time.deltaTime;
            float deslocamento = Random.Range(-intensidadeSacudida, intensidadeSacudida);
            teclaAtual.transform.localPosition = posicaoOriginal + new Vector3(deslocamento, 0f, 0f);
            yield return null;
        }

        teclaAtual.transform.localPosition = posicaoOriginal;
    }

    private void ConcluirMinigame()
    {
        minigameConcluido = true;
        Concluido = true;

        Debug.Log($"TAPETE CONCLUÍDO! Pontuação final obtida: {pontuacaoTapete}");
        GameScoreManager.AdicionarPontosMinigame(SceneManager.GetActiveScene().name, pontuacaoTapete);

        StartCoroutine(FinalizarComAtraso());
    }

    private IEnumerator FinalizarComAtraso()
    {
        yield return new WaitForSeconds(tempoAntesDeVoltar);
        InteracaoMinigame.FinalizarMinigame(true);
    }
}