using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class InteracaoMinigame : MonoBehaviour, IObjetivoMinigame
{
    // =========================================================
    // IDENTIFICAÇÃO DO OBJETIVO
    // =========================================================

    [Header("Identificação do Objetivo")]
    [Tooltip("ID único deste objetivo. Exemplo: tapete_sanitizante")]
    [SerializeField] private string idObjetivo = "objetivo_01";

    // =========================================================
    // INTERAÇÃO
    // =========================================================

    [Header("Interação")]
    [Tooltip("Texto mostrado quando o jogador estiver perto.")]
    [SerializeField] private string mensagemInteracao = "E - Interagir";

    [Tooltip("Texto do Canvas global que mostrará a mensagem.")]
    [SerializeField] private TMP_Text mensagemUI;

    [Tooltip("Posição da mensagem em relação ao objeto.")]
    [SerializeField] private Vector3 deslocamentoMensagem = new Vector3(0f, 1f, 0f);

    // =========================================================
    // MINIGAME
    // =========================================================

    [Header("Minigame")]
    [Tooltip("Nome exato da cena do minigame.")]
    [SerializeField] private string cenaMinigame;

    [Tooltip("Desativa este componente depois que o objetivo for concluído.")]
    [SerializeField] private bool desativarAoConcluir = true;

    // =========================================================
    // ESTADO LOCAL
    // =========================================================

    private bool jogadorNaArea;
    private bool concluido;

    private Collider2D colisor;

    // =========================================================
    // ESTADO GLOBAL DA PARTIDA
    // =========================================================

    // Guarda quais objetivos já foram concluídos nesta execução do jogo.
    private static readonly HashSet<string> objetivosConcluidos =
        new HashSet<string>();

    // Qual interação está mostrando o prompt atualmente.
    private static InteracaoMinigame interacaoComPrompt;

    // Interação que iniciou o minigame.
    private static InteracaoMinigame interacaoAtual;

    // Cena original onde o jogador estava.
    private static Scene cenaOrigem;

    // Todas as raízes da cena original.
    // Elas serão desativadas durante o minigame,
    // mas NÃO serão destruídas.
    private static List<GameObject> raizesCenaOrigem;

    // Cena do minigame carregada de forma aditiva.
    private static Scene cenaMinigameAtual;

    // ID do objetivo que iniciou o minigame.
    private static string idObjetivoAtual;

    // Evita duas transições acontecendo ao mesmo tempo.
    private static bool transicaoEmAndamento;

    // =========================================================
    // INTERFACE
    // =========================================================

    public bool Concluido => concluido;

    // =========================================================
    // CICLO DE VIDA
    // =========================================================

    private void Awake()
    {
        colisor = GetComponent<Collider2D>();

        GarantirRuntimeManager();
    }

    private void Start()
    {
        AtualizarEstadoDoObjetivo();
        EsconderMensagem();
    }

    private void OnEnable()
    {
        EsconderMensagem();
    }

    private void OnDisable()
    {
        EsconderMensagem();

        if (interacaoComPrompt == this)
        {
            interacaoComPrompt = null;
        }
    }

    // =========================================================
    // ESTADO DO OBJETIVO
    // =========================================================

    private void AtualizarEstadoDoObjetivo()
    {
        if (string.IsNullOrWhiteSpace(idObjetivo))
        {
            concluido = false;

            Debug.LogWarning(
                $"InteracaoMinigame em '{gameObject.name}' não possui ID de objetivo."
            );

            return;
        }

        concluido = ObjetivoFoiConcluido(idObjetivo);
    }

    public static bool ObjetivoFoiConcluido(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return false;

        return objetivosConcluidos.Contains(id);
    }

    // =========================================================
    // TRIGGER
    // =========================================================

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (concluido)
            return;

        if (transicaoEmAndamento)
            return;

        jogadorNaArea = true;

        interacaoComPrompt = this;

        MostrarMensagem();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        jogadorNaArea = false;

        if (interacaoComPrompt == this)
        {
            EsconderMensagem();
            interacaoComPrompt = null;
        }
    }

    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (concluido)
            return;

        if (!jogadorNaArea)
            return;

        if (transicaoEmAndamento)
            return;

        // Só o objeto que está controlando o prompt pode receber E.
        if (interacaoComPrompt != this)
            return;

        AtualizarPosicaoMensagem();

        if (Input.GetKeyDown(KeyCode.E))
        {
            IniciarMinigame();
        }
    }

    // =========================================================
    // INICIAR MINIGAME
    // =========================================================

    private void IniciarMinigame()
    {
        if (concluido)
            return;

        if (transicaoEmAndamento)
            return;

        if (string.IsNullOrWhiteSpace(cenaMinigame))
        {
            Debug.LogError(
                $"InteracaoMinigame em '{gameObject.name}': " +
                "nenhuma cena de minigame foi configurada."
            );

            return;
        }

        if (string.IsNullOrWhiteSpace(idObjetivo))
        {
            Debug.LogError(
                $"InteracaoMinigame em '{gameObject.name}': " +
                "o ID do objetivo está vazio."
            );

            return;
        }

        GarantirRuntimeManager();

        interacaoAtual = this;
        idObjetivoAtual = idObjetivo;

        interacaoComPrompt = null;

        EsconderMensagem();

        InteracaoMinigameRuntime.IniciarTransicao(this);
    }

    // =========================================================
    // MOSTRAR / ESCONDER MENSAGEM
    // =========================================================

    private void MostrarMensagem()
    {
        if (mensagemUI == null)
            return;

        mensagemUI.text = mensagemInteracao;
        mensagemUI.gameObject.SetActive(true);

        AtualizarPosicaoMensagem();
    }

    private void EsconderMensagem()
    {
        if (mensagemUI == null)
            return;

        mensagemUI.gameObject.SetActive(false);
    }

    private void AtualizarPosicaoMensagem()
    {
        if (mensagemUI == null)
            return;

        if (!mensagemUI.gameObject.activeSelf)
            return;

        Camera cameraPrincipal = Camera.main;

        if (cameraPrincipal == null)
            return;

        Vector3 posicaoMundo =
            transform.position + deslocamentoMensagem;

        Vector3 posicaoTela =
            cameraPrincipal.WorldToScreenPoint(posicaoMundo);

        mensagemUI.transform.position = posicaoTela;
    }

    // =========================================================
    // RETORNO DA CENA
    // =========================================================

    private void AoRetornarDaCena()
    {
        AtualizarEstadoDoObjetivo();

        jogadorNaArea = false;
        EsconderMensagem();

        // Objetivo concluído:
        // esta interação fica permanentemente indisponível
        // pelo resto desta partida.
        if (concluido && desativarAoConcluir)
        {
            enabled = false;
            return;
        }

        // Caso tenha voltado sem concluir o minigame,
        // verificamos se o jogador ainda está dentro do trigger.
        StartCoroutine(VerificarJogadorAoRetornar());
    }

    private IEnumerator VerificarJogadorAoRetornar()
    {
        yield return null;

        if (!isActiveAndEnabled)
            yield break;

        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player == null || colisor == null)
            yield break;

        Collider2D colliderPlayer =
            player.GetComponent<Collider2D>();

        if (colliderPlayer == null)
            yield break;

        Physics2D.SyncTransforms();

        if (colisor.IsTouching(colliderPlayer))
        {
            jogadorNaArea = true;
            interacaoComPrompt = this;

            MostrarMensagem();
        }
    }

    // =========================================================
    // FINALIZAR MINIGAME
    // =========================================================

    /// <summary>
    /// Chame este método quando o minigame terminar.
    /// true = objetivo concluído.
    /// false = voltar sem concluir.
    /// </summary>
    public static void FinalizarMinigame(bool objetivoConcluido)
    {
        if (!transicaoEmAndamento)
        {
            Debug.LogWarning(
                "InteracaoMinigame: não existe uma transição de minigame ativa."
            );

            return;
        }

        GarantirRuntimeManager();

        InteracaoMinigameRuntime.FinalizarTransicao(objetivoConcluido);
    }

    // =========================================================
    // LIMPAR OBJETIVOS
    // =========================================================

    /// <summary>
    /// Limpa todos os objetivos concluídos.
    /// Útil quando o jogador começa uma nova partida.
    /// </summary>
    public static void LimparEstadoDaPartida()
    {
        objetivosConcluidos.Clear();

        interacaoAtual = null;
        interacaoComPrompt = null;

        idObjetivoAtual = null;

        Debug.Log("Estado dos objetivos da partida foi resetado.");
    }

    // =========================================================
    // REGISTRO DO OBJETIVO
    // =========================================================

    private static void RegistrarObjetivoConcluido()
    {
        if (string.IsNullOrWhiteSpace(idObjetivoAtual))
        {
            Debug.LogWarning(
                "InteracaoMinigame: tentativa de concluir objetivo sem ID."
            );

            return;
        }

        objetivosConcluidos.Add(idObjetivoAtual);

        Debug.Log(
            $"Objetivo concluído: {idObjetivoAtual}"
        );
    }

    // =========================================================
    // RUNTIME MANAGER
    // =========================================================

    private static InteracaoMinigameRuntime runtimeManager;

    private static void GarantirRuntimeManager()
    {
        if (runtimeManager != null)
            return;

        GameObject objetoRuntime =
            new GameObject("InteracaoMinigame_Runtime");

        runtimeManager =
            objetoRuntime.AddComponent<InteracaoMinigameRuntime>();

        Object.DontDestroyOnLoad(objetoRuntime);
    }

    // =========================================================
    // MANAGER INTERNO
    // =========================================================

    internal class InteracaoMinigameRuntime : MonoBehaviour
    {
        private void Awake()
        {
            if (runtimeManager != null && runtimeManager != this)
            {
                Destroy(gameObject);
                return;
            }

            runtimeManager = this;

            DontDestroyOnLoad(gameObject);
        }

        // -----------------------------------------------------
        // INICIAR
        // -----------------------------------------------------

        public static void IniciarTransicao(
            InteracaoMinigame interacao)
        {
            if (runtimeManager == null)
                return;

            runtimeManager.StartCoroutine(
                runtimeManager.CarregarMinigame(interacao)
            );
        }

        private IEnumerator CarregarMinigame(
            InteracaoMinigame interacao)
        {
            if (transicaoEmAndamento)
                yield break;

            transicaoEmAndamento = true;

            // -------------------------------------------------
            // SALVA REFERÊNCIA DA CENA ATUAL
            // -------------------------------------------------

            cenaOrigem =
                SceneManager.GetActiveScene();

            if (!cenaOrigem.IsValid() || !cenaOrigem.isLoaded)
            {
                Debug.LogError(
                    "InteracaoMinigame: cena de origem inválida."
                );

                transicaoEmAndamento = false;
                yield break;
            }

            // -------------------------------------------------
            // SALVA TODAS AS RAÍZES DA CENA
            // -------------------------------------------------

            raizesCenaOrigem =
                new List<GameObject>(
                    cenaOrigem.GetRootGameObjects()
                );

            // -------------------------------------------------
            // DESATIVA A CENA ORIGINAL
            // -------------------------------------------------
            //
            // IMPORTANTE:
            // Os objetos NÃO são destruídos.
            //
            // Eles continuam existindo na memória exatamente
            // com o estado que tinham.
            //
            // -------------------------------------------------

            foreach (GameObject raiz in raizesCenaOrigem)
            {
                if (raiz != null)
                {
                    raiz.SetActive(false);
                }
            }

            // -------------------------------------------------
            // CARREGA O MINIGAME SEM DESCARREGAR A CENA ATUAL
            // -------------------------------------------------

            AsyncOperation operacao =
                SceneManager.LoadSceneAsync(
                    interacao.cenaMinigame,
                    LoadSceneMode.Additive
                );

            if (operacao == null)
            {
                Debug.LogError(
                    $"Não foi possível carregar a cena '{interacao.cenaMinigame}'."
                );

                RestaurarCenaOriginalSemConcluir();
                yield break;
            }

            while (!operacao.isDone)
            {
                yield return null;
            }

            // -------------------------------------------------
            // LOCALIZA A CENA DO MINIGAME
            // -------------------------------------------------

            cenaMinigameAtual =
                SceneManager.GetSceneByName(
                    interacao.cenaMinigame
                );

            if (!cenaMinigameAtual.IsValid() ||
                !cenaMinigameAtual.isLoaded)
            {
                Debug.LogError(
                    $"A cena '{interacao.cenaMinigame}' não foi carregada corretamente."
                );

                RestaurarCenaOriginalSemConcluir();
                yield break;
            }

            // O minigame passa a ser a cena ativa.
            SceneManager.SetActiveScene(cenaMinigameAtual);

            Debug.Log(
                $"Minigame '{interacao.cenaMinigame}' iniciado."
            );
        }

        // -----------------------------------------------------
        // FINALIZAR
        // -----------------------------------------------------

        public static void FinalizarTransicao(
            bool objetivoConcluido)
        {
            if (runtimeManager == null)
                return;

            runtimeManager.StartCoroutine(
                runtimeManager.VoltarParaCenaOriginal(
                    objetivoConcluido
                )
            );
        }

        private IEnumerator VoltarParaCenaOriginal(
            bool objetivoConcluido)
        {
            if (!transicaoEmAndamento)
                yield break;

            // -------------------------------------------------
            // REGISTRA O OBJETIVO
            // -------------------------------------------------

            if (objetivoConcluido)
            {
                RegistrarObjetivoConcluido();
            }

            // -------------------------------------------------
            // DESCARREGA SOMENTE O MINIGAME
            // -------------------------------------------------

            if (cenaMinigameAtual.IsValid() &&
                cenaMinigameAtual.isLoaded)
            {
                AsyncOperation operacaoUnload =
                    SceneManager.UnloadSceneAsync(
                        cenaMinigameAtual
                    );

                if (operacaoUnload != null)
                {
                    while (!operacaoUnload.isDone)
                    {
                        yield return null;
                    }
                }
            }

            // -------------------------------------------------
            // REATIVA A CENA ORIGINAL
            // -------------------------------------------------

            if (raizesCenaOrigem != null)
            {
                foreach (GameObject raiz in raizesCenaOrigem)
                {
                    if (raiz != null)
                    {
                        raiz.SetActive(true);
                    }
                }
            }

            // -------------------------------------------------
            // RESTAURA A CENA ATIVA
            // -------------------------------------------------

            if (cenaOrigem.IsValid() &&
                cenaOrigem.isLoaded)
            {
                SceneManager.SetActiveScene(cenaOrigem);
            }

            Physics2D.SyncTransforms();

            // -------------------------------------------------
            // AVISA A INTERAÇÃO ORIGINAL
            // -------------------------------------------------

            InteracaoMinigame interacaoQueIniciou =
                interacaoAtual;

            transicaoEmAndamento = false;

            if (interacaoQueIniciou != null)
            {
                interacaoQueIniciou.AoRetornarDaCena();
            }

            Debug.Log(
                objetivoConcluido
                    ? "Minigame concluído e cena original restaurada."
                    : "Minigame encerrado sem conclusão e cena original restaurada."
            );

            // -------------------------------------------------
            // LIMPA ESTADO DA TRANSIÇÃO
            // -------------------------------------------------

            interacaoAtual = null;
            idObjetivoAtual = null;
            cenaMinigameAtual = default;
            raizesCenaOrigem = null;
        }

        // -----------------------------------------------------
        // FALHA AO CARREGAR
        // -----------------------------------------------------

        private void RestaurarCenaOriginalSemConcluir()
        {
            if (raizesCenaOrigem != null)
            {
                foreach (GameObject raiz in raizesCenaOrigem)
                {
                    if (raiz != null)
                    {
                        raiz.SetActive(true);
                    }
                }
            }

            if (cenaOrigem.IsValid() &&
                cenaOrigem.isLoaded)
            {
                SceneManager.SetActiveScene(cenaOrigem);
            }

            Physics2D.SyncTransforms();

            transicaoEmAndamento = false;

            interacaoAtual = null;
            idObjetivoAtual = null;
            cenaMinigameAtual = default;
            raizesCenaOrigem = null;
        }
    }
}