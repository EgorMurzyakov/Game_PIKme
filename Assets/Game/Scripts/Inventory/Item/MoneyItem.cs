using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Money Item", menuName = "Inventory/Items/New Money Item")]

public class MoneyItem : ItemScriptableObject
{    

    public override ItemScriptableObject Clone()
    {
        FoodItem clone = ScriptableObject.CreateInstance<FoodItem>();

        // �������� ��� ����
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
        type = ItemType.Money;
    }
}
