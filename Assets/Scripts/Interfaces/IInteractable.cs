// Uma interface não herda de MonoBehaviour
public interface IInteractable
{
    // Qualquer script que implementar esta interface DEVE ter esta função
    void Interact(Inventory inventory);

    // Também é útil ter uma mensagem para mostrar ao jogador
    string GetPromptMessage();
}