using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorMenuESC : MonoBehaviour
{
    [SerializeField] private string nomeCenaMenu = "MenuPrincipal";

    void Update()
    {
        // Detecta quando a tecla ESC é pressionada
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Verifica qual cena está ativa para decidir o comportamento
            if (SceneManager.GetActiveScene().name == nomeCenaMenu)
            {
                // Se já estiver no menu, você pode optar por sair do jogo ou fechar um painel
                Debug.Log("Já está no Menu Principal.");
            }
            else
            {
                // Se estiver no jogo principal, carrega a cena do menu
                SceneManager.LoadScene(nomeCenaMenu);
            }
        }
    }
}
