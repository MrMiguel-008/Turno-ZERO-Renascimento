using UnityEngine;

public class CarregarPosicaoJogador : MonoBehaviour
{
    [SerializeField] private Transform jogadorTransform;

    void Start()
    {
        // Se houver progresso salvo, move o jogador para onde ele estava
        if (PlayerPrefs.GetInt("TemProgressoSalvo", 0) == 1)
        {
            float x = PlayerPrefs.GetFloat("PlayerX");
            float y = PlayerPrefs.GetFloat("PlayerY");
            float z = PlayerPrefs.GetFloat("PlayerZ");

            if (jogadorTransform != null)
            {
                jogadorTransform.position = new Vector3(x, y, z);
            }
        }
    }
}