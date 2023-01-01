using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropCollectible : MonoBehaviour
{
    [Header("Drop Settings")]
    [SerializeField] bool isLaunching;
    [SerializeField] float launchStrength;
    [SerializeField] float launchRotationStrength;
    [SerializeField] float launchHeight;
    Rigidbody rbody;
    Vector3 playerPos;

    
    protected void Launch() //Launches the drop on spawn
    {
        playerPos = GameObject.FindWithTag("Player").transform.position;

        if (isLaunching)
        {
            if (!GetComponent<Rigidbody>())
            {
                gameObject.AddComponent<Rigidbody>();
            }
            rbody = GetComponent<Rigidbody>();

            Vector3 launchDir = (playerPos - transform.position).normalized;
            launchDir.y = launchHeight;
            Vector3 launchRotation = new Vector3(Random.value, Random.value, Random.value);

            rbody.AddForce(launchDir * launchStrength, ForceMode.Impulse);
            rbody.AddTorque(launchRotation * launchRotationStrength, ForceMode.Impulse);
        }
    }
}
