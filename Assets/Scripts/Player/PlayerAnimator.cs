using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NodeWalker nodeWalker;
    [SerializeField] private Transform modelTransform;

    private const string IsWalkingBoolName = "Walking";


    private void Start()
    {
        nodeWalker.OnStartMoving.AddListener(() => animator.SetBool(IsWalkingBoolName, true));
        nodeWalker.OnPathComplete.AddListener((value) => animator.SetBool(IsWalkingBoolName, false));
    }
}
