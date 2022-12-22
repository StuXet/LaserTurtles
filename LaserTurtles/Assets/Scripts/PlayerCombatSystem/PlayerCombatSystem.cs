using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatSystem : MonoBehaviour
{
    [SerializeField] private InputManager _inputManagerRef;
    private PlayerInputActions _plInputActions;

    [SerializeField] GameObject sword;
    [SerializeField] GameObject fireBall;
    [SerializeField] Transform shootingPoint;
    public bool isAttacking = false;
    [SerializeField] private float _attackCooldown = 1f;
    [SerializeField] private float _damageLength = 0.1f;
    private float _timer;
    private float mouseHoldCounter;

    private void Start()
    {
        _plInputActions = _inputManagerRef.PlInputActions;
        _plInputActions.Player.RangedAttack.performed += RangedAttack;
        _plInputActions.Player.LightAttack.performed += LightAttack;
        _plInputActions.Player.HeavyAttack.performed += HeavyAttack;
        _plInputActions.Player.SpecialAttack.performed += SpecialAttack;  
    }



    // Update is called once per frame
    void Update()
    {
        //MouseHoldCounter();
        //InputHandler();
        AttackTimer();
    }


    //void InputHandler()
    //{
    //    //if (Input.GetKeyUp(KeyCode.Mouse0))
    //    //{
    //    //    if (mouseHoldCounter < 0.6f)
    //    //    {
    //    //        Attack();
    //    //    }
    //    //    else
    //    //    {
    //    //        HeavyAttack();
    //    //    }
    //    //    mouseHoldCounter = 0;
    //    //}
    //}


    private void LightAttack(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Attack();
    }

    private void HeavyAttack(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        HeavyAttack();
    } 
    private void RangedAttack(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Shooting();
    }
    
    private void SpecialAttack(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        SpecialAttack();
    }
    

    void Attack()
    {
        if (!isAttacking)
        {
            Debug.Log("Attack");
            isAttacking = true;
            Animator anim = sword.GetComponent<Animator>();
            anim.SetTrigger("Attack");
            //StartCoroutine(ResetAttackCooldown());
        }
    }

    void HeavyAttack()
    {
        if (!isAttacking)
        {
            Debug.Log("Heavy Attack");
            isAttacking = true;
            sword.GetComponent<Damager>().UsingHeavy = true;
            Animator anim = sword.GetComponent<Animator>();
            anim.SetTrigger("HeavyAttack");
            //StartCoroutine(ResetAttackCooldown());
        }
    }

    void SpecialAttack()
    {

    }

    void Shooting()
    {
        Instantiate(fireBall, shootingPoint.position, shootingPoint.rotation);
    }

    //IEnumerator ResetAttackCooldown()
    //{
    //    yield return new WaitForSeconds(_attackCooldown);
    //    isAttacking = false;
    //}

    private void AttackTimer()
    {
        if (isAttacking)
        {
            if (_timer >= _attackCooldown)
            {
                isAttacking = false;
            }
            else if (_timer <= _damageLength)
            {
                sword.GetComponent<Damager>().CanDamage = true;
            }
            else if (_timer >= _damageLength)
            {
                sword.GetComponent<Damager>().CanDamage = false;
            }

            _timer += Time.deltaTime;
        }
        else
        {
            _timer = 0;
        }
    }


    //void MouseHoldCounter()
    //{
    //    if (_plActions.Player.LightAttack.IsPressed())
    //    {
    //        mouseHoldCounter += Time.deltaTime;
    //    }
    //}
}