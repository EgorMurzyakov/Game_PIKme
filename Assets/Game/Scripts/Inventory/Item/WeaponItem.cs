using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Item", menuName = "Inventory/Items/New Weapon Item")]

public class WeaponItem : ItemScriptableObject
{
    public int baceDamage;

    public void Start()
    {
        type = ItemType.Weapon;
    }
}
