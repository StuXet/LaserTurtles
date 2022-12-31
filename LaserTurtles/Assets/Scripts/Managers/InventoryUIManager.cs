using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private InventorySystem PlayerInventoryRef;
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private Transform _contentBar;

    // Start is called before the first frame update
    void Start()
    {
        PlayerInventoryRef.OnInventoryChanged += OnUpdateInventory;
    }

    private void OnUpdateInventory(object sender, System.EventArgs e)
    {
        foreach (Transform t in _contentBar)
        {
            Destroy(t.gameObject);
        }

        DrawInventory();
    }

    private void DrawInventory()
    {
        foreach (InventoryItem item in PlayerInventoryRef.InventoryItems)
        {
            AddInventorySlot(item);
        }
    }

    private void AddInventorySlot(InventoryItem item)
    {
        GameObject obj = Instantiate(_slotPrefab,_contentBar,false);
        obj.GetComponent<InventorySlot>().Set(item);
    }
}
