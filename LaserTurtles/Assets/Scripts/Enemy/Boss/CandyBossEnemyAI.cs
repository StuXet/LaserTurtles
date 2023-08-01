using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyBossEnemyAI : EnemyAI
{
    private MeleeAttack _meleeAttackRef;
    private bool _bossFightBegin;
    private bool _bossFightStarted;

    private void Start()
    {
        _meleeAttackRef = GetComponent<MeleeAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        RunEnemy();

        AssignHPBar();
    }

    public override void AttackPatternController()
    {
        _meleeAttackRef.DoAttack();
    }

    public override void AnimationHandler()
    {
        if (AnimatorRef != null) AnimatorRef.SetBool("Attack", _meleeAttackRef.IsActive);
    }

    private void AssignHPBar()
    {
        if (!_bossFightBegin && GetPlayerInSightRange)
        {
            _bossFightBegin = true;
        }
        if (_bossFightBegin && !_bossFightStarted)
        {
            HealthHandlerRef.AssignHealthBar(UIMediator.Instance.BossHPUI.GetComponent<HealthBar>());
            _bossFightStarted = true;
        }
    }
}
