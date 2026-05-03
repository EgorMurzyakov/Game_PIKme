using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using NUnit.Framework.Constraints;
using static UnityEditor.Progress;
using System.Collections;

public class InventoryManager : MonoBehaviour
{
    public GameObject UIPanel;
    public GameObject UIActionPanel;
    public GameObject UIPumpPanel;
    private bool isTrader = false;
    public GameObject UIHelp;
    public Transform inventoryPanel;
    public PlayerStateMachine playerStateMachine;
    public TMP_Text itemInfoText;
    public TMP_Text startStatUI; // ��������� ���� (�������� ����) - ��������
    public TMP_Text newStatUI; // ��������� ���� (����� ����) - ��������
    public TMP_Text moneyUI;
    public TMP_Text priceUI;
    private float improvCoeff = 1.1f; // ��������� ���������� ����� ��� ��������
    public int playerMoney = 0;
    //public List<InventorySlot> slots = new List<InventorySlot>();
    public InventorySlot[,] slots;
    public InventorySlot weaponSlot;
    public InventorySlot bookSlot;
    [SerializeField] private InventorySlot pumpSlot;
    public ItemScriptableObject startWeapon;
    public ItemScriptableObject startBook;
    private bool isOpened = false; // �������� � ������ ����

    public event Action<WeaponItem> ChangeWeapon;
    public event Action<FoodItem> EatFood;   

    private int row; // ������ 
    private int col; // �������
    private int curRow = 0;
    private int curCol = 0;
        
    private string savePath; // ���� � ����� ����������
    ItemScriptableObject[] allItems; // Item Asset

    private List<GameObject> itemsInRange = new List<GameObject>(); // ������ ���������, ������� ����� ���������   

    public void Start()
    {
        // ����� ���� ��� ���������� (����������� ����� ��� ����)
        savePath = Application.persistentDataPath + "/money.json";
        //Debug.Log("���� ���������� ��������� �����: " + Application.persistentDataPath);


        col = inventoryPanel.childCount;
        row = inventoryPanel.GetChild(0).childCount;
        slots = new InventorySlot[col, row];

        for (int i = 0; i < inventoryPanel.childCount; i++)
        {
            for (int j = 0; j < inventoryPanel.GetChild(i).childCount; j++)
            {
                if (inventoryPanel.GetChild(i).GetChild(j).GetComponent<InventorySlot>() != null)
                {
                    slots[i, j] = inventoryPanel.GetChild(i).GetChild(j).GetComponent<InventorySlot>();
                }
            }
        }

        allItems = Resources.LoadAll<ItemScriptableObject>("Items"); // ����������� �� LoadMoney()
        LoadMoney();
        StartCoroutine(InitializeWeaponLater()); // �������� ������ � ��������� � ���� (������ ������� � ������, ���� ChangeWeapon �� ������ ����������� �� �������)

        UIPanel.SetActive(false); // ������������� ��������� ��� ������ ����
        UIActionPanel.SetActive(false);
        UIPumpPanel.SetActive(false);
        UIHelp.SetActive(false);

        if (startWeapon != null)
        {
            AddItem(startWeapon.Clone(), 1);
        }
        if (startBook != null)
        {
            AddItem(startBook.Clone(), 1);
        }

        moneyUI.text = playerMoney.ToString();
        startStatUI.text = "Current damage";
        newStatUI.text = "Future damage";
        priceUI.text = "Price";
    }

    IEnumerator InitializeWeaponLater()
    {
        // ���� 1 ����, ����� ��� Start() �����������
        yield return null;

        if (weaponSlot.item != null)
        {
            playerStateMachine.SetWeaponInHand(true); // � ����� ��������� ������ -> ����� ���������
            ChangeWeapon?.Invoke((WeaponItem)weaponSlot.item); // ������� - �������� ������ � ����, ����� - ActiveWeapon
        }
    }

