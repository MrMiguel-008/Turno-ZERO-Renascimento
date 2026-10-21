using UnityEngine;
using UnityEngine.SceneManagement;

public class AlternarMenuESC : MonoBehaviour
{
    [Header("Nomes Exatos das Cenas")]
    [SerializeField] private string nomeCenaJogo = "JogoPrincipal";
    [SerializeField] private string nomeCenaMenu = "MenuPrincipal"; // Substitui pelo nome exato da tua cena de menu principal

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            string cenaAtual = SceneManager.GetActiveScene().name;

            // Se estás no Jogo Principal, aperta ESC para ir ao Menu Principal (e pausa o jogo)
            if (cenaAtual == nomeCenaJogo)
            {
                Time.timeScale = 0f;
                SceneManager.LoadScene(nomeCenaMenu);
            }
            // Se estás no Menu Principal, aperta ESC para voltar ao Jogo Principal (e retoma o jogo)
            else if (cenaAtual == nomeCenaMenu)
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(nomeCenaJogo);
            }
        }
    }
}