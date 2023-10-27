using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShootProjectile : AttackBase
{
    [Header("Class Variables")]
    [SerializeField] private EnemyAI _enemyAIRef;

    [SerializeField] private GameObject _projectile;
    [SerializeField] private Transform _firePoint;
    [SerializeField] Transform _target;
    //[SerializeField] private LayerMask _layerToHit;
    //[SerializeField] private float _attackRange;
    [SerializeField] private float _projectileSpeed = 15f;
    [SerializeField] private float _startDelay = 0.75f;
    [SerializeField] private float _cooldown = 1f;
    private float _delayTimer;
    private bool _fired;
    [SerializeField] private bool _grow;
    private bool _growing;
    private float _growElapsedTime;
    private GameObject _tempProj;

    [SerializeField] private GameObject _prepAttackIcon, _attackingIcon;

    private void Awake()
    {
        _currentAttack = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _prepAttackIcon.SetActive(false);
        _attackingIcon.SetActive(false);

        _target = _enemyAIRef.Player;
    }

    void Update()
    {
        DelayShoot();
    }

    private void DelayShoot()
    {
        if (_initAttack)
        {
            if (_enemyAIRef.isStunned)
            {
                _delayTimer = _cooldown;

                if (_tempProj != null)
                {
                    Destroy(_tempProj);
                    _tempProj = null;
                    _growing = false;
                }
            }
            if (_delayTimer >= _cooldown)
            {
                _initAttack = false;
                _fired = false;

                _prepAttackIcon.SetActive(false);
                _attackingIcon.SetActive(false);
            }
            else if (_delayTimer >= _startDelay && !_fired)
            {
                _fired = true;
                SetTargetDestination();
                if (!_grow)
                {
                    var projectile = Instantiate(_projectile, _firePoint.position, _firePoint.rotation);
                    FireProjectile(projectile);
                }
                else
                {
                    _growing = false;
                    FireProjectile(_tempProj);
                    _tempProj = null;
                }
                _prepAttackIcon.SetActive(false);
                _attackingIcon.SetActive(true);
                _enemyAIRef.PlayAttackSFX();
            }
            else
            {
                _delayTimer += Time.deltaTime;
                _prepAttackIcon.SetActive(true);
                if (_grow) GrowProjectile();
            }
        }
        else
        {
            _delayTimer = 0;
            //_prepAttackIcon.SetActive(false);
            //_attackingIcon.SetActive(false);
        }
    }

    public void Shoot()
    {
        _initAttack = true;
    }
    void SetTargetDestination()
    {
        _firePoint.LookAt(_target);
    }
    void FireProjectile(GameObject projectile)
    {
        projectile.transform.SetParent(null);
        projectile.SetActive(true);
        projectile.GetComponent<Damager>().CanDamage = true;
        projectile.GetComponent<Destroyer>().CanBeDestroyed = true;
        projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * _projectileSpeed, ForceMode.Impulse);
    }

    void GrowProjectile()
    {
        if (!_growing && !_fired)
        {
            _growing = true;
            _tempProj = Instantiate(_projectile, _firePoint.position, _firePoint.rotation, _firePoint);
            _tempProj.transform.localScale = Vector3.zero;
            _growElapsedTime = 0;
        }

        if (_tempProj != null)
        {
            if (_growElapsedTime < _startDelay)
            {
                _tempProj.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, _growElapsedTime / _startDelay);
                _growElapsedTime += Time.deltaTime;
            }
            else
            {
                _tempProj.transform.localScale = Vector3.one;
            }
        }
    }
}
