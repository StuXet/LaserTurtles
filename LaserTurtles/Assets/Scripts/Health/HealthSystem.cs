using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public event EventHandler OnHealthChanged;
    [SerializeField] private GameObject _mainObj;
    private int _maxHealth;
    private int _currentHealth;
    public int CurrentHealth { get { return _currentHealth; } }

    public HealthSystem(int maxHealth, GameObject mainObj)
    {
        _maxHealth = maxHealth;
        _currentHealth = _maxHealth;
        _mainObj = mainObj;
    }

    public float GetHealthPercent()
    {
        return (float)_currentHealth / _maxHealth;
    }

    public void Damage(int damageAmount)
    {
        _currentHealth -= damageAmount;
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Death();
        }
        if (OnHealthChanged != null) OnHealthChanged(this, EventArgs.Empty);
    }

    public void Heal(int healAmount)
    {
        _currentHealth += healAmount;
        if (_currentHealth >= _maxHealth)
        {
            _currentHealth = _maxHealth;
        }
        if (OnHealthChanged != null) OnHealthChanged(this, EventArgs.Empty);
    }

    public void RefillHealth()
    {
        _currentHealth = _maxHealth;
        if (OnHealthChanged != null) OnHealthChanged(this, EventArgs.Empty);
    }

    virtual public void Death()
    {
        Destroy(_mainObj);
    }
}
