using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEffect : MonoBehaviour
{
    [SerializeField] private GameObject _weaponParticle;

    public void EffectState(bool state)
    {
        _weaponParticle.SetActive(state);
    }
}
