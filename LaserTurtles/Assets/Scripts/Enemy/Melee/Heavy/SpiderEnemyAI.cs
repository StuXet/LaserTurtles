using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderEnemyAI : EnemyAI
{
    private MeleeAttack _meleeAttackRef;

    private void Start()
    {
        _meleeAttackRef = GetComponent<MeleeAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        RunEnemy();
    }

    public override void AttackPatternController()
    {
        _meleeAttackRef.DoAttack();
    }

    public override void AnimationHandler()
    {
        if (AnimatorRef != null) AnimatorRef.SetBool("Attack", _meleeAttackRef.IsActive);
    }
}
