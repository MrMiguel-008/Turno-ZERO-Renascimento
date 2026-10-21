using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorMenuESC : MonoBehaviour
{
    [SerializeField] private Transform jogadorTransform; // Arraste seu Player aqui no Inspector

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IrParaMenuPrincipal();
        }
    }

    public void IrParaMenuPrincipal()
    {
        // Salva as coordenadas do jogador
        if (jogadorTransform != null)
        {
            PlayerPrefs.SetFloat("PlayerX", jogadorTransform.position.x);
            PlayerPrefs.SetFloat("PlayerY", jogadorTransform.position.y);
            PlayerPrefs.SetFloat("PlayerZ", jogadorTransform.position.z);
            PlayerPrefs.SetInt("TemProgressoSalvo", 1); // Marca que existe um salvamento
            PlayerPrefs.Save();
        }

        Time.timeScale = 1f; // Garante que o tempo continue normal
        SceneManager.LoadScene("MenuPrincipal"); // Nome exato da cena do menu
    }
}