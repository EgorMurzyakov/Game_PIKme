using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Data;
using UnityEditor.Overlays;
using UnityEngine.SceneManagement;
using System.Collections;

public class SaveManager : MonoBehaviour
{
    private string savePath; // Путь к файлу сохранения
    private string savePathScene_1;
    private string savePathScene_2;
    private string savePathScene_3;
    private string savePathScene_4;
    private string savePathScene_test;
    private string currentSavePathScene;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private Transform saveItem; // Пустышка с предметами коротые нужно сохранять  

    public void Start()
    {
        // Задаём путь для сохранения (специальная папка для игры)
        savePath = Application.persistentDataPath + "/money.json";
        savePathScene_1 = Application.persistentDataPath + "/scene1.json";
        savePathScene_2 = Application.persistentDataPath + "/scene2.json";
        savePathScene_3 = Application.persistentDataPath + "/scene3.json";
        savePathScene_4 = Application.persistentDataPath + "/scene4.json";
        savePathScene_test = Application.persistentDataPath + "/sceneTest.json";

        StartCoroutine(InitializeWeaponLater());
        //LoadSceneItem();
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
        SaveScene();
        SaveInventory();

        Debug.Log("Сохраняю...");
    }

    public void SaveInventory()
    {
        SaveDate data = new SaveDate();

        data.money = inventoryManager.playerMoney; // Деньги

        if (inventoryManager.weaponSlot.item != null) // Слот оружия
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

        if (inventoryManager.bookSlot.item != null) // Слот книги
        {
            data.bookSlotItemID = inventoryManager.bookSlot.item.itemID;
        }
        else
        {
            data.bookSlotItemID = null;
        }

        foreach (InventorySlot _slot in inventoryManager.slots) // Инчентарь
        {
            if (_slot.item != null)
            {
                if (_slot.item.type == ItemType.Weapon) // Оружие
                {
                    data.inventoryWeapon.Add(new ItemSaveDateWeapon(_slot.item.itemID, ((WeaponItem)_slot.item).GetBaceDamage(),
                        ((WeaponItem)_slot.item).GetUpgradePrice()));
                }
                else // Остальное (включая книги)
                {
                    data.inventory.Add(new ItemSaveDate(_slot.item.itemID, _slot.amount));
                }
            }
        }

        string json = JsonUtility.ToJson(data, true); // true = красивое форматирование
        File.WriteAllText(savePath, json);
    }

    public void SaveScene()
    {
        // Получаем имя текущей сцены
        string currentLocation = SceneManager.GetActiveScene().name;

        switch (currentLocation)
        {
            case "village1 valera":
                currentSavePathScene = savePathScene_1;
                break;
            case "VladTest2":
                currentSavePathScene = savePathScene_2;
                break;
            //case "":
            //    break;
            case "ValeraAD_old_from_commit":
                currentSavePathScene = savePathScene_4;
                break;
            case "EgorTest":
                currentSavePathScene = savePathScene_test;
                break;
        }

        SaveSceneData sceneData = new SaveSceneData();

        for (int i = 0; i < saveItem.childCount; i++)
        {
            //sceneData.itemSceneData.Add(new ItemSaveScaneData(saveItem.GetChild(i).position,
            //    saveItem.GetChild(i).GetComponent<Item>().itemScriptableObject.itemID,
            //    saveItem.GetChild(i).GetComponent<Item>().amount));
            sceneData.itemName.Add(saveItem.GetChild(i).name);
        }

        string json = JsonUtility.ToJson(sceneData, true); // true = красивое форматирование
        File.WriteAllText(currentSavePathScene, json);
    }

    public void LoadSceneItem()
    {
        // Получаем имя текущей сцены
        string currentLocation = SceneManager.GetActiveScene().name;

        switch (currentLocation)
        {
            case "village1 valera":
                currentSavePathScene = savePathScene_1;
                break;
            case "VladTest2":
                currentSavePathScene = savePathScene_2;
                break;
            //case "":
            //    break;
            case "ValeraAD_old_from_commit":
                currentSavePathScene = savePathScene_4;
                break;
            case "EgorTest":
                currentSavePathScene = savePathScene_test;
                break;
        }

        if (File.Exists(currentSavePathScene))
        {
            string json = File.ReadAllText(currentSavePathScene);
            SaveSceneData sceneData = JsonUtility.FromJson<SaveSceneData>(json);

            if (sceneData != null)
            {
                for (int i = 0; i < saveItem.childCount; i++)
                {
                    foreach (string _name in sceneData.itemName)
                    {
                        if (saveItem.GetChild(i).name == _name)
                        {
                            saveItem.GetChild(i).gameObject.SetActive(true);
                            continue;
                        }
                    }
                }
            }
        }
    }

    IEnumerator InitializeWeaponLater()
    {
        // Ждем 1 кадр, чтобы все Start() выполнились
        yield return null;
        LoadSceneItem();
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
public class ItemSaveDate // Сохраняемые данные о дефолтных вещах
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
public class ItemSaveDateWeapon // Сохраняемые данные о оружие
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

// НЕ НУЖНО (нет механики для изменения заклинаний в книге, поэтому нет смысла сохранять их), но пусть будет

//public class ItemSaveDataBook // Сохраняемые данные о книгах
//{
//    public string itemID;
//    public Spells spells;

//    public ItemSaveDataBook(string _id, Spells _sp)
//    {
//        this.itemID = _id;
//        this.spells = _sp;
//    }
//}

[System.Serializable]
public class SaveSceneData
{
    //public List<ItemSaveScaneData> itemSceneData = new List<ItemSaveScaneData>();
    public List<string> itemName = new List<string>();
}

[System.Serializable]
public class ItemSaveScaneData
{
    public Vector3 position;
    public string itemID;
    public int amount;

    public ItemSaveScaneData(Vector3 _position, string _itemID, int _amount)
    {
        this.position = _position;
        this.itemID = _itemID;
        this.amount = _amount;
    }
}