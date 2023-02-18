using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEffect : MonoBehaviour
{
    private Damager _damager;
    private BoxCollider _boxCollider;

    [SerializeField] private GameObject _weaponParticle, _hitVFX;

    private void Awake()
    {
        _damager = GetComponent<Damager>();
        _boxCollider = GetComponent<BoxCollider>();
    }

    public void EffectState(bool state)
    {
        _weaponParticle.SetActive(state);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (_damager.CanDamage && (other.CompareTag("Enemy") || other.CompareTag("Environment")))
    //    {
    //        GameObject vfx = Instantiate(_hitVFX, other.transform.position, other.transform.rotation, null);
    //        Destroy(vfx, 0.5f);
    //    }
    //}
}
