using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPAdderCollectable : MonoBehaviour
{
    public int hpToAdd = 10;

    private ItemObject _itemObjectRef;

    private void Awake()
    {
        _itemObjectRef = GetComponent<ItemObject>();
        _itemObjectRef.PickedUpItem += _itemObjectRef_PickedUpItem;
    }

    private void _itemObjectRef_PickedUpItem(GameObject player)
    {
        HealthHandler healthHandler = player.gameObject.GetComponent<HealthHandler>();
        healthHandler.IncreaseMaxHP(hpToAdd);
        healthHandler.HealHP(hpToAdd);
    }
}
