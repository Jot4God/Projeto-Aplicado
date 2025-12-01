using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // Lista de itens do jogador
    [HideInInspector] public List<ShopItem> items = new List<ShopItem>();

    // Adiciona item ao inventário
    public void AddItem(ShopItem item)
    {
        items.Add(item);
        Debug.Log("Item adicionado ao inventário: " + item.itemName);
    }

    // Remove item do inventário
    public void RemoveItem(ShopItem item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            Debug.Log("Item removido do inventário: " + item.itemName);
        }
    }
}
