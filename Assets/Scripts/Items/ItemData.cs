using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string description;
    // No futuro, você pode adicionar:
    // public Sprite icon;
    // public GameObject itemPrefab;
}