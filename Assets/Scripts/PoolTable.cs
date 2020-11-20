using UnityEngine;
using UnityEngine.Assertions;

public class PoolTable : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] private float _friction = 0.1f;
    [Range(0f, 1f)]
    [SerializeField] private float _bounceness = 1f;
    [SerializeField] private CustomCollider _customCollider = null;
    protected virtual Vector3 _normal => transform.up;
    protected virtual Vector3 position => transform.position;

    private float _staticVelocityLimit = 0.2f;

    private void Start()
    {
        if (_customCollider == null)
        {
            _customCollider = GetComponent<CustomCollider>();
            Assert.IsNotNull(_customCollider);
        }
    }
    private void FixedUpdate()
    {
        if (PoolBallsContainer.PoolBallHashSet.Count > 0)
        {
            foreach (Ball ball in PoolBallsContainer.PoolBallHashSet)
            {
                if (ball == null)
                    break;

                if ( !ball.IsInHole && IsColliding(ball) && _customCollider.IsInsideColliderBounds(ball.transform.position))
                {
                    ball.transform.position = CorrectedPosition(ball);
                    Impact(ball, _friction);
                }
            }
        }
    }

    private void Impact(Ball ball, float friction)
    {
        if (IsBallStaticOnTable(ball))
        {
            ball.ApplyForce(-ball.Mass * Physics.gravity);
            return;
        }
        ball.Velocity = Reflect(ball.Velocity, friction);
    }

    private Vector3 Reflect(Vector3 velocity, float friction)
    {
        return (velocity - 2f * _bounceness * Vector3.Dot(velocity, _normal) * _normal) * (1f - friction);
    }

    private bool IsBallStaticOnTable(Ball sphere)
    {
        bool lowVelocity = sphere.Velocity.magnitude < _staticVelocityLimit;
        return lowVelocity;
    }

    protected virtual float GetDistance(Ball ball)
    {
        if (ball == null)
            return 0;

        Vector3 ThisToBall = ball.transform.position - transform.position;
        return Vector3.Dot(_normal, ThisToBall);
    }

    protected virtual Vector3 Projection(Ball ball)
    {
        Vector3 ballToProjection = GetDistance(ball) * _normal;
        return ball.transform.position - ballToProjection;
    }

    protected virtual bool IsColliding(Ball ball)
    {
        if (!WillBeCollision(ball))
            return false;

        return GetDistance(ball) <= 0f || Mathf.Abs(GetDistance(ball)) <= (ball.ContactRadius);
    }

    protected virtual bool WillBeCollision(Ball ball)
    {
        return Vector3.Dot(_normal, ball.Velocity) < 0f;
    }

    protected virtual Vector3 CorrectedPosition(Ball ball)
    {
        return Projection(ball) + (_normal * ball.ContactRadius);
    }

}


