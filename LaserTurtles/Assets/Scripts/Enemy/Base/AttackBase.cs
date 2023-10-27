using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBase : MonoBehaviour
{
    [Header("Base Variables")]
    public AttackBase _currentAttack;
    public bool _initAttack;
    public float _attackRange;
    public float _cooldownTime;
    public string _animationParam;
}
