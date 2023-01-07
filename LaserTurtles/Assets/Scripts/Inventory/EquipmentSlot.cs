using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private ItemType EquipType;
    [SerializeField] private InventorySystem _inventorySystemRef;

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            GameObject droppedObj = eventData.pointerDrag;
            InventorySlot invSlot = droppedObj.GetComponent<InventorySlot>();
            if (EquipType == invSlot.ItemData.Type)
            {
                DraggableItem draggableItem = droppedObj.GetComponent<DraggableItem>();
                draggableItem.OriginalParent = transform;
                _inventorySystemRef.Remove(invSlot.ItemData);
                Debug.Log("Dropped");
            }
        }
    }
}
