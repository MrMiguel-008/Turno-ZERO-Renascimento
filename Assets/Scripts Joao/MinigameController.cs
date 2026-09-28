using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameController : MonoBehaviour
{
    public void FinishMinigame()
    {
        string previousScene = PlayerPrefs.GetString("PreviousScene", "SampleScene");
        SceneManager.LoadScene(previousScene);
    }
}