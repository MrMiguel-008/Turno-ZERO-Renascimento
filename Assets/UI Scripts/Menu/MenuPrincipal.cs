using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Configuração do jogo")]
    [SerializeField] private string nomeCenaJogo = "JogoPrincipal";

    // Chamado pelo botão JOGAR.
    public void Jogar()
    {
        if (string.IsNullOrWhiteSpace(nomeCenaJogo))
        {
            Debug.LogError(
                "[MenuPrincipal] O nome da cena do jogo está vazio."
            );

            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(nomeCenaJogo))
        {
            Debug.LogError(
                $"[MenuPrincipal] A cena '{nomeCenaJogo}' não está disponível. " +
                "Confira o nome e a Scene List do Build Profile."
            );

            return;
        }

        Debug.Log(
            $"[MenuPrincipal] Abrindo a cena: {nomeCenaJogo}"
        );

        SceneManager.LoadScene(
            nomeCenaJogo,
            LoadSceneMode.Single
        );
    }

    // Chamado pelo botão SAIR.
    public void Sair()
    {
#if UNITY_EDITOR
        Debug.Log(
            "[MenuPrincipal] Encerrando o Play Mode no Editor."
        );

        UnityEditor.EditorApplication.isPlaying = false;
#else
        Debug.Log(
            "[MenuPrincipal] Encerrando o jogo."
        );

        Application.Quit();
#endif
    }
}