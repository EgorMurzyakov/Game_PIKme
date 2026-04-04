using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemScriptableObject itemScriptableObject;
    public int amount;

    void Awake()
    {
        // Создаем копию вместо ссылки
        if (itemScriptableObject != null)
        {
            itemScriptableObject = itemScriptableObject.Clone();
        }
    }
}
