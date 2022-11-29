using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatSystem : MonoBehaviour
{
    GameObject sword;
    [SerializeField] bool attacking = false;
    float attackCooldown = 0;
    public float mouseHoldCounter;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MouseHoldCounter();
        InputHandler();
    }


    void InputHandler()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && mouseHoldCounter >= .5f)
        {
            HeavyAttack();
        }
        else
        {
            attacking = false;
        }
    }
    
    void Attack()
    {
        if (!attacking)
        {
            Debug.Log("Attack");
            attacking = true;
            attackCooldown = 0.5f;
            //sword.GetComponent<Collider>().enabled = true;
        }
    }

    void HeavyAttack()
    {
        if (!attacking)
        {
            Debug.Log("Heavy Attack");
            attacking = true;
            attackCooldown = 0.5f;
        }
    }

    void MouseHoldCounter()
    {
        if (Input.GetMouseButton(0))
        {
            mouseHoldCounter += Time.deltaTime;
        }
        else
        {
            mouseHoldCounter = 0;
        }
    }
}