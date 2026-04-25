using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Book Item", menuName = "Inventory/Items/New Book Item")]

public class BookItem : ItemScriptableObject
{
    [SerializeField] private Spells magicSpells;

    public override ItemScriptableObject Clone()
    {
        BookItem clone = ScriptableObject.CreateInstance<BookItem>();

        clone.itemID = itemID;
        clone.type = this.type;
        clone.ItemPrefab = this.ItemPrefab;
        clone.itemName = this.itemName;
        clone.itemDescription = this.itemDescription;
        clone.maximumAmount = this.maximumAmount;
        clone.icon = this.icon;
        clone.magicSpells = magicSpells;
        return clone;
    }

    public void Start()
    {
        type = ItemType.Book;
    }

    public Spells GetMagicSpells()
    {
        return magicSpells.Clone();
    }
}

[System.Serializable]
public class Spells
{
    public bool fireball;
    public bool tornado;

    public Spells Clone()
    {
        return new Spells(this);
    }

    public Spells(bool _fireball, bool _tornado)
    {
        this.fireball = _fireball;
        this.tornado = _tornado;
    }

    public Spells(Spells _other)
    {
        if (_other != null)
        {
            this.fireball = _other.fireball;
            this.tornado = _other.tornado;
        }
    }
}
