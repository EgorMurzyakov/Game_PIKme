using UnityEngine;

public enum ItemType { Default, Food, Weapon, Book, Money }

public class ItemScriptableObject : ScriptableObject
{
    public ItemType type;
    public GameObject ItemPrefab;
    public string itemName;
    public string itemDescription;
    public int maximumAmount;
    public Sprite icon;

    public virtual ItemScriptableObject Clone()
    {
        ItemScriptableObject clone = ScriptableObject.CreateInstance<ItemScriptableObject>();

        // Копируем все поля
        clone.type = this.type;
        clone.ItemPrefab = this.ItemPrefab;
        clone.itemName = this.itemName;
        clone.itemDescription = this.itemDescription;
        clone.maximumAmount = this.maximumAmount;
        clone.icon = this.icon;

        return clone;
    }

}
