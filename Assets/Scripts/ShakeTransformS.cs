using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeTransformS : MonoBehaviour {
    [Header("Info")]
    private Vector3 _startPos;
    private bool _doSake = false;
    private Vector3 _randomPos;

    [Header("Settings")]
    [Range(0f, 2f)]
    public float _time = 0.2f;
    [Range(0f, 2f)]
    public float _distance = 0.1f;
    [Range(0f, 0.1f)]
    public float _delayBetweenShakes = 0f;
    public bool IsSaking {
        get { return _doSake; }
    }
    private void Awake() {
        _startPos = transform.position;
    }

    private void OnValidate() {
        if (_delayBetweenShakes > _time)
            _delayBetweenShakes = _time;
    }
    /// <summary>
    /// Start the shake
    /// </summary>
    public void Begin() {
        StopAllCoroutines();
        _doSake = true;
        StartCoroutine(Shake());
    }
    /// <summary>
    /// Stop shake
    /// </summary>
    public void Stop() {

        _doSake = false;
        StopAllCoroutines();

    }
    /// <summary>
    /// Shake the transform
    /// </summary>
    /// <returns></returns>
    private IEnumerator Shake() {

        while (_doSake) {
            _randomPos = _startPos + (Random.insideUnitSphere * _distance);

            transform.position = _randomPos;

            if (_delayBetweenShakes > 0f) {
                yield return new WaitForSeconds(_delayBetweenShakes);
            } else {
                yield return null;
            }
        }

        transform.position = _startPos;
    }

}
