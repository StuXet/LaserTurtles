using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryPanel : MonoBehaviour, IDropHandler
{
    private InventorySystem _inventorySystemRef;
    private Transform _contentBar;

    private void Awake()
    {
        _inventorySystemRef = transform.GetComponentInParent<InventoryUIManager>().PlayerInventoryRefrence;
        _contentBar = transform.GetComponentInParent<InventoryUIManager>().ContentBar;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObj = eventData.pointerDrag;
        if (droppedObj.TryGetComponent(out DraggableItem draggableItem))
        {
            if (draggableItem.OriginalParent != _contentBar)
            {
                InventorySlot invSlot = droppedObj.GetComponent<InventorySlot>();
                _inventorySystemRef.Add(invSlot.ItemData);
                Destroy(droppedObj);

                Debug.Log("Added");
            }
        }
    }
}
