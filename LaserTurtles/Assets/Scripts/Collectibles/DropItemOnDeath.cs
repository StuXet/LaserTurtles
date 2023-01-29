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
        [Range(0,100)]
        public int dropChance = 1;
        public GameObject dropObject;
    }

    [SerializeField] List<DropItem> itemsToDrop;
    HealthHandler healthHandler;

    public List<DropItem> ItemsToDrop { get => itemsToDrop;}

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
                if (item != null)
                {
                    GameObject drop = Instantiate(item.dropObject, transform.position, transform.rotation);
                    drop.GetComponent<ItemObject>().CanBePicked = true;
                }
                break;
            }
        }
    }
}
