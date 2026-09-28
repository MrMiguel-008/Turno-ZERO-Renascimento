using UnityEngine;

public class ItemColetavel : MonoBehaviour
{
    [Header("Configurações do Item")]
    [SerializeField] private string nomeItem;
    [SerializeField] private Sprite iconeItem; // Ícone que vai aparecer no HUD do inventário
    [SerializeField] private float distanciaColeta = 2f;
    [SerializeField] private KeyCode teclaColeta = KeyCode.F; // Tecla para pegar

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

        // Se estiver perto e apertar a tecla de coleta
        if (distancia <= distanciaColeta && Input.GetKeyDown(teclaColeta))
        {
            Coletar();
        }
    }

    void Coletar()
    {
        // Procura o inventário na cena e adiciona o item
        InventarioHUD inventario = FindFirstObjectByType<InventarioHUD>();
        if (inventario != null)
        {
            bool foiAdicionado = inventario.AdicionarItem(nomeItem, iconeItem);

            if (foiAdicionado)
            {
                // Destri o objeto do cenário estilo Minecraft ao coletar
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