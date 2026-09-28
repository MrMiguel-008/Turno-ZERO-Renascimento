using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestTrigger : MonoBehaviour
{
    [Header("O que este objeto representa?")]
    public QuestType triggerType;
    [Tooltip("Deve ser exatamente igual ao 'targetIdentifier' configurado na lista do QuestManager.")]
    public string myIdentifier;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && triggerType == QuestType.EnterArea)
        {
            ProcessTrigger();
        }
    }

    public void Interact()
    {
        if (triggerType == QuestType.TalkToNPC || triggerType == QuestType.InteractWithItem || triggerType == QuestType.ChangeScene)
        {
            ProcessTrigger();
        }
    }

    private void ProcessTrigger()
    {
        if (QuestManager.Instance == null) return;

        QuestManager.Instance.CheckObjective(triggerType, myIdentifier);

        if (triggerType == QuestType.ChangeScene)
        {
            if (QuestManager.Instance.CurrentQuest != null && QuestManager.Instance.CurrentQuest.targetIdentifier == myIdentifier)
            {
                PlayerPrefs.SetString("PreviousScene", SceneManager.GetActiveScene().name);
                SceneManager.LoadScene(myIdentifier);
            }
        }
    }
}