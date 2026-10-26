using UnityEngine;

public class ControleMenuPausa : MonoBehaviour
{
    [Header("Referência ao Canvas do Menu")]
    [SerializeField] private GameObject canvasMenu;

    void Update()
    {
        // Ao carregar na tecla ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AlternarEstadoMenu();
        }
    }

    public void AlternarEstadoMenu()
    {
        if (canvasMenu != null)
        {
            // Inverte a visibilidade
            bool estaAtivo = !canvasMenu.activeSelf;
            canvasMenu.SetActive(estaAtivo);

            // Pausa o tempo se o menu abrir (0f) e retoma se fechar (1f)
            Time.timeScale = estaAtivo ? 0f : 1f;
        }
    }

    // Função para ligar diretamente ao botão "JOGAR / CONTINUAR" do Menu
    public void BotaoContinuar()
    {
        if (canvasMenu != null)
        {
            canvasMenu.SetActive(false);
        }
        Time.timeScale = 1f; // Retoma o jogo normalmente
    }
} 