using UnityEngine;

public class PlatformRotator : PlatformInteractable
{
    [SerializeField] private bool clickRotate = true;

    private RotationPlatform platform;
    private RotationPlatform.PlatformRotation rotation;
    private Vector3 _vectorDir;


    private void Start()
    {
        if (transform.parent == null)
        {
            Debug.LogError("Please put rotating platform rotator " + gameObject.name + " as a child of a MovingPlatform object.");
            return;
        }
        platform = GetComponentInParent<RotationPlatform>();
        rotation = platform.RotationDir;
        _vectorDir = platform.GetPlatformVectorDir();

        if (clickRotate) startInteractEvent.AddListener(() => platform.SnapRotate());
        stopInteractEvent.AddListener(() => platform.FinalizePlatformRotation());
        platform.onWalkOn.AddListener(SetWalkOn);
    }



    private void Update()
    {
        if (clickRotate) return;
        if (currentInteractable != this) return;

        var rotateAxis = Camera.main.WorldToScreenPoint(transform.position + _vectorDir);
        var targetPos = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        var dotProduct = Vector2.Dot(rotateAxis, _vectorDir);
        var angle = Vector2.SignedAngle(rotateAxis, targetPos) + 180f;

        platform.SetNewPlatformRotation(angle);
    }



    private void OnDrawGizmos()
    {
        if (clickRotate) return;
        if (currentInteractable != this) return;

        Gizmos.color = Color.deepPink;
        var targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var selfDir = rotation == RotationPlatform.PlatformRotation.Y ? Vector3.forward : Vector3.up;
        var dir = transform.position + selfDir * 4;

        Gizmos.DrawLine(transform.position, targetPos);
        Gizmos.DrawLine(transform.position, dir);
    }
}
