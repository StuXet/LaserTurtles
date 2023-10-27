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
    [SerializeField] List<Phase> _phasesList;
    //[SerializeField] private float _invulnerabilityTime;

    [Header("Invulnerability")]
    [SerializeField] private float _invulnerabilityMinTime;
    [SerializeField] private float _invulnerabilityMaxTime;
    [SerializeField] private float _invulnerabilityActiveMinTime;
    [SerializeField] private float _invulnerabilityActiveMaxTime;
    private float _invTimer;
    private float _invTime;
    private float _invTimeActive;
    private bool _invTimeSet;
    private bool _invTimeActiveSet;



    // Update is called once per frame
    void Update()
    {
        PhaseStatus();

        RunEnemy();

        HandleInvulnerability();

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
        if (AnimatorRef != null && phaseState.AttackRef._currentAttack._animationParam != "") AnimatorRef.SetBool(phaseState.AttackRef._currentAttack._animationParam, phaseState.AttackRef._currentAttack._initAttack);
        if (AnimatorRef != null) AnimatorRef.SetBool("Staggered", isStunned);
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
            if (AnimatorRef != null && lastPhaseState.AttackRef._currentAttack._animationParam != "") AnimatorRef.SetBool(lastPhaseState.AttackRef._currentAttack._animationParam, lastPhaseState.AttackRef._currentAttack._initAttack);
            _lastPhase = _currentPhase;

            //StartCoroutine(TimedInvulnerability());
            //StartCoroutine(HandleStun(3, 7));
        }
        else
        {
            var currentPhaseState = _phasesList[GetCurrentPhase()].State;
            AttackRange = currentPhaseState.AttackRef._currentAttack._attackRange;
            AttackCoolDownTime = currentPhaseState.AttackRef._currentAttack._cooldownTime;
        }
    }

    private void PhaseStatus()
    {
        _currentPhase = GetCurrentPhase();
        ChangePhase();
    }

    IEnumerator TimedInvulnerability()
    {
        HealthHandlerRef.Invulnerable = true;

        //yield return new WaitForSeconds(_invulnerabilityTime);
        yield return new WaitForSeconds(_invTimeActive);
        HealthHandlerRef.Invulnerable = false;
        _invTimer = 0;
        _invTimeSet = false;
        _invTimeActiveSet = false;
    }

    private void HandleInvulnerability()
    {
        if (!isStunned)
        {
            if (!_invTimeSet)
            {
                _invTime = Random.Range(_invulnerabilityMinTime, _invulnerabilityMaxTime);
                _invTimeSet = true;
            }
            else
            {
                if (_invTimer < _invTime)
                {
                    _invTimer += Time.deltaTime;
                }
                else
                {
                    if (!_invTimeActiveSet)
                    {
                        _invTimeActive = Random.Range(_invulnerabilityActiveMinTime, _invulnerabilityActiveMaxTime);
                        StartCoroutine(TimedInvulnerability());
                        _invTimeActiveSet = true;
                    }
                }
            }
        }
        else
        {
            if (_invTimeActiveSet)
            {
                StopCoroutine(TimedInvulnerability());
                HealthHandlerRef.Invulnerable = false;
                _invTimeActiveSet = false;
            }
            _invTimeSet = false;
            _invTimer = 0;
        }
    }
}
