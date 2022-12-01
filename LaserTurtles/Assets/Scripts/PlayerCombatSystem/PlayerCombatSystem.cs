using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatSystem : MonoBehaviour
{
    [SerializeField] GameObject Sword;
    public bool isAttacking = false;
    public bool canAttack = true;
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
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
        }
    }
    
    void Attack()
    {
        if (canAttack)
        {
            Debug.Log("Attack");
            isAttacking = true;
            canAttack = false;
            Animator anim = Sword.GetComponent<Animator>();
            anim.SetTrigger("Attack");
            StartCoroutine(ResetAttackCooldown());
        }
    }

    void HeavyAttack()
    {
        if (canAttack)
        {
            Debug.Log("Heavy Attack");
            isAttacking = true;
            canAttack = false;
            Animator anim = Sword.GetComponent<Animator>();
            anim.SetTrigger("HeavyAttack");
            StartCoroutine(ResetAttackCooldown());
        }
    }

    IEnumerator ResetAttackCooldown()
    {
        StartCoroutine(ResetAttackBool());
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    IEnumerator ResetAttackBool()
    {
        yield return new WaitForSeconds(1f);
        isAttacking = false;
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