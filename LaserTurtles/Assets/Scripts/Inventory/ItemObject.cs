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
    [SerializeField] private AudioSource _pickUpSFX;
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

    public InventoryItemData OnHandlePickupItem(InventorySystem inventoryRef)
    {
        if (CanBePicked)
        {
            PlayerInventoryRef = inventoryRef;
            if (_referenceItem.Type == ItemType.Coin)
            {
                if (PickedUpItem != null) { PickedUpItem.Invoke(PlayerInventoryRef.transform.parent.gameObject); }
                PlayerInventoryRef.WalletRef.AddCoins(_referenceItem.Value);
                StartCoroutine(Destroyer());
            }
            else if (_referenceItem.Type == ItemType.Key)
            {
                if (PickedUpItem != null) { PickedUpItem.Invoke(PlayerInventoryRef.transform.parent.gameObject); }
                StartCoroutine(Destroyer());
            }
            else if (_referenceItem.Type == ItemType.Ammo)
            {
                if (PickedUpItem != null) { PickedUpItem.Invoke(PlayerInventoryRef.transform.parent.gameObject); }
                PlayerInventoryRef.CombatSystem.AddAmmo(_referenceItem.Value);
                StartCoroutine(Destroyer());
            }
            else if (_referenceItem.Type == ItemType.Consumable)
            {
                if (PickedUpItem != null) { PickedUpItem.Invoke(PlayerInventoryRef.transform.parent.gameObject); }
                StartCoroutine(Destroyer());
            }
            else
            {
                if (!PlayerInventoryRef.CheckIfInInventory(_referenceItem) || _referenceItem.IsStackable)
                {
                    if (PickedUpItem != null) { PickedUpItem.Invoke(PlayerInventoryRef.transform.parent.gameObject); }
                    PlayerInventoryRef.Add(_referenceItem, false);
                    StartCoroutine(Destroyer());
                }
            }
            return _referenceItem;
        }
        return null;
    }

    IEnumerator Destroyer()
    {
        yield return new WaitForEndOfFrame();
        if (_pickUpSFX != null)
        {
            _pickUpSFX.Play();
            _pickUpSFX.transform.parent = null;
            Destroy(_pickUpSFX.gameObject, 1f);
        }
        Destroy(gameObject);
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
