using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDestination : MonoBehaviour
{
    public event EventHandler ReachedDest;

    private void OnTriggerEnter(Collider other)
    {
        if (ReachedDest!= null) { ReachedDest.Invoke(this, EventArgs.Empty); }
    }
}
