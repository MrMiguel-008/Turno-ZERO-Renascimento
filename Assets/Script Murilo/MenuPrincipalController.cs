using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipalController : MonoBehaviour
{
    public void BotaoJogar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("JogoPrincipal"); // Nome exato da sua cena do jogo
    }
}