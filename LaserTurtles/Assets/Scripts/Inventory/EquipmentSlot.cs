using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private ItemType EquipType;
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            GameObject droppedObj = eventData.pointerDrag;
            if (EquipType == droppedObj.GetComponent<InventorySlot>().ItemData.Type)
            {
                DraggableItem draggableItem = droppedObj.GetComponent<DraggableItem>();
                draggableItem.OriginalParent = transform;
                Debug.Log("Dropped");
            }
        }
    }
}
