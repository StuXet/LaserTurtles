using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public InventoryItemData DataRef { get; private set; }
    public int StackSize { get; private set; }

    public InventoryItem(InventoryItemData source)
    {
        DataRef = source;
        AddToStack();
    }

    public void AddToStack()
    {
        StackSize++;
    }

    public void RemoveFromStack()
    {
        StackSize--;
    }
}
