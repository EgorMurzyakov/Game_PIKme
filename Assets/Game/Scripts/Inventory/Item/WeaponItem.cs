using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Item", menuName = "Inventory/Items/New Weapon Item")]

public class WeaponItem : ItemScriptableObject
{
    [SerializeField] private int baceDamage;

    public override ItemScriptableObject Clone()
    {
        WeaponItem clone = ScriptableObject.CreateInstance<WeaponItem>();

        // Копируем все поля
        clone.type = this.type;
        clone.ItemPrefab = this.ItemPrefab;
        clone.itemName = this.itemName;
        clone.itemDescription = this.itemDescription;
        clone.maximumAmount = this.maximumAmount;
        clone.icon = this.icon;
        clone.baceDamage = this.baceDamage;

        return clone;
    }

    public void Start()
    {
        type = ItemType.Weapon;
    }

    public int GetBaceDamage()
    {
        return baceDamage;
    }

    public void SetBaceDamage(int _baceDm)
    {
        baceDamage = _baceDm;
    }

}
