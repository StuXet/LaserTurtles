using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponEffect : MonoBehaviour
{
    private Damager _damager;
    private BoxCollider _boxCollider;

    [SerializeField] private GameObject _weaponParticle, _hitVFX;
    [SerializeField] private AudioSource _attackSFX, _hitSFX;

    [Range(-3, 3)]
    [SerializeField] private float _pitchLow = 0.8f, _pitchHigh = 1.2f;

    private bool _isAttacking = false;

    private void Awake()
    {
        _damager = GetComponent<Damager>();
        _boxCollider = GetComponent<BoxCollider>();
    }

    public void EffectState(bool state)
    {
        _weaponParticle.SetActive(state);
        if (state)
        {
            if (!_isAttacking)
            {
                if (_attackSFX != null)
                {
                    _isAttacking = true;
                    float attackPitch = Random.Range(_pitchLow, _pitchHigh);
                    _attackSFX.pitch = attackPitch;
                    _attackSFX.Play();
                }
            }
        }
        else
        {
            _isAttacking = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_damager.CanDamage && (other.CompareTag("Enemy") || other.CompareTag("Environment")))
        {
            Vector3 hitPosition = other.ClosestPoint(_boxCollider.bounds.center);
            GameObject vfx = Instantiate(_hitVFX, hitPosition, Quaternion.identity);
            Destroy(vfx, 0.5f);
            if (_hitSFX != null)
            {
                float hitPitch = Random.Range(_pitchLow, _pitchHigh);
                _hitSFX.pitch = hitPitch;
                _hitSFX.Play();
            }
        }
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    if (_damager.CanDamage && (collision.transform.CompareTag("Enemy") || collision.transform.CompareTag("Environment")))
    //    {
    //        ContactPoint contact = collision.contacts[0];
    //        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
    //        Vector3 pos = contact.point;
    //        GameObject vfx = Instantiate(_hitVFX, pos, rot);
    //        Destroy(vfx, 5f);
    //    }
    //}
}