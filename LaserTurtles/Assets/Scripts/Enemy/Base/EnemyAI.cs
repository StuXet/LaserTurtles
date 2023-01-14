using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // Enemy Fields
    public LayerMask GroundLayer, PlayerLayer;
    public NavMeshAgent Agent;
    [HideInInspector] public Transform Player;
    [SerializeField] private HealthHandler _healthHandlerRef;
    public bool DestroyOnDeath;

    // Patroling
    public bool CanPatrol = false;
    public bool CanChase = true;
    public bool CanFly = false;
    public float PatrolRange;
    private Vector3 WalkPoint;
    private bool _walkPointSet;

    // Attacking
    public float AttackCoolDownTime;
    public bool AlreadyAttacked;

    // States
    public float SightRange, AttackRange;
    private bool PlayerInSightRange, PlayerInAttackRange;


    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Player = GameObject.FindGameObjectWithTag("Player").transform;
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

        if (!PlayerInSightRange && !PlayerInAttackRange) Patroling();
        if (PlayerInSightRange && !PlayerInAttackRange) ChasePlayer();
        if (PlayerInAttackRange && PlayerInSightRange) AttackPlayer(false);

        AnimationHandler();
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

    //private void ToggleHPBarState()
    //{
    //    if (PlayerInSightRange)
    //    {
    //        _healthHandlerRef.ToggleHealthBar(true);
    //    }
    //    else
    //    {
    //        _healthHandlerRef.ToggleHealthBar(false);
    //    }
    //}

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

    private void _healthHandlerRef_OnDeathOccured(object sender, System.EventArgs e)
    {
        EnemyDeath();
    }

    public virtual void EnemyDeath()
    {
        if (DestroyOnDeath)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
            _healthHandlerRef._healthSystem.RefillHealth();
            AlreadyAttacked = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, SightRange);
    }
}
