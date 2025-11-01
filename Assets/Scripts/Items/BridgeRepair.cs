using UnityEngine;

public class BridgeRepair : MonoBehaviour, IInteractable
{
    [Header("Configuração")]
    [SerializeField] private ItemData requiredItem;
    [SerializeField] private Sprite repairedSprite;
    [SerializeField] private Collider2D wallCollider; // O colisor que bloqueia a passagem

    private bool isRepaired = false;

    public void Interact(Inventory inventory)
    {
        if (isRepaired) return; // Já foi reparada, não faz nada

        // Verifica se o jogador tem o item necessário
        if (inventory.HasItem(requiredItem))
        {
            inventory.RemoveItem(requiredItem); // Usa o item
            
            // Repara a ponte
            isRepaired = true;
            GetComponent<SpriteRenderer>().sprite = repairedSprite;
            wallCollider.enabled = false;
            
            Debug.Log("A ponte foi consertada!");
        }
        else
        {
            Debug.Log("Você precisa de '" + requiredItem.itemName + "' para consertar a ponte.");
        }
    }

    public string GetPromptMessage()
    {
        if (isRepaired)
        {
            return "A ponte está consertada.";
        }
        return "A ponte está quebrada. Talvez madeira ajude.";
    }
}