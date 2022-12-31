using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image m_icon;
    [SerializeField] private TextMeshProUGUI m_label;
    [SerializeField] private GameObject m_stackObj;
    [SerializeField] private TextMeshProUGUI m_stackLabel;

    public void Set(InventoryItem item)
    {
        m_icon.sprite = item.DataRef.Icon;
        m_label.text = item.DataRef.DisplayName;
        if (item.StackSize <= 1)
        {
            m_stackObj.SetActive(false);
        }
        else
        {
            m_stackObj.SetActive(true);
        }
        m_stackLabel.text = item.StackSize.ToString();
    }
}
