using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Food Item", menuName = "Inventory/Items/New Food Item")]

public class FoodItem : ItemScriptableObject
{
    public int healthAmount;

    public void Start()
    {
        type = ItemType.Food;
    }
}
