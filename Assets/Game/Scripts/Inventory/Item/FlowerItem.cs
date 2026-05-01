using UnityEngine;

[CreateAssetMenu(fileName = "Flower Item", menuName = "Inventory/Items/New Flower Item")]
public class FlowerItem : ItemScriptableObject
{
    private void Awake()
    {
        if (string.IsNullOrEmpty(itemID))
            itemID = "FlowerItem";
    }

    public override ItemScriptableObject Clone()
    {
        FlowerItem clone = ScriptableObject.CreateInstance<FlowerItem>();

        clone.itemID = itemID;
        clone.type = this.type;
        clone.ItemPrefab = this.ItemPrefab;
        clone.itemName = this.itemName;
        clone.itemDescription = this.itemDescription;
        clone.maximumAmount = this.maximumAmount;
        clone.icon = this.icon;

        return clone;
    }

    public void Start()
    {
        type = ItemType.Flower;
    }

    public bool IsBlackRose()
    {
        string id = itemID != null ? itemID.ToLowerInvariant() : "";
        string name = itemName != null ? itemName.ToLowerInvariant() : "";
        return id == "black_rose" || id == "chornaya_roza" || name.Contains("роза") || name.Contains("rose");
    }
} 
