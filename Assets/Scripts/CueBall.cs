using System;
using System.Linq;
using UnityEngine;

public class CueBall : Ball
{
    [SerializeField] private Vector3 _startPoint = default;
    [SerializeField] private float _mass = 1.0f;

    protected override Vector3 _startPosition => _startPoint;
    protected override bool _useGravity => true;
    public override float ContactRadius => transform.localScale.x * 0.5f;
    public override float Mass => _mass;

    private void OnEnable()
    {
        CueStick.OnHitingBall += HitByCueStick;
    }
    protected override void RunStart()
    {
        base.RunStart();
        PoolBallsContainer.Register(this);
    }

    protected override void RunFixUpdate()
    {
        // Use Verlet && rotation
        ApplyForce(Vector3.zero, true, true);
        base.RunFixUpdate();

        if (transform.position.y < 2f)
            FallIntoHole();
    }

    private void HitByCueStick(CueStick cueStick, float pushForce)
    {
        if (cueStick == null)
            return;

        Vector3 moveDirection = (transform.position - cueStick.transform.position).normalized;
        Velocity = moveDirection * pushForce;
    }


    private void FallIntoHole()
    {
        FallingToHole(true);
        PoolBallsContainer.Deregister(this);
        gameObject.SetActive(false);
        OnFallToHole();
    }


    private void OnDisable()
    {
        CueStick.OnHitingBall -= HitByCueStick;
    }
}
