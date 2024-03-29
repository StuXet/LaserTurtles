using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class EnemyAI : MonoBehaviour
{
    // Enemy Fields
    public string EnemyName = "Default Enemy";
    public LayerMask GroundLayer, PlayerLayer;
    public NavMeshAgent Agent;
    [HideInInspector] public Transform Player;
    [SerializeField] private HealthHandler _healthHandlerRef;
    [SerializeField] private Animator _animatorRef;
    [SerializeField] private GameObject _GFX;
    [SerializeField] private GameObject _enemyUI;
    [SerializeField] private float _deathLength = 2f;
    public bool DestroyOnDeath;
    private bool _isDead;

    // Patroling
    public bool CanPatrol = false;
    public bool CanChase = true;
    public bool CanFly = false;
    public bool UseToggleHP = false;
    public float PatrolRange;
    private Vector3 WalkPoint;
    private bool _walkPointSet;

    // Attacking
    public float AttackCoolDownTime;
    public bool AlreadyAttacked;

    // States
    [SerializeField] private bool _inControl = true;
    public float SightRange, AttackRange;
    private bool PlayerInSightRange, PlayerInAttackRange;

    // Physics
    [Header("Gravity")]
    [SerializeField] private bool _gravityEnabled = true;
    [SerializeField] private Vector3 _groundCheckOffset;
    [SerializeField] private float _groundRadius = 0.5f;
    [SerializeField] private float _gravityModifier = 10f;
    private Vector3 _velocity;
    private bool _isGrounded;

    // Knockback
    private bool _knockbacked;
    private float _knockbackForce;
    private float _knockbackTimer;
    private Vector3 _knockbackDirection;

    //Stun
    [Header("Stun")]
    [SerializeField] private bool _canBeStunned;
    public bool isStunned;
    [Range(0, 100)]
    [SerializeField] private int _stunChance = 0;
    public float StunEffectMultiplier = 1;

    // Audio Sources
    [Header("Audios")]
    [SerializeField] private Transform _sFXTransform;
    [SerializeField] private AudioSource _deathSFX;
    [SerializeField] private AudioSource _attackSFX;
    [SerializeField] private AudioSource _hurtSFX;
    [SerializeField] private AudioSource _voiceSFX;
    [SerializeField] private AudioSource _moveSFX;
    [Range(-3, 3)]
    [SerializeField] private float _pitchLow = 0.8f, _pitchHigh = 1.2f;
    private bool _voiceSFXActivated;
    private bool _moveSFXActivated;

    // Visual Effects
    [Header("VFX")]
    [SerializeField] private Transform _vFXTransform;
    [SerializeField] private VisualEffect _spawnVFX;
    [SerializeField] private ParticleSystem _stunEffect;


    public HealthHandler HealthHandlerRef { get => _healthHandlerRef; set => _healthHandlerRef = value; }
    public Animator AnimatorRef { get => _animatorRef; }
    public VisualEffect SpawnVFX { get => _spawnVFX; }
    public bool GetPlayerInSightRange { get => PlayerInSightRange; }
    public bool GetPlayerInAttackRange { get => PlayerInAttackRange; }

    private void OnEnable()
    {
        _isDead = false;
    }

    private void Awake()
    {
        _healthHandlerRef.CharacterName = EnemyName;
        Agent = GetComponent<NavMeshAgent>();
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        _healthHandlerRef.OnDamageOccured += _healthHandlerRef_OnDamageOccured;
        _healthHandlerRef.OnDeathOccured += _healthHandlerRef_OnDeathOccured;
    }

    private void Update()
    {
        RunEnemy();
    }


    public void RunEnemy()
    {
        //Check for sight and attack range
        PlayerInSightRange = Physics.CheckSphere(transform.position, SightRange, PlayerLayer);
        PlayerInAttackRange = Physics.CheckSphere(transform.position, AttackRange, PlayerLayer) && CanSeePlayer();

        //ToggleHPBarState();

        if (_inControl && _isGrounded && !isStunned)
        {
            Agent.enabled = true;
            if (!PlayerInSightRange && !PlayerInAttackRange) Patroling();
            if (PlayerInSightRange && !PlayerInAttackRange) ChasePlayer();
            if (PlayerInAttackRange && PlayerInSightRange) AttackPlayer(false);
        }
        else
        {
            Agent.enabled = false;
        }

        ToggleHPBarState();

        Gravity();
        HandleKnockback();

        AnimationHandler();

        SFXHandler();
    }

    private void SFXHandler()
    {
        // Voice SFX
        if (!_isDead)
        {
            if (!_voiceSFXActivated && PlayerInSightRange)
            {
                if (_voiceSFX != null)
                {
                    float pitch = Random.Range(_pitchLow, _pitchHigh);
                    _voiceSFX.pitch = pitch;
                    _voiceSFX.Play();
                    _voiceSFXActivated = true;
                }
            }
            else if (!PlayerInSightRange)
            {
                _voiceSFXActivated = false;
            }
        }
    }

    virtual public void Patroling()
    {
        if (CanPatrol)
        {
            if (!_walkPointSet) SearchWalkPoint();

            if (_walkPointSet)
                Agent.SetDestination(WalkPoint);

            Vector3 distanceToWalkPoint = transform.position - WalkPoint;

            //Walkpoint reached
            if (distanceToWalkPoint.magnitude < 1f)
                _walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-PatrolRange, PatrolRange);
        float randomX = Random.Range(-PatrolRange, PatrolRange);

        WalkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(WalkPoint, -transform.up, 2f, GroundLayer))
            _walkPointSet = true;
    }

    private void ToggleHPBarState()
    {
        if (UseToggleHP)
        {
            if (PlayerInSightRange && !_isDead)
            {
                _healthHandlerRef.ToggleHealthBar(true);
            }
            else
            {
                _healthHandlerRef.ToggleHealthBar(false);
            }
        }
    }

    private bool CanSeePlayer()
    {
        bool CanSee = false;
        if (Physics.Raycast(transform.position, transform.forward, AttackRange, PlayerLayer))
        {
            CanSee = true;
        }
        return CanSee;
    }

    virtual public void ChasePlayer()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
        if (CanChase)
        {
            if (!CanFly)
            {
                Agent.SetDestination(Player.position);
            }
            else
            {
                Vector3 tempEn = transform.position;
                Vector3 tempPl = Player.position;
                tempEn.y = 0;
                tempPl.y = 0;
                if (Vector3.Distance(tempEn, tempPl) > AttackRange)
                {
                    Agent.SetDestination(Player.position);
                }
                else
                {
                    //Make sure enemy doesn't move
                    tempEn.y = Player.position.y;
                    Agent.SetDestination(tempEn);
                }
                FlightControl();
            }
        }
    }

    virtual public void FlightControl()
    {
        if (Player.transform.position.y != transform.position.y && CanFly)
        {
            transform.position = new Vector3(transform.position.x, Player.position.y, transform.position.z);
        }
    }

    private void Gravity()
    {
        _isGrounded = Physics.CheckSphere(transform.position + _groundCheckOffset, _groundRadius, GroundLayer);

        if (_gravityEnabled)
        {

            if (_isGrounded && _velocity.y <= 0)
            {
                _velocity.y = 0;
            }
            else
            {
                _velocity.y -= _gravityModifier * Time.deltaTime;
                transform.position += _velocity * Time.deltaTime;
            }
        }
    }

    public void Knockback(float duration, float force, Vector3 dir)
    {
        if (!_knockbacked)
        {
            _knockbacked = true;
            _knockbackForce = force;
            _knockbackTimer = duration;
            _knockbackDirection = dir;
        }
    }

    private void HandleKnockback()
    {
        if (_knockbacked)
        {
            _inControl = false;
            if (_knockbackTimer > 0)
            {
                _knockbackTimer -= Time.deltaTime;
                Vector3 knockParams = _knockbackDirection * _knockbackForce * Time.deltaTime;
                transform.position += knockParams;
            }
            else
            {
                _knockbacked = false;
            }
        }
        else
        {
            if (!_isDead) _inControl = true;
            _knockbackForce = 0;
            _knockbackTimer = 0;
            _knockbackDirection = Vector3.zero;
        }
    }

    virtual public void AttackPlayer(bool changeAttack)
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
        //Make sure enemy doesn't move
        Agent.SetDestination(transform.position);

        //transform.LookAt(Player);
        var lookPos = Player.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * Agent.angularSpeed);

        if (!changeAttack)
        {
            if (!AlreadyAttacked)
            {
                //Attack code here
                AttackPatternController();

                //Debug.Log("Attacked");
                //End of attack code

                AlreadyAttacked = true;
                StartCoroutine(ResetAttack(AttackCoolDownTime, delegate () { AlreadyAttacked = false; }));
            }
        }
    }

    public void PlayAttackSFX()
    {
        if (_attackSFX != null)
        {
            float pitch = Random.Range(_pitchLow, _pitchHigh);
            _attackSFX.pitch = pitch;
            _attackSFX.Play();
        }
    }

    public delegate void Callback();

    public IEnumerator ResetAttack(float attackCoolDown, Callback boolToReset)
    {
        //Debug.Log("Reseting");
        yield return new WaitForSeconds(attackCoolDown);
        boolToReset();
        //Debug.Log("Done");
    }

    virtual public void AttackPatternController()
    {

    }

    virtual public void AnimationHandler()
    {

    }

    private void _healthHandlerRef_OnDamageOccured(object sender, System.EventArgs e)
    {
        if (_hurtSFX != null && !_deathSFX.isPlaying)
        {
            float pitch = Random.Range(_pitchLow, _pitchHigh);
            _hurtSFX.pitch = pitch;
            _hurtSFX.Play();
        }
    }

    private void _healthHandlerRef_OnDeathOccured(object sender, System.EventArgs e)
    {
        EnemyDeath();
    }

    public virtual void EnemyDeath()
    {
        _isDead = true;

        if (DestroyOnDeath)
        {
            if (_deathSFX != null)
            {
                _deathSFX.Play();
                _deathSFX.transform.parent = null;
                Destroy(_deathSFX.gameObject, _deathLength);
            }
            if (_spawnVFX != null)
            {
                _spawnVFX.Play();
                _spawnVFX.transform.parent = null;
                Destroy(_spawnVFX.gameObject, _deathLength);

            }
            Destroy(gameObject);
        }
        else
        {
            // Stop Stun Effect
            isStunned = false;
            if (_stunEffect != null) _stunEffect.Stop();
            if (_stunEffect != null) _stunEffect.Clear();
            // Play Death Effects
            if (_deathSFX != null) _deathSFX.Play();
            if (_spawnVFX != null) _spawnVFX.Play();
            // Disable Controls
            _inControl = false;
            _voiceSFXActivated = false;
            // Disable UI
            _enemyUI.SetActive(false);
            // Disable GFX
            _GFX.gameObject.SetActive(false);
            // After Timer Death ReEnables
            StartCoroutine(DeathTransition());
        }

        ToggleHPBarState();
    }

    IEnumerator DeathTransition()
    {
        yield return new WaitForSeconds(_deathLength);
        // Enable Controls
        _inControl = true;
        // Enable GFX
        _GFX.gameObject.SetActive(true);
        // Enable UI
        _healthHandlerRef._healthSystem.RefillHealth();
        _enemyUI.SetActive(true);
        // Disable EnemyObj
        gameObject.SetActive(false);
    }
    public IEnumerator HandleStun(float minTime, float maxTime)
    {
        isStunned = true;
        if (_stunEffect != null) _stunEffect.Play();
        yield return new WaitForSeconds(Random.Range(minTime, maxTime));
        isStunned = false;
        if (_stunEffect != null) _stunEffect.Stop();
        if (_stunEffect != null) _stunEffect.Clear();
    }

    public void Stun(float minTime, float maxTime)
    {
        if (gameObject.activeSelf && _canBeStunned && !isStunned && !_isDead)
        {
            int chance = Random.Range(0, 101);
            //Debug.Log(chance);

            if (chance < _stunChance)
            {
                StartCoroutine(HandleStun(minTime, maxTime));
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Sight & Attack Ranges
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, SightRange);

        // Grounded
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (_isGrounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(transform.position + _groundCheckOffset, _groundRadius);
    }

}
