using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiAttacks : AttackBase
{
    [Header("Class Variables")]
    [SerializeField] private List<AttackBase> _attackBases = new List<AttackBase>(); 
    private bool _rolled;

    private void Start()
    {
        RollCurrentAttack();
    }

    // Update is called once per frame
    void Update()
    {
        if (_initAttack)
        {
            if (!_rolled)
            {
                RollCurrentAttack();
                _currentAttack._initAttack = true;
                _rolled = true;
            }
            else
            {
                _initAttack = _currentAttack._initAttack;
            }
        }
        else
        {
            _rolled = false;
        }
    }

    public void RollCurrentAttack()
    {
        int attackInd = Random.Range(0, _attackBases.Count);
        _currentAttack = _attackBases[attackInd];
    }
}
