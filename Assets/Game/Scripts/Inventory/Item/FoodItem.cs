using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Food Item", menuName = "Inventory/Items/New Food Item")]

public class FoodItem : ItemScriptableObject
{
    public int healthAmount;
    [SerializeField] private float upCoeff = 1f;
    [SerializeField] private int durationEffect = 0;

    public override ItemScriptableObject Clone()
    {
        FoodItem clone = ScriptableObject.CreateInstance<FoodItem>();

        // Копируем все поля
        clone.type = this.type;
        clone.ItemPrefab = this.ItemPrefab;
        clone.itemName = this.itemName;
        clone.itemDescription = this.itemDescription;
        clone.maximumAmount = this.maximumAmount;
        clone.icon = this.icon;
        clone.healthAmount = this.healthAmount;
        clone.upCoeff = this.upCoeff;
        clone.durationEffect = this.durationEffect;


        return clone;
    }
    public void Start()
    {
        type = ItemType.Food;
    }

    public float GetUpCoeff()
    {
        return upCoeff;
    }

    public int GetDurationEffect()
    {
        return durationEffect;
    }
}
