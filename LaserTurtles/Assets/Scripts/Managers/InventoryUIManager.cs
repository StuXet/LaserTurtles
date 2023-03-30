using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    [SerializeField] private Transform _canvas;
    [SerializeField] private GameObject _itemAddedIcon;
    [SerializeField] private ItemDescriptionView _descriptionPanel;
    [SerializeField] private InventoryFilter _currentFilter;
    [SerializeField] private bool _openDescription;
    public InventorySystem PlayerInventoryRefrence { get => PlayerInventoryRef;}
    public Transform ContentBar { get => _contentBar;}
    public GameObject ItemAddedIcon { get => _itemAddedIcon;}


    // Start is called before the first frame update
    void Start()
    {
        PlayerInventoryRef.OnInventoryChanged += OnUpdateInventory;
        _itemAddedIcon.SetActive(false);
    }

    private void Update()
    {
        HandleDescriptionPanel();
    }

    private void OnUpdateInventory(object sender, System.EventArgs e)
    {
        RedrawInventory();
    }

    public void RedrawInventory()
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
        obj.GetComponent<DraggableItem>().CanvasParent = _canvas;
        obj.GetComponent<DraggableItem>().InventoryUIRef = this;
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

    private void HandleDescriptionPanel()
    {
        if (_openDescription)
        {
            _descriptionPanel.gameObject.SetActive(true);
        }
        else
        {
            _descriptionPanel.gameObject.SetActive(false);
        }

        if (!_descriptionPanel.gameObject.activeInHierarchy)
        {
            _descriptionPanel.ClearDescription();
            _openDescription = false;
        }
    }

    public void UpdateSelectedItemData(InventoryItemData data)
    {
        _descriptionPanel.UpdateDescriptionView(data);
        _openDescription = true;
    }
}
