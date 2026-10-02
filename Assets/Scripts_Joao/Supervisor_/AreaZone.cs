using UnityEngine;

public class AreaZone : MonoBehaviour
{
    [Header("Nome da Área")]
    public string areaName; // Ex: "Recepcao", "Cozinha"

    private void OnTriggerEnter2D(Collider2D collision)
    {
        VerificarEAtualizar(collision, areaName);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        VerificarEAtualizar(collision, areaName);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Supervisor") || collision.CompareTag("Player") || collision.GetComponent<NPCSupervisorController>() != null)
        {
            NPCSupervisorController supervisor = FindObjectOfType<NPCSupervisorController>();
            if (supervisor != null)
            {
                if (supervisor.currentLocation == areaName)
                {
                    supervisor.currentLocation = "Corredor";
                }
            }
        }
    }

    void VerificarEAtualizar(Collider2D collision, string novaArea)
    {
        if (collision.CompareTag("Supervisor") || collision.CompareTag("Player") || collision.GetComponent<NPCSupervisorController>() != null)
        {
            NPCSupervisorController supervisor = FindObjectOfType<NPCSupervisorController>();
            if (supervisor != null)
            {
                supervisor.currentLocation = novaArea;

                // **NOVIDADE:** Informa o QuestManager que o player entrou nesta área
                if (QuestManager.Instance != null)
                {
                    QuestManager.Instance.CheckAreaQuest(novaArea);
                }
            }
        }
    }
}