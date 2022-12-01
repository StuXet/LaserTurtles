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
    [SerializeField] private GameObject _usedBy;
    [SerializeField] private CanAffect _canAffect;
    [SerializeField] private DamagerType _damagerType;
    [SerializeField] private int _damageAmount;


    // Properties
    // --------------------
    public GameObject UsedBy { get { return _usedBy; } }
    internal CanAffect CanAffect { get { return _canAffect; } }
    internal DamagerType DamagerType { get { return _damagerType; } }
    public int DamageAmount { get { return _damageAmount; } }

}
