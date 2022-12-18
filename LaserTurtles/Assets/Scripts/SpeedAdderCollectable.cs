using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedAdderCollectable : MonoBehaviour
{
    public float speedToAdd = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            playerController.Speed += speedToAdd;
            Destroy(gameObject);
        }
    }
}
