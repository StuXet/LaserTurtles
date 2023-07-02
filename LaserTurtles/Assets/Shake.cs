using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    public static Shake instance;

    [SerializeField] bool _start = false;
    [SerializeField] AnimationCurve _curve;
    [SerializeField]  float _duration = 0.2f;
    [SerializeField]  float _power = 1f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Update()
    {
        if (_start)
        {
            //_start = false;
            StartCoroutine(Shaking());
        }
    }

    public void ScreenShake(float duration,float power)
    {
        _start = true;
        _duration = duration;
        _power = power;
    }

    private IEnumerator Shaking()
    {
        //Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < _duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = _curve.Evaluate(elapsedTime / _duration) * _power;
            transform.position += Random.insideUnitSphere * strength;
            yield return null;
        }

        _start = false;

        //transform.position = startPosition;
    }
}
