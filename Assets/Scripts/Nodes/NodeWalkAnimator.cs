using System.Collections;
using UnityEngine;

public class NodeWalkAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NodeWalker nodeWalker;
    [SerializeField] private Transform modelTransform;

    [SerializeField] private float turnSpeed = 0.12f;

    private float _lastAngle;

    private const string IsWalkingBoolName = "Walking";
    private const string ShockBoolName = "Shock";


    private void Start()
    {
        nodeWalker.OnNextOccupied.AddListener(() => Shock());
        nodeWalker.OnStartMoving.AddListener(() => Walk(true));
        nodeWalker.OnPathComplete.AddListener((value) => Walk(false));

        nodeWalker.OnExit.AddListener(SetDirection);
        SetDirection();
    }


    private void Shock()
    {
        animator.SetBool(ShockBoolName, true);
        animator.SetBool(IsWalkingBoolName, false);
    }

    private void Walk(bool isWalking)
    {
        if (isWalking == false && animator.GetBool(ShockBoolName) == true) return; 

        animator.SetBool(IsWalkingBoolName, isWalking);
        animator.SetBool(ShockBoolName, false);
    }


    private void SetDirection()
    {
        var direction = nodeWalker.Direction;
        var flatDirection = new Vector3(direction.x, 0, direction.z);
        var angle = Vector3.SignedAngle(Vector3.forward, flatDirection, Vector3.up) + RotUtil.HalfRotation;
        var newAngle = RotUtil.GetNearestRotation(angle);

        if (newAngle == _lastAngle) return;

        _lastAngle = newAngle;
        StopAllCoroutines();
        StartCoroutine(Turn(newAngle));
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
