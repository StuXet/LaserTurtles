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
    [SerializeField] private GameObject PickupPressIcon;
    [SerializeField] private Outline _outline;
    private InventorySystem PlayerInventoryRef;

    public bool CanBePicked;
    public bool RequiresInteraction;

    private bool _colliding;

    public InventoryItemData ReferenceItem { get => _referenceItem; }

    private void OnEnable()
    {
        if (PickupPressIcon)
        {
            PickupPressIcon.SetActive(false);
        }
    }

    private void Update()
    {
        if (ItemAnimator)
        {
            if (CanBePicked)
            {
                ItemAnimator.enabled = true;
                if (_outline != null) _outline.OutlineMode = Outline.Mode.OutlineAndSilhouette;
            }
            else
            {
                ItemAnimator.enabled = false;
                if (_outline != null) _outline.OutlineMode = Outline.Mode.SilhouetteOnly;
            }
        }

        if (PickupPressIcon)
        {
            if (CanBePicked)
            {
                if (RequiresInteraction && _colliding)
                {
                    PickupPressIcon.SetActive(true);
                }
                else
                {
                    PickupPressIcon.SetActive(false);
                }
            }
            else
            {
                PickupPressIcon.SetActive(false);
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
                Destroy(gameObject, 0.1f);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _colliding = true;
            if (PickupPressIcon && CanBePicked && RequiresInteraction)
            {
                PickupPressIcon.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _colliding = false;
            if (PickupPressIcon && CanBePicked && RequiresInteraction)
            {
                PickupPressIcon.SetActive(false);
            }
        }
    }
}
