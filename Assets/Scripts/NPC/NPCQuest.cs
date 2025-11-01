using UnityEngine;

public class NPCQuest : MonoBehaviour, IInteractable // Implementa nossa interface existente!
{
    // Enum para controlar o estado da quest
    private enum QuestState { NotStarted, Started, Completed }
    private QuestState currentState = QuestState.NotStarted;

    [Header("Quest Item")]
    [SerializeField] private ItemData requiredItem;
    
    [Header("Dialogues")]
    [TextArea(3, 10)] // Faz a caixa de texto no Inspector ser maior
    [SerializeField] private string dialogue_FirstTime;
    [TextArea(3, 10)]
    [SerializeField] private string dialogue_QuestInProgress;
    [TextArea(3, 10)]
    [SerializeField] private string dialogue_QuestComplete;
    [TextArea(3, 10)]
    [SerializeField] private string dialogue_AfterQuest;

    // --- Implementação do Contrato IInteractable ---

    public string GetPromptMessage()
    {
        // A mensagem pode mudar dependendo do estado
        if (currentState == QuestState.Completed)
        {
            return "Falar com o Aldeão";
        }
        return "Falar com o Aldeão da Quest";
    }

    public void Interact(Inventory inventory)
    {
        // A lógica principal acontece aqui, baseada no estado atual da quest
        switch (currentState)
        {
            case QuestState.NotStarted:
                // Primeira vez falando com o NPC
                Debug.Log("NPC: " + dialogue_FirstTime);
                currentState = QuestState.Started; // A quest começa agora!
                break;

            case QuestState.Started:
                // A quest está ativa. Verificamos se o player tem o item.
                if (inventory.HasItem(requiredItem))
                {
                    // Player tem o item!
                    Debug.Log("NPC: " + dialogue_QuestComplete);
                    inventory.RemoveItem(requiredItem); // Remove o item do inventário
                    currentState = QuestState.Completed; // A quest está completa!
                }
                else
                {
                    // Player ainda não tem o item.
                    Debug.Log("NPC: " + dialogue_QuestInProgress);
                }
                break;

            case QuestState.Completed:
                // A quest já foi completada.
                Debug.Log("NPC: " + dialogue_AfterQuest);
                break;
        }
    }
}