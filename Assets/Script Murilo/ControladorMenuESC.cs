using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorMenuESC : MonoBehaviour
{
    [SerializeField] private Transform jogadorTransform;

    void Update()
    {
        // Ao apertar ESC, salva a posição e vai para o Menu Principal
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IrParaMenuPrincipal();
        }
    }

    public void IrParaMenuPrincipal()
    {
        if (jogadorTransform != null)
        {
            PlayerPrefs.SetFloat("PlayerX", jogadorTransform.position.x);
            PlayerPrefs.SetFloat("PlayerY", jogadorTransform.position.y);
            PlayerPrefs.SetFloat("PlayerZ", jogadorTransform.position.z);
            PlayerPrefs.SetInt("TemProgressoSalvo", 1);
            PlayerPrefs.Save();
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    }
}