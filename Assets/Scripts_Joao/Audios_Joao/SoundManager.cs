using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Toca a música e os efeitos do jogo. Nasce sozinho antes da primeira cena e sobrevive às trocas
// de cena, então não precisa ser colocado em nenhuma cena. Os sons ficam em Assets/Resources/BibliotecaSons.
public class SoundManager : MonoBehaviour
{
    private const string NOME_BIBLIOTECA = "BibliotecaSons";
    private const string CENA_MENU = "MenuPrincipal";
    private const float DURACAO_FADE_MUSICA = 0.6f;
    private const float TEMPO_PARA_PARAR_PASSOS = 0.1f;
    private const float INTERVALO_MINIMO_REPETICAO = 0.05f;

    private static readonly BibliotecaSons.Entrada SEM_MUSICA = new BibliotecaSons.Entrada();

    private static SoundManager instancia;

    private BibliotecaSons biblioteca;

    private AudioSource fonteMusica;
    private AudioSource fonteEfeitos;
    private AudioSource fontePassos;
    private AudioSource fonteAmbiente;
    private AudioSource fonteEsteira;
    private AudioSource fonteProducao;
    private AudioSource[] fontesPausaveis;

    private AudioClip musicaAtual;
    private Coroutine rotinaMusica;

    private AudioListener ouvinteReserva;
    private AudioListener ouvinteDaCena;

    private float[] ultimoToque;
    private float ultimoPasso = -1f;
    private float proximoPasso;
    private int indiceUltimoPasso = -1;
    private bool estavaPausado;

    // =========================================================
    // CHAMADAS USADAS PELOS OUTROS SCRIPTS
    // =========================================================

    public static void Tocar(Som som)
    {
        if (instancia != null) instancia.TocarEfeito(som);
    }

    public static void TocarColeta(string nomeItem)
    {
        switch (nomeItem)
        {
            case "acucar":
            case "agua":
            case "farinha":
            case "gordura":
            case "gotas":
                Tocar(Som.ColetaProducao);
                break;
            case "biscoito":
                Tocar(Som.ColetaBiscoito);
                break;
            default:
                Tocar(Som.ColetaItem);
                break;
        }
    }

    // O script do jogador chama a cada frame em que está andando. Quando as chamadas param
    // (parou de andar, entrou em um minigame, trocou de cena), os passos param sozinhos.
    public static void ManterPassos()
    {
        if (instancia != null) instancia.ultimoPasso = Time.unscaledTime;
    }

    public static void DefinirEsteira(bool ligada)
    {
        if (instancia == null) return;

        if (ligada)
        {
            instancia.IniciarLoop(instancia.fonteEsteira, instancia.biblioteca.esteiraQualidade, instancia.biblioteca.volumeEfeitos);
        }
        else
        {
            PararFonte(instancia.fonteEsteira);
        }
    }

    public static void TocarProducao()
    {
        if (instancia == null) return;

        BibliotecaSons.Entrada entrada = instancia.biblioteca.producaoFuncionando;
        if (entrada.clip == null) return;

        instancia.fonteProducao.clip = entrada.clip;
        instancia.fonteProducao.volume = entrada.volume * instancia.biblioteca.volumeEfeitos;
        instancia.fonteProducao.Play();
    }

    // =========================================================
    // CRIAÇÃO
    // =========================================================

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Criar()
    {
        if (instancia != null) return;

        GameObject objeto = new GameObject("SoundManager");
        DontDestroyOnLoad(objeto);
        instancia = objeto.AddComponent<SoundManager>();
    }

    private void Awake()
    {
        biblioteca = Resources.Load<BibliotecaSons>(NOME_BIBLIOTECA);
        if (biblioteca == null)
        {
            Debug.LogWarning($"[SoundManager] Assets/Resources/{NOME_BIBLIOTECA} não foi encontrada. O jogo vai rodar sem som.");
            biblioteca = ScriptableObject.CreateInstance<BibliotecaSons>();
        }

        fonteMusica = CriarFonte(true);
        fonteMusica.priority = 0;
        fonteMusica.volume = 0f;
        fonteEfeitos = CriarFonte(false);
        fontePassos = CriarFonte(false);
        fonteAmbiente = CriarFonte(true);
        fonteEsteira = CriarFonte(true);
        fonteProducao = CriarFonte(false);
        fontesPausaveis = new[] { fontePassos, fonteAmbiente, fonteEsteira, fonteProducao };

        ouvinteReserva = gameObject.AddComponent<AudioListener>();
        ouvinteReserva.enabled = false;

        ultimoToque = new float[System.Enum.GetValues(typeof(Som)).Length];
    }

    private AudioSource CriarFonte(bool emLoop)
    {
        AudioSource fonte = gameObject.AddComponent<AudioSource>();
        fonte.playOnAwake = false;
        fonte.loop = emLoop;
        return fonte;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += AoCarregarCena;
        SceneManager.activeSceneChanged += AoTrocarCenaAtiva;
        SceneManager.sceneUnloaded += AoDescarregarCena;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= AoCarregarCena;
        SceneManager.activeSceneChanged -= AoTrocarCenaAtiva;
        SceneManager.sceneUnloaded -= AoDescarregarCena;
    }

    // =========================================================
    // MÚSICA E AMBIENTE POR CENA
    // =========================================================

    private void AoCarregarCena(Scene cena, LoadSceneMode modo)
    {
        AplicarCenaAtiva();
    }

    // Os minigames são carregados por cima do jogo principal. Na volta não existe sceneLoaded,
    // só a troca de cena ativa, então é ela que garante a música certa nos dois sentidos.
    private void AoTrocarCenaAtiva(Scene anterior, Scene nova)
    {
        AplicarCenaAtiva();
    }

