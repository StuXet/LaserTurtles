using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private float _destroyTime = 1;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, _destroyTime);
    }
}