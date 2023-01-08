using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
                if (draggableItem.EquipIconRef != null)
                {
                    Destroy(draggableItem.EquipIconRef.gameObject);
                    draggableItem.EquipIconRef = null;
                }
                var icon = Instantiate(invSlot.Icon, transform);
                icon.AddComponent<GridLayoutGroup>();
                draggableItem.OriginalParent = icon.transform;
                draggableItem.EquipIconRef = icon;
                invSlot.SetTransparency(0);
                _inventorySystemRef.Remove(invSlot.ItemData);
                Debug.Log("Dropped");
            }
        }
    }
}
