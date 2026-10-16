using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public struct ItemSlot
{
    public string nomeItem;
    public Sprite iconeItem;
}

public class InventarioHUD : MonoBehaviour
{
    public static InventarioHUD Instance;

    [Header("Lista de Itens Guardados")]
    public List<ItemSlot> itensGuardados = new List<ItemSlot>();

    [Header("Configuração Visual do HUD")]
    [Tooltip("O painel (Contentor com GridLayoutGroup ou HorizontalLayoutGroup) onde os ícones vão aparecer.")]
    public Transform containerHUD;
    [Tooltip("Um prefab de UI simples que contenha apenas um componente Image para representar o ícone do item.")]
    public GameObject iconePrefabUI;

    // Dicionário interno para controlar os ícones gerados na tela
    private Dictionary<string, GameObject> iconesInstanciados = new Dictionary<string, GameObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Adiciona o item ao inventário garantindo que não duplica o mesmo tipo (máximo 1 por tipo)
    public bool AdicionarItem(string nome, Sprite icone)
    {
        // Verifica se já tem o item no inventário
        foreach (var slot in itensGuardados)
        {
            if (slot.nomeItem == nome)
            {
                Debug.Log($"Já tens o item '{nome}' no inventário!");
                return false;
            }
        }

        // Guarda o novo item único
        ItemSlot novoSlot = new ItemSlot
        {
            nomeItem = nome,
            iconeItem = icone
        };

        itensGuardados.Add(novoSlot);
        Debug.Log($"Item único guardado: {nome}");

        // Atualiza a interface gráfica para mostrar o novo item na lista
        AtualizarUIInventario(novoSlot);
        return true;
    }

    public bool PossuiItem(string nomeItem)
    {
        foreach (var slot in itensGuardados)
        {
            if (slot.nomeItem == nomeItem) return true;
        }
        return false;
    }

    // Cria visualmente o ícone do item recolhido no painel do HUD
    void AtualizarUIInventario(ItemSlot novoItem)
    {
        if (containerHUD != null && iconePrefabUI != null)
        {
            // Instancia o prefab do ícone dentro do painel do inventário
            GameObject novoIconeObj = Instantiate(iconePrefabUI, containerHUD);

            // Altera a imagem do prefab para o sprite do item coletado
            Image imagemItem = novoIconeObj.GetComponent<Image>();
            if (imagemItem != null && novoItem.iconeItem != null)
            {
                imagemItem.sprite = novoItem.iconeItem;
            }

            novoIconeObj.SetActive(true);

            // Guarda a referência caso precises de removê-lo no futuro
            if (!iconesInstanciados.ContainsKey(novoItem.nomeItem))
            {
                iconesInstanciados.Add(novoItem.nomeItem, novoIconeObj);
            }
        }
    }
}