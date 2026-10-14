using UnityEngine;

public class ItemColetavel : MonoBehaviour
{
    [Header("Configurações do Item")]
    [SerializeField] private string nomeItem; // Ex: "Chave", "CaixaDeFerramentas"
    [SerializeField] private Sprite iconeItem;
    [SerializeField] private float distanciaColeta = 2f;
    [SerializeField] private KeyCode teclaColeta = KeyCode.F;

    private Transform playerTransform;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distancia = Vector2.Distance(transform.position, playerTransform.position);

        if (distancia <= distanciaColeta && Input.GetKeyDown(teclaColeta))
        {
            Coletar();
        }
    }

    void Coletar()
    {
        InventarioHUD inventario = FindFirstObjectByType<InventarioHUD>();
        if (inventario != null)
        {
            bool foiAdicionado = inventario.AdicionarItem(nomeItem, iconeItem);

            if (foiAdicionado)
            {
                // **NOVIDADE:** Avisa o QuestManager que um item foi coletado
                if (QuestManager.Instance != null)
                {
                    QuestManager.Instance.CheckItemCollectionQuest(nomeItem);
                }

                Destroy(gameObject);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, distanciaColeta);
    }
}