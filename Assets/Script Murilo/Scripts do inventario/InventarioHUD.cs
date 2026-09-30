using UnityEngine;
using UnityEngine.UI;

public class InventarioHUD : MonoBehaviour
{
    [Header("Configuração do Inventário")]
    [SerializeField] private int espacosInventario = 5; // Quantidade de slots
    [SerializeField] private Image[] slotsIcones;      // Arraste as imagens dos slots da sua UI aqui no Inspector

    private string[] itensNomes;
    private Sprite[] itensIcones;

    void Start()
    {
        itensNomes = new string[espacosInventario];
        itensIcones = new Sprite[espacosInventario];
        AtualizarHUD();
    }

    public bool AdicionarItem(string nome, Sprite icone)
    {
        // Procura o primeiro slot vazio no inventário
        for (int i = 0; i < espacosInventario; i++)
        {
            if (itensNomes[i] == null)
            {
                itensNomes[i] = nome;
                itensIcones[i] = icone;
                AtualizarHUD();
                Debug.Log("Item coletado: " + nome);
                return true; // Item coletado com sucesso
            }
        }

        Debug.Log("Inventário cheio!");
        return false; // Inventário cheio
    }

    void AtualizarHUD()
    {
        for (int i = 0; i < slotsIcones.Length; i++)
        {
            if (i < itensIcones.Length && itensIcones[i] != null)
            {
                slotsIcones[i].sprite = itensIcones[i];
                slotsIcones[i].gameObject.SetActive(true); // Mostra o ícone
            }
            else
            {
                slotsIcones[i].sprite = null;
                slotsIcones[i].gameObject.SetActive(false); // Esconde se estiver vazio
            }
        }
    }
}