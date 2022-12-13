using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPAdderCollectable : MonoBehaviour
{
    public int hpToAdd = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            HealthHandler healthHandler = other.gameObject.GetComponent<HealthHandler>();
            healthHandler.IncreaseMaxHP(hpToAdd);
            Destroy(gameObject);
        }
    }
}
