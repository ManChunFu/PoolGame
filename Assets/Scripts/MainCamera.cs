using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class MainCamera : MonoBehaviour
{
    [SerializeField] private Vector3 _startViewPosition = default;
    [SerializeField] private Vector3 _startViewRotation = default;
    [SerializeField] private Vector3 _aimViewPosition = default;
    [SerializeField] private Vector3 _aimViewRotation = default;
    [SerializeField] private float _moveSpeed = 1.5f;

    [SerializeField] private CueStick _cueStick = null;

    private void OnEnable()
    {
        _cueStick.OnAimingChanged += StartChangeViewCoroutine;
    }
    private void OnDisable()
    {
        _cueStick.OnAimingChanged -= StartChangeViewCoroutine;
    }

    void Start()
    {
        transform.position = _startViewPosition;
        transform.rotation = Quaternion.Euler(_startViewRotation);

        if (_cueStick == null)
        {
            _cueStick = FindObjectOfType<CueStick>();
            Assert.IsNotNull(_cueStick, "Failed to get reference to the CueStick script");
        }
        else
            _cueStick.OnAimingChanged += StartChangeViewCoroutine;

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.UpArrow) && !_cueStick.IsAiming)
        {
             _cueStick.Aiming();
        }

    }

    private void StartChangeViewCoroutine(bool value)
    {
        StartCoroutine(ChangeViewPoint(value));
    }

    private IEnumerator ChangeViewPoint(bool value)
    {
        if (value)
        {
            SetParentAndSize(_cueStick.transform);
            transform.localPosition = _aimViewPosition;
            transform.localRotation = Quaternion.Euler(_aimViewRotation);
        }
        else
        {
            SetParentAndSize(null);
            while (transform.position != _startViewPosition && transform.rotation != Quaternion.Euler(_startViewRotation))
            {
                transform.position = Vector3.Lerp(transform.position, _startViewPosition, _moveSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(_startViewRotation), _moveSpeed * Time.deltaTime);
                yield return null;
            }
            yield return null;
        }
    }

    private void SetParentAndSize(Transform parent)
    {
        transform.SetParent(parent);
        transform.localScale = Vector3.one;
    }

}
