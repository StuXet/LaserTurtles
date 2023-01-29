using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDrop : MonoBehaviour
{
    [Header("Effect Settings")]
    [SerializeField] bool isRandom;
    [SerializeField] int healAmount = 5;
    [SerializeField] int randHealAmountMin = 5;
    [SerializeField] int randHealAmountMax = 10;

    private ItemObject _itemObjectRef;

    private void Awake()
    {
        _itemObjectRef = GetComponent<ItemObject>();
        _itemObjectRef.PickedUpItem += _itemObjectRef_PickedUpItem;
    }

    private void _itemObjectRef_PickedUpItem(GameObject player)
    {
        if (isRandom)
        {
            player.GetComponent<HealthHandler>().HealHP(Random.Range(randHealAmountMin, randHealAmountMax)); //heal random amount from range
        }
        else
        {
            player.GetComponent<HealthHandler>().HealHP(healAmount); //heals specific amount
        }
    }
}
