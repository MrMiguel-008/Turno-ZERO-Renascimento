using UnityEngine;

public class RestaurarPosicaoPlayer : MonoBehaviour
{
    void Start()
    {
        // Verifica se existem posições guardadas
        if (PlayerPrefs.HasKey("PlayerPosX") && PlayerPrefs.HasKey("PlayerPosY"))
        {
            float x = PlayerPrefs.GetFloat("PlayerPosX");
            float y = PlayerPrefs.GetFloat("PlayerPosY");

            transform.position = new Vector3(x, y, transform.position.y);

            // Limpa as chaves para não afetar futuros inícios de jogo limpos
            PlayerPrefs.DeleteKey("PlayerPosX");
            PlayerPrefs.DeleteKey("PlayerPosY");
        }
    }
}