using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// PlayerScript requires the GameObject to have a HealthHandler component
[RequireComponent(typeof(HealthHandler))]
public class DropItemOnDeath : MonoBehaviour
{
    [System.Serializable]
    public class DropItem
    {
        public int dropChance = 1;
        public GameObject dropObject;
    }

    [SerializeField] List<DropItem> itemsToDrop;
    HealthHandler healthHandler;

    // Start is called before the first frame update
    void Start()
    {
        healthHandler = GetComponent<HealthHandler>();
        healthHandler.OnDeathOccured += Drop;
    }

    void Drop(object sender, System.EventArgs e)
    {
        int total = 0;
        foreach (var item in itemsToDrop)
        {
            total += item.dropChance;
        }

        float value = total * Random.value;

        int sum = 0;
        foreach (var item in itemsToDrop)
        {
            sum += item.dropChance;
            if (value <= sum)
            {
                Instantiate(item.dropObject, transform.position, transform.rotation);
                break;
            }
        }
    }
}
