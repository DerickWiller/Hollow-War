using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable // Note a vírgula e o IInteractable
{
    [SerializeField] private ItemData itemToGive;

    public void Interact(Inventory inventory)
    {
        inventory.AddItem(itemToGive);
        Destroy(gameObject); // O item some do mundo após ser pego
    }

    public string GetPromptMessage()
    {
        return "Apertar 'E' para pegar " + itemToGive.itemName;
    }
}