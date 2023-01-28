using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public event EventHandler PickedUpItem;

    [SerializeField] private InventoryItemData ReferenceItem;
    [SerializeField] private Animator ItemAnimator;
    private InventorySystem PlayerInventoryRef;

    public bool CanBePicked;
    public bool RequiresInteraction;

    private void Awake()
    {
        if (ItemAnimator)
        {
            if (CanBePicked)
            {
                ItemAnimator.enabled = true;
            }
            else
            {
                ItemAnimator.enabled = false;
            }
        }
    }

    public void OnHandlePickupItem(InventorySystem inventoryRef)
    {
        if (CanBePicked)
        {
            PlayerInventoryRef = inventoryRef;
            if (ReferenceItem.Type == ItemType.Coin)
            {
                if (PickedUpItem != null) { PickedUpItem.Invoke(this, EventArgs.Empty); }
                PlayerInventoryRef.WalletRef.AddCoins(ReferenceItem.Value);
                Destroy(gameObject);
            }
            else if (ReferenceItem.Type == ItemType.Key)
            {
                if (PickedUpItem != null) { PickedUpItem.Invoke(this, EventArgs.Empty); }
                Destroy(gameObject);
            }
            else if (ReferenceItem.Type == ItemType.Ammo)
            {
                if (PickedUpItem != null) { PickedUpItem.Invoke(this, EventArgs.Empty); }
                PlayerInventoryRef.CombatSystem.AddAmmo(ReferenceItem.Value);
                Destroy(gameObject);
            }
            else
            {
                if (!PlayerInventoryRef.CheckIfInInventory(ReferenceItem) || ReferenceItem.IsStackable)
                {
                    if (PickedUpItem != null) { PickedUpItem.Invoke(this, EventArgs.Empty); }
                    PlayerInventoryRef.Add(ReferenceItem);
                    Destroy(gameObject);
                }
            }
        }
    }
}
