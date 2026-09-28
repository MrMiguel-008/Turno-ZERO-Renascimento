using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private QuestTrigger currentInteractable;

    [Header("Configuração de UI de Interação")]
    public GameObject interactionPromptUI;

    private void Start()
    {
        if (interactionPromptUI != null)
        {
            interactionPromptUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentInteractable != null)
            {
                currentInteractable.Interact();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        QuestTrigger questTrigger = collision.GetComponent<QuestTrigger>();

        if (questTrigger != null)
        {
            currentInteractable = questTrigger;
            if (interactionPromptUI != null) interactionPromptUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        QuestTrigger questTrigger = collision.GetComponent<QuestTrigger>();

        if (questTrigger != null && currentInteractable == questTrigger)
        {
            currentInteractable = null;
            if (interactionPromptUI != null) interactionPromptUI.SetActive(false);
        }
    }
}