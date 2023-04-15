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
    private float _scrollVal;

    [SerializeField] private GameObject _equippedMeleeWeapon;
    [SerializeField] private GameObject _equippedRangedWeapon;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private Transform _meleeHoldPoint;
    [SerializeField] private Transform _rangedHoldPoint;

    [Header("Attacking")]
    public bool isAttacking = false;
    public bool isLightAttacking = false;
    public bool isHeavyAttacking = false;
    public bool inDialogue = false;
    private bool _isHeavy;
    [SerializeField] private float _lightAttackCooldown = 1f;
    [SerializeField] private float _lightDamageStart = 0.2f;
    [SerializeField] private float _lightDamageEnd = 0.7f;
    [SerializeField] private float _heavyAttackCooldown = 1.5f;
    [SerializeField] private float _heavyDamageStart = 0.5f;
    [SerializeField] private float _heavyDamageEnd = 1f;
    private float _timer;
    //private bool _isDamaging;
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

        _plInputActions.Player.MeleeAttack.started += LightAttack;
        _plInputActions.Player.MeleeAttack.performed += HeavyAttack;
        _plInputActions.Player.MeleeAttack.canceled += ReleaseMelee;

        _plInputActions.Player.ShootAttack.started += ShootAttackStarted;
        //_plInputActions.Player.ShootAttack.performed += ShootAttackPerformed;
        _plInputActions.Player.ShootAttack.canceled += ShootAttackCancel;

        _plInputActions.Player.SpecialAttack.performed += SpecialAttack;
        _plInputActions.Player.WeaponSlot1.performed += WeaponSlot1;
        _plInputActions.Player.WeaponSlot2.performed += WeaponSlot2;
        _plInputActions.Player.WeaponSlot3.performed += WeaponSlot3;
        _plInputActions.Player.WeaponSlot4.performed += WeaponSlot4;

        SelectedSlotIcons();
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

        ScrollThroughWeapons();
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
            Debug.Log("Started melee");
            Attack();
        }
    }
    private void HeavyAttack(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!inDialogue)
        {
            Debug.Log("Performed melee");
            HeavyAttack();
        }
    }
    private void ReleaseMelee(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!inDialogue)
        {
            Debug.Log("Release melee");
            //isAttacking = false;
            isHeavyAttacking = false;
        }
    }

    private void ShootAttackStarted(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!inDialogue)
        {
            Debug.Log("Shoot pressed");
        }
    }
    //private void ShootAttackPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    //{
    //    if (!inDialogue)
    //    {
    //        Debug.Log("Shoot performed");
    //    }
    //}
    private void ShootAttackCancel(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!inDialogue)
        {
            Debug.Log("shooooooooooooooot");
            Shooting();
        }
    }


    private void SpecialAttack(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {

        if (!inDialogue)
        {
            SpecialAttack();
        }
    }

    private void ScrollThroughWeapons()
    {
        _scrollVal = _plInputActions.Player.ScrollWeapons.ReadValue<Vector2>().y;

        if (_scrollVal > 0)
        {
            if (_currentSlot == _equipmentSlots.Count - 1)
            {
                _currentSlot = 1;
            }
            else
            {
                _currentSlot++;
            }
            ChangeWeapon(_currentSlot);
        }
        else if (_scrollVal < 0)
        {
            if (_currentSlot == 1)
            {
                _currentSlot = 3;
            }
            else
            {
                _currentSlot--;
            }
            ChangeWeapon(_currentSlot);
        }
    }

    private void LiveSlotUpdate()
    {
        //Update melee slot
        if (_equipmentSlots[_currentSlot - 1].EquippedItemData == null)
        {
            if (_meleeHoldPoint.childCount != 0)
            {
                Destroy(_meleeHoldPoint.GetChild(0).gameObject);
                _equippedMeleeWeapon = null;
            }
            
        }
        else
        {
            if (_meleeHoldPoint.childCount == 0)
            {
                GameObject weapon = Instantiate(_equipmentSlots[_currentSlot - 1].EquippedItemData.Prefab, _meleeHoldPoint);
                weapon.transform.localPosition = _meleeHoldPoint.transform.localPosition;
                _equippedMeleeWeapon = weapon;
            }
        }

        //Update ranged slot 
        if (_equipmentSlots[_equipmentSlots.Count - 1].EquippedItemData == null)
        {
            if (_rangedHoldPoint.childCount != 0)
            {
                Destroy(_rangedHoldPoint.GetChild(0).gameObject);
                _equippedRangedWeapon = null;
            }
        }
        else
        {
            if (_rangedHoldPoint.childCount == 0)
            {
                GameObject rangedWeapon = Instantiate(_equipmentSlots[_equipmentSlots.Count - 1].EquippedItemData.Prefab, _rangedHoldPoint);
                rangedWeapon.transform.localPosition = _rangedHoldPoint.transform.localPosition;
                _equippedRangedWeapon = rangedWeapon;
            }
        }

        //Display used weapon
        if (_isShooting)
        {
            _rangedHoldPoint.gameObject.SetActive(true);
            _meleeHoldPoint.gameObject.SetActive(false);
        }
        else
        {
            _meleeHoldPoint.gameObject.SetActive(true);
            _rangedHoldPoint.gameObject.SetActive(false);
        }

    }

    private void ChangeWeapon(int slot)
    {
        if (_currentSlot != slot && slot <= _equipmentSlots.Count)
        {
            _currentSlot = slot;
        }

        if (_meleeHoldPoint.childCount != 0)
        {
            Destroy(_meleeHoldPoint.GetChild(0).gameObject);
        }

        if (_equipmentSlots[slot - 1].EquippedItemData != null)
        {
            GameObject weapon = Instantiate(_equipmentSlots[slot - 1].EquippedItemData.Prefab, _meleeHoldPoint);
            weapon.transform.localPosition = _meleeHoldPoint.transform.localPosition;
            _equippedMeleeWeapon = weapon;
        }
        else
        {
            _equippedMeleeWeapon = null;
        }

        SelectedSlotIcons();
    }

    private void SelectedSlotIcons()
    {
        for (int i = 0; i < _equipmentSlots.Count; i++)
        {
            if (_currentSlot == i + 1)
            {
                _equipmentSlots[i].SlotSelectIcon.SetActive(true);
            }
            else
            {
                _equipmentSlots[i].SlotSelectIcon.SetActive(false);
            }
        }
    }

    public bool AutoEquipWeapon(InventoryItemData itemData)
    {
        foreach (var slot in _equipmentSlots)
        {
            if (slot.EquippedItemData != null)
            {
                if (slot.EquippedItemData == itemData)
                {
                    return false;
                }
            }
        }

        for (int i = 0; i < _equipmentSlots.Count; i++)
        {
            if (_equipmentSlots[i].EquippedItemData == null && _equipmentSlots[i].EquipType == itemData.Type)
            {
                _equipmentSlots[i].EquippedItemData = itemData;
                _equipmentSlots[i].SetupWeaponEquip();
                return true;
            }
        }
        return false;
    }

    void Attack()
    {
        if (!_isShooting && !isLightAttacking && _equippedMeleeWeapon != null && _currentSlot != 4)
        {
            //Debug.Log("Attack");
            isLightAttacking = true;
            _isHeavy = false;
            //Animator anim = _equippedWeapon.GetComponent<Animator>();
            //anim.SetTrigger("Attack");
            //StartCoroutine(ResetAttackCooldown());
        }
    }

    void HeavyAttack()
    {
        if (!_isShooting && isLightAttacking && !isHeavyAttacking && _equippedMeleeWeapon != null && _currentSlot != 4)
        {
            //Debug.Log("Heavy Attack");
            isHeavyAttacking = true;
            //_isHeavy = true;
            //_equippedWeapon.GetComponent<Damager>().UsingHeavy = true;
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
        if (!isAttacking && !_isShooting && _equippedRangedWeapon != null)
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
        if (isLightAttacking && _equippedMeleeWeapon != null)
        {
            isAttacking = true;

            if (_timer >= _lightAttackCooldown)
            {
                isLightAttacking = false;
                _timer = 0;
            }
            else if (_timer >= _lightDamageStart && _timer <= _lightDamageEnd)
            {
                _equippedMeleeWeapon.GetComponent<Damager>().CanDamage = true;
                //_isDamaging = true;
            }
            else
            {
                _equippedMeleeWeapon.GetComponent<Damager>().CanDamage = false;
                //_isDamaging = false;
            }

            if (_equippedMeleeWeapon.TryGetComponent(out WeaponEffect effect))
            {
                effect.EffectState(true);
            }

            _timer += Time.deltaTime;

        }
        else if (!isLightAttacking && isHeavyAttacking && _equippedMeleeWeapon != null)
        {
            _isHeavy = true;
            isAttacking = true;

            if (_timer >= _heavyAttackCooldown)
            {
                isHeavyAttacking = false;
            }
            else if (_timer >= _heavyDamageStart && _timer <= _heavyDamageEnd)
            {
                _equippedMeleeWeapon.GetComponent<Damager>().UsingHeavy = true;
                _equippedMeleeWeapon.GetComponent<Damager>().CanDamage = true;
                //_isDamaging = true;
            }
            else
            {
                _equippedMeleeWeapon.GetComponent<Damager>().CanDamage = false;
                //_isDamaging = false;
            }

            if (_equippedMeleeWeapon.TryGetComponent(out WeaponEffect effect))
            {
                effect.EffectState(true);
            }

            _timer += Time.deltaTime;
        }
        else
        {
            if (_equippedMeleeWeapon != null)
            {
                if (_equippedMeleeWeapon.TryGetComponent(out WeaponEffect effect))
                {
                    effect.EffectState(false);
                }
            }

            _timer = 0;
            isLightAttacking = false;
            isHeavyAttacking = false;
            isAttacking = false;
        }
    }

    private void ShootTimer()
    {
        if (_isShooting && _equippedRangedWeapon != null)
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
                _playerAnimator.SetBool("LightAttack", isLightAttacking);
            }

            if (_isHeavy)
            {
                _playerAnimator.SetBool("HeavyAttack", isHeavyAttacking);
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