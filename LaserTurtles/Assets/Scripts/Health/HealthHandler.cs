using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class HealthHandler : MonoBehaviour
{
    public event EventHandler OnDeathOccured;
    public event EventHandler OnDamageOccured;

    public HealthSystem _healthSystem;
    [SerializeField] private EnemyAI _enemyAI;
    [SerializeField] private WeaknessResistance _weakness;
    [SerializeField] private WeaknessResistance _resistance;
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private int _maxHP;
    [SerializeField] private int _currentHP;
    [SerializeField] private bool _invulnerable = false;
    //[Header("Damage Popup")]
    //[SerializeField] private GameObject _dmgPopup;
    //[SerializeField] private float _dmgPopupYOffset = 2;
    //[SerializeField] private Color _normalDmgColor = Color.white;
    //[SerializeField] private Color _resDmgColor = Color.magenta;
    //[SerializeField] private Color _weakDmgColor = Color.red;
    [Header("Knockback")]
    [SerializeField] bool knockbackable = true;
    //[SerializeField] private float _knockbackForceModifier = 1f;

    public bool Invulnerable { get => _invulnerable; set => _invulnerable = value; }


    private void Awake()
    {
        _healthSystem = new HealthSystem(_maxHP);
        if (_healthBar != null) _healthBar.Setup(_healthSystem);
        _healthSystem.OnDeath += _healthSystem_OnDeath;
        _healthSystem.OnDamaged += _healthSystem_OnDamaged;
    }

    private void _healthSystem_OnDamaged(object sender, EventArgs e)
    {
        if (OnDamageOccured != null) OnDamageOccured(this, EventArgs.Empty);
    }

    private void _healthSystem_OnDeath(object sender, System.EventArgs e)
    {
        if (OnDeathOccured != null) OnDeathOccured(this, EventArgs.Empty);
        if (gameObject.CompareTag("Enemy"))
        {
            CombatHandler.Instance.killCounter++;
            CombatHandler.Instance.OnKill.Invoke();
        }
    }

    // Update is called once per frame
    void Update()
    {
        _currentHP = _healthSystem.CurrentHealth;
    }

    public void HealHP(int hp)
    {
        _healthSystem.Heal(hp);
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
                if (gameObject.tag == "Enemy")
                {
                    Shake.instance.ScreenShake(0.2f);
                }

                // If Damager is One Hit
                if (tempDamager.DamagerType == DamagerType.OneHit)
                {

                    if (tempDamager.CanKnockback && knockbackable)
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
        //Color modColor;
        if (_normalModifier || (_weakness && _resistance))
        {
            modVal = 1;
            //modColor = _normalDmgColor;
        }
        else if (_weakness)
        {
            modVal = 2;
            //modColor = _weakDmgColor;
        }
        else if (_resistance)
        {
            modVal = 0.5f;
            //modColor = _resDmgColor;
        }
        else
        {
            modVal = 1;
            //modColor = _normalDmgColor;
        }


        if (!_damager.UsingHeavy)
        {
            _healthSystem.Damage((int)(_damager.LightDamageAmount * modVal * _damager.DamageModifier));
            //EnemyDmgPopUp((int)(_damager.LightDamageAmount * modVal), modColor, gameObject.tag);
        }
        else
        {
            _healthSystem.Damage((int)(_damager.HeavyDamageAmount * modVal));
            //EnemyDmgPopUp((int)(_damager.HeavyDamageAmount * modVal), modColor, gameObject.tag);
        }
    }


    // Collision Handling
    // --------------------
    private void OnTriggerEnter(Collider other)
    {
        if (!_invulnerable)
        {
            if (other.CompareTag("Damager"))
            {
                TakeDamage(other.gameObject);
            }
        }
    }

    //KnockBack
    // --------------------
    private void Knockback(Damager damager, bool isHeavy)
    {
        if (_enemyAI)
        {
            //_enemyAI.Knockback(1, 1, transform.forward * -1);
            Vector3 knockBackDir = _enemyAI.transform.position - damager.transform.position;
            knockBackDir.y = 0;
            knockBackDir = knockBackDir.normalized;
            if (isHeavy)
            {
                _enemyAI.Knockback(damager.KnockbackStunTime, damager.KnockbackPower * damager.KnockbackHeavyMultiplier /** _knockbackForceModifier*/, knockBackDir);
            }
            else
            {
                _enemyAI.Knockback(damager.KnockbackStunTime, damager.KnockbackPower /** _knockbackForceModifier*/, knockBackDir);
            }
        }
    }


    //private void EnemyDmgPopUp(int dmg, Color txtColor, string tag)
    //{
    //    if (_dmgPopup)
    //    {
    //        GameObject popup = Instantiate(_dmgPopup, new Vector3(transform.position.x, transform.position.y + _dmgPopupYOffset, transform.position.z), Quaternion.identity, transform);
    //        TextMeshPro dmgText = popup.GetComponent<TextMeshPro>();
    //        dmgText.text = dmg.ToString();
    //        dmgText.color = txtColor;
    //    }
    //}

}