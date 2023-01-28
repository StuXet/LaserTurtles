using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Destroyer : MonoBehaviour
{
    public bool CanBeDestroyed;
    [SerializeField] float _time = 5;
    private bool _activated;

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
        if ((other.CompareTag("Enemy") || other.CompareTag("Environment")) && CanBeDestroyed)
        {
            _activated = true;
            Destroy(gameObject);
        }
    }
}
