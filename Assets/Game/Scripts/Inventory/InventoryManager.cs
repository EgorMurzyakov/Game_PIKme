using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject UIPanel;
    public GameObject UIHelp;
    public Transform inventoryPanel;
    public List<InventorySlot> slots = new List<InventorySlot>();
    private bool isOpened = false; // Выключен в начале игры

    private List<GameObject> itemsInRange = new List<GameObject>(); // Список предметов, которые можно подобрать   

    public void Start()
    {
        for (int i = 0; i < inventoryPanel.childCount; i++)
        {
            if (inventoryPanel.GetChild(i).GetComponent<InventorySlot>() != null)
            {
                slots.Add(inventoryPanel.GetChild(i).GetComponent<InventorySlot>());
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
                slot.amount += _amount;
                slot.textItemAmount.text = slot.amount.ToString();
                return; 
            }
        }
    }
}
