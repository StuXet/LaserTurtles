using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerCombatSystem : MonoBehaviour
{
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private InputManager _inputManagerRef;
    private PlayerController _playerController;
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

    [Header("EquippingCards")]
    [SerializeField] private bool _usingCardView;
    [SerializeField] private Transform _meleeCardsHolder;
    [SerializeField] private List<Transform> _meleeCards = new List<Transform>();
    private Vector3 _cardOGPos;

    [Header("Attacking")]
    public bool isAttacking = false;
    public bool isLightAttacking = false;
    public bool isHeavyAttacking = false;
    public bool inDialogue = false;
    private bool _isHeavy;
    //[SerializeField] private float _lightAttackCooldown = 1f;
    //[SerializeField] private float _lightDamageStart = 0.2f;
    //[SerializeField] private float _lightDamageEnd = 0.7f;
    [SerializeField] private float _heavyAttackCooldown = 1.5f;
    [SerializeField] private float _heavyDamageStart = 0.5f;
    [SerializeField] private float _heavyDamageEnd = 1f;
    [SerializeField] private PlayerCombo combo;
    private float _timer;
    //private bool _isDamaging;
    private float mouseHoldCounter;
    //[SerializeField] float poolForce = 2f;
    //[SerializeField] float specialAttackLength = 10f;

    [Header("Shooting")]
    [SerializeField] private bool _isShooting;
    [SerializeField] private bool _isPrepShooting;
    [SerializeField] private float _shootForce = 25f;
    [SerializeField] private float _fireRate = 0.5f;
    private float _fireRateTimer;
    public int CurrentAmmo;
    [SerializeField] private TextMeshProUGUI _ammoText;
    [SerializeField] private GameObject _aimingArrow;

    [Header("SpecialAttack")]
    public bool AllowSpecial;
    [SerializeField] private bool _specialKnockback;
    [SerializeField] private float _specialAttackDuration = 2f;
    [SerializeField] private float _specialDamageStart = 0f;
    [SerializeField] private float _specialDamageEnd = 2f;
    [SerializeField] private float _specialDamageModifier = 1f;
    public Image specialAttackBar;
    [SerializeField] private float _maxChargeBar = 100;
    [SerializeField] private float _currentChargeBar;
    [SerializeField] private float _chargeSpeedInSec = 1;
    [SerializeField] private float _timeChargeAmount = 1;
    [SerializeField] private float _killChargeAmount = 5;
    private bool _isUsingSpecial;

    private void Start()
    {
        _plInputActions = _inputManagerRef.PlInputActions;
        _playerController = GetComponent<PlayerController>();

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


        specialAttackBar.fillAmount = _currentChargeBar / _maxChargeBar;
        if (AllowSpecial) InvokeRepeating("RechargeSpecialAttackBar", 1f, _chargeSpeedInSec);
        else specialAttackBar.gameObject.SetActive(false);
        CombatHandler.Instance.OnKill.AddListener(KillRecharge);

        if (_meleeCardsHolder.childCount != 0)
        {
            _cardOGPos = _meleeCardsHolder.GetChild(_meleeCardsHolder.childCount - 1).localPosition;
        }

        SelectedSlotIcons();
    }


    // Update is called once per frame
    void Update()
    {
        //MouseHoldCounter();
        //InputHandler();
        //Debug.DrawRay(transform.position, transform.forward * specialAttackLength, Color.red);

        AttackTimer();
        ShootTimer();

        ScrollThroughWeapons();
        LiveSlotUpdate();

        RefreshUI();
        AnimationHandler();

        DisableMovementOnAttack();
    }

    void RechargeSpecialAttackBar()
    {
        if (_currentChargeBar < 0)
        {
            _currentChargeBar = 0;
        }
        else if (_currentChargeBar < _maxChargeBar)
        {
            _currentChargeBar += _timeChargeAmount;
            specialAttackBar.fillAmount = _currentChargeBar / _maxChargeBar;
        }
        else if (_currentChargeBar >= _maxChargeBar)
        {
            _currentChargeBar = _maxChargeBar;
            specialAttackBar.fillAmount = _currentChargeBar / _maxChargeBar;
        }
    }
    void KillRecharge()
    {
        if (_currentChargeBar < _maxChargeBar)
        {
            _currentChargeBar += _killChargeAmount;
            specialAttackBar.fillAmount = _currentChargeBar / _maxChargeBar;
        }
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
        // To Be Replace with SwitchAmmoType
        //ChangeWeapon(4);
    }


    private void LightAttack(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!inDialogue)
        {
            //Debug.Log("Started melee");
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
            if (isHeavyAttacking)
            {
                //isHeavyAttacking = false;
            }
            else
            {
                Debug.Log("Light attack");
                Attack();
            }

            Debug.Log("Release melee");
            //isAttacking = false;
        }
    }

    private void ShootAttackStarted(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!inDialogue && _equippedRangedWeapon != null)
        {
            Debug.Log("Shoot pressed");
            _isPrepShooting = true;
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
        if (!inDialogue && _equippedRangedWeapon != null && _isPrepShooting)
        {
            Debug.Log("shooooooooooooooot");
            Shooting();
            _isPrepShooting = false;
        }
        if (_equippedRangedWeapon == null)
        {
            StartCoroutine(FlashCardRed(_equipmentSlots[_equipmentSlots.Count - 1].transform.parent.gameObject));
        }
    }


    private void SpecialAttack(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {

        if (AllowSpecial && !inDialogue && _currentChargeBar >= _maxChargeBar)
        {
            Debug.Log("Special attack");
            SpecialAttack();
            _currentChargeBar = 0;
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
        if (_isPrepShooting || _isShooting)
        {
            _rangedHoldPoint.gameObject.SetActive(true);
            _meleeHoldPoint.gameObject.SetActive(false);
            _playerController.RotateToCursor();
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
        if (!_usingCardView)
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
        else
        {
            if (_currentSlot > 0 && _currentSlot <= _meleeCards.Count)
            {
                if (_currentSlot == 1)
                {
                    _meleeCards[0].SetSiblingIndex(2);
                    _meleeCards[1].SetSiblingIndex(1);
                    _meleeCards[2].SetSiblingIndex(0);
                }
                else if (_currentSlot == 2)
                {
                    _meleeCards[0].SetSiblingIndex(0);
                    _meleeCards[1].SetSiblingIndex(2);
                    _meleeCards[2].SetSiblingIndex(1);
                }
                else if (_currentSlot == 3)
                {
                    _meleeCards[0].SetSiblingIndex(1);
                    _meleeCards[1].SetSiblingIndex(0);
                    _meleeCards[2].SetSiblingIndex(2);
                }

                // Center Card
                Transform centerTransform = _meleeCardsHolder.GetChild(2);
                Vector3 cardPosCent = new Vector3(0, _cardOGPos.y + 10, 0);
                centerTransform.transform.localPosition = cardPosCent;
                Vector3 cardRotCent = centerTransform.localRotation.eulerAngles;
                cardRotCent.z = 0;
                centerTransform.transform.localRotation = Quaternion.Euler(cardRotCent);

                // Left Card
                Transform leftTransform = _meleeCardsHolder.GetChild(1);
                Vector3 cardPosLeft = new Vector3(75, _cardOGPos.y, 0);
                leftTransform.localPosition = cardPosLeft;
                Vector3 cardRotLeft = leftTransform.localRotation.eulerAngles;
                cardRotLeft.z = -10;
                leftTransform.localRotation = Quaternion.Euler(cardRotLeft);

                // Right Card
                Transform rightTransform = _meleeCardsHolder.GetChild(0);
                Vector3 cardPosRight = new Vector3(-75, _cardOGPos.y, 0);
                rightTransform.localPosition = cardPosRight;
                Vector3 cardRotRight = rightTransform.localRotation.eulerAngles;
                cardRotRight.z = 10;
                rightTransform.localRotation = Quaternion.Euler(cardRotRight);
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
        _playerController.RotateToCursor();
        if (!isLightAttacking && !_isShooting && _equippedMeleeWeapon != null && _currentSlot != 4)
        {
            isLightAttacking = true;
            _isHeavy = false;
            combo.OnClick();

            //Debug.Log("Attack");

            //Animator anim = _equippedWeapon.GetComponent<Animator>();
            //anim.SetTrigger("Attack");
            //StartCoroutine(ResetAttackCooldown());
        }
        else if ((combo.GetDuration() / 100 * 75 <= _timer && combo.GetDuration() >= _timer) && (isLightAttacking && !_isShooting && _equippedMeleeWeapon != null && _currentSlot != 4))
        {
            combo.OnClick();
            isLightAttacking = true;
            _isHeavy = false;
            _timer = 0;
        }
        if (_equippedMeleeWeapon == null)
        {
            StartCoroutine(FlashCardRed(_meleeCards[_currentSlot - 1].gameObject));
        }
    }

    void HeavyAttack()
    {
        if (!_isShooting && !isHeavyAttacking && _equippedMeleeWeapon != null && _currentSlot != 4)
        {
            _playerController.RotateToCursor();
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
        if (!_isShooting && !isLightAttacking && !isHeavyAttacking && _equippedMeleeWeapon != null && _currentSlot != 4)
        {
            _isUsingSpecial = true;
        }

        //if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, specialAttackLength))
        //{
        //    if (hit.collider.tag == "Enemy")
        //    {
        //        //Adds a rigidbody to the object if it does'nt have one
        //        Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
        //        EnemyAI eAI = hit.collider.GetComponent<EnemyAI>();
        //        NavMeshAgent navAgent = hit.collider.GetComponent<NavMeshAgent>();
        //        if (!rb)
        //        {
        //            hit.collider.AddComponent<Rigidbody>();
        //            rb = hit.collider.GetComponent<Rigidbody>();
        //        }

        //        if (eAI)
        //        {
        //            eAI.enabled = false;
        //        }

        //        if (navAgent)
        //        {
        //            navAgent.enabled = false;
        //        }

        //        rb.isKinematic = false;
        //        rb.detectCollisions = true;
        //        rb.freezeRotation = true;

        //        Vector3 dir = transform.position - hit.transform.position;
        //        rb = hit.collider.GetComponent<Rigidbody>();
        //        rb.AddForce(dir * poolForce, ForceMode.Impulse);
        //        Debug.Log("Special Attack");
        //        StartCoroutine(ResetAI(1.5f, hit.collider));
        //    }
        //}
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
            else
            {
                StartCoroutine(FlashCardRed(_ammoText.transform.parent.gameObject));
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
        if (isLightAttacking && !_isUsingSpecial && _equippedMeleeWeapon != null)
        {
            isAttacking = true;

            if (_timer >= combo.GetDuration())
            {
                Damager currentDamager = _equippedMeleeWeapon.GetComponent<Damager>();
                currentDamager.CanDamage = false;
                isLightAttacking = false;
                _timer = 0;
            }
            else if (_timer >= combo.GetActiveStart() && _timer <= combo.GetActiveEnd())
            {
                //_equippedMeleeWeapon.GetComponent<Damager>().CanDamage = true;
                Damager currentDamager = _equippedMeleeWeapon.GetComponent<Damager>();
                currentDamager.CanDamage = true;
                currentDamager.CanKnockback = combo.GetCanKnockback();
                currentDamager.DamageModifier = combo.GetDamageMultiplier();

                //_isDamaging = true;
            }
            else
            {
                //_equippedMeleeWeapon.GetComponent<Damager>().CanDamage = false;
                Damager currentDamager = _equippedMeleeWeapon.GetComponent<Damager>();
                currentDamager.CanDamage = false;
                //_isDamaging = false;
            }

            if (_equippedMeleeWeapon.TryGetComponent(out WeaponEffect effect))
            {
                effect.EffectState(true);
            }

            _timer += Time.deltaTime;

        }
        else if (!isLightAttacking && isHeavyAttacking && !_isUsingSpecial && _equippedMeleeWeapon != null)
        {
            _isHeavy = true;
            isAttacking = true;

            if (_timer >= _heavyAttackCooldown)
            {
                isHeavyAttacking = false;
            }
            else if (_timer >= _heavyDamageStart && _timer <= _heavyDamageEnd)
            {
                Damager currentDamager = _equippedMeleeWeapon.GetComponent<Damager>();
                currentDamager.UsingHeavy = true;
                currentDamager.CanDamage = true;
                currentDamager.CanKnockback = true;
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
        else if (_isUsingSpecial && !isLightAttacking && !isHeavyAttacking && _equippedMeleeWeapon != null)
        {
            isAttacking = true;

            if (_timer >= _specialAttackDuration)
            {
                Damager currentDamager = _equippedMeleeWeapon.GetComponent<Damager>();
                currentDamager.CanDamage = false;
                _isUsingSpecial = false;
                _timer = 0;
            }
            else if (_timer >= _specialDamageStart && _timer <= _specialDamageEnd)
            {
                //_equippedMeleeWeapon.GetComponent<Damager>().CanDamage = true;
                Damager currentDamager = _equippedMeleeWeapon.GetComponent<Damager>();
                currentDamager.CanDamage = true;
                currentDamager.CanKnockback = _specialKnockback;
                currentDamager.DamageModifier = _specialDamageModifier;

                //_isDamaging = true;
            }
            else
            {
                //_equippedMeleeWeapon.GetComponent<Damager>().CanDamage = false;
                Damager currentDamager = _equippedMeleeWeapon.GetComponent<Damager>();
                currentDamager.CanDamage = false;
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

    private void RefreshUI()
    {
        //// Ammo Counter UI
        //if (CurrentAmmo == 0)
        //{
        //    _ammoText.enabled = false;
        //}
        //else 
        //{ 
        //    _ammoText.enabled = true;
        _ammoText.text = CurrentAmmo.ToString();
        if (CurrentAmmo == 0)
        {
            _ammoText.color = Color.red;
        }
        else
        {
            _ammoText.color = Color.white;
        }
        //}

        // Aiming Arrow UI
        if (_isPrepShooting)
        {
            _aimingArrow.SetActive(true);
        }
        else
        {
            _aimingArrow.SetActive(false);
        }
    }

    IEnumerator FlashCardRed(GameObject card)
    {
        Image image = card.GetComponent<Image>();
        image.color = new Color(1f, 0.5f, 0.5f, 1f);
        yield return new WaitForSeconds(0.25f);
        image.color = Color.white;
        yield return new WaitForSeconds(0.25f);
        image.color = new Color(1f, 0.5f, 0.5f, 1f);
        yield return new WaitForSeconds(0.25f);
        image.color = Color.white;
        yield return new WaitForSeconds(0.25f);
    }

    private void AnimationHandler()
    {
        if (_playerAnimator)
        {
            if (!_isHeavy)
            {
                //_playerAnimator.SetBool("LightAttack", isLightAttacking);
            }

            if (_isHeavy)
            {
                _playerAnimator.SetBool("HeavyAttack", isHeavyAttacking);
            }

            if (AllowSpecial)
            {
                _playerAnimator.SetBool("SpecialAttack", _isUsingSpecial);
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

    private void DisableMovementOnAttack()
    {
        _playerController.CurrentMaxSpeed = _playerController.MaxSpeed;
        if (!_isUsingSpecial && !_isPrepShooting)
        {
            if (isAttacking || isHeavyAttacking || isLightAttacking)
            {
                //_playerController.InControl = false;
                _playerController.InControl = true;
                _playerController.CurrentMaxSpeed = _playerController.MaxSpeed / 4;
            }
            else
            {
                if (!_playerController.IsDead)
                {
                    _playerController.InControl = true;
                }
            }
        }
        else if (_isUsingSpecial)
        {
            _playerController.InControl = true;
            _playerController.CurrentMaxSpeed = _playerController.MaxSpeed / 4 * 3;
        }
        else if (_isPrepShooting)
        {
            _playerController.InControl = true;
            _playerController.CurrentMaxSpeed = _playerController.MaxSpeed / 2;
        }
    }
}