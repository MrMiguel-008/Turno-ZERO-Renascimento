using UnityEngine;
using UnityEngine.SceneManagement;

public class Bancada : MonoBehaviour
{
    private static bool MinigameFeito = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !MinigameFeito)
        {
            MinigameFeito = true;

            PlayerPrefs.SetString("CenaOriginalNome", SceneManager.GetActiveScene().name);
            PlayerPrefs.SetFloat("PlayerX", collision.transform.position.x);
            PlayerPrefs.SetFloat("PlayerY", collision.transform.position.y);
            PlayerPrefs.SetFloat("PlayerZ", collision.transform.position.z);
            PlayerPrefs.Save();

            SceneManager.LoadScene(1);
        }
    }
}
