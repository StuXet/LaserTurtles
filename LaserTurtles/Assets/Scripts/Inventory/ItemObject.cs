using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public delegate void PickedUp(GameObject player);
    public event PickedUp PickedUpItem;

    [SerializeField] private InventoryItemData _referenceItem;
    [SerializeField] private Animator ItemAnimator;
    private InventorySystem PlayerInventoryRef;

    public bool CanBePicked;
    public bool RequiresInteraction;

    public InventoryItemData ReferenceItem { get => _referenceItem;}

    private void Update()
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
            if (_referenceItem.Type == ItemType.Coin)
            {
                if (PickedUpItem != null) { PickedUpItem.Invoke(PlayerInventoryRef.transform.parent.gameObject); }
                PlayerInventoryRef.WalletRef.AddCoins(_referenceItem.Value);
                Destroy(gameObject);
            }
            else if (_referenceItem.Type == ItemType.Key)
            {
                if (PickedUpItem != null) { PickedUpItem.Invoke(PlayerInventoryRef.transform.parent.gameObject); }
                Destroy(gameObject,0.1f);
            }
            else if (_referenceItem.Type == ItemType.Ammo)
            {
                if (PickedUpItem != null) { PickedUpItem.Invoke(PlayerInventoryRef.transform.parent.gameObject); }
                PlayerInventoryRef.CombatSystem.AddAmmo(_referenceItem.Value);
                Destroy(gameObject);
            }
            else if (_referenceItem.Type == ItemType.Consumable)
            {
                if (PickedUpItem != null) { PickedUpItem.Invoke(PlayerInventoryRef.transform.parent.gameObject); }
                Destroy(gameObject, 0.1f);
            }
            else
            {
                if (!PlayerInventoryRef.CheckIfInInventory(_referenceItem) || _referenceItem.IsStackable)
                {
                    if (PickedUpItem != null) { PickedUpItem.Invoke(PlayerInventoryRef.transform.parent.gameObject); }
                    PlayerInventoryRef.Add(_referenceItem);
                    Destroy(gameObject);
                }
            }
        }
    }
}
