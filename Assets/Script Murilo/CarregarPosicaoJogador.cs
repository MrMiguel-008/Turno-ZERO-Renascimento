using UnityEngine;

public class CarregarPosicaoJogador : MonoBehaviour
{
    [SerializeField] private Transform jogadorTransform;

    void Start()
    {
        // Verifica se existe uma posição salva do jogo
        if (PlayerPrefs.GetInt("TemProgressoSalvo", 0) == 1)
        {
            float x = PlayerPrefs.GetFloat("PlayerX");
            float y = PlayerPrefs.GetFloat("PlayerY");
            float z = PlayerPrefs.GetFloat("PlayerZ");

            // Move o jogador para onde ele estava quando apertou ESC
            jogadorTransform.position = new Vector3(x, y, z);
        }
    }
}