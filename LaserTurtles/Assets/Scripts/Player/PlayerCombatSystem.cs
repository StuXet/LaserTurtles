using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerCombatSystem : MonoBehaviour
{
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private InputManager _inputManagerRef;
    private PlayerInputActions _plInputActions;

    [Header("Equipping")]
    [SerializeField] private List<EquipmentSlot> _equipmentSlots = new List<EquipmentSlot>();
    private int _currentSlot = 1;

    [SerializeField] private GameObject _equippedWeapon;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private Transform _weaponHoldPoint;

    [Header("Attacking")]
    public bool isAttacking = false;
    public bool inDialogue = false;
    private bool _isHeavy;
    [SerializeField] private float _attackCooldown = 1f;
    [SerializeField] private float _damageStart = 0.2f;
    [SerializeField] private float _damageEnd = 0.7f;
    private float _timer;
    private float mouseHoldCounter;
    [SerializeField] float poolForce = 2f;
    [SerializeField] float specialAttackLength = 10f;

    [Header("Shooting")]
    [SerializeField] private bool _isShooting;
    [SerializeField] private float _shootForce = 25f;
    [SerializeField] private float _fireRate = 0.5f;
    private float _fireRateTimer;
    public int CurrentAmmo;
    [SerializeField] private TextMeshProUGUI _ammoText;


    private void Start()
    {
        _plInputActions = _inputManagerRef.PlInputActions;
        _plInputActions.Player.LightAttack.performed += LightAttack;
        _plInputActions.Player.HeavyAttack.performed += HeavyAttack;
        _plInputActions.Player.SpecialAttack.performed += SpecialAttack;

        _plInputActions.Player.WeaponSlot1.performed += WeaponSlot1;
        _plInputActions.Player.WeaponSlot2.performed += WeaponSlot2;
        _plInputActions.Player.WeaponSlot3.performed += WeaponSlot3;
        _plInputActions.Player.WeaponSlot4.performed += WeaponSlot4;
    }


    // Update is called once per frame
    void Update()
    {
        //MouseHoldCounter();
        //InputHandler();
        Debug.DrawRay(transform.position, transform.forward * specialAttackLength, Color.red);

        AttackTimer();
        ShootTimer();
        AmmoCountHandler();

        LiveSlotUpdate();

        AnimationHandler();
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



    private void WeaponSlot1(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        ChangeWeapon(1);
    }
    private void WeaponSlot2(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        ChangeWeapon(2);
    }
    private void WeaponSlot3(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        ChangeWeapon(3);
    }
    private void WeaponSlot4(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        ChangeWeapon(4);
    }


    private void LightAttack(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!inDialogue)
        {
            Attack();
            Shooting();
        }
    }

    private void HeavyAttack(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!inDialogue)
        {
            HeavyAttack();
        }
    }


    private void SpecialAttack(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {

        if (!inDialogue)
        {
            SpecialAttack();
        }
    }

    private void LiveSlotUpdate()
    {
        if (_equipmentSlots[_currentSlot - 1].EquippedItemData == null)
        {
            if (_weaponHoldPoint.childCount != 0)
            {
                Destroy(_weaponHoldPoint.GetChild(0).gameObject);
                _equippedWeapon = null;
            }
        }
        else
        {
            if (_weaponHoldPoint.childCount == 0)
            {
                GameObject weapon = Instantiate(_equipmentSlots[_currentSlot - 1].EquippedItemData.Prefab, _weaponHoldPoint);
                weapon.transform.localPosition = _weaponHoldPoint.transform.localPosition;
                _equippedWeapon = weapon;
            }
        }
    }

    private void ChangeWeapon(int slot)
    {
        if (_currentSlot != slot && slot <= _equipmentSlots.Count)
        {
            _currentSlot = slot;

            if (_weaponHoldPoint.childCount != 0)
            {
                Destroy(_weaponHoldPoint.GetChild(0).gameObject);
            }

            if (_equipmentSlots[slot - 1].EquippedItemData != null)
            {
                GameObject weapon = Instantiate(_equipmentSlots[slot - 1].EquippedItemData.Prefab, _weaponHoldPoint);
                weapon.transform.localPosition = _weaponHoldPoint.transform.localPosition;
                _equippedWeapon = weapon;
            }
            else
            {
                _equippedWeapon = null;
            }
        }
    }


    void Attack()
    {
        if (!isAttacking && _equippedWeapon != null && _currentSlot != 4)
        {
            Debug.Log("Attack");
            isAttacking = true;
            _isHeavy = false;
            //Animator anim = _equippedWeapon.GetComponent<Animator>();
            //anim.SetTrigger("Attack");
            //StartCoroutine(ResetAttackCooldown());
        }
    }

    void HeavyAttack()
    {
        if (!isAttacking && _equippedWeapon != null && _currentSlot != 4)
        {
            Debug.Log("Heavy Attack");
            isAttacking = true;
            _isHeavy = true;
            _equippedWeapon.GetComponent<Damager>().UsingHeavy = true;
            //Animator anim = _equippedWeapon.GetComponent<Animator>();
            //anim.SetTrigger("HeavyAttack");
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
        if (!_isShooting && _equippedWeapon != null && _currentSlot == 4)
        {
            if (CurrentAmmo > 0)
            {
                CurrentAmmo--;
                _isShooting = true;
                GameObject projectile = Instantiate(_projectile, shootingPoint.position, shootingPoint.rotation);
                projectile.GetComponent<Damager>().CanDamage = true;
                projectile.GetComponent<Destroyer>().CanBeDestroyed = true;
                projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * _shootForce, ForceMode.Impulse);
            }
        }
    }

    public void AddAmmo(int val)
    {
        CurrentAmmo += val;
    }

    //IEnumerator ResetAttackCooldown()
    //{
    //    yield return new WaitForSeconds(_attackCooldown);
    //    isAttacking = false;
    //}

    private void AttackTimer()
    {
        if (isAttacking && _equippedWeapon != null)
        {
            if (_timer >= _attackCooldown)
            {
                isAttacking = false;
            }
            else if (_timer >= _damageStart && _timer <= _damageEnd)
            {
                _equippedWeapon.GetComponent<Damager>().CanDamage = true;
            }
            else
            {
                _equippedWeapon.GetComponent<Damager>().CanDamage = false;
            }

            _timer += Time.deltaTime;
        }
        else
        {
            _timer = 0;
            isAttacking = false;
        }
    }

    private void ShootTimer()
    {
        if (_isShooting && _equippedWeapon != null)
        {
            if (_fireRateTimer >= _fireRate)
            {
                _isShooting = false;
            }

            _fireRateTimer += Time.deltaTime;
        }
        else
        {
            _fireRateTimer = 0;
            _isShooting = false;
        }
    }

    private void AmmoCountHandler()
    {
        //if (CurrentAmmo == 0)
        //{
        //    _ammoText.enabled = false;
        //}
        //else 
        //{ 
        //    _ammoText.enabled = true;
        _ammoText.text = CurrentAmmo.ToString();
        //}
    }

    private void AnimationHandler()
    {
        if (_playerAnimator)
        {
            if (!_isHeavy)
            {
                _playerAnimator.SetBool("LightAttack", isAttacking);
            }

            if (_isHeavy)
            {
                _playerAnimator.SetBool("HeavyAttack", isAttacking);
            }
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