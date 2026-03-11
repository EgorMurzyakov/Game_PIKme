using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public GameObject UIPanel;
    public GameObject UIHelp;
    public Transform inventoryPanel;
    public TMP_Text itemInfoText;
    //public List<InventorySlot> slots = new List<InventorySlot>();
    public InventorySlot[,] slots;
    [SerializeField] private InventorySlot weaponSlot;
    [SerializeField] private InventorySlot bookSlot;
    private bool isOpened = false; // Выключен в начале игры

    public event Action<ItemScriptableObject> ChangeWeapon;

    private int row; // Строки 
    private int col; // Столбцы
    private int curRow = 0;
    private int curCol = 0;

    private List<GameObject> itemsInRange = new List<GameObject>(); // Список предметов, которые можно подобрать   

    public void Start()
    {
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

        UIPanel.SetActive(false); // Принудительно выключаем при старте игры
        UIHelp.SetActive(false);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) // Вкл/Выкл инвентаря в игре
        {
            isOpened = !isOpened;
            if (isOpened)
            {
                UIPanel.SetActive(true);               
            }
            else
            {
                UIPanel.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            TryPickupItem();
        }

        if (isOpened) // Если инвентарь открыт
        {
            InventoryNavigation(); // Навигация по инвентарю
            ShowItemInfo(); // Показывает описание предмета

            if (Input.GetKeyDown(KeyCode.Q)) // Выбрасываем предмет
            {
                DropItem(true);
            }
            if (Input.GetKeyDown(KeyCode.E)) // Действие с предметом
            {
                ActionItem();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            itemsInRange.Add(other.gameObject);
            Debug.Log($"Предмет {other.name} в зоне подбора");
        }
        if (itemsInRange.Count > 0)
        {
            UIHelp.SetActive(true);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            itemsInRange.Remove(other.gameObject);
            Debug.Log($"Предмет {other.name} покинул зону");
        }
        if (itemsInRange.Count == 0)
        {
            UIHelp.SetActive(false);
        }
    }

    void TryPickupItem()
    {
        if (itemsInRange.Count > 0)
        {
            // Берем первый предмет в списке (или ближайший)
            GameObject itemToPick = GetClosestItem();

            if (itemToPick != null)
            {
                Item item = itemToPick.GetComponent<Item>();
                Debug.Log($"Вы подобрали - {item.itemScriptableObject.itemName}");
                AddItem(item.itemScriptableObject, item.amount);
                Destroy(itemToPick);
                itemsInRange.Remove(itemToPick);
            }
            if (itemsInRange.Count == 0)
            {
                UIHelp.SetActive(false);
            }
        }
    }

    GameObject GetClosestItem() // Выбирает предмет из списка
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

    private void AddItem(ItemScriptableObject _itemSO, int _amount)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.isEmpty) // Если слот пустой
            {
                slot.item = _itemSO;
                slot.amount = _amount;
                slot.isEmpty = false;
                slot.SetIcon(_itemSO.icon);
                slot.textItemAmount.text = _amount.ToString();
                return;
            }
            else if (slot.item == _itemSO) // Если слот НЕ пустой 
            {
                if (slot.amount + _amount <= _itemSO.maximumAmount)
                {
                    slot.amount += _amount;
                    Debug.Log("Вызвалось, кол-во " + slot.amount);
                    slot.textItemAmount.text = slot.amount.ToString();
                    return;
                }
            }
        }
    }

    private void InventoryNavigation() // Навигация по инвентарю
    {
        // Стрелки
        if (Input.GetKeyDown(KeyCode.UpArrow)) // Нажата стрелка вверх
        {
            slots[curCol, curRow].GetComponent<Image>().color = new Color(255,255,255,255);

            --curCol;
            if (curCol == -1)
            {
                curCol = col - 1;
            }

            slots[curCol, curRow].GetComponent<Image>().color = new Color(255,0,0,255);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) // Нажата стрелка вниз
        {
            slots[curCol, curRow].GetComponent<Image>().color = new Color(255, 255, 255, 255);

            ++curCol;
            if (curCol == col)
            {
                curCol = 0;
            }

            slots[curCol, curRow].GetComponent<Image>().color = new Color(255, 0, 0, 255);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) // Нажата стрелка влево
        {
            slots[curCol, curRow].GetComponent<Image>().color = new Color(255, 255, 255, 255);

            --curRow;
            if (curRow == -1)
            {
                curRow = row - 1;
            }

            slots[curCol, curRow].GetComponent<Image>().color = new Color(255, 0, 0, 255);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) // Нажата стрелка вправо
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

    private void ShowItemInfo() // Показывает описание предмета
    {
        if (slots[curCol, curRow].isEmpty == false) // Если слот НЕ пустой
        {
            itemInfoText.text = slots[curCol, curRow].item.itemDescription;
        }
        else
        {
            itemInfoText.text = " ";
        }
    }

    private void DropItem(bool _dropPrefab) // true - спавним предмет в мире, false - нет
    {
        if (slots[curCol, curRow].isEmpty == false) // Если слот НЕ пустой
        {
            slots[curCol, curRow].amount -= 1;
            slots[curCol, curRow].textItemAmount.text = slots[curCol, curRow].amount.ToString();

            if (_dropPrefab)
            {
                // Спавн выброшенного предмета
                GameObject newObject2 = Instantiate(slots[curCol, curRow].item.ItemPrefab, transform.position + new Vector3(2f, 2f, 0), Quaternion.identity);
            }
            if (slots[curCol, curRow].amount == 0)
            {
                Debug.Log("Больше нечего выкинуть");
                slots[curCol, curRow].isEmpty = true;
                slots[curCol, curRow].item = null;
                slots[curCol, curRow].iconGO.GetComponent<Image>().sprite = null;
                slots[curCol, curRow].textItemAmount.text = " ";
            }
        }
    }

    private void ActionItem()
    {
        if (slots[curCol, curRow].isEmpty == false)
        {
            if (slots[curCol, curRow].item.type == ItemType.Weapon)
            {
                // Запоминаем орижие в слоте
                ItemScriptableObject prevWeaponSlotItem = null;
                if (weaponSlot != null)
                {
                    prevWeaponSlotItem = weaponSlot.item;                    
                }
                // Добавляем в слот оружия
                weaponSlot.item = slots[curCol, curRow].item;
                weaponSlot.amount = slots[curCol, curRow].amount;
                weaponSlot.isEmpty = false;
                weaponSlot.SetIcon(slots[curCol, curRow].item.icon);

                ChangeWeapon?.Invoke(weaponSlot.item); // Событие - положили оружие в слот, класс - ActiveWeapon

                // Удаляем оружие из инвентаря
                DropItem(false);
                // Добавляем оружие
                if (prevWeaponSlotItem != null)
                {
                    AddItem(prevWeaponSlotItem, 1);
                }
            }
            else if (slots[curCol, curRow].item.type == ItemType.Food)
            {

            }
            else if (slots[curCol, curRow].item.type == ItemType.Default)
            {

            }
        }
    }
}


