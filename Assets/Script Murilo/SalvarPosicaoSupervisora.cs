using UnityEngine;

public class SalvarPosicaoSupervisora : MonoBehaviour
{
    void Start()
    {
        // Quando a cena do jogo abre, verifica se existem coordenadas salvas para a supervisora
        if (PlayerPrefs.HasKey("SupervisoraX") && PlayerPrefs.HasKey("SupervisoraY"))
        {
            float x = PlayerPrefs.GetFloat("SupervisoraX");
            float y = PlayerPrefs.GetFloat("SupervisoraY");
            float z = PlayerPrefs.GetFloat("SupervisoraZ");

            // Coloca a supervisora exatamente no último lugar onde ela estava
            transform.position = new Vector3(x, y, z);
        }
    }

    void OnDisable()
    {
        // Assim que saíres da cena (por exemplo, ao apertar ESC para ir ao menu), 
        // este método guarda automaticamente a posição atual dela
        PlayerPrefs.SetFloat("SupervisoraX", transform.position.x);
        PlayerPrefs.SetFloat("SupervisoraY", transform.position.y);
        PlayerPrefs.SetFloat("SupervisoraZ", transform.position.z);
        PlayerPrefs.Save();
    }
}
