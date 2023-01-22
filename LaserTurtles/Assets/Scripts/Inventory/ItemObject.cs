using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private InventoryItemData ReferenceItem;
    private InventorySystem PlayerInventoryRef;

    public bool CanBePicked;
    public bool RequiresInteraction;

    public void OnHandlePickupItem(InventorySystem inventoryRef)
    {
        if (CanBePicked)
        {
            PlayerInventoryRef = inventoryRef;
            if (ReferenceItem.Type != ItemType.Coin)
            {
                if (!PlayerInventoryRef.CheckIfInInventory(ReferenceItem) || ReferenceItem.IsStackable)
                {
                    PlayerInventoryRef.Add(ReferenceItem);
                    Destroy(gameObject);
                }
            }
            else
            {
                PlayerInventoryRef.WalletRef.AddCoins(1);
                Destroy(gameObject);
            }
        }
    }
}
