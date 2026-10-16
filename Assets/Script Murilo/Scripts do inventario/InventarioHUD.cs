using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct ItemSlot
{
    public string nomeItem;
    public Sprite iconeItem;
    public int quantidade;
}

public class InventarioHUD : MonoBehaviour
{
    public static InventarioHUD Instance;

    [Header("Lista de Itens no Inventário")]
    public List<ItemSlot> itensGuardados = new List<ItemSlot>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Adiciona o item ao inventário (chamado pelo ItemColetavel)
    public bool AdicionarItem(string nome, Sprite icone)
    {
        // Procura se já tem o item no inventário para apenas somar a quantidade
        foreach (var slot in itensGuardados)
        {
            if (slot.nomeItem == nome)
            {
                // Como structs são por valor, precisamos atualizar a lista por índice ou recriar
                int index = itensGuardados.FindIndex(i => i.nomeItem == nome);
                if (index != -1)
                {
                    ItemSlot atualizado = itensGuardados[index];
                    atualizado.quantidade++;
                    itensGuardados[index] = atualizado;

                    Debug.Log($"Item incrementado no inventário: {nome} (Total: {atualizado.quantidade})");
                    AtualizarUIInventario();
                    return true;
                }
            }
        }

        // Se não existir, cria um novo slot
        ItemSlot novoSlot = new ItemSlot
        {
            nomeItem = nome,
            iconeItem = icone,
            quantidade = 1
        };

        itensGuardados.Add(novoSlot);
        Debug.Log($"Novo item adicionado ao inventário: {nome}");

        AtualizarUIInventario();
        return true;
    }

    // Verifica se o player possui uma quantidade específica de um determinado item
    public bool PossuiItem(string nomeItem, int quantidadeDesejada = 1)
    {
        foreach (var slot in itensGuardados)
        {
            if (slot.nomeItem == nomeItem)
            {
                return slot.quantidade >= quantidadeDesejada;
            }
        }
        return false;
    }

    void AtualizarUIInventario()
    {
        // Aqui você pode adicionar lógica visual futura para desenhar os ícones no ecrã do Canvas.
        // Por enquanto, gere os dados com segurança em segundo plano.
    }
}