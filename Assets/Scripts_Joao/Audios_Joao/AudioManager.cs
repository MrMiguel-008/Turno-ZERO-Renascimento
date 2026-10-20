using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Componente de Áudio")]
    [SerializeField] private AudioSource audioSource;

    [Header("Músicas por Cena")]
    [Tooltip("Arraste aqui o AudioClip da música do Menu / Cena Principal")]
    [SerializeField] private AudioClip musicaCenaPrincipal;

    [Tooltip("Arraste aqui o AudioClip da música dos Minigames")]
    [SerializeField] private AudioClip musicaMinigame;

    void Awake()
    {
        // Garante que só existe um AudioManager no jogo todo e que ele não é destruído ao mudar de cena
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Executado automaticamente toda vez que uma cena nova é carregada
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TocarMusicaParaCena(scene.name);
    }

    private void TocarMusicaParaCena(string nomeCena)
    {
        AudioClip musicaDesejada = null;

        // Define qual música tocar com base no nome da cena
        // Podes adicionar mais condições 'else if' se tiveres mais cenas
        if (nomeCena.Contains("Minigame") || nomeCena.Contains("Tapete") || nomeCena.Contains("Bancada"))
        {
            musicaDesejada = musicaMinigame;
        }
        else
        {
            musicaDesejada = musicaCenaPrincipal;
        }

        // Se a música da cena atual for diferente da que está a tocar, troca-a
        if (musicaDesejada != null)
        {
            if (audioSource.clip != musicaDesejada)
            {
                audioSource.clip = musicaDesejada;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
    }
}