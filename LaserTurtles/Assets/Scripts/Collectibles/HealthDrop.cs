using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDrop : DropCollectible
{
    [Header("Effect Settings")]
    [SerializeField] bool isRandom;
    [SerializeField] int healAmount = 5;
    [SerializeField] int randHealAmountMin = 5;
    [SerializeField] int randHealAmountMax = 10;

    // Start is called before the first frame update
    void Start()
    {
        Launch();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            HealthHandler healthHandler = other.gameObject.GetComponent<HealthHandler>();
            if (isRandom)
            {
                healthHandler.IncreaseMaxHP(Random.Range(randHealAmountMin, randHealAmountMax)); //heal random amount from range
            }
            else
            {
                healthHandler.IncreaseMaxHP(healAmount); //heals specific amount
            }
            Destroy(gameObject);
        }
    }
}
