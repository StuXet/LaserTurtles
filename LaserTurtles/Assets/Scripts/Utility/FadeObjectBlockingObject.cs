using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeObjectBlockingObject : MonoBehaviour
{
    [SerializeField]
    private LayerMask _layerMask;
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private Camera _camera;
    private CameraFollow _camFollow;
    [SerializeField]
    [Range(0, 1f)]
    private float _fadedAlpha = 0.33f;
    [SerializeField]
    private bool _retainShadows = true;
    [SerializeField]
    private Vector3 _targetPositionOffset = Vector3.up;
    [SerializeField]
    private float _fadeSpeed = 1f;

    [Header("Read Only Data")]
    [SerializeField]
    private List<FadingObject> _objectsBlockingView = new List<FadingObject>();
    private Dictionary<FadingObject, Coroutine> _runningCoroutines = new Dictionary<FadingObject, Coroutine>();

    private RaycastHit[] _hits = new RaycastHit[10];


    private void Awake()
    {
        if (_camera) _camFollow = _camera.GetComponent<CameraFollow>();
    }

    private void Start()
    {
        StartCoroutine(CheckForObjects());
    }

    private IEnumerator CheckForObjects()
    {
        while (true)
        {
            Vector3 camPos = _camFollow.CamOffset * 5 + _camFollow.ObjToFollow.transform.position;

            int hits = Physics.RaycastNonAlloc(camPos,
                (_target.transform.position + _targetPositionOffset - camPos).normalized,
                _hits, Vector3.Distance(camPos, _target.transform.position + _targetPositionOffset), _layerMask);

            if (hits > 0)
            {
                for (int i = 0; i < hits; i++)
                {
                    FadingObject fadingObject = GetFadingObjectFromHit(_hits[i]);

                    if (fadingObject != null && !_objectsBlockingView.Contains(fadingObject))
                    {
                        if (_runningCoroutines.ContainsKey(fadingObject))
                        {
                            if (_runningCoroutines[fadingObject] != null)
                            {
                                StopCoroutine(_runningCoroutines[fadingObject]);
                            }

                            _runningCoroutines.Remove(fadingObject);
                        }

                        _runningCoroutines.Add(fadingObject, StartCoroutine(FadeObjectOut(fadingObject)));
                        _objectsBlockingView.Add(fadingObject);
                    }

                }
            }

            FadeObjectsNoLongerBeingHit();

            ClearHits();

            yield return null;
        }
    }

    private void FadeObjectsNoLongerBeingHit()
    {
        List<FadingObject> objectsToRemove = new List<FadingObject>(_objectsBlockingView.Count);

        foreach (FadingObject fadingObject in _objectsBlockingView)
        {
            bool objectIsBeingHit = false;
            for (int i = 0; i < _hits.Length; i++)
            {
                FadingObject hitFadingObject = GetFadingObjectFromHit(_hits[i]);
                if (hitFadingObject != null && fadingObject == hitFadingObject)
                {
                    objectIsBeingHit = true;
                    break;
                }
            }

            if (!objectIsBeingHit)
            {
                if (_runningCoroutines.ContainsKey(fadingObject))
                {
                    if (_runningCoroutines[fadingObject] != null)
                    {
                        StopCoroutine(_runningCoroutines[fadingObject]);
                    }
                    _runningCoroutines.Remove(fadingObject);
                }

                _runningCoroutines.Add(fadingObject, StartCoroutine(FadeObjectIn(fadingObject)));
                objectsToRemove.Add(fadingObject);
            }
        }

        foreach (FadingObject removeObject in objectsToRemove)
        {
            _objectsBlockingView.Remove(removeObject);
        }
    }

    private IEnumerator FadeObjectOut(FadingObject fadingObject)
    {
        // Setting the Materials
        foreach (Material material in fadingObject.Materials)
        {
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.SetInt("_Surface", 1);

            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            material.SetShaderPassEnabled("DepthOnly", false);
            material.SetShaderPassEnabled("SHADOWCASTER", _retainShadows);

            material.SetOverrideTag("RenderType", "Transparent");

            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        }

        float time = 0;

        // Fading the Materials
        while (fadingObject.Materials[0].color.a > _fadedAlpha)
        {
            foreach (Material material in fadingObject.Materials)
            {
                if (material.HasProperty("_Color"))
                {
                    material.color = new Color(material.color.r, material.color.g, material.color.b,
                        Mathf.Lerp(fadingObject.InitialAlpha, _fadedAlpha, time * _fadeSpeed));
                }
            }

            time += Time.deltaTime;
            yield return null;
        }

        // Stopping Coroutine
        if (_runningCoroutines.ContainsKey(fadingObject))
        {
            StopCoroutine(_runningCoroutines[fadingObject]);
            _runningCoroutines.Remove(fadingObject);
        }
    }

    private IEnumerator FadeObjectIn(FadingObject fadingObject)
    {

        float time = 0;

        // Fading the Materials
        while (fadingObject.Materials[0].color.a < fadingObject.InitialAlpha)
        {
            foreach (Material material in fadingObject.Materials)
            {
                if (material.HasProperty("_Color"))
                {
                    material.color = new Color(material.color.r, material.color.g, material.color.b,
                        Mathf.Lerp(_fadedAlpha, fadingObject.InitialAlpha, time * _fadeSpeed));
                }
            }

            time += Time.deltaTime;
            yield return null;
        }

        // Setting the Materials
        foreach (Material material in fadingObject.Materials)
        {
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.SetInt("_Surface", 0);

            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;

            material.SetShaderPassEnabled("DepthOnly", true);
            material.SetShaderPassEnabled("SHADOWCASTER", true);

            material.SetOverrideTag("RenderType", "Opaque");

            material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        }

        // Stopping Coroutine
        if (_runningCoroutines.ContainsKey(fadingObject))
        {
            StopCoroutine(_runningCoroutines[fadingObject]);
            _runningCoroutines.Remove(fadingObject);
        }
    }

    private void ClearHits()
    {
        System.Array.Clear(_hits, 0, _hits.Length);
    }

    private FadingObject GetFadingObjectFromHit(RaycastHit Hit)
    {
        return Hit.collider != null ? Hit.collider.GetComponent<FadingObject>() : null;
    }

    private void OnDrawGizmos()
    {
        if (_camera && _camFollow)
        {
            Vector3 camPos = _camFollow.CamOffset * 5 + _camFollow.ObjToFollow.transform.position;

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(camPos, _target.transform.position + _targetPositionOffset - camPos);
        }
    }
}
