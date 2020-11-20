using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Ball : MonoBehaviour
{
    protected virtual bool _useGravity => false;
    protected virtual Vector3 _gravity => Physics.gravity;
    protected virtual Vector3 _startPosition => Vector3.zero;
    public virtual float Mass => 1.0f;
    public virtual bool IsInHole { get; set; }
    public virtual float ContactRadius => 0.5f;
    
    public Vector3 Velocity = Vector3.zero;
    public int BallID = 0;

    private Ball _lastCollidingObject = null;
    private const float _lastCollisionDelay = 0.01f;
    private float _lastCollisionDelayCounter = _lastCollisionDelay;
    private float _staticVelocityLimit = 0.2f;

    public static Action OnFallToHole = delegate { };
    private void Start()
    {
        RunStart();
    }

    private void Update()
    {
        RunUpdate();
    }

    private void FixedUpdate()
    {
        RunFixUpdate();
    }
    protected virtual void RunStart() 
    {
        transform.position = _startPosition;
    }

    protected virtual void RunUpdate() {}

    protected virtual void RunFixUpdate() 
    {
        if (PoolBallsContainer.PoolBallHashSet.Count > 0)
        {
            if (IsMoving(this))
                return;

            foreach (var ball in PoolBallsContainer.PoolBallHashSet.Where(ob => ob != this))
            {
                if (ball.IsInHole)
                    break;

                Collide(ball);
            }
        }
    }

    public void ApplyForce(Vector3 force, bool isVerlet = false, bool applyRotation = false)
    {
        Vector3 totalForce = (_useGravity) ? force + (Mass * _gravity) : force;
        Vector3 acceleration = totalForce / Mass;
        
        Movement(acceleration, isVerlet, applyRotation);
    }

    private void Movement(Vector3 acceleration, bool isVerlet = false, bool applyRotation = true)
    {
        if (!isVerlet)
        {
            Velocity += acceleration * Time.deltaTime;
            transform.position += Velocity * Time.deltaTime;
        }
        else
        {
            transform.position += Velocity * Time.deltaTime + acceleration * Time.deltaTime * Time.deltaTime * 0.5f;
            Velocity += acceleration * Time.deltaTime;
        }

        if (applyRotation)
        {
            Vector3 rotationForce = new Vector3(Velocity.z, 0, -Velocity.x);
            transform.Rotate(rotationForce);
        }
    }

    public bool FallingToHole(bool value)
    {
        return IsInHole = value;
    }

    public bool IsLastCollidingEqual(Ball ball)
    {
        if (_lastCollidingObject == null)
            return false;

        return ball == _lastCollidingObject;
    }

    public void RegisterCollision(Ball ball)
    {
        _lastCollisionDelayCounter = _lastCollisionDelay;
        _lastCollidingObject = ball;

        StartCoroutine(CollisionCounterRoutine());
    }

    private IEnumerator CollisionCounterRoutine()
    {
        if(_lastCollidingObject != null)
        {
            _lastCollisionDelayCounter -= Time.deltaTime;
            if (_lastCollisionDelayCounter <= 0f)
            {
                _lastCollisionDelayCounter = _lastCollisionDelay;
                _lastCollidingObject = null;
            }
            yield return null;
        }
    }

    protected bool IsMoving(Ball ball)
    {
        return ball.Velocity.magnitude < _staticVelocityLimit;
    }

    protected void Collide(Ball otherBall)
    {
        if (this.IsLastCollidingEqual(otherBall) || otherBall.IsLastCollidingEqual(this))
            return;

        this.RegisterCollision(otherBall);
        otherBall.RegisterCollision(this);

        float distance = Vector3.Distance(this.transform.position, otherBall.transform.position);
        float radiusSum = this.ContactRadius + otherBall.ContactRadius;

        if (distance > radiusSum)
            return;

        Vector3 ballToOtherBallDirection = (otherBall.transform.position - this.transform.position).normalized;
        Vector3 pushFromA = radiusSum * ballToOtherBallDirection;

        otherBall.transform.position = this.transform.position + pushFromA;

        TwoDimensionReflection(this, otherBall);
    }

    protected void TwoDimensionReflection(Ball a, Ball b)
    {
        float massSum = a.Mass + b.Mass;
        float doubleMassA = a.Mass * 2f;
        float doubleMassB = b.Mass * 2f;

        float doubleAOverSum = doubleMassA / massSum;
        float doubleBOverSum = doubleMassB / massSum;

        Vector3 vAMinusVB = a.Velocity - b.Velocity;
        Vector3 vBMinusVA = -vAMinusVB;

        Vector3 posAMinusPosB = a.transform.position - b.transform.position;
        Vector3 posBMinusPosA = -posAMinusPosB;

        float posesSquare = Vector3.Dot(posAMinusPosB, posAMinusPosB);

        float dotA = Vector3.Dot(vAMinusVB, posAMinusPosB);
        float dotB = Vector3.Dot(vBMinusVA, posBMinusPosA);

        a.Velocity -= doubleBOverSum * (dotA / posesSquare) * posAMinusPosB;
        b.Velocity -= doubleAOverSum * (dotB / posesSquare) * posBMinusPosA;
    }

}
