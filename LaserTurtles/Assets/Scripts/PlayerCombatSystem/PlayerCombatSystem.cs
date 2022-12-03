using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatSystem : MonoBehaviour
{
    [SerializeField] GameObject sword;
    [SerializeField] GameObject fireBall;
    [SerializeField] Transform shootingPoint;
    public bool isAttacking = false;
    float attackCooldown = 1f;
    public float mouseHoldCounter;
    
    
    // Update is called once per frame
    void Update()
    {
        MouseHoldCounter();
        InputHandler();
    }


    void InputHandler()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (mouseHoldCounter < 0.6f)
            {
                Attack();
            }
            else
            {
                HeavyAttack();
            }
            mouseHoldCounter = 0;
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Shooting();
        }
    }
    
    void Attack()
    {
        if (!isAttacking)
        {
            Debug.Log("Attack");
            isAttacking = true;
            Animator anim = sword.GetComponent<Animator>();
            anim.SetTrigger("Attack");
            StartCoroutine(ResetAttackCooldown());
        }
    }

    void HeavyAttack()
    {
        if (!isAttacking)
        {
            Debug.Log("Heavy Attack");
            isAttacking = true;
            Animator anim = sword.GetComponent<Animator>();
            anim.SetTrigger("HeavyAttack");
            StartCoroutine(ResetAttackCooldown());
        }
    }

    void Shooting()
    {
        Instantiate(fireBall, shootingPoint.position, shootingPoint.rotation);
    }

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }
    

    void MouseHoldCounter()
    {
        if (Input.GetMouseButton(0))
        {
            mouseHoldCounter += Time.deltaTime;
        }
    }
}