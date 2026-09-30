using UnityEngine;

public class AreaTrigger : MonoBehaviour
{
    [Header("Identificação da Área")]
    [Tooltip("Nome exato da área que será enviado para o sistema de diálogo (Ex: Recepcao, Cozinha).")]
    public string areaName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se quem entrou na área foi o Player
        if (collision.CompareTag("Player"))
        {
            // Opcional: Se quiser que a área mude baseada onde o player pisa,
            // podemos atualizar um GameManager global ou diretamente o NPC Supervisor.
            Debug.Log($"Player entrou na área: {areaName}");

            // Exemplo buscando o NPC na cena para atualizar a área dele automaticamente:
            NPCSupervisor supervisor = FindObjectOfType<NPCSupervisor>();
            if (supervisor != null)
            {
                supervisor.currentLocation = areaName;
            }
        }
    }
}