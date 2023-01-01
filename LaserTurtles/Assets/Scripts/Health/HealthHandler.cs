using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HealthHandler : MonoBehaviour
{
    public event EventHandler OnDeathOccured;

    private HealthSystem _healthSystem;

    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private int _maxHP;
    [SerializeField] private int _currentHP;
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
                    if (!tempDamager.UsingHeavy)
                    {
                        if (knockbackable)
                        {
                            Knockback(tempDamager, false);
                        }
                        _healthSystem.Damage(tempDamager.LightDamageAmount);
                    }
                    else
                    {
                        if (knockbackable)
                        {
                            Knockback(tempDamager, true);
                        }
                        _healthSystem.Damage(tempDamager.HeavyDamageAmount);
                        tempDamager.UsingHeavy = false;
                    }
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
        //Adds a rigidbody to the object if it does'nt have one
        Rigidbody rb = GetComponent<Rigidbody>();
        EnemyAI eAI = GetComponent<EnemyAI>();
        NavMeshAgent navAgent = GetComponent<NavMeshAgent>();
        kbTimer = kbDelay;
        if (!rb)
        {
            gameObject.AddComponent<Rigidbody>();
            rb = GetComponent<Rigidbody>();
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
        //rb.mass = kbMass;

        Vector3 knockBackDir = damager.KnockbackPower * (gameObject.transform.position - damager.transform.root.position).normalized;
        //knockBackDir *= strength;
        knockBackDir.y = damager.KnockbackHeight;
        if (isHeavy)
        {
            rb.AddForce(knockBackDir * damager.KnockbackHeavyMultiplier, ForceMode.VelocityChange);
        }
        else
        {
            rb.AddForce(knockBackDir, ForceMode.VelocityChange);

        }
        //StartCoroutine(ResetKnockback(damager.KnockbackStunTime));
        isKnockedBack = true;
    }

    //Resets enemy AI and rigidbody to their origianl state if rigidbody's velocity reaches zero
    private IEnumerator ResetKnockback(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        NavMeshAgent navAgent = GetComponent<NavMeshAgent>();
        EnemyAI eAI = gameObject.GetComponent<EnemyAI>();
        if (rb && eAI && navAgent)
        {
            if (!rb.isKinematic && rb.detectCollisions && !eAI.enabled && rb.velocity == Vector3.zero)
            {
                rb.isKinematic = true;

                eAI.enabled = true;
                navAgent.enabled = true;
            }
        }
    }

    private void ResetKB()
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        NavMeshAgent navAgent = GetComponent<NavMeshAgent>();
        EnemyAI eAI = gameObject.GetComponent<EnemyAI>();
        if (rb && eAI && navAgent)
        {
            if (!rb.isKinematic && rb.detectCollisions && !eAI.enabled && rb.velocity == Vector3.zero)
            {
                rb.isKinematic = true;

                eAI.enabled = true;
                navAgent.enabled = true;
                isKnockedBack = false;
            }
        }
    }
}
