using UnityEngine;

public class CarregarPosicaoJogador : MonoBehaviour
{
    [SerializeField] private Transform jogadorTransform;

    void Start()
    {
        // Só carrega a posição se "TemProgressoSalvo" for igual a 1
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