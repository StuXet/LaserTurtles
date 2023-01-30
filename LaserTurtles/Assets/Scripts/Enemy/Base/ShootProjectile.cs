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
            }
            else
            {
                _delayTimer += Time.deltaTime;
            }
        }
        else
        {
            _delayTimer = 0;
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
        var projectile = Instantiate(_projectile, _firePoint.position, _firePoint.rotation) as GameObject;
        projectile.transform.SetParent(null);
        projectile.SetActive(true);
        projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * _projectileSpeed;
    }
}
