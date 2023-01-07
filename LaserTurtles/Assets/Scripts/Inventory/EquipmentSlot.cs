using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            Debug.Log("Dropped");
            GameObject droppedObj = eventData.pointerDrag;
            DraggableItem draggableItem = droppedObj.GetComponent<DraggableItem>();
            draggableItem.OriginalParent = transform;
        }
    }
}
