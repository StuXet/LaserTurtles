using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyBossEnemyAI : EnemyAI
{
    private bool _bossFightBegin;
    private bool _bossFightStarted;

    // Phases
    [System.Serializable]
    public class Phase
    {
        [Range(0, 100)]
        public int HPPercentage;
        public PhaseStateBase State;
    }

    [Header("Phases")]
    //[SerializeField] private bool _usePhases;
    [SerializeField] private int _currentPhase;
    private int _lastPhase;
    [SerializeField] private float _invulnerabilityTime;
    [SerializeField] List<Phase> _phasesList;


    // Update is called once per frame
    void Update()
    {
        PhaseStatus();

        RunEnemy();

        AssignHPBar();
    }


    public override void AttackPatternController()
    {
        var phaseState = _phasesList[GetCurrentPhase()].State;
        phaseState.AttackRef._initAttack = true;
    }

    public override void AnimationHandler()
    {
        var phaseState = _phasesList[GetCurrentPhase()].State;
        if (AnimatorRef != null) AnimatorRef.SetBool(phaseState.AnimationParam, phaseState.AttackRef._initAttack);
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

    private int GetCurrentPhase()
    {
        int currentHPPerc = (int)(HealthHandlerRef._healthSystem.GetHealthPercent() * 100);
        int phaseInd = 0;

        for (int i = 0; i < _phasesList.Count; i++)
        {
            if (_phasesList[i].HPPercentage >= currentHPPerc)
            {
                phaseInd = i;
            }
        }

        return phaseInd;
    }

    private void ChangePhase()
    {
        if (_lastPhase != _currentPhase)
        {
            var lastPhaseState = _phasesList[_lastPhase].State;
            lastPhaseState.AttackRef._initAttack = false;
            if (AnimatorRef != null) AnimatorRef.SetBool(lastPhaseState.AnimationParam, lastPhaseState.AttackRef._initAttack);
            _lastPhase = _currentPhase;

            StartCoroutine(TimedInvulnerability());
        }
        else
        {
            var currentPhaseState = _phasesList[GetCurrentPhase()].State;
            AttackRange = currentPhaseState.AttackRef._attackRange;
            AttackCoolDownTime = currentPhaseState.AttackRef._cooldownTime;
        }
    }

    IEnumerator TimedInvulnerability()
    {
        HealthHandlerRef.Invulnerable = true;
        yield return new WaitForSeconds(2);
        HealthHandlerRef.Invulnerable = false;
    }

    private void PhaseStatus()
    {
        _currentPhase = GetCurrentPhase();
        ChangePhase();
    }
}
