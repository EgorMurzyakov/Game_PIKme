using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Item", menuName = "Inventory/Items/New Weapon Item")]

public class WeaponItem : ItemScriptableObject
{
    [SerializeField] private int baceDamage;
    [SerializeField] private int upgradePrice;
    [SerializeField] private float upgradePriceCoeff;

    public override ItemScriptableObject Clone()
    {
        WeaponItem clone = ScriptableObject.CreateInstance<WeaponItem>();

        // Копируем все поля
        clone.itemID = itemID;
        clone.type = this.type;
        clone.ItemPrefab = this.ItemPrefab;
        clone.itemName = this.itemName;
        clone.itemDescription = this.itemDescription;
        clone.maximumAmount = this.maximumAmount;
        clone.icon = this.icon;
        clone.baceDamage = this.baceDamage;
        clone.upgradePrice = this.upgradePrice;
        clone.upgradePriceCoeff = this.upgradePriceCoeff;

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

    public int GetUpgradePrice()
    {
        return upgradePrice;
    }
    public void UpdateUpgradePrice()
    {
        upgradePrice = (int)(upgradePrice * upgradePriceCoeff);
    }

    public void UpdateUpgradePrice(int _price)
    {
        upgradePrice = _price;
    }

}
