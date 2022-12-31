using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Melee,
    Ranged,
    Consumable
}

[CreateAssetMenu(menuName = "Scriptable Object/InventoryItemData")]
public class InventoryItemData : ScriptableObject
{
    public string ItemID;
    public string DisplayName;
    public Sprite Icon;
    public GameObject Prefab;
    public ItemType Type;
    public bool IsStackable;
    [TextArea(5,20)]
    public string Description;
}
