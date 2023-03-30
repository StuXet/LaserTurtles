using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private InventoryUIManager _inventoryUIRef;
    private Transform _originalParent;
    private Transform _canvasParent;
    private Image _img;
    private InventorySlot _invSlot;

    private Image _equipIconRef;
    private EquipmentSlot _equipSlotRef;

    public Transform OriginalParent { get => _originalParent; set => _originalParent = value; }
    public Transform CanvasParent { get => _canvasParent; set => _canvasParent = value; }
    public InventoryUIManager InventoryUIRef { get => _inventoryUIRef; set => _inventoryUIRef = value; }
    public Image EquipIconRef { get => _equipIconRef; set => _equipIconRef = value; }
    public EquipmentSlot EquipSlotRef { get => _equipSlotRef; set => _equipSlotRef = value; }


    private void Awake()
    {
        _img = GetComponent<Image>();
        _invSlot = GetComponent<InventorySlot>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("Begin Drag");
        _originalParent = transform.parent;
        //transform.SetParent(transform.parent.parent.parent.parent.parent);
        transform.SetParent(_canvasParent);
        _img.raycastTarget = false;
        _invSlot.SetTransparency(0.75f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("Dragging");
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("End Drag");
        transform.SetParent(_originalParent);
        _img.raycastTarget = true;
        InventoryUIRef.RedrawInventory();

        if (_equipIconRef != null)
        {
            if (_originalParent.name == _equipIconRef.name)
            {
                _invSlot.SetTransparency(0);
            }
        }

        _inventoryUIRef.UpdateSelectedItemData(null);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 1)
        {
            _inventoryUIRef.UpdateSelectedItemData(_invSlot.ItemData);
            //Debug.Log(name + " Selected!");
        }
        else
        {
            if (_equipSlotRef == null)
            {
                if (_inventoryUIRef.PlayerInventoryRefrence.CombatSystem.AutoEquipWeapon(_invSlot.ItemData))
                {
                    _inventoryUIRef.PlayerInventoryRefrence.Remove(_invSlot.ItemData);
                    //Debug.Log(name + " Equipped!");
                }
            }
            else
            {
                Destroy(_equipIconRef.gameObject);
                _equipIconRef = null;
                _equipSlotRef.EquippedItemData = null;
                _equipIconRef = null;
                _inventoryUIRef.PlayerInventoryRefrence.Add(_invSlot.ItemData, true);
                //Debug.Log(name + " DeEquipped!");
            }

            _inventoryUIRef.UpdateSelectedItemData(null);
        }
    }
}
