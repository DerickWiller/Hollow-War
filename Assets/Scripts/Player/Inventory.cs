using System.Collections.Generic;
using UnityEngine;
using System.Text; // Usado para construir a string do inventário

public class Inventory : MonoBehaviour
{
    // Usamos uma lista para guardar os itens que o player coletou
    public List<ItemData> items = new List<ItemData>();

    public void AddItem(ItemData itemToAdd)
    {
        items.Add(itemToAdd);
        Debug.Log("Adicionado ao inventário: " + itemToAdd.itemName);
    }

    public void RemoveItem(ItemData itemToRemove)
    {
        if (items.Contains(itemToRemove))
        {
            items.Remove(itemToRemove);
            Debug.Log("Removido do inventário: " + itemToRemove.itemName);
        }
    }

    public bool HasItem(ItemData itemToCheck)
    {
        return items.Contains(itemToCheck);
    }

    public void DisplayItems()
    {
        if (items.Count == 0)
        {
            Debug.Log("O inventário está vazio.");
            return;
        }

        // StringBuilder é mais eficiente para montar strings grandes
        StringBuilder sb = new StringBuilder("Inventário:\n");
        foreach (ItemData item in items)
        {
            sb.AppendLine("- " + item.itemName);
        }
        Debug.Log(sb.ToString());
    }
}