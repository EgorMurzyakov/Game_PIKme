using UnityEngine;

[CreateAssetMenu(fileName = "Book Item", menuName = "Inventory/Items/New Book Item")]

public class BookItem : ItemScriptableObject
{

    public void Start()
    {
        type = ItemType.Book;
    }
}
