using UnityEngine;

[CreateAssetMenu(fileName = "Tornado Book", menuName = "Inventory/Items/New Tornado Book")]
public class TornadoBook : BookItem
{
    public override ItemScriptableObject Clone()
    {
        TornadoBook clone = ScriptableObject.CreateInstance<TornadoBook>();

        clone.itemID = itemID;
        clone.type = this.type;
        clone.ItemPrefab = this.ItemPrefab;
        clone.itemName = this.itemName;
        clone.itemDescription = this.itemDescription;
        clone.maximumAmount = this.maximumAmount;
        clone.icon = this.icon;
        clone.magicSpells = magicSpells != null ? magicSpells.Clone() : null;

        return clone;
    }
}
