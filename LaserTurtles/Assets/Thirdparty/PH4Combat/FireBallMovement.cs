using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallMovement : MonoBehaviour
{
    public GameObject HitParticle;

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 1000 * Time.deltaTime);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Debug.Log("Hit");
            Instantiate(HitParticle, other.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
