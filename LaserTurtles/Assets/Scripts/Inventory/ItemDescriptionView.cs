using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescriptionView : MonoBehaviour
{
    [SerializeField] private GameObject _visualsHolder;
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TextMeshProUGUI _itemName;
    [SerializeField] private TextMeshProUGUI _itemDescription;
    private InventoryItemData _selectedItemData;

    private void Update()
    {
        if (_selectedItemData == null)
        {
            _visualsHolder.SetActive(false);
        }
        else
        {
            _visualsHolder.SetActive(true);
        }
    }

    public void ClearDescription()
    {
        _selectedItemData = null;
        _itemIcon.sprite = null;
        _itemName.text = null;
        _itemDescription.text = null;
    }

    public void UpdateDescriptionView(InventoryItemData data)
    {
        if (data == null)
        {
            ClearDescription();
        }
        else
        {
            _selectedItemData = data;
            _itemIcon.sprite = _selectedItemData.Icon;
            _itemName.text = _selectedItemData.DisplayName;
            _itemDescription.text = _selectedItemData.Description;
        }
    }
}
