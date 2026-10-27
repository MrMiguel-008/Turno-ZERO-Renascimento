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
    [Tooltip("ID ÚNICO deste objetivo. Deve corresponder ao 'targetMinigameID' da missão no QuestDatabase.")]
    [SerializeField] private string idObjetivo = "objetivo_01";

    // =========================================================
    // INTERAÇÃO
    // =========================================================

    [Header("Interação")]
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
    // ESTADO LOCAL E GLOBAL
    // =========================================================

    private bool jogadorNaArea;
    private bool concluido;
    private Collider2D colisor;

    private static readonly HashSet<string> objetivosConcluidos = new HashSet<string>();
    private static InteracaoMinigame interacaoComPrompt;
    private static InteracaoMinigame interacaoAtual;
    private static Scene cenaOrigem;
    private static Scene cenaMinigameAtual;
    private static string idObjetivoAtual;
    private static bool transicaoEmAndamento;

    private class EstadoRaiz
    {
        public GameObject objeto;
        public bool estavaAtivo;
    }

    private static List<EstadoRaiz> estadosRaizes;
    private static Transform playerTransform;
    private static Vector3 playerPosicaoSalva;
    private static Quaternion playerRotacaoSalva;
    private static bool playerEstavaAtivo;
    private static bool playerFoiSalvo;
    private static Rigidbody2D playerRigidbody;
    private static Vector2 playerVelocidadeSalva;
    private static float playerVelocidadeAngularSalva;

    private static InteracaoMinigameRuntime runtimeManager;

    public bool Concluido => concluido;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
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
    // VALIDAÇÃO SE PODE INTERAGIR (AJUSTADO PARA PERMITIR INTERAÇÃO)
    // =========================================================
    private bool PodeInteragir()
    {
        // Se já foi concluído nesta partida, bloqueia
        if (concluido || ObjetivoFoiConcluido(idObjetivo))
            return false;

        // Liberado para interagir sem travar na verificação estrita de missão ativa
        return true;
    }

    private void ValidarID()
    {
        if (string.IsNullOrWhiteSpace(idObjetivo)) return;

        InteracaoMinigame[] interacoes = FindObjectsByType<InteracaoMinigame>(FindObjectsSortMode.None);
        foreach (InteracaoMinigame outra in interacoes)
        {
            if (outra == this) continue;
            if (outra.idObjetivo == idObjetivo)
            {
                Debug.LogError($"[InteracaoMinigame] ID DUPLICADO: '{idObjetivo}' em '{gameObject.name}' e '{outra.gameObject.name}'");
            }
        }
    }

    private void AtualizarEstadoDoObjetivo()
    {
        if (string.IsNullOrWhiteSpace(idObjetivo))
        {
            concluido = false;
            return;
        }
        concluido = ObjetivoFoiConcluido(idObjetivo);
    }

    public static bool ObjetivoFoiConcluido(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return false;
        return objetivosConcluidos.Contains(id);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!PodeInteragir() || transicaoEmAndamento) return;

        jogadorNaArea = true;
        interacaoComPrompt = this;
        MostrarMensagem();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        jogadorNaArea = false;
        if (interacaoComPrompt == this)
        {
            EsconderMensagem();
            interacaoComPrompt = null;
        }
    }

    private void Update()
    {
        if (!PodeInteragir())
        {
            if (interacaoComPrompt == this)
            {
                EsconderMensagem();
                interacaoComPrompt = null;
            }
            return;
        }

        if (!jogadorNaArea || transicaoEmAndamento || interacaoComPrompt != this)
            return;

        AtualizarPosicaoMensagem();

        if (Input.GetKeyDown(KeyCode.E))
        {
            IniciarMinigame();
        }
    }

    private void IniciarMinigame()
    {
        if (!PodeInteragir() || transicaoEmAndamento) return;

        if (string.IsNullOrWhiteSpace(cenaMinigame) || string.IsNullOrWhiteSpace(idObjetivo))
        {
            Debug.LogError($"[InteracaoMinigame] Configuração ausente em '{gameObject.name}'.");
            return;
        }

        GarantirRuntimeManager();
        interacaoAtual = this;
        idObjetivoAtual = idObjetivo;
        interacaoComPrompt = null;
        EsconderMensagem();

        runtimeManager.IniciarTransicao(this);
    }

    private void MostrarMensagem()
    {
        if (mensagemUI == null) return;
        mensagemUI.text = mensagemInteracao;
        mensagemUI.gameObject.SetActive(true);
        AtualizarPosicaoMensagem();
    }

    private void EsconderMensagem()
    {
        if (mensagemUI == null) return;
        mensagemUI.gameObject.SetActive(false);
    }

    private void AtualizarPosicaoMensagem()
    {
        if (mensagemUI == null || !mensagemUI.gameObject.activeSelf) return;

        Camera cameraPrincipal = Camera.main;
        if (cameraPrincipal == null) return;

        Vector3 posicaoMundo = transform.position + deslocamentoMensagem;
        Vector3 posicaoTela = cameraPrincipal.WorldToScreenPoint(posicaoMundo);
        mensagemUI.transform.position = posicaoTela;
    }

    private void AoRetornarDaCena()
    {
        AtualizarEstadoDoObjetivo();
        jogadorNaArea = false;
        EsconderMensagem();

        if (concluido && desativarAoConcluir)
        {
            enabled = false;
        }
    }

    private static void ReavaliarInteracoes()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Collider2D colliderPlayer = player.GetComponent<Collider2D>();
        if (colliderPlayer == null) return;

        InteracaoMinigame[] interacoes = FindObjectsByType<InteracaoMinigame>(FindObjectsSortMode.None);
        foreach (InteracaoMinigame interacao in interacoes)
        {
            if (!interacao.isActiveAndEnabled) continue;

            interacao.AtualizarEstadoDoObjetivo();

            if (interacao.concluido || !interacao.PodeInteragir())
            {
                interacao.EsconderMensagem();
                continue;
            }

            if (interacao.colisor != null && interacao.colisor.IsTouching(colliderPlayer))
            {
                interacao.jogadorNaArea = true;
                interacaoComPrompt = interacao;
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

    public static void FinalizarMinigame(bool objetivoConcluido)
    {
        if (!transicaoEmAndamento || runtimeManager == null) return;
        runtimeManager.FinalizarTransicao(objetivoConcluido);
    }

    public static void LimparEstadoDaPartida()
    {
        objetivosConcluidos.Clear();
        interacaoAtual = null;
        interacaoComPrompt = null;
        idObjetivoAtual = null;
    }

    private static void RegistrarObjetivoConcluido()
    {
        if (string.IsNullOrWhiteSpace(idObjetivoAtual)) return;

        objetivosConcluidos.Add(idObjetivoAtual);

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.CheckMinigameQuest(idObjetivoAtual);
        }
    }

    private static void GarantirRuntimeManager()
    {
        if (runtimeManager != null) return;
        GameObject objetoRuntime = new GameObject("InteracaoMinigame_Runtime");
        runtimeManager = objetoRuntime.AddComponent<InteracaoMinigameRuntime>();
        DontDestroyOnLoad(objetoRuntime);
    }

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

        public void IniciarTransicao(InteracaoMinigame interacao)
        {
            if (transicaoEmAndamento) return;
            StartCoroutine(CarregarMinigame(interacao));
        }

        private IEnumerator CarregarMinigame(InteracaoMinigame interacao)
        {
            transicaoEmAndamento = true;
            cenaOrigem = SceneManager.GetActiveScene();

            if (!cenaOrigem.IsValid() || !cenaOrigem.isLoaded)
            {
                transicaoEmAndamento = false;
                yield break;
            }

            SalvarEstadoPlayer();

            estadosRaizes = new List<EstadoRaiz>();
            GameObject[] raizes = cenaOrigem.GetRootGameObjects();

            foreach (GameObject raiz in raizes)
            {
                if (raiz != null)
                {
                    estadosRaizes.Add(new EstadoRaiz { objeto = raiz, estavaAtivo = raiz.activeSelf });
                    raiz.SetActive(false);
                }
            }

            AsyncOperation operacao = SceneManager.LoadSceneAsync(interacao.cenaMinigame, LoadSceneMode.Additive);
            while (!operacao.isDone) yield return null;

            cenaMinigameAtual = SceneManager.GetSceneByName(interacao.cenaMinigame);
            SceneManager.SetActiveScene(cenaMinigameAtual);
        }

        private void SalvarEstadoPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            playerTransform = player.transform;
            playerPosicaoSalva = playerTransform.position;
            playerRotacaoSalva = playerTransform.rotation;
            playerEstavaAtivo = player.activeSelf;
            playerRigidbody = player.GetComponent<Rigidbody2D>();

            if (playerRigidbody != null)
            {
                playerVelocidadeSalva = playerRigidbody.linearVelocity;
                playerVelocidadeAngularSalva = playerRigidbody.angularVelocity;
            }

            playerFoiSalvo = true;
        }

        private IEnumerator RestaurarPlayer()
        {
            if (!playerFoiSalvo) yield break;
            yield return null;

            GameObject player = playerTransform != null ? playerTransform.gameObject : GameObject.FindGameObjectWithTag("Player");
            if (player == null) yield break;

            player.SetActive(playerEstavaAtivo);
            player.transform.position = playerPosicaoSalva;
            player.transform.rotation = playerRotacaoSalva;

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.position = playerPosicaoSalva;
                rb.rotation = playerRotacaoSalva.eulerAngles.z;
                rb.linearVelocity = playerVelocidadeSalva;
                rb.angularVelocity = playerVelocidadeAngularSalva;
            }

            Physics2D.SyncTransforms();
        }

        public void FinalizarTransicao(bool objetivoConcluido)
        {
            if (!transicaoEmAndamento) return;
            StartCoroutine(VoltarParaCenaOriginal(objetivoConcluido));
        }

        private IEnumerator VoltarParaCenaOriginal(bool objetivoConcluido)
        {
            if (objetivoConcluido) RegistrarObjetivoConcluido();

            if (cenaMinigameAtual.IsValid() && cenaMinigameAtual.isLoaded)
            {
                AsyncOperation unload = SceneManager.UnloadSceneAsync(cenaMinigameAtual);
                if (unload != null)
                {
                    while (!unload.isDone) yield return null;
                }
            }

            if (estadosRaizes != null)
            {
                foreach (EstadoRaiz estado in estadosRaizes)
                {
                    if (estado.objeto != null)
                    {
                        estado.objeto.SetActive(estado.estavaAtivo);
                    }
                }
            }

            if (cenaOrigem.IsValid() && cenaOrigem.isLoaded)
            {
                SceneManager.SetActiveScene(cenaOrigem);
            }

            Physics2D.SyncTransforms();
            yield return StartCoroutine(RestaurarPlayer());

            InteracaoMinigame interacaoQueIniciou = interacaoAtual;
            transicaoEmAndamento = false;

            if (interacaoQueIniciou != null)
            {
                interacaoQueIniciou.AoRetornarDaCena();
            }

            ReavaliarInteracoes();
            interacaoAtual = null;
            idObjetivoAtual = null;
            cenaMinigameAtual = default;
            estadosRaizes = null;
            playerTransform = null;
            playerRigidbody = null;
            playerFoiSalvo = false;
        }
    }
}