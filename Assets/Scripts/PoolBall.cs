using System;
using UnityEngine;

public class PoolBall : Ball
{
    [SerializeField] private Vector3 _startPoint = default;
    [SerializeField] private float _mass = 1.0f;

    protected override Vector3 _startPosition => _startPoint;
    protected override bool _useGravity => true;
    public override float ContactRadius => transform.localScale.x * 0.5f;
    public override float Mass => _mass;

    public static event Action<int> IntoHole = delegate { };
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

    private void FallIntoHole()
    {
        FallingToHole(true);
        IntoHole(BallID);
        PoolBallsContainer.Deregister(this);
        gameObject.SetActive(false);
    }

}
