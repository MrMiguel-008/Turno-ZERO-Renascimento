using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Necessário para usar Corrotinas

public class GerenciadorDesafios : MonoBehaviour
{
    private static string cenaOriginal;
    private static bool retornando = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InicializarOuvinte()
    {
        SceneManager.sceneLoaded += AoEntrarNaCena;
    }

    public void FinalizarDesafioEVoltar()
    {
        cenaOriginal = PlayerPrefs.GetString("CenaOriginalNome", "");

        if (!string.IsNullOrEmpty(cenaOriginal))
        {
            retornando = true;
            SceneManager.LoadScene(cenaOriginal);
        }
        else
        {
            Debug.LogError("Erro: O nome da cena original não foi encontrado no PlayerPrefs!");
        }
    }

    private static void AoEntrarNaCena(Scene novaCena, LoadSceneMode modo)
    {
        if (retornando && novaCena.name == cenaOriginal)
        {
            // Cria um objeto temporário apenas para rodar a Corrotina de teletransporte com segurança
            GameObject runner = new GameObject("TeleportRunner");
            var script = runner.AddComponent<MonoBehaviourRunner>();
            script.IniciarTeletransporte();

            retornando = false;
        }
    }
}

// Classe auxiliar interna criada automaticamente para gerenciar o tempo do teletransporte
public class MonoBehaviourRunner : MonoBehaviour
{
    public void IniciarTeletransporte()
    {
        StartCoroutine(ExecutarTeletransporteComAtraso());
    }

    private IEnumerator ExecutarTeletransporteComAtraso()
    {
        // Espera o frame atual terminar e a Unity estabilizar todos os objetos na cena
        yield return new WaitForEndOfFrame();

        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            // Recupera a posição salva
            float x = PlayerPrefs.GetFloat("PlayerX", 0);
            float y = PlayerPrefs.GetFloat("PlayerY", 0);
            float z = PlayerPrefs.GetFloat("PlayerZ", 0);
            Vector3 posicaoOriginal = new Vector3(x, y, z);

            Rigidbody2D rb2d = player.GetComponent<Rigidbody2D>();

            if (rb2d != null)
            {
                rb2d.isKinematic = true;
                rb2d.linearVelocity = Vector2.zero;
                // Força o Rigidbody a aceitar a nova posição imediatamente na física
                rb2d.position = new Vector2(x, y);
            }

            // Move o Transform (garante o Z correto também)
            player.transform.position = posicaoOriginal;

            // Espera mais um frame minúsculo antes de devolver a física ao jogador
            yield return new WaitForFixedUpdate();

            if (rb2d != null)
            {
                rb2d.isKinematic = false;
            }

            Debug.Log("Jogador teletransportado com sucesso para: " + posicaoOriginal);
        }
        else
        {
            Debug.LogError("Erro: Não foi possível encontrar um GameObject com a Tag 'Player' na cena carregada!");
        }

        // Destrói o objeto temporário de suporte para não poluir a cena
        Destroy(gameObject);
    }
}
