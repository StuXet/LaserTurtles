using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public PlayerCombatSystem playerCombatSystem;
    public GameObject HitParticle;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy" && playerCombatSystem.isLightAttacking)
        {
            Debug.Log("Hit");
            Instantiate(HitParticle, other.transform.position, Quaternion.identity);
        }
    }
}
