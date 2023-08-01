using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Destroyer : MonoBehaviour
{
    public bool ShootByPlayer;
    public bool CanBeDestroyed;
    [SerializeField] float _time = 5;
    private bool _activated;

    [SerializeField] private GameObject _hitVFX;

    [SerializeField] private AudioSource _hitSFX;

    [Range(-3, 3)]
    [SerializeField] private float _pitchLow = 0.8f, _pitchHigh = 1.2f;

    private void Update()
    {
        if (CanBeDestroyed && !_activated)
        {
            _activated = true;
            Destroy(gameObject, _time);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CanBeDestroyed && ((ShootByPlayer && other.CompareTag("Enemy")) || (!ShootByPlayer && other.CompareTag("Player")) || other.CompareTag("Environment") || other.CompareTag("Ground")))
        {
            _activated = true;
            if (_hitSFX != null)
            {
                float pitch = Random.Range(_pitchLow, _pitchHigh);
                _hitSFX.pitch = pitch;
                _hitSFX.Play();
                _hitSFX.transform.parent = null;
                Destroy(_hitSFX.gameObject, 1f);
            }
            if (_hitVFX != null)
            {
                GameObject vfx = Instantiate(_hitVFX, transform.position, transform.rotation, null);
                Destroy(vfx, 0.5f);
            }
            Destroy(gameObject);
        }
    }
}
