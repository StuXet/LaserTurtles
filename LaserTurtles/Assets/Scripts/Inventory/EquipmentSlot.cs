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
    private InventoryItemData _equippedItemData;

    public InventoryItemData EquippedItemData { get => _equippedItemData; set => _equippedItemData = value; }


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
                    draggableItem.EquipSlotRef.EquippedItemData = null;
                }

                var icon = Instantiate(invSlot.Icon, transform);
                icon.AddComponent<GridLayoutGroup>();
                draggableItem.OriginalParent = icon.transform;
                draggableItem.EquipIconRef = icon;
                invSlot.SetTransparency(0);

                _equippedItemData = invSlot.ItemData;
                draggableItem.EquipSlotRef = this;

                _inventorySystemRef.Remove(invSlot.ItemData);
                Debug.Log("Dropped");
            }
        }
    }
}
