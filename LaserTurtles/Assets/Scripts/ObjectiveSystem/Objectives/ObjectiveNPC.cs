using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveNPC : ObjectiveBase
{
    [SerializeField] private Merchant _merchant;
    [SerializeField] private InventoryItemData _inventoryItemData;

    private void Awake()
    {
        _merchant.OnSoldItem += _merchant_OnSoldItem;
    }

    // Update is called once per frame
    void Update()
    {
        FinishedObjective();
    }


    private void _merchant_OnSoldItem(InventoryItemData itemData)
    {
        BeginObjective();
        if (itemData.ItemID == _inventoryItemData.ItemID)
        {
            ObjectiveRequirementMet();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!CompletedObjective)
        {
            BeginObjective();
        }
    }
}
