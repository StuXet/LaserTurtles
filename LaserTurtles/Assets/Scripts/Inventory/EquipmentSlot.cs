using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private ItemType _equipType;
    [SerializeField] private InventorySystem _inventorySystemRef;
    [SerializeField] private InventoryItemData _equippedItemData;
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private GameObject _slotTypeIcon;
    [SerializeField] private GameObject _slotSelectIcon;
    [SerializeField] private Transform _canvas;
    [SerializeField] private InventoryUIManager _inventoryUIRef;

    public ItemType EquipType { get => _equipType; }
    public InventoryItemData EquippedItemData { get => _equippedItemData; set => _equippedItemData = value; }
    public GameObject SlotSelectIcon { get => _slotSelectIcon; }

    private void Awake()
    {
        SetupPreEquipped();
    }

    private void Update()
    {
        if (_equippedItemData)
        {
            _slotTypeIcon.SetActive(false);
        }
        else
        {
            _slotTypeIcon.SetActive(true);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            GameObject droppedObj = eventData.pointerDrag;
            InventorySlot invSlot = droppedObj.GetComponent<InventorySlot>();
            if (_equipType == invSlot.ItemData.Type)
            {
                DraggableItem draggableItem = droppedObj.GetComponent<DraggableItem>();
                if (draggableItem.EquipIconRef != null)
                {
                    Destroy(draggableItem.EquipIconRef.gameObject);
                    draggableItem.EquipIconRef = null;
                    draggableItem.EquipSlotRef._equippedItemData = null;
                }

                var icon = Instantiate(invSlot.Icon, transform);
                icon.AddComponent<GridLayoutGroup>();
                draggableItem.OriginalParent = icon.transform;
                draggableItem.EquipIconRef = icon;
                invSlot.SetTransparency(0);
                icon.color = Color.white;

                _equippedItemData = invSlot.ItemData;
                draggableItem.EquipSlotRef = this;

                _inventorySystemRef.Remove(invSlot.ItemData);
                Debug.Log("Dropped");
            }
        }
    }

    private void SetupPreEquipped()
    {
        SetupWeaponEquip();
    }

    [ContextMenu("Setup Weapon Equip")]
    public void SetupWeaponEquip()
    {
        if (_equippedItemData != null)
        {
            GameObject obj = Instantiate(_slotPrefab, null, false);
            obj.GetComponent<InventorySlot>().Set(_equippedItemData);
            obj.GetComponent<DraggableItem>().CanvasParent = _canvas;
            obj.GetComponent<DraggableItem>().InventoryUIRef = _inventoryUIRef;

            InventorySlot invSlot = obj.GetComponent<InventorySlot>();
            if (_equipType == invSlot.ItemData.Type)
            {
                DraggableItem draggableItem = obj.GetComponent<DraggableItem>();
                if (draggableItem.EquipIconRef != null)
                {
                    Destroy(draggableItem.EquipIconRef.gameObject);
                    draggableItem.EquipIconRef = null;
                    draggableItem.EquipSlotRef._equippedItemData = null;
                }

                var icon = Instantiate(invSlot.Icon, transform);
                icon.AddComponent<GridLayoutGroup>();
                draggableItem.OriginalParent = icon.transform;
                draggableItem.EquipIconRef = icon;
                invSlot.SetTransparency(0);

                _equippedItemData = invSlot.ItemData;
                draggableItem.EquipSlotRef = this;

                obj.transform.SetParent(icon.transform);
            }
        }
    }
}
