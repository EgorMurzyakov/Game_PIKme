using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Data;
using UnityEditor.Overlays;

public class SaveManager : MonoBehaviour
{
    private string savePath; // Путь к файлу сохранения
    [SerializeField] private InventoryManager inventoryManager;

    public void Start()
    {
        // Задаём путь для сохранения (специальная папка для игры)
        savePath = Application.persistentDataPath + "/money.json";
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SaveGame();
        }
    }

    public void SaveGame()
    {
        SaveDate data = new SaveDate();

        data.money = inventoryManager.playerMoney;
        if (inventoryManager.weaponSlot.item != null)
        {
            data.weaponSlotItem = new ItemSaveDateWeapon(inventoryManager.weaponSlot.item.itemID, 
                ((WeaponItem)inventoryManager.weaponSlot.item).GetBaceDamage(), 
                ((WeaponItem)inventoryManager.weaponSlot.item).GetUpgradePrice());
            //data.weaponSlotItem.itemID = inventoryManager.weaponSlot.item.itemID;
            //data.weaponSlotItem.baseDamage = ((WeaponItem)inventoryManager.weaponSlot.item).GetBaceDamage();
            //data.weaponSlotItem.UpPrice = ((WeaponItem)inventoryManager.weaponSlot.item).GetUpgradePrice();
        }
        else
        {
            data.weaponSlotItem = null;
        }
        if (inventoryManager.bookSlot.item != null)
        {
            data.bookSlotItemID = inventoryManager.bookSlot.item.itemID;
        }
        else
        {
            data.bookSlotItemID = null;
        }
        foreach (InventorySlot _slot in inventoryManager.slots)
        {
            if (_slot.item != null)
            {
                if (_slot.item.type != ItemType.Weapon) // Не оружие
                {
                    data.inventory.Add(new ItemSaveDate(_slot.item.itemID, _slot.amount));
                }
                else
                {
                    data.inventoryWeapon.Add(new ItemSaveDateWeapon(_slot.item.itemID, ((WeaponItem)_slot.item).GetBaceDamage(),
                        ((WeaponItem)_slot.item).GetUpgradePrice()));
                }
            }
        }

        string json = JsonUtility.ToJson(data, true); // true = красивое форматирование
        File.WriteAllText(savePath, json);
        Debug.Log("Сохраняю...");
    }
}

[System.Serializable]
public class SaveDate
{
    public int money;
    public ItemSaveDateWeapon weaponSlotItem;
    public string bookSlotItemID;
    public List<ItemSaveDate> inventory = new List<ItemSaveDate>();
    public List<ItemSaveDateWeapon> inventoryWeapon = new List<ItemSaveDateWeapon>();
}

[System.Serializable]
public class ItemSaveDate
{
    public string itemID;
    public int amount;
    //public int slotIndex; 

    public ItemSaveDate(string _id, int _amt)
    {
        this.itemID = _id;
        this.amount = _amt;
        //this.slotIndex = _slot;
    }
}

[System.Serializable]
public class ItemSaveDateWeapon
{
    public string itemID;
    public int baseDamage;
    public int UpPrice;

    public ItemSaveDateWeapon(string _id, int _bd, int _up)
    {
        this.itemID = _id;
        this.baseDamage = _bd;
        this.UpPrice = _up;  
    }
}

