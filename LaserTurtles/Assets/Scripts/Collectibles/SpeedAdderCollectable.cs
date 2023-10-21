using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedAdderCollectable : MonoBehaviour
{
    public float speedToAdd = 2;

    private ItemObject _itemObjectRef;

    private void Awake()
    {
        _itemObjectRef = GetComponent<ItemObject>();
        _itemObjectRef.PickedUpItem += _itemObjectRef_PickedUpItem;
    }

    private void _itemObjectRef_PickedUpItem(GameObject player)
    {
        PlayerController playerController = player.gameObject.GetComponent<PlayerController>();
        playerController.MaxSpeed += speedToAdd;
    }
}
