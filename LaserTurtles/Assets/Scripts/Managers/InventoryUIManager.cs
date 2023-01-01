using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InventoryFilter
{
    All,
    Melee,
    Ranged,
}

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private InventorySystem PlayerInventoryRef;
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private Transform _contentBar;
    [SerializeField] private InventoryFilter _currentFilter;


    // Start is called before the first frame update
    void Start()
    {
        PlayerInventoryRef.OnInventoryChanged += OnUpdateInventory;
    }

    private void OnUpdateInventory(object sender, System.EventArgs e)
    {
        RedrawInventory();
    }

    private void RedrawInventory()
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
            if (PassFilter(item))
            {
                AddInventorySlot(item);
            }
        }
    }

    private void AddInventorySlot(InventoryItem item)
    {
        GameObject obj = Instantiate(_slotPrefab, _contentBar, false);
        obj.GetComponent<InventorySlot>().Set(item);
    }

    private bool PassFilter(InventoryItem item)
    {
        if (_currentFilter == InventoryFilter.All)
        {
            return true;
        }
        else if (_currentFilter.ToString() == item.DataRef.Type.ToString())
        {
            return true;
        }
        return false;
    }

    public void SetFilter(int filterVal)
    {
        _currentFilter = (InventoryFilter)filterVal;
        RedrawInventory();
    }
}
