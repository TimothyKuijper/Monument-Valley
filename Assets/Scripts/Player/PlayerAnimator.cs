using UnityEngine;
using Yakanashe.Yautl;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NodeWalker nodeWalker;
    [SerializeField] private Transform modelTransform;

    [SerializeField] private float turnSpeed = 0.25f;

    private float _lastAngle;

    private const string IsWalkingBoolName = "Walking";


    private void Start()
    {
        nodeWalker.OnStartMoving.AddListener(() => animator.SetBool(IsWalkingBoolName, true));
        nodeWalker.OnPathComplete.AddListener((value) => animator.SetBool(IsWalkingBoolName, false));
        nodeWalker.OnExit.AddListener(SetDirection);
    }


    private void SetDirection()
    {
        var direction = nodeWalker.Direction;
        var angle = Vector3.SignedAngle(-Vector3.forward, direction, Vector3.up);

        if (angle == _lastAngle) return;
        print(angle);
        print(direction);

        _lastAngle = angle;
        modelTransform.RotateTo(new Vector3(0, angle, 0), turnSpeed, EaseType.Linear);
    }
}
