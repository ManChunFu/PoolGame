using UnityEngine;
using UnityEngine.Assertions;

public class CueStickHead : MonoBehaviour
{
    [SerializeField] private CueBall _cueBall = null;

    private Vector3 _normal => -transform.right;
    private float _contactRadius => transform.localScale.x * 0.5f;

    public bool IsHitingBall { get; private set; }
    
    private void Start()
    {
        if (_cueBall == null)
        {
            _cueBall = FindObjectOfType<CueBall>();
            Assert.IsNotNull(_cueBall, "Failed to get reference to the CueBall script");
        }
    }

    private void Update()
    {
        if (IsColliding(_cueBall))
            IsHitingBall = true;
        else
            IsHitingBall = false;

    }

    private bool IsColliding(Ball ball)
    {
        Vector3 ThisToBall = ball.transform.position - transform.position;
        return Mathf.Abs(Vector3.Dot(_normal, ThisToBall)) <= (ball.ContactRadius + _contactRadius);
    }


}
