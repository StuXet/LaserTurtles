using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] private InputManager _inputManagerRef;
    private PlayerInputActions _plInputActions;

    private Dictionary<InventoryItemData, InventoryItem> m_itemDictionary;
    public List<InventoryItem> InventoryItems { get; private set; }

    private ItemObject _currentCollisionObj;

    public event EventHandler OnInventoryChanged;

    private void Awake()
    {
        InventoryItems = new List<InventoryItem>();
        m_itemDictionary = new Dictionary<InventoryItemData, InventoryItem>();
    }

    private void Start()
    {
        _plInputActions = _inputManagerRef.PlInputActions;
        SubscribeToInputs();
    }

    public InventoryItem Get(InventoryItemData referenceData)
    {
        if (m_itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            return value;
        }
        return null;
    }

    public bool CheckIfInInventory(InventoryItemData referenceData)
    {
        if (m_itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            return true;
        }
        return false;
    }

    public void Add(InventoryItemData referenceData)
    {
        if (m_itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            value.AddToStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(referenceData);
            InventoryItems.Add(newItem);
            m_itemDictionary.Add(referenceData, newItem);
        }

        if (OnInventoryChanged != null) OnInventoryChanged(this, EventArgs.Empty);
    }

    public void Remove(InventoryItemData referenceData)
    {
        if (m_itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            value.RemoveFromStack();

            if (value.StackSize == 0)
            {
                InventoryItems.Remove(value);
                m_itemDictionary.Remove(referenceData);
            }
        }

        if (OnInventoryChanged != null) OnInventoryChanged(this, EventArgs.Empty);
    }

    private void SubscribeToInputs()
    {
        _plInputActions.Player.Interact.performed += Interact;
    }

    private void Interact(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (_currentCollisionObj != null)
        {
            _currentCollisionObj.OnHandlePickupItem(this);
            _currentCollisionObj = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ItemObject>())
        {
            if (other.GetComponent<ItemObject>().CanBePicked)
            {
                if (!other.GetComponent<ItemObject>().RequiresInteraction)
                {
                    _currentCollisionObj = other.GetComponent<ItemObject>();
                    _currentCollisionObj.OnHandlePickupItem(this);
                    _currentCollisionObj = null;
                }
                else
                {
                    _currentCollisionObj = other.GetComponent<ItemObject>();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<ItemObject>())
        {
            if (other.GetComponent<ItemObject>().CanBePicked)
            {
                _currentCollisionObj = null;
            }
        }
    }
}
