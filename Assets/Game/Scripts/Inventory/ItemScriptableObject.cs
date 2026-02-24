using UnityEngine;

public enum ItemType { Default, Food, Weapon,  }

public class ItemScriptableObject : ScriptableObject
{
    public ItemType type;
    //public GameObject ItemPrefab;
    public string itemName;
    public string itemDescription;
    public int maximumAmount;



}
