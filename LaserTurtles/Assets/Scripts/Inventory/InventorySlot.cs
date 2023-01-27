using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image m_itemSlot;
    [SerializeField] private Image m_icon;
    [SerializeField] private TextMeshProUGUI m_label;
    [SerializeField] private Image m_stackObj;
    [SerializeField] private TextMeshProUGUI m_stackLabel;
    private InventoryItemData _itemData;

    public Image Icon { get => m_icon; }
    public InventoryItemData ItemData { get => _itemData; }

    public void Set(InventoryItem item)
    {
        _itemData = item.DataRef;

        m_icon.sprite = item.DataRef.Icon;
        m_label.text = item.DataRef.DisplayName;
        if (item.StackSize <= 1)
        {
            m_stackObj.gameObject.SetActive(false);
        }
        else
        {
            m_stackObj.gameObject.SetActive(true);
        }
        m_stackLabel.text = item.StackSize.ToString();
    }

    public void Set(InventoryItemData itemData)
    {
        _itemData = itemData;

        m_icon.sprite = itemData.Icon;
        m_label.text = itemData.DisplayName;
        m_stackObj.gameObject.SetActive(false);
        m_stackLabel.text = 0.ToString();
    }

    public void SetTransparency(float aValue)
    {
        Color tempCol = m_itemSlot.color;
        tempCol.a = aValue;
        m_itemSlot.color = tempCol;

        tempCol = m_icon.color;
        tempCol.a = aValue;
        m_icon.color = tempCol;

        tempCol = m_label.color;
        tempCol.a = aValue;
        m_label.color = tempCol;

        tempCol = m_stackObj.color;
        tempCol.a = aValue;
        m_stackObj.color = tempCol;

        tempCol = m_stackLabel.color;
        tempCol.a = aValue;
        m_stackLabel.color = tempCol;
    }
}
