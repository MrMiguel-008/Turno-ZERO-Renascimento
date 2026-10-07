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
    [Tooltip("ID ÚNICO deste objetivo. Exemplo: tapete_sanitizante")]
    [SerializeField] private string idObjetivo = "objetivo_01";

    // =========================================================
    // INTERAÇÃO
    // =========================================================

    [Header("Interação")]
    [SerializeField] private string mensagemInteracao = "E - Interagir";

    [Tooltip("Texto do Canvas global que mostrará a mensagem.")]
    [SerializeField] private TMP_Text mensagemUI;

    [Tooltip("Posição da mensagem em relação ao objeto.")]
    [SerializeField]
    private Vector3 deslocamentoMensagem =
        new Vector3(0f, 1f, 0f);

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

    private static readonly HashSet<string> objetivosConcluidos =
        new HashSet<string>();

    private static InteracaoMinigame interacaoComPrompt;
    private static InteracaoMinigame interacaoAtual;

    private static Scene cenaOrigem;

    private static Scene cenaMinigameAtual;

    private static string idObjetivoAtual;

    private static bool transicaoEmAndamento;

    // =========================================================
    // RAÍZES DA CENA
    // =========================================================

    private class EstadoRaiz
    {
        public GameObject objeto;
        public bool estavaAtivo;
    }

    private static List<EstadoRaiz> estadosRaizes;

    // =========================================================
    // ESTADO DO PLAYER
    // =========================================================

    private static Transform playerTransform;

    private static Vector3 playerPosicaoSalva;

    private static Quaternion playerRotacaoSalva;

    private static bool playerEstavaAtivo;

    private static bool playerFoiSalvo;

    private static Rigidbody2D playerRigidbody;

    private static Vector2 playerVelocidadeSalva;

    private static float playerVelocidadeAngularSalva;

    // =========================================================
    // RUNTIME MANAGER
    // =========================================================

    private static InteracaoMinigameRuntime runtimeManager;

    // =========================================================
    // INTERFACE
    // =========================================================

    public bool Concluido => concluido;

    // =========================================================
    // RESET DOS ESTADOS ESTÁTICOS
    // =========================================================

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetarEstadosEstaticos()
    {
        objetivosConcluidos.Clear();

        interacaoComPrompt = null;
        interacaoAtual = null;

        cenaOrigem = default;
        cenaMinigameAtual = default;

        idObjetivoAtual = null;

        transicaoEmAndamento = false;

        estadosRaizes = null;

        playerTransform = null;
        playerRigidbody = null;

        playerFoiSalvo = false;

        runtimeManager = null;
    }

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

        ValidarID();

        EsconderMensagem();
    }

    private void OnEnable()
    {
        EsconderMensagem();
    }

    private void OnDisable()
    {
        EsconderMensagem();

        jogadorNaArea = false;

        if (interacaoComPrompt == this)
        {
            interacaoComPrompt = null;
        }
    }

    // =========================================================
    // VALIDAÇÃO DO ID
    // =========================================================

    private void ValidarID()
    {
        if (string.IsNullOrWhiteSpace(idObjetivo))
        {
            Debug.LogError(
                $"[InteracaoMinigame] '{gameObject.name}' " +
                "está sem ID de objetivo."
            );

            return;
        }

        InteracaoMinigame[] interacoes =
            FindObjectsByType<InteracaoMinigame>(
                FindObjectsSortMode.None
            );

        foreach (InteracaoMinigame outra in interacoes)
        {
            if (outra == this)
                continue;

            if (outra.idObjetivo == idObjetivo)
            {
                Debug.LogError(
                    $"[InteracaoMinigame] ID DUPLICADO!\n" +
                    $"Objetivo: '{idObjetivo}'\n" +
                    $"Objeto 1: '{gameObject.name}'\n" +
                    $"Objeto 2: '{outra.gameObject.name}'\n\n" +
                    "Cada objetivo precisa possuir um ID único."
                );
            }
        }

        Debug.Log(
            $"[InteracaoMinigame] " +
            $"{gameObject.name} → ID: {idObjetivo} | " +
            $"Concluído: {concluido}"
        );
    }

    // =========================================================
    // ESTADO DO OBJETIVO
    // =========================================================

    private void AtualizarEstadoDoObjetivo()
    {
        if (string.IsNullOrWhiteSpace(idObjetivo))
        {
            concluido = false;
            return;
        }

        concluido =
            ObjetivoFoiConcluido(idObjetivo);
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
                $"[InteracaoMinigame] '{gameObject.name}' " +
                "não possui cena de minigame."
            );

            return;
        }

        if (string.IsNullOrWhiteSpace(idObjetivo))
        {
            Debug.LogError(
                $"[InteracaoMinigame] '{gameObject.name}' " +
                "não possui ID de objetivo."
            );

            return;
        }

        if (cenaMinigame == SceneManager.GetActiveScene().name)
        {
            Debug.LogError(
                $"[InteracaoMinigame] '{gameObject.name}' " +
                "está tentando carregar a própria cena como minigame."
            );

            return;
        }

        GarantirRuntimeManager();

        interacaoAtual = this;

        idObjetivoAtual = idObjetivo;

        interacaoComPrompt = null;

        EsconderMensagem();

        runtimeManager.IniciarTransicao(this);
    }

    // =========================================================
    // MENSAGEM
    // =========================================================

    private void MostrarMensagem()
    {
        if (mensagemUI == null)
            return;

        mensagemUI.text =
            mensagemInteracao;

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

        Camera cameraPrincipal =
            Camera.main;

        if (cameraPrincipal == null)
            return;

        Vector3 posicaoMundo =
            transform.position +
            deslocamentoMensagem;

        Vector3 posicaoTela =
            cameraPrincipal.WorldToScreenPoint(
                posicaoMundo
            );

        mensagemUI.transform.position =
            posicaoTela;
    }

    // =========================================================
    // RETORNO
    // =========================================================

    private void AoRetornarDaCena()
    {
        AtualizarEstadoDoObjetivo();

        jogadorNaArea = false;

        EsconderMensagem();

        if (concluido && desativarAoConcluir)
        {
            enabled = false;

            return;
        }
    }

    // =========================================================
    // REAVALIAR TODAS AS INTERAÇÕES
    // =========================================================

    private static void ReavaliarInteracoes()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player == null)
            return;

        Collider2D colliderPlayer =
            player.GetComponent<Collider2D>();

        if (colliderPlayer == null)
            return;

        InteracaoMinigame[] interacoes =
            FindObjectsByType<InteracaoMinigame>(
                FindObjectsSortMode.None
            );

        foreach (InteracaoMinigame interacao in interacoes)
        {
            if (!interacao.isActiveAndEnabled)
                continue;

            interacao.AtualizarEstadoDoObjetivo();

            if (interacao.concluido)
            {
                interacao.EsconderMensagem();
                continue;
            }

            if (interacao.colisor == null)
                continue;

            if (interacao.colisor.IsTouching(
                colliderPlayer))
            {
                interacao.jogadorNaArea = true;

                interacaoComPrompt =
                    interacao;

                interacao.MostrarMensagem();
            }
            else
            {
                interacao.jogadorNaArea = false;

                if (interacaoComPrompt == interacao)
                {
                    interacao.EsconderMensagem();

                    interacaoComPrompt = null;
                }
            }
        }
    }

    // =========================================================
    // FINALIZAR MINIGAME
    // =========================================================

    public static void FinalizarMinigame(
        bool objetivoConcluido)
    {
        if (!transicaoEmAndamento)
        {
            Debug.LogWarning(
                "[InteracaoMinigame] " +
                "não existe transição ativa."
            );

            return;
        }

        if (runtimeManager == null)
        {
            Debug.LogError(
                "[InteracaoMinigame] " +
                "Runtime Manager inexistente."
            );

            return;
        }

        runtimeManager.FinalizarTransicao(
            objetivoConcluido
        );
    }

    // =========================================================
    // LIMPAR PARTIDA
    // =========================================================

    public static void LimparEstadoDaPartida()
    {
        objetivosConcluidos.Clear();

        interacaoAtual = null;
        interacaoComPrompt = null;

        idObjetivoAtual = null;

        Debug.Log(
            "[InteracaoMinigame] " +
            "Estado da partida resetado."
        );
    }

    // =========================================================
    // REGISTRAR OBJETIVO
    // =========================================================

    private static void RegistrarObjetivoConcluido()
    {
        if (string.IsNullOrWhiteSpace(
            idObjetivoAtual))
        {
            Debug.LogError(
                "[InteracaoMinigame] " +
                "Tentativa de concluir objetivo sem ID."
            );

            return;
        }

        objetivosConcluidos.Add(
            idObjetivoAtual
        );

        Debug.Log(
            $"[InteracaoMinigame] " +
            $"OBJETIVO CONCLUÍDO: {idObjetivoAtual}"
        );
    }

    // =========================================================
    // RUNTIME MANAGER
    // =========================================================

    private static void GarantirRuntimeManager()
    {
        if (runtimeManager != null)
            return;

        GameObject objetoRuntime =
            new GameObject(
                "InteracaoMinigame_Runtime"
            );

        runtimeManager =
            objetoRuntime.AddComponent<
                InteracaoMinigameRuntime
            >();

        DontDestroyOnLoad(
            objetoRuntime
        );
    }

    // =========================================================
    // MANAGER INTERNO
    // =========================================================

    internal class InteracaoMinigameRuntime :
        MonoBehaviour
    {
        private void Awake()
        {
            if (runtimeManager != null &&
                runtimeManager != this)
            {
                Destroy(gameObject);

                return;
            }

            runtimeManager = this;

            DontDestroyOnLoad(gameObject);
        }

        // =====================================================
        // INICIAR TRANSIÇÃO
        // =====================================================

        public void IniciarTransicao(
            InteracaoMinigame interacao)
        {
            if (transicaoEmAndamento)
                return;

            StartCoroutine(
                CarregarMinigame(interacao)
            );
        }

        private IEnumerator CarregarMinigame(
            InteracaoMinigame interacao)
        {
            transicaoEmAndamento = true;

            // -------------------------------------------------
            // CENA DE ORIGEM
            // -------------------------------------------------

            cenaOrigem =
                SceneManager.GetActiveScene();

            if (!cenaOrigem.IsValid() ||
                !cenaOrigem.isLoaded)
            {
                Debug.LogError(
                    "[InteracaoMinigame] " +
                    "Cena de origem inválida."
                );

                transicaoEmAndamento = false;

                yield break;
            }

            // -------------------------------------------------
            // SALVAR PLAYER
            // -------------------------------------------------

            SalvarEstadoPlayer();

            // -------------------------------------------------
            // SALVAR RAÍZES DA CENA
            // -------------------------------------------------

            estadosRaizes =
                new List<EstadoRaiz>();

            GameObject[] raizes =
                cenaOrigem.GetRootGameObjects();

            foreach (GameObject raiz in raizes)
            {
                if (raiz == null)
                    continue;

                estadosRaizes.Add(
                    new EstadoRaiz
                    {
                        objeto = raiz,
                        estavaAtivo = raiz.activeSelf
                    }
                );
            }

            // -------------------------------------------------
            // DESATIVAR CENA ORIGINAL
            // -------------------------------------------------

            foreach (EstadoRaiz estado in estadosRaizes)
            {
                if (estado.objeto != null)
                {
                    estado.objeto.SetActive(false);
                }
            }

            // -------------------------------------------------
            // VERIFICAR SE MINIGAME JÁ ESTÁ CARREGADO
            // -------------------------------------------------

            Scene cenaExistente =
                SceneManager.GetSceneByName(
                    interacao.cenaMinigame
                );

            if (cenaExistente.IsValid() &&
                cenaExistente.isLoaded)
            {
                Debug.LogError(
                    $"[InteracaoMinigame] " +
                    $"A cena '{interacao.cenaMinigame}' " +
                    "já está carregada."
                );

                RestaurarCenaOriginalSemConcluir();

                yield break;
            }

            // -------------------------------------------------
            // CARREGAR MINIGAME
            // -------------------------------------------------

            AsyncOperation operacao =
                SceneManager.LoadSceneAsync(
                    interacao.cenaMinigame,
                    LoadSceneMode.Additive
                );

            if (operacao == null)
            {
                Debug.LogError(
                    $"[InteracaoMinigame] " +
                    $"Não foi possível carregar '{interacao.cenaMinigame}'."
                );

                RestaurarCenaOriginalSemConcluir();

                yield break;
            }

            while (!operacao.isDone)
            {
                yield return null;
            }

            // -------------------------------------------------
            // LOCALIZAR MINIGAME
            // -------------------------------------------------

            cenaMinigameAtual =
                SceneManager.GetSceneByName(
                    interacao.cenaMinigame
                );

            if (!cenaMinigameAtual.IsValid() ||
                !cenaMinigameAtual.isLoaded)
            {
                Debug.LogError(
                    "[InteracaoMinigame] " +
                    "Cena do minigame não foi carregada."
                );

                RestaurarCenaOriginalSemConcluir();

                yield break;
            }

            // -------------------------------------------------
            // MINIGAME VIRA CENA ATIVA
            // -------------------------------------------------

            SceneManager.SetActiveScene(
                cenaMinigameAtual
            );

            Debug.Log(
                $"[InteracaoMinigame] " +
                $"Minigame iniciado: " +
                $"{interacao.cenaMinigame}"
            );
        }

        // =====================================================
        // SALVAR PLAYER
        // =====================================================

        private void SalvarEstadoPlayer()
        {
            playerTransform = null;
            playerRigidbody = null;

            playerFoiSalvo = false;

            GameObject player =
                GameObject.FindGameObjectWithTag(
                    "Player"
                );

            if (player == null)
            {
                Debug.LogWarning(
                    "[InteracaoMinigame] " +
                    "Player não encontrado para salvar posição."
                );

                return;
            }

            playerTransform =
                player.transform;

            playerPosicaoSalva =
                playerTransform.position;

            playerRotacaoSalva =
                playerTransform.rotation;

            playerEstavaAtivo =
                player.activeSelf;

            playerRigidbody =
                player.GetComponent<Rigidbody2D>();

            if (playerRigidbody != null)
            {
                playerVelocidadeSalva =
                    playerRigidbody.linearVelocity;

                playerVelocidadeAngularSalva =
                    playerRigidbody.angularVelocity;
            }

            playerFoiSalvo = true;

            Debug.Log(
                $"[InteracaoMinigame] " +
                $"Player salvo em: {playerPosicaoSalva}"
            );
        }

        // =====================================================
        // RESTAURAR PLAYER
        // =====================================================

        private IEnumerator RestaurarPlayer()
        {
            if (!playerFoiSalvo)
                yield break;

            yield return null;

            GameObject player =
                playerTransform != null
                    ? playerTransform.gameObject
                    : GameObject.FindGameObjectWithTag(
                        "Player"
                    );

            if (player == null)
            {
                Debug.LogWarning(
                    "[InteracaoMinigame] " +
                    "Player não encontrado ao restaurar."
                );

                yield break;
            }

            player.SetActive(
                playerEstavaAtivo
            );

            player.transform.position =
                playerPosicaoSalva;

            player.transform.rotation =
                playerRotacaoSalva;

            Rigidbody2D rb =
                player.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.position =
                    playerPosicaoSalva;

                rb.rotation =
                    playerRotacaoSalva.eulerAngles.z;

                rb.linearVelocity =
                    playerVelocidadeSalva;

                rb.angularVelocity =
                    playerVelocidadeAngularSalva;
            }

            Physics2D.SyncTransforms();

            Debug.Log(
                $"[InteracaoMinigame] " +
                $"Player restaurado em: " +
                $"{playerPosicaoSalva}"
            );
        }

        // =====================================================
        // FINALIZAR TRANSIÇÃO
        // =====================================================

        public void FinalizarTransicao(
            bool objetivoConcluido)
        {
            if (!transicaoEmAndamento)
                return;

            StartCoroutine(
                VoltarParaCenaOriginal(
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
            // REGISTRAR OBJETIVO
            // -------------------------------------------------

            if (objetivoConcluido)
            {
                RegistrarObjetivoConcluido();
            }

            // -------------------------------------------------
            // DESCARREGAR MINIGAME
            // -------------------------------------------------

            if (cenaMinigameAtual.IsValid() &&
                cenaMinigameAtual.isLoaded)
            {
                AsyncOperation unload =
                    SceneManager.UnloadSceneAsync(
                        cenaMinigameAtual
                    );

                if (unload != null)
                {
                    while (!unload.isDone)
                    {
                        yield return null;
                    }
                }
            }

            // -------------------------------------------------
            // RESTAURAR ESTADOS DAS RAÍZES
            // -------------------------------------------------

            if (estadosRaizes != null)
            {
                foreach (EstadoRaiz estado in estadosRaizes)
                {
                    if (estado.objeto == null)
                        continue;

                    estado.objeto.SetActive(
                        estado.estavaAtivo
                    );
                }
            }

            // -------------------------------------------------
            // CENA ORIGINAL ATIVA
            // -------------------------------------------------

            if (cenaOrigem.IsValid() &&
                cenaOrigem.isLoaded)
            {
                SceneManager.SetActiveScene(
                    cenaOrigem
                );
            }

            Physics2D.SyncTransforms();

            // -------------------------------------------------
            // RESTAURAR PLAYER
            // -------------------------------------------------

            yield return StartCoroutine(
                RestaurarPlayer()
            );

            // -------------------------------------------------
            // FINALIZAR ESTADO DA INTERAÇÃO
            // -------------------------------------------------

            InteracaoMinigame interacaoQueIniciou =
                interacaoAtual;

            transicaoEmAndamento = false;

            if (interacaoQueIniciou != null)
            {
                interacaoQueIniciou
                    .AoRetornarDaCena();
            }

            // -------------------------------------------------
            // REAVALIAR OUTRAS INTERAÇÕES
            // -------------------------------------------------

            ReavaliarInteracoes();

            Debug.Log(
                objetivoConcluido
                    ? "[InteracaoMinigame] " +
                      "Minigame concluído. " +
                      "Cena restaurada."
                    : "[InteracaoMinigame] " +
                      "Minigame encerrado sem conclusão."
            );

            // -------------------------------------------------
            // LIMPAR TRANSIÇÃO
            // -------------------------------------------------

            interacaoAtual = null;

            idObjetivoAtual = null;

            cenaMinigameAtual =
                default;

            estadosRaizes = null;

            playerTransform = null;

            playerRigidbody = null;

            playerFoiSalvo = false;
        }

        // =====================================================
        // FALHA AO CARREGAR
        // =====================================================

        private void RestaurarCenaOriginalSemConcluir()
        {
            if (estadosRaizes != null)
            {
                foreach (EstadoRaiz estado in estadosRaizes)
                {
                    if (estado.objeto == null)
                        continue;

                    estado.objeto.SetActive(
                        estado.estavaAtivo
                    );
                }
            }

            if (cenaOrigem.IsValid() &&
                cenaOrigem.isLoaded)
            {
                SceneManager.SetActiveScene(
                    cenaOrigem
                );
            }

            transicaoEmAndamento = false;

            interacaoAtual = null;

            idObjetivoAtual = null;

            cenaMinigameAtual =
                default;

            estadosRaizes = null;

            playerTransform = null;

            playerRigidbody = null;

            playerFoiSalvo = false;
        }
    }
}