using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Who can get Damaged
enum CanAffect
{
    Player,
    Enemy,
    Both
}

// What kind of Damage
enum DamagerType
{
    OneHit,
    OverTime,
    InstaDeath
}


public class Damager : MonoBehaviour
{
    // Variables
    // --------------------
    [Header("Damage")]
    [SerializeField] private CanAffect _canAffect;
    [SerializeField] private DamagerType _damagerType;
    [SerializeField] private ElementalModifiers _modifierType;
    [SerializeField] private int _lightDamageAmount;
    [SerializeField] private int _heavyDamageAmount;
    public float DamageModifier = 1;
    public bool UsingHeavy = false;
    public bool UsingSpecial = false;
    public bool CanDamage = false;

    [Header("Knockback")]
    [SerializeField] private float _knockbackPower = 2;
    //[SerializeField] private float _knockbackHeight = 1;
    [SerializeField] private float _knockbackHeavyMultiplier = 2;
    [SerializeField] private float _knockbackStunTime = 2;
    public bool CanKnockback;

    [Header("Stun")]
    public float minStunTime;
    public float maxStunTime;
    

    // Properties
    // --------------------
    internal CanAffect CanAffect { get { return _canAffect; } }
    internal DamagerType DamagerType { get { return _damagerType; } }
    public ElementalModifiers ModifierType { get { return _modifierType; } }

    public int LightDamageAmount { get { return _lightDamageAmount; } }
    public int HeavyDamageAmount { get { return _heavyDamageAmount; } }
    public float KnockbackPower { get { return _knockbackPower; } }
    //public float KnockbackHeight { get { return _knockbackHeight; } }
    public float KnockbackHeavyMultiplier { get { return _knockbackHeavyMultiplier; } }
    public float KnockbackStunTime { get { return _knockbackStunTime; } }
}
