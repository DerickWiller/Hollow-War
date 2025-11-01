using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private Inventory inventory;
    private IInteractable currentInteractable; // O objeto com o qual podemos interagir agora

    void Awake()
    {
        inventory = GetComponent<Inventory>();
    }

    void Update()
    {
        // Lógica de interação com 'E'
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.Interact(inventory);
        }

        // Lógica para mostrar inventário com 'I'
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventory.DisplayItems();
        }
    }

    // Chamado quando o nosso trigger entra em contato com outro collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Tenta pegar o componente que implementa IInteractable no objeto que colidimos
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            currentInteractable = interactable;
            Debug.Log("Pode interagir com: " + currentInteractable.GetPromptMessage());
        }
    }

    // Chamado quando o nosso trigger para de tocar no outro collider
    private void OnTriggerExit2D(Collider2D other)
    {
        // Se o objeto que estamos saindo é o nosso interativo atual, limpamos a referência
        if (other.GetComponent<IInteractable>() == currentInteractable)
        {
            currentInteractable = null;
            Debug.Log("Fora do alcance de interação.");
        }
    }
}