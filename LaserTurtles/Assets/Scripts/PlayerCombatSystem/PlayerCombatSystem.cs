using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

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
    [SerializeField] float poolForce = 2f;
    [SerializeField] float specialAttackLength = 10f;

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
        Debug.DrawRay(transform.position, transform.forward * specialAttackLength, Color.red);
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
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, specialAttackLength))
        {
            if (hit.collider.tag == "Enemy")
            {
                //Adds a rigidbody to the object if it does'nt have one
                Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
                EnemyAI eAI = hit.collider.GetComponent<EnemyAI>();
                NavMeshAgent navAgent = hit.collider.GetComponent<NavMeshAgent>();
                if (!rb)
                {
                    hit.collider.AddComponent<Rigidbody>();
                    rb = hit.collider.GetComponent<Rigidbody>();
                }

                if (eAI)
                {
                    eAI.enabled = false;
                }

                if (navAgent)
                {
                    navAgent.enabled = false;
                }

                rb.isKinematic = false;
                rb.detectCollisions = true;
                rb.freezeRotation = true;
                
                Vector3 dir = transform.position - hit.transform.position;
                rb = hit.collider.GetComponent<Rigidbody>();
                rb.AddForce(dir * poolForce, ForceMode.Impulse);
                Debug.Log("Special Attack");
                StartCoroutine(ResetAI(1.5f, hit.collider));
            }
        }
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

    //Resets enemy AI and rigidbody to their origianl state if rigidbody's velocity reaches zero
    private IEnumerator ResetAI(float seconds, Collider col)
    {
        yield return new WaitForSeconds(seconds);
        Rigidbody rb = col.GetComponent<Rigidbody>();
        NavMeshAgent navAgent = col.GetComponent<NavMeshAgent>();
        EnemyAI eAI = col.GetComponent<EnemyAI>();
        if (rb && eAI && navAgent)
        {
            if (!rb.isKinematic && rb.detectCollisions && !eAI.enabled)
            {
                rb.isKinematic = true;

                eAI.enabled = true;
                navAgent.enabled = true;
                Debug.Log("BIG PP");
            }
        }
    }
}