using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class CueStick : MonoBehaviour
{
    [Header("Basic Setup")]
    [SerializeField] private Transform _cueBall = null;
    [SerializeField] private CueStickHead _childStickHead = null; 
    [SerializeField] private float _rotateSpeed = 20.0f;
    [SerializeField] private float _pushForce = 0f;
    [SerializeField] private float _speed = 3f;

    [Header("View Camera Setup")]
    [SerializeField] private float _switchCameraPositionDelay = 0.5f;

    private readonly Vector3 _offsetPos = new Vector3(-0.15f, 8f, -13.0f); // 3D model offset center according to the position of the ball
    private readonly Quaternion _aimOffset = Quaternion.Euler(9f, 0.0f, 0.0f);
    private readonly Vector3 _parkingLot = new Vector3(7.0f, 3.4f, -14.5f);
    private readonly Quaternion _parkingOffset = Quaternion.Euler(-60.0f, 0.0f, 0.0f);

    private const float _pushAcceleration = 0.09f;
    private const float _pullBackTimeLimit = 1.0f;
    private const string _axisInput = "Horizontal";

    private Vector3 _currentAimPos = default;
    private float timerCounter = 0;

    public static event Action<CueStick, float> OnHitingBall = delegate { };

    public event Action<bool> OnAimingChanged;
    private bool _isAiming = false;
    public Vector3 Velocity;
    public bool IsAiming
    {
        get => _isAiming;
        set
        {
            if (_isAiming != value)
            {
                _isAiming = value;
                OnAimingChanged?.Invoke(value);
            }
        }
    }

    private void Start()
    {
        if (_cueBall == null)
        {
            _cueBall = FindObjectOfType<CueBall>()?.transform;
            Assert.IsNotNull(_cueBall, "No reference to the CueBall class.");
        }
        
        StandBy();
    }

    private void Update()
    {
        if (IsAiming)
        {
            MoveHorizontally();
            if (Input.GetKey(KeyCode.Space))
                Pull();
            if (Input.GetKeyUp(KeyCode.Space))
                StartCoroutine(Push());
        }
    }


    private void MoveHorizontally()
    {
        Vector3 rotateAxis = new Vector3(0.0f, -Input.GetAxis(_axisInput), 0.0f);

        transform.RotateAround(_cueBall.position,  rotateAxis, _rotateSpeed * Time.deltaTime);

    }

    private void Pull()
    {
        _childStickHead.enabled = true;
        timerCounter += Time.deltaTime;
        if (timerCounter < _pullBackTimeLimit)
        {
            _pushForce += _pushAcceleration;
            transform.Translate(Vector3.back * _speed * Time.deltaTime);
        }
    }

    private IEnumerator Push()
    {
        timerCounter = 0;
        while(!_childStickHead.IsHitingBall)
        {
            transform.Translate(Vector3.forward * _speed * _pushForce * Time.deltaTime);
            yield return null;
        }
        OnHitingBall(this, _pushForce);
        _pushForce = 0;
        yield return new WaitForSeconds(_switchCameraPositionDelay);
        StandBy();
    }

    private void StandBy()
    {
        _childStickHead.enabled = false;
        IsAiming = false;
        transform.position = _parkingLot;
        transform.rotation = _parkingOffset;
    }
    public void Aiming()
    {
        IsAiming = true;
        _currentAimPos = new Vector3(_cueBall.position.x +_offsetPos.x, _offsetPos.y, _cueBall.position.z + _offsetPos.z);
        transform.position = _currentAimPos;
        transform.rotation = _aimOffset;
    }
}
