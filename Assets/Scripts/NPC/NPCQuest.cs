using UnityEditorInternal;
using UnityEngine;




public class NPCQuest : MonoBehaviour, IInteractable 
{
    [Header("Referência ao Sistema de Diálogo")]
    [SerializeField] private DialogueSystem dialogueSystem;

    [Header("Diálogos baseados no estado")]
    [SerializeField] private DialogueData dialogueNotStarted;       
    [SerializeField] private DialogueData dialogueNoItem;           
    [SerializeField] private DialogueData dialogueCompleted;        
    [SerializeField] private DialogueData dialogueCompletedNoItem;  

    private enum QuestState { NotStarted, Started, CompletedNoItem, Completed }
    [SerializeField] private QuestState state = QuestState.NotStarted; // Serializado para persistir

    [Header("Item Necessário para Completar")]
    [SerializeField] private ItemData requiredItem;

    [Header("Recompensa (Opcional)")]
    [SerializeField] private ItemData rewardItem;

    [Header("Identificador Único do NPC")]
    [Tooltip("ID único para salvar o estado da quest.")]
    [SerializeField] private string npcId;

    void OnEnable() 
    {
        // Assina o evento de Game Over
        GameManager.OnGameOver += ResetQuestOnGameOver;
    }

    void OnDisable() 
    {
        // Desassina o evento para evitar erros
        GameManager.OnGameOver -= ResetQuestOnGameOver;
    }

    void Start() 
    {
        if (string.IsNullOrEmpty(npcId))
        {
            npcId = gameObject.name;
        }
        // Validações...

        // Carrega o estado salvo da quest
        LoadQuestStateFromGameManager();
    }
    
    /// <summary>
    /// Reseta o estado da quest para NotStarted apenas no Game Over.
    /// Chamado pelo evento GameManager.OnGameOver.
    /// </summary>
    void ResetQuestOnGameOver() 
    {
        state = QuestState.NotStarted;
        // Salva o reset no GameManager
        SaveQuestStateToGameManager();
        Debug.Log($"Quest do NPC {gameObject.name} resetada devido ao Game Over.");
    }


    void LoadQuestStateFromGameManager()
    {
        if (GameManager.Instance == null) return;

        int savedState = GameManager.Instance.LoadQuestState(npcId);
        
        if (savedState != -1)
        {
            state = (QuestState)savedState;
            Debug.Log($"Quest de '{npcId}' carregada com estado: {state}");
        }
    }

    void SaveQuestStateToGameManager()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveQuestState(npcId, (int)state);
        }
    }

    public string GetPromptMessage() 
    {
        // ... (lógica de GetPromptMessage)
        switch (state) 
        {
            case QuestState.NotStarted: return "Falar";
            case QuestState.Started: return "Entregar Item";
            case QuestState.CompletedNoItem:
            case QuestState.Completed: return "Conversar";
            default: return "Interagir";
        }
    }

    public void Interact(Inventory inventory) 
    {
        // ... (lógica de Interact)
        if (dialogueSystem == null || dialogueSystem.IsActive()) return;

        switch (state) 
        {
            case QuestState.NotStarted: StartQuest(); break;
            case QuestState.Started: CheckQuestCompletion(inventory); break;
            case QuestState.Completed: ShowCompletionDialogue(); break;
            case QuestState.CompletedNoItem: ShowPostCompletionDialogue(); break;
        }
    }

    void StartQuest() 
    {
        if (dialogueNotStarted != null) 
        {
            dialogueSystem.SetDialogue(dialogueNotStarted);
            dialogueSystem.StartDialogue();
            state = QuestState.Started;
            SaveQuestStateToGameManager();
        } 
        else 
        {
            Debug.LogWarning("NPCQuest: dialogueNotStarted não atribuído!");
        }
    }

    void CheckQuestCompletion(Inventory inventory) 
    {
        // ... (Verificações de null)

        if (inventory.HasItem(requiredItem) && state == QuestState.Started) 
        {
            inventory.RemoveItem(requiredItem);
            state = QuestState.CompletedNoItem;

            if (rewardItem != null) inventory.AddItem(rewardItem);

            dialogueSystem.SetDialogue(dialogueCompleted);
            dialogueSystem.StartDialogue();

            SaveQuestStateToGameManager();
        } 
        else 
        {
            dialogueSystem.SetDialogue(dialogueNoItem);
            dialogueSystem.StartDialogue();
        }
    }

    void ShowCompletionDialogue() 
    {
        dialogueSystem.SetDialogue(dialogueCompleted);
        dialogueSystem.StartDialogue();
    }

    void ShowPostCompletionDialogue() 
    {
        if (dialogueCompletedNoItem != null) 
        {
            dialogueSystem.SetDialogue(dialogueCompletedNoItem);
            dialogueSystem.StartDialogue();
        }
    }

    [ContextMenu("Reset Quest")]
    public void ResetQuest() 
    {
        state = QuestState.NotStarted;
        SaveQuestStateToGameManager();
    }

    

}