    private void AoDescarregarCena(Scene cena)
    {
        PararFonte(fonteEsteira);
    }

    private void AplicarCenaAtiva()
    {
        string cena = SceneManager.GetActiveScene().name;

        TrocarMusica(EscolherMusica(cena));

        if (cena == CENA_MENU)
        {
            PararFonte(fonteAmbiente);
            PararFonte(fonteEsteira);
            PararFonte(fonteProducao);
        }
        else
        {
            IniciarLoop(fonteAmbiente, biblioteca.ambienteIndustrial, biblioteca.volumeAmbiente);
        }
    }

    private BibliotecaSons.Entrada EscolherMusica(string cena)
    {
        if (cena == CENA_MENU) return biblioteca.musicaMenu;

        foreach (BibliotecaSons.MusicaDeCena musica in biblioteca.musicasDeCena)
        {
            if (musica.cena == cena) return musica;
        }

        if (System.Array.IndexOf(biblioteca.cenasComMusica, cena) >= 0) return biblioteca.musicaJogo;
        return SEM_MUSICA;
    }

    // Se a música pedida já é a atual, ela continua de onde está, sem reiniciar.
    private void TrocarMusica(BibliotecaSons.Entrada musica)
    {
        if (musica.clip == musicaAtual) return;

        musicaAtual = musica.clip;

        if (rotinaMusica != null) StopCoroutine(rotinaMusica);
        rotinaMusica = StartCoroutine(TrocarMusicaComFade(musica));
    }

    private IEnumerator TrocarMusicaComFade(BibliotecaSons.Entrada musica)
    {
        if (fonteMusica.isPlaying) yield return MudarVolumeMusica(0f);

        fonteMusica.Stop();
        fonteMusica.clip = musica.clip;

        if (musica.clip == null) yield break;

        fonteMusica.Play();
        yield return MudarVolumeMusica(musica.volume * biblioteca.volumeMusica);
    }

    // Usa o tempo real porque o menu de pausa e a tela de fim de jogo deixam o timeScale em 0.
    private IEnumerator MudarVolumeMusica(float volumeFinal)
    {
        float volumeInicial = fonteMusica.volume;
        float tempo = 0f;

        while (tempo < DURACAO_FADE_MUSICA)
        {
            tempo += Time.unscaledDeltaTime;
            fonteMusica.volume = Mathf.Lerp(volumeInicial, volumeFinal, tempo / DURACAO_FADE_MUSICA);
            yield return null;
        }

        fonteMusica.volume = volumeFinal;
    }

    // =========================================================
    // EFEITOS E LOOPS
    // =========================================================

    private void TocarEfeito(Som som)
    {
        BibliotecaSons.Entrada entrada = biblioteca.Obter(som);
        if (entrada == null || entrada.clip == null) return;

        // Evita o mesmo som empilhado várias vezes no mesmo instante, o que estoura o volume.
        int indice = (int)som;
        if (Time.unscaledTime - ultimoToque[indice] < INTERVALO_MINIMO_REPETICAO) return;
        ultimoToque[indice] = Time.unscaledTime;

        fonteEfeitos.PlayOneShot(entrada.clip, entrada.volume * biblioteca.volumeEfeitos);
    }

    private void IniciarLoop(AudioSource fonte, BibliotecaSons.Entrada entrada, float volumeGrupo)
    {
        if (entrada.clip == null || fonte.clip == entrada.clip) return;

        fonte.clip = entrada.clip;
        fonte.volume = entrada.volume * volumeGrupo;
        fonte.Play();
    }

    private static void PararFonte(AudioSource fonte)
    {
        fonte.Stop();
        fonte.clip = null;
    }

    private void Update()
    {
        bool pausado = Time.timeScale == 0f;

        if (pausado != estavaPausado)
        {
            estavaPausado = pausado;

            // A música continua na pausa. Só os sons da fábrica ficam parados.
            foreach (AudioSource fonte in fontesPausaveis)
            {
                if (pausado) fonte.Pause();
                else fonte.UnPause();
            }
        }

        bool andando = !pausado && ultimoPasso >= 0f && Time.unscaledTime - ultimoPasso < TEMPO_PARA_PARAR_PASSOS;

        if (andando && Time.unscaledTime >= proximoPasso)
        {
            TocarPasso();
        }
    }

    private void TocarPasso()
    {
        AudioClip[] clips = biblioteca.passos;
        if (clips == null || clips.Length == 0) return;

        int indice = Random.Range(0, clips.Length);
        if (clips.Length > 1 && indice == indiceUltimoPasso) indice = (indice + 1) % clips.Length;
        indiceUltimoPasso = indice;

        proximoPasso = Time.unscaledTime + biblioteca.intervaloPassos;

        if (clips[indice] != null)
        {
            fontePassos.PlayOneShot(clips[indice], biblioteca.volumePassos * biblioteca.volumeEfeitos);
        }
    }

    // =========================================================
    // OUVINTE DE RESERVA
    // =========================================================

    // Ao abrir ou fechar um minigame existe um intervalo em que nenhuma câmera está ativa.
    // Sem nenhum AudioListener o Unity deixa tudo mudo, e a música parece cortar nesse momento.
    private void LateUpdate()
    {
        if (ouvinteDaCena != null && ouvinteDaCena.isActiveAndEnabled)
        {
            if (ouvinteReserva.enabled) ouvinteReserva.enabled = false;
            return;
        }

        ouvinteDaCena = null;

        foreach (AudioListener ouvinte in FindObjectsByType<AudioListener>(FindObjectsSortMode.None))
        {
            if (ouvinte != ouvinteReserva && ouvinte.isActiveAndEnabled)
            {
                ouvinteDaCena = ouvinte;
                break;
            }
        }

        ouvinteReserva.enabled = ouvinteDaCena == null;
    }
}
