using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class HealthHandler : MonoBehaviour
{
    public event EventHandler OnDeathOccured;

    public HealthSystem _healthSystem;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private WeaknessResistance _weakness;
    [SerializeField] private WeaknessResistance _resistance;
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private int _maxHP;
    [SerializeField] private int _currentHP;
    [Header("Damage Popup")]
    [SerializeField] private GameObject _dmgPopup;
    [SerializeField] private float _dmgPopupYOffset = 2;
    [SerializeField] private Color _normalDmgColor = Color.white;
    [SerializeField] private Color _resDmgColor = Color.magenta;
    [SerializeField] private Color _weakDmgColor = Color.red;
    [Header("Knockback")]
    [SerializeField] bool knockbackable = true;
    [SerializeField] float kbMass;
    private bool isKnockedBack;
    private float kbDelay = 0.2f;
    private float kbTimer;

    private void Awake()
    {
        _healthSystem = new HealthSystem(_maxHP);
        if (_healthBar != null) _healthBar.Setup(_healthSystem);
        _healthSystem.OnDeath += _healthSystem_OnDeath;
    }

    private void FixedUpdate()
    {
        kbTimer -= Time.fixedDeltaTime;
        if (isKnockedBack && kbTimer <= 0)
        {
            ResetKB();
        }
    }

    private void _healthSystem_OnDeath(object sender, System.EventArgs e)
    {
        if (OnDeathOccured != null) OnDeathOccured(this, EventArgs.Empty);
    }

    // Update is called once per frame
    void Update()
    {
        _currentHP = _healthSystem.CurrentHealth;
    }

    public void IncreaseMaxHP(int addHealth)
    {
        _maxHP += addHealth;
    }

    public void ToggleHealthBar(bool state)
    {
        if (_healthBar != null) _healthBar.gameObject.SetActive(state);
    }

    private void TakeDamage(GameObject damagerObj)
    {
        // Get Damage Info from Damager GameObject
        Damager tempDamager = damagerObj.GetComponent<Damager>();

        if (tempDamager.CanDamage)
        {
            // Check if GameObject can be Affected by Damager
            if (gameObject.tag == "Player" && tempDamager.CanAffect == CanAffect.Player || gameObject.tag == "Enemy" && tempDamager.CanAffect == CanAffect.Enemy || tempDamager.CanAffect == CanAffect.Both)
            {
                // If Damager is One Hit
                if (tempDamager.DamagerType == DamagerType.OneHit)
                {

                    if (knockbackable)
                    {
                        Knockback(tempDamager, tempDamager.UsingHeavy);
                    }

                    if (_weakness == WeaknessResistance.none && _resistance == WeaknessResistance.none)
                    {
                        HandleDamageModifierType(tempDamager, true, false, false);
                    }
                    else if (_weakness == _resistance)
                    {
                        HandleDamageModifierType(tempDamager, false, true, true);
                    }
                    else if (_weakness == tempDamager.ModifierType)
                    {
                        HandleDamageModifierType(tempDamager, false, true, false);
                    }
                    else if (_resistance == tempDamager.ModifierType)
                    {
                        HandleDamageModifierType(tempDamager, false, false, true);
                    }
                    else
                    {
                        HandleDamageModifierType(tempDamager, false, false, false);
                    }

                    tempDamager.UsingHeavy = false;
                }
                // If Damager is Over Time
                else if (tempDamager.DamagerType == DamagerType.OverTime)
                {
                    // To Be Written
                }
                // If Damager is Insta Death
                else if (tempDamager.DamagerType == DamagerType.InstaDeath)
                {
                    _healthSystem.Damage(_maxHP);
                }
            }
        }
    }

    private void HandleDamageModifierType(Damager _damager, bool _normalModifier, bool _weakness, bool _resistance)
    {
        float modVal;
        Color modColor;
        if (_normalModifier || (_weakness && _resistance))
        {
            modVal = 1;
            modColor = _normalDmgColor;
        }
        else if (_weakness)
        {
            modVal = 2;
            modColor = _weakDmgColor;
        }
        else if (_resistance)
        {
            modVal = 0.5f;
            modColor = _resDmgColor;
        }
        else
        {
            modVal = 1;
            modColor = _normalDmgColor;
        }

        if (!_damager.UsingHeavy)
        {
            _healthSystem.Damage((int)(_damager.LightDamageAmount * modVal));
            EnemyDmgPopUp((int)(_damager.LightDamageAmount * modVal), modColor, gameObject.tag);
        }
        else
        {
            _healthSystem.Damage((int)(_damager.HeavyDamageAmount * modVal));
            EnemyDmgPopUp((int)(_damager.HeavyDamageAmount * modVal), modColor, gameObject.tag);
        }
    }


    // Collision Handling
    // --------------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Damager"))
        {
            TakeDamage(other.gameObject);
        }
    }

    //KnockBack
    // --------------------
    private void Knockback(Damager damager, bool isHeavy)
    {
        if (_rb)
        {
            EnemyAI eAI = GetComponent<EnemyAI>();
            NavMeshAgent navAgent = GetComponent<NavMeshAgent>();
            kbTimer = kbDelay;

            if (eAI)
            {
                eAI.enabled = false;
            }

            if (navAgent)
            {
                navAgent.enabled = false;
            }

            _rb.isKinematic = false;
            _rb.detectCollisions = true;
            _rb.freezeRotation = true;
            //rb.mass = kbMass;

            Vector3 knockBackDir = damager.KnockbackPower * (gameObject.transform.position - damager.transform.root.position).normalized;
            //knockBackDir *= strength;
            knockBackDir.y = damager.KnockbackHeight;
            if (isHeavy)
            {
                _rb.AddForce(knockBackDir * damager.KnockbackHeavyMultiplier, ForceMode.VelocityChange);
            }
            else
            {
                _rb.AddForce(knockBackDir, ForceMode.VelocityChange);

            }
            //StartCoroutine(ResetKnockback(damager.KnockbackStunTime));
            isKnockedBack = true;
        }
    }

    //Resets enemy AI and rigidbody to their origianl state if rigidbody's velocity reaches zero
    private IEnumerator ResetKnockback(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        NavMeshAgent navAgent = GetComponent<NavMeshAgent>();
        EnemyAI eAI = gameObject.GetComponent<EnemyAI>();
        if (_rb && eAI && navAgent)
        {
            if (!_rb.isKinematic && _rb.detectCollisions && !eAI.enabled && _rb.velocity == Vector3.zero)
            {
                _rb.isKinematic = true;

                eAI.enabled = true;
                navAgent.enabled = true;
            }
        }
    }

    private void ResetKB()
    {
        NavMeshAgent navAgent = GetComponent<NavMeshAgent>();
        EnemyAI eAI = gameObject.GetComponent<EnemyAI>();
        if (_rb && eAI && navAgent)
        {
            if (!_rb.isKinematic && _rb.detectCollisions && !eAI.enabled && _rb.velocity == Vector3.zero)
            {
                _rb.isKinematic = true;

                eAI.enabled = true;
                navAgent.enabled = true;
                isKnockedBack = false;
            }
        }
    }

    private void EnemyDmgPopUp(int dmg, Color txtColor, string tag)
    {
        if (_dmgPopup)
        {
            GameObject popup = Instantiate(_dmgPopup, new Vector3(transform.position.x, transform.position.y + _dmgPopupYOffset, transform.position.z), Quaternion.identity, transform);
            TextMeshPro dmgText = popup.GetComponent<TextMeshPro>();
            dmgText.text = dmg.ToString();
            dmgText.color = txtColor;
        }
    }
}