        public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) // ���/���� ��������� � ����
        {
            isOpened = !isOpened;
            if (isOpened)
            {
                UIPanel.SetActive(true);
                UIActionPanel.SetActive(!isTrader);
                UIPumpPanel.SetActive(isTrader);
            }
            else
            {
                UIPanel.SetActive(false);
                pumpSlot.SetIcon(null);
                pumpSlot.item = null;

                startStatUI.text = "Current damage";
                newStatUI.text = "Future damage";
                priceUI.text = "Price";
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            TryPickupItem();
        }

        if (isOpened) // ���� ��������� ������
        {
            InventoryNavigation(); // ��������� �� ���������
            ShowItemInfo(); // ���������� �������� ��������

            if (Input.GetKeyDown(KeyCode.Q)) // ����������� �������
            {
                DropItem(true);
            }
            if (Input.GetKeyDown(KeyCode.E)) // �������� � ���������
            {
                ActionItem();
            }

            if (isTrader)
            {
                if (Input.GetKeyDown(KeyCode.R) && pumpSlot.item != null) // <<<<<<-------------------------------------------------|
                {
                    int upgradePrice = ((WeaponItem)pumpSlot.item).GetUpgradePrice();
                    if (playerMoney >= upgradePrice) {
                        ((WeaponItem)pumpSlot.item).SetBaceDamage((int)(((WeaponItem)pumpSlot.item).GetBaceDamage() * improvCoeff));
                        ((WeaponItem)pumpSlot.item).UpdateUpgradePrice(); // ��������� ����
                        playerMoney -= upgradePrice;
                        UpdateUI();
                    }
                }
            }
        }

    }

    // ��������� ������
    //public void SaveMoney()
    //{
    //    // ���������� ����� � ����� (JSON)
    //    string json = "{\"money\":" + playerMoney + "}"; // ������

    //    foreach (InventorySlot _slot in slots) // ���������� ���������
    //    {
    //        if (_slot.item != null)
    //        {
    //            json += "{\"itemID\":" + _slot.item.itemID + "}";
    //        }
    //    }

    //    // ���������� ����� � ����
    //    File.WriteAllText(savePath, json);

    //    Debug.Log("������ ���������: " + playerMoney);
    //}

    public void LoadMoney()
    {
        string path = Path.Combine(Application.persistentDataPath, "money.json");
    
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveDate data = JsonUtility.FromJson<SaveDate>(json);
        
            if (data != null)
            {
                playerMoney = data.money;
                if (data.weaponSlotItem != null)
                {
                    string ID = data.weaponSlotItem.itemID;
                    foreach (ItemScriptableObject _itemSO in allItems)
                    {
                        if (_itemSO.itemID == ID)
                        {
                            WeaponItem weapon = (WeaponItem)_itemSO.Clone();
                            weapon.SetBaceDamage(data.weaponSlotItem.baseDamage);
                            weapon.UpdateUpgradePrice(data.weaponSlotItem.UpPrice);
                            weaponSlot.item = weapon;
                            weaponSlot.SetIcon(weapon.icon);
                        }
                    }
                }
                if (data.bookSlotItemID != null)
                {
                    string ID = data.bookSlotItemID;
                    foreach (ItemScriptableObject _itemSO in allItems)
                    {
                        if (_itemSO.itemID == ID)
                        {
                            bookSlot.item = _itemSO;
                            bookSlot.SetIcon(_itemSO.icon);
                        }
                    }
                }

                foreach (ItemSaveDate _itemDate in data.inventory) // �������� ����� ������
                {
                    string ID = _itemDate.itemID;
                    foreach (ItemScriptableObject _itemSO in allItems)
                    {
                        if (_itemSO.itemID == ID)
                        {
                            AddItem(_itemSO.Clone(), _itemDate.amount); // ��� ������ �������
                        }
                    }
                }

                foreach (ItemSaveDateWeapon _itemDate in data.inventoryWeapon) // ������
                {
                    string ID = _itemDate.itemID;
                    foreach (ItemScriptableObject _itemSO in allItems)
                    {
                        if (_itemSO.itemID == ID)
                        {
                            WeaponItem weapon = (WeaponItem)_itemSO.Clone(); // ����� �� �� ������, �� ������ ���-�� ������� ����� ������
                            weapon.SetBaceDamage(_itemDate.baseDamage);
                            weapon.UpdateUpgradePrice(_itemDate.UpPrice);
                            AddItem(weapon, 1); 
                        }
                    }
                }
            }
            else
            {
                playerMoney = 0;
                Debug.LogWarning("�� ������� ���������, ���������� �������� �� ���������");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item")) // ������ ���������
        {
            itemsInRange.Add(other.gameObject);
            Debug.Log($"������� {other.name} � ���� �������");
        }
        if (itemsInRange.Count > 0)
        {
            UIHelp.SetActive(true);
        }

        if (other.CompareTag("TraderNPC")) // �������� � ��������
        {
            isTrader = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item")) // ������ ���������
        {
            itemsInRange.Remove(other.gameObject);
            Debug.Log($"������� {other.name} ������� ����");
        }
        if (itemsInRange.Count == 0)
        {
            UIHelp.SetActive(false);
        }

        if (other.CompareTag("TraderNPC")) // �������� � ��������
        {
            isTrader = false;
        }
    }

    void TryPickupItem()
    {
        if (itemsInRange.Count > 0)
        {
            // ����� ������ ������� � ������ (��� ���������)
            GameObject itemToPick = GetClosestItem();

            if (itemToPick != null)
            {
                Item item = itemToPick.GetComponent<Item>();
                if (AddItem(item.itemScriptableObject, item.amount))
                {
                    Debug.Log($"�� ��������� - {item.itemScriptableObject.itemName}");
                    Destroy(itemToPick);
                    itemsInRange.Remove(itemToPick);
                }
            }
            if (itemsInRange.Count == 0)
            {
                UIHelp.SetActive(false);
            }
        }
    }

    GameObject GetClosestItem() // �������� ������� �� ������
    {
        GameObject closest = null;
        float minDistance = float.MaxValue;

        foreach (GameObject item in itemsInRange)
        {
            if (item == null) continue;

            float distance = Vector3.Distance(transform.position, item.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = item;
            }
        }

        return closest;
    }

    private bool AddItem(ItemScriptableObject _itemSO, int _amount) // ���������� true, ���� ���� �����
    {
        if (_itemSO.type == ItemType.Money)
        {
            playerMoney += _amount;
            UpdateUI();
            return true;
        }

        foreach (InventorySlot slot in slots)
        {
            if (slot.isEmpty) // ���� ���� ������
            {
                slot.item = _itemSO;
                slot.amount = _amount;
                slot.isEmpty = false;
                slot.SetIcon(_itemSO.icon);
                slot.textItemAmount.text = _amount.ToString();
                return true;
            }
            else if (slot.item.itemName == _itemSO.itemName) // ���� ���� �� ������ 
            {
                if (slot.amount + _amount <= _itemSO.maximumAmount)
                {
                    slot.amount += _amount;
                    Debug.Log("���������, ���-�� " + slot.amount);
                    slot.textItemAmount.text = slot.amount.ToString();
                    return true;
                }
            }
        }
        return false; // ��������� �����
    }

    public bool HasTornadoBook()
    {
        if (bookSlot != null && bookSlot.item is BookItem book && book.HasTornadoSpell)
            return true;

        foreach (InventorySlot slot in slots)
        {
            if (slot != null && !slot.isEmpty && slot.item is BookItem inventoryBook && inventoryBook.HasTornadoSpell)
                return true;
        }

        return false;
    }

    public bool HasBlackRose()
    {
        if (bookSlot != null && bookSlot.item is FlowerItem bookFlower && bookFlower.IsBlackRose())
            return true;

        foreach (InventorySlot slot in slots)
        {
            if (slot != null && !slot.isEmpty && slot.item is FlowerItem inventoryFlower && inventoryFlower.IsBlackRose())
                return true;
        }

        return false;
    }

    public bool GiveItemByID(string itemID, int amount)
    {
        if (allItems == null || allItems.Length == 0)
            allItems = Resources.LoadAll<ItemScriptableObject>("Items");

        foreach (ItemScriptableObject item in allItems)
        {
            if (item == null) continue;

            if (!string.IsNullOrEmpty(item.itemID) && item.itemID.Equals(itemID, StringComparison.OrdinalIgnoreCase))
            {
                return AddItem(item.Clone(), amount);
            }

            if (!string.IsNullOrEmpty(item.itemName) && item.itemName.Equals(itemID, StringComparison.OrdinalIgnoreCase))
            {
                return AddItem(item.Clone(), amount);
            }
        }

        Debug.LogWarning($"InventoryManager: item с ID или именем '{itemID}' не найден.");
        return false;
    }

    private void InventoryNavigation() // ��������� �� ���������
    {
        // �������
        if (Input.GetKeyDown(KeyCode.UpArrow)) // ������ ������� �����
        {
            slots[curCol, curRow].GetComponent<Image>().color = new Color(255,255,255,255);

            --curCol;
            if (curCol == -1)
            {
                curCol = col - 1;
            }

            slots[curCol, curRow].GetComponent<Image>().color = new Color(255,0,0,255);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) // ������ ������� ����
        {
            slots[curCol, curRow].GetComponent<Image>().color = new Color(255, 255, 255, 255);

            ++curCol;
            if (curCol == col)
            {
                curCol = 0;
            }

            slots[curCol, curRow].GetComponent<Image>().color = new Color(255, 0, 0, 255);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) // ������ ������� �����
        {
            slots[curCol, curRow].GetComponent<Image>().color = new Color(255, 255, 255, 255);

            --curRow;
            if (curRow == -1)
            {
                curRow = row - 1;
            }

            slots[curCol, curRow].GetComponent<Image>().color = new Color(255, 0, 0, 255);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) // ������ ������� ������
        {
            slots[curCol, curRow].GetComponent<Image>().color = new Color(255, 255, 255, 255);

            ++curRow;
            if (curRow == row)
            {
                curRow = 0;
            }

            slots[curCol, curRow].GetComponent<Image>().color = new Color(255, 0, 0, 255);
        }
    }

    private void ShowItemInfo() // ���������� �������� ��������
    {
        if (slots[curCol, curRow].isEmpty == false) // ���� ���� �� ������
        {
            if (slots[curCol, curRow].item.type == ItemType.Weapon)
            {
                itemInfoText.text = slots[curCol, curRow].item.itemDescription + ". Damage: " + ((WeaponItem)slots[curCol, curRow].item).GetBaceDamage();
            }
            else
            {
                itemInfoText.text = slots[curCol, curRow].item.itemDescription;
            }
        }
        else
        {
            itemInfoText.text = " ";
        }
    }

    private void DropItem(bool _dropPrefab) // true - ������� ������� � ����, false - ���
    {
        if (slots[curCol, curRow].isEmpty == false) // ���� ���� �� ������
        {
            slots[curCol, curRow].amount -= 1;
            slots[curCol, curRow].textItemAmount.text = slots[curCol, curRow].amount.ToString();

            if (_dropPrefab)
            {
                // ����� ������������ ��������
                GameObject newObject2 = Instantiate(slots[curCol, curRow].item.ItemPrefab, transform.position + new Vector3(2f, 2f, 0), Quaternion.identity);
            }
            if (slots[curCol, curRow].amount == 0)
            {
                Debug.Log("������ ������ ��������");
                slots[curCol, curRow].isEmpty = true;
                slots[curCol, curRow].item = null;
                slots[curCol, curRow].iconGO.GetComponent<Image>().color = new Color(0, 0, 0, 0); // ����� ���� ���������� ����� �� �� ����������� ���������
                slots[curCol, curRow].iconGO.GetComponent<Image>().sprite = null;
                slots[curCol, curRow].textItemAmount.text = " ";
            }
        }
    }

    private void ActionItem()
    {
        if (slots[curCol, curRow].isEmpty == false)
        {
            if (isTrader) 
            {
                PumpItem();
            }
            else
            {
                UseItem();
            }
        }
    }

    private void PumpItem()
    {
        if (slots[curCol, curRow].item.type == ItemType.Weapon)
        {
            pumpSlot.item = slots[curCol, curRow].item;
            pumpSlot.SetIcon(slots[curCol, curRow].item.icon);
            UpdateUI();
        }
        else if (slots[curCol, curRow].item.type == ItemType.Book)
        {

        }
    }

    private void UseItem()
    {
        if (slots[curCol, curRow].item.type == ItemType.Weapon)
        {
            // ���������� ������ � �����
            ItemScriptableObject prevWeaponSlotItem = null;
            if (weaponSlot != null)
            {
                prevWeaponSlotItem = weaponSlot.item;
            }            
            // ��������� � ���� ������
            weaponSlot.item = slots[curCol, curRow].item;
            weaponSlot.amount = slots[curCol, curRow].amount;
            weaponSlot.isEmpty = false;
            weaponSlot.SetIcon(slots[curCol, curRow].item.icon);

            playerStateMachine.SetWeaponInHand(true); // � ����� ��������� ������ -> ����� ���������

            ChangeWeapon?.Invoke((WeaponItem)weaponSlot.item); // ������� - �������� ������ � ����, ����� - ActiveWeapon

            // ������� ������ �� ���������
            DropItem(false);
            // ��������� ������
            if (prevWeaponSlotItem != null)
            {
                AddItem(prevWeaponSlotItem, 1);
            }
        }
        else if (slots[curCol, curRow].item.type == ItemType.Book)
        {
            // ���������� ����� � �����
            ItemScriptableObject prevBookSlotItem = null;
            if (bookSlot != null)
            {
                prevBookSlotItem = bookSlot.item;
            }
            // ��������� � ���� �����
            bookSlot.item = slots[curCol, curRow].item;
            bookSlot.amount = slots[curCol, curRow].amount;
            bookSlot.isEmpty = false;
            bookSlot.SetIcon(slots[curCol, curRow].item.icon);

            // ������� ����� �� ���������
            DropItem(false);
            // ��������� �����
            if (prevBookSlotItem != null)
            {
                AddItem(prevBookSlotItem, 1);
            }
        }
        else if (slots[curCol, curRow].item.type == ItemType.Food)
        {
            EatFood?.Invoke((FoodItem)slots[curCol, curRow].item); // �������� �������
            DropItem(false); // ������� �������
        }
        else if (slots[curCol, curRow].item.type == ItemType.Default)
        {

        }
    }

    private void UpdateUI()
    {
        moneyUI.text = playerMoney.ToString(); // ��������� ������� � UI
        if (pumpSlot.item != null) {
            priceUI.text = ((WeaponItem)pumpSlot.item).GetUpgradePrice().ToString(); // ��������� ���� � UI        
            startStatUI.text = "Start: " + ((WeaponItem)slots[curCol, curRow].item).GetBaceDamage();
            newStatUI.text = "New: " + (int)(((WeaponItem)slots[curCol, curRow].item).GetBaceDamage() * improvCoeff);
        }
    }
}


