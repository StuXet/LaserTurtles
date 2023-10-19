using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectivePickUpItem : ObjectiveBase
{
    [SerializeField] private ItemObject _item;

    private void Awake()
    {
        _item.PickedUpItem += _item_PickedUpItem;
    }

    // Update is called once per frame
    void Update()
    {
        FinishedObjective();
    }


    private void _item_PickedUpItem(GameObject player)
    {
        BeginObjective();
        ObjectiveRequirementMet();
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!CompletedObjective)
    //    {
    //        BeginObjective();
    //    }
    //}
}
