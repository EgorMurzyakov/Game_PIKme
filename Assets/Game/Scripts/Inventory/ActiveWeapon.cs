using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private Transform weaponArrayEmpty;
    private int weaponCount;
    private int currentWeaponNumb = -1;

    public void Start()
    {
        weaponCount = weaponArrayEmpty.transform.childCount;
        inventoryManager.ChangeWeapon += SetActivWeapon;
    }

    public void OnDestroy()
    {
        inventoryManager.ChangeWeapon -= SetActivWeapon;
    }

    public void SetActivWeapon(ItemScriptableObject _i)
    {
        Debug.Log("Событие вызвалось");

        if (currentWeaponNumb > -1) // Отключаем предыдущее оружие, если оно было
        {
            weaponArrayEmpty.GetChild(currentWeaponNumb).gameObject.SetActive(false);
        }

        for (int i = 0; i < weaponCount; i++)
        {
            if (weaponArrayEmpty.GetChild(i).gameObject.name == _i.itemName) // Находим нужное оружие по имени
            {
                Debug.Log("Имя подошло");
                weaponArrayEmpty.GetChild(i).gameObject.SetActive(true);
                currentWeaponNumb = i;
                break;
            }
        }
    }

}
