using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShootProjectile : MonoBehaviour
{
    [SerializeField] private GameObject _projectile;
    [SerializeField] private Transform _firePoint;
    [SerializeField] Transform _target;
    //[SerializeField] private LayerMask _layerToHit;
    //[SerializeField] private float _attackRange;
    [SerializeField] private float _projectileSpeed = 15f;
    [SerializeField] private float _startDelay = 0.25f;
    private float _delayTimer;
    private bool _firing;

    [SerializeField] private GameObject _prepAttackIcon, _attackingIcon;


    // Start is called before the first frame update
    void Start()
    {
        _target = GetComponent<EnemyAI>().Player;
    }

    void Update()
    {
        DelayShoot();
    }

    private void DelayShoot()
    {
        if (_firing)
        {
            if (_delayTimer >= _startDelay)
            {
                _firing = false;
                SetTargetDestination();
                InstantiateProjectile(_firePoint);
                _prepAttackIcon.SetActive(false);
                _attackingIcon.SetActive(true);
            }
            else
            {
                _delayTimer += Time.deltaTime;
                _prepAttackIcon.SetActive(true);
            }
        }
        else
        {
            _delayTimer = 0;
            _prepAttackIcon.SetActive(false);
            _attackingIcon.SetActive(false);
        }
    }

    public void Shoot()
    {
        _firing = true;
    }
    void SetTargetDestination()
    {
        _firePoint.LookAt(_target);
    }
    void InstantiateProjectile(Transform firePoint)
    {
        var projectile = Instantiate(_projectile, _firePoint.position, _firePoint.rotation);
        projectile.transform.SetParent(null);
        projectile.SetActive(true);
        projectile.GetComponent<Damager>().CanDamage = true;
        projectile.GetComponent<Destroyer>().CanBeDestroyed = true;
        projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * _projectileSpeed, ForceMode.Impulse);
    }
}
