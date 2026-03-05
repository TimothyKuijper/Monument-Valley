using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Yakanashe.Yautl;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NodeWalker nodeWalker;
    [SerializeField] private Transform modelTransform;

    [SerializeField] private float turnSpeed = 0.12f;

    private float _lastAngle;

    private const string IsWalkingBoolName = "Walking";


    private void Start()
    {
        nodeWalker.OnStartMoving.AddListener(() => animator.SetBool(IsWalkingBoolName, true));
        nodeWalker.OnPathComplete.AddListener((value) => animator.SetBool(IsWalkingBoolName, false));
        nodeWalker.OnExit.AddListener(SetDirection);
        SetDirection();
    }


    private void SetDirection()
    {
        var direction = nodeWalker.Direction;
        var angle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up) + 180f;

        if (angle == _lastAngle) return;

        _lastAngle = angle;
        StopAllCoroutines();
        StartCoroutine(Turn(angle));
    }

    private IEnumerator Turn(float angle)
    {
        var elapsedTime = 0f;
        var startRotation = modelTransform.rotation;

        while (elapsedTime < turnSpeed)
        {
            modelTransform.rotation = Quaternion.Lerp(startRotation, Quaternion.Euler(0, angle, 0), elapsedTime / turnSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        modelTransform.rotation = Quaternion.Euler(0, angle, 0);
    }
}
