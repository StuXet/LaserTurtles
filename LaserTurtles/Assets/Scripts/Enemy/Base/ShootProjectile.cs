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

    // Start is called before the first frame update
    void Start()
    {
        _target = GetComponent<EnemyAI>().Player;
    }

    void Update()
    {

    }

    public void Shoot()
    {
        SetTargetDestination();
        InstantiateProjectile(_firePoint);
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
