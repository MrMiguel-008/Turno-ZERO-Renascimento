using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TapeteSanitizanteMinigame : MonoBehaviour, IObjetivoMinigame
{
    // =========================================================
    // UI
    // =========================================================

    [Header("UI")]
    [SerializeField] private TMP_Text teclaAtual;
    [SerializeField] private Slider barraProgresso;

    // =========================================================
    // BOTA
    // =========================================================

    [Header("Bota")]
    [SerializeField] private RectTransform bota;

    [Tooltip("Deslocamento da bota quando a tecla A for pressionada.")]
    [SerializeField] private Vector2 posicaoA = new Vector2(-60f, 0f);

    [Tooltip("Deslocamento da bota quando a tecla D for pressionada.")]
    [SerializeField] private Vector2 posicaoD = new Vector2(60f, 0f);

    [Tooltip("Tempo que a bota leva para se movimentar.")]
    [SerializeField] private float duracaoMovimentoPe = 0.12f;

    // =========================================================
    // BOLHAS
    // =========================================================

    [Header("Bolhas")]
    [SerializeField] private RectTransform bolhasContainer;
    [SerializeField] private GameObject bolhaPrefab;

    [Tooltip("Quantidade de bolhas criadas a cada acerto.")]
    [SerializeField] private int quantidadeBolhas = 4;

    [Tooltip("Distância máxima horizontal que as bolhas podem nascer do pé.")]
    [SerializeField] private float espalhamentoHorizontal = 35f;

    [Tooltip("Distância máxima vertical que as bolhas podem nascer do pé.")]
    [SerializeField] private float espalhamentoVertical = 10f;

    [Tooltip("Quanto tempo uma bolha leva para subir e desaparecer.")]
    [SerializeField] private float duracaoBolha = 0.7f;

    [Tooltip("Altura que a bolha sobe.")]
    [SerializeField] private float alturaBolha = 60f;

    // =========================================================
    // MINIGAME
    // =========================================================

    [Header("Minigame")]
    [SerializeField] private int quantidadeAcertosNecessarios = 10;

    [Tooltip("Tempo de espera depois da conclusão antes de voltar para o jogo.")]
    [SerializeField] private float tempoAntesDeVoltar = 0.8f;

    // =========================================================
    // FEEDBACK DE ERRO
    // =========================================================

    [Header("Feedback de erro")]
    [SerializeField] private float intensidadeSacudida = 8f;

    [SerializeField] private float duracaoSacudida = 0.12f;

    // =========================================================
    // ESTADO INTERNO
    // =========================================================

    private bool esperandoA = true;

    private int acertos = 0;

    private bool minigameConcluido = false;

    private Vector2 posicaoInicialBota;

    private Coroutine movimentoBotaAtual;

    public bool Concluido { get; private set; }

    // =========================================================
    // UNITY
    // =========================================================

    private void Start()
    {
        // Guarda a posição original da bota.
        posicaoInicialBota = bota.anchoredPosition;

        // Configura a barra.
        ConfigurarBarra();

        // Mostra a primeira tecla.
        AtualizarTeclaNaTela();

        Debug.Log("Minigame do Tapete Sanitizante iniciado!");
    }

    private void Update()
    {
        // Se o minigame já terminou, não aceita mais comandos.
        if (minigameConcluido)
            return;

        VerificarTeclas();
    }

    // =========================================================
    // CONFIGURAÇÃO
    // =========================================================

    private void ConfigurarBarra()
    {
        barraProgresso.minValue = 0;
        barraProgresso.maxValue = quantidadeAcertosNecessarios;
        barraProgresso.value = 0;
    }

    // =========================================================
    // INPUT
    // =========================================================

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
        KeyCode teclaEsperada;

        if (esperandoA)
        {
            teclaEsperada = KeyCode.A;
        }
        else
        {
            teclaEsperada = KeyCode.D;
        }

        // -----------------------------------------------------
        // ACERTO
        // -----------------------------------------------------

        if (teclaPressionada == teclaEsperada)
        {
            Acertou();
        }

        // -----------------------------------------------------
        // ERRO
        // -----------------------------------------------------

        else
        {
            Errou();
        }
    }

    // =========================================================
    // ACERTO
    // =========================================================

    private void Acertou()
    {
        acertos++;

        // Atualiza a barra.
        barraProgresso.value = acertos;

        Debug.Log(
            "Acertou! Progresso: " +
            acertos +
            "/" +
            quantidadeAcertosNecessarios
        );

        // -----------------------------------------------------
        // FEEDBACK VISUAL
        // -----------------------------------------------------

        MovimentarBota();

        CriarBolhas();

        // -----------------------------------------------------
        // VERIFICA SE TERMINOU
        // -----------------------------------------------------

        if (acertos >= quantidadeAcertosNecessarios)
        {
            ConcluirMinigame();
            return;
        }

        // -----------------------------------------------------
        // TROCA A TECLA
        // -----------------------------------------------------

        esperandoA = !esperandoA;

        AtualizarTeclaNaTela();
    }

    // =========================================================
    // ERRO
    // =========================================================

    private void Errou()
    {
        Debug.Log("Tecla errada!");

        StartCoroutine(SacudirTecla());
    }

    // =========================================================
    // ATUALIZA TEXTO A/D
    // =========================================================

    private void AtualizarTeclaNaTela()
    {
        if (esperandoA)
        {
            teclaAtual.text = "A";
        }
        else
        {
            teclaAtual.text = "D";
        }
    }

    // =========================================================
    // MOVIMENTO DA BOTA
    // =========================================================

    private void MovimentarBota()
    {
        Vector2 destino;

        if (esperandoA)
        {
            destino = posicaoInicialBota + posicaoA;
        }
        else
        {
            destino = posicaoInicialBota + posicaoD;
        }

        if (movimentoBotaAtual != null)
        {
            StopCoroutine(movimentoBotaAtual);
        }

        movimentoBotaAtual = StartCoroutine(
            MoverBota(destino)
        );
    }

    private IEnumerator MoverBota(Vector2 destino)
    {
        Vector2 inicio = bota.anchoredPosition;

        float tempo = 0f;

        Vector3 escalaOriginal = bota.localScale;

        Vector3 escalaComprimida = new Vector3(
            escalaOriginal.x * 0.95f,
            escalaOriginal.y * 0.90f,
            escalaOriginal.z
        );

        while (tempo < duracaoMovimentoPe)
        {
            tempo += Time.deltaTime;

            float progresso =
                tempo / duracaoMovimentoPe;

            // SmoothStep deixa o movimento menos robótico.
            float suavizado =
                Mathf.SmoothStep(0f, 1f, progresso);

            bota.anchoredPosition =
                Vector2.Lerp(
                    inicio,
                    destino,
                    suavizado
                );

            // Pequeno efeito de "pisada".
            if (progresso < 0.5f)
            {
                bota.localScale =
                    Vector3.Lerp(
                        escalaOriginal,
                        escalaComprimida,
                        progresso * 2f
                    );
            }
            else
            {
                bota.localScale =
                    Vector3.Lerp(
                        escalaComprimida,
                        escalaOriginal,
                        (progresso - 0.5f) * 2f
                    );
            }

            yield return null;
        }

        bota.anchoredPosition = destino;
        bota.localScale = escalaOriginal;

        movimentoBotaAtual = null;
    }

    // =========================================================
    // BOLHAS
    // =========================================================

    private void CriarBolhas()
    {
        if (bolhaPrefab == null)
        {
            Debug.LogWarning(
                "Bolha Prefab não foi configurado no Inspector."
            );

            return;
        }

        if (bolhasContainer == null)
        {
            Debug.LogWarning(
                "Bolhas Container não foi configurado no Inspector."
            );

            return;
        }

        for (int i = 0; i < quantidadeBolhas; i++)
        {
            GameObject novaBolha =
                Instantiate(
                    bolhaPrefab,
                    bolhasContainer
                );

            RectTransform rectBolha =
                novaBolha.GetComponent<RectTransform>();

            if (rectBolha == null)
            {
                Debug.LogWarning(
                    "O prefab da bolha precisa ter um RectTransform."
                );

                Destroy(novaBolha);
                continue;
            }

            Vector2 posicaoBolha =
                bota.anchoredPosition;

            posicaoBolha.x += Random.Range(
                -espalhamentoHorizontal,
                espalhamentoHorizontal
            );

            posicaoBolha.y += Random.Range(
                -espalhamentoVertical,
                espalhamentoVertical
            );

            rectBolha.anchoredPosition =
                posicaoBolha;

            StartCoroutine(
                AnimarBolha(
                    novaBolha,
                    rectBolha
                )
            );
        }
    }

    private IEnumerator AnimarBolha(
        GameObject bolha,
        RectTransform rectBolha
    )
    {
        CanvasGroup canvasGroup =
            bolha.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup =
                bolha.AddComponent<CanvasGroup>();
        }

        Vector2 posicaoInicial =
            rectBolha.anchoredPosition;

        Vector2 posicaoFinal =
            posicaoInicial +
            Vector2.up * alturaBolha;

        float tempo = 0f;

        while (tempo < duracaoBolha)
        {
            tempo += Time.deltaTime;

            float progresso =
                tempo / duracaoBolha;

            float suavizado =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    progresso
                );

            rectBolha.anchoredPosition =
                Vector2.Lerp(
                    posicaoInicial,
                    posicaoFinal,
                    suavizado
                );

            canvasGroup.alpha =
                Mathf.Lerp(
                    1f,
                    0f,
                    progresso
                );

            float escala =
                Mathf.Lerp(
                    0.7f,
                    1f,
                    progresso
                );

            rectBolha.localScale =
                Vector3.one * escala;

            yield return null;
        }

        Destroy(bolha);
    }

    // =========================================================
    // FEEDBACK DE ERRO
    // =========================================================

    private IEnumerator SacudirTecla()
    {
        Vector3 posicaoOriginal =
            teclaAtual.transform.localPosition;

        float tempo = 0f;

        while (tempo < duracaoSacudida)
        {
            tempo += Time.deltaTime;

            float deslocamento =
                Random.Range(
                    -intensidadeSacudida,
                    intensidadeSacudida
                );

            teclaAtual.transform.localPosition =
                posicaoOriginal +
                new Vector3(
                    deslocamento,
                    0f,
                    0f
                );

            yield return null;
        }

        teclaAtual.transform.localPosition =
            posicaoOriginal;
    }

    // =========================================================
    // CONCLUSÃO
    // =========================================================

    private void ConcluirMinigame()
    {
        minigameConcluido = true;
        Concluido = true;

        Debug.Log("TAPETE CONCLUÍDO!");

        StartCoroutine(FinalizarComAtraso());
    }

    private IEnumerator FinalizarComAtraso()
    {
        // Dá tempo para o jogador ver o resultado final.
        yield return new WaitForSeconds(tempoAntesDeVoltar);

        // Avisa o sistema universal que o objetivo terminou.
        // O InteracaoMinigame cuidará de:
        //
        // 1. Registrar o objetivo como concluído.
        // 2. Descarregar esta cena.
        // 3. Reativar a cena original.
        // 4. Manter todos os estados da cena original.
        // 5. Desativar a interação deste objeto.
        //
        InteracaoMinigame.FinalizarMinigame(true);
    }
}