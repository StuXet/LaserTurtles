using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private InventoryItemData ReferenceItem;
    private InventorySystem PlayerInventoryRef;

    public bool CanBePicked;

    public void OnHandlePickupItem(InventorySystem inventoryRef)
    {
        if (CanBePicked)
        {
            PlayerInventoryRef = inventoryRef;
            if (!PlayerInventoryRef.CheckIfInInventory(ReferenceItem) || ReferenceItem.IsStackable)
            {
                PlayerInventoryRef.Add(ReferenceItem);
                Destroy(gameObject);
            }
        }
    }
}
