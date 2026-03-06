using UnityEngine;

public class PlatformRotator : PlatformInteractable
{
    [SerializeField] private bool clickRotate = true;

    private RotationPlatform platform;
    private RotationPlatform.PlatformRotation rotation;


    private void Start()
    {
        if (transform.parent == null)
        {
            Debug.LogError("Please put rotating platform rotator " + gameObject.name + " as a child of a MovingPlatform object.");
            return;
        }
        platform = GetComponentInParent<RotationPlatform>();
        rotation = platform.RotationDir;

        if (clickRotate) startInteractEvent.AddListener(() => platform.SnapRotate());
        stopInteractEvent.AddListener(() => platform.FinalizePlatformRotation());
        platform.onWalkOn.AddListener(SetWalkOn);
    }



    private void Update()
    {
        if (clickRotate) return;
        if (currentInteractable != this) return;

        var rotateAxis = _camera.WorldToScreenPoint(transform.position + platform.GetPlatformVectorDir());
        var targetPos = Input.mousePosition - _camera.WorldToScreenPoint(transform.position);
        var dotProduct = Vector2.Dot(rotateAxis, platform.GetPlatformVectorDir());
        var angle = Vector2.SignedAngle(rotateAxis, targetPos) + 180f;

        platform.SetNewPlatformRotation(angle);
    }



    private new void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (clickRotate) return;
        if (currentInteractable != this) return;

        Gizmos.color = Color.deepPink;
        var targetPos = _camera.ScreenToWorldPoint(Input.mousePosition);
        var selfDir = rotation == RotationPlatform.PlatformRotation.Y ? Vector3.forward : Vector3.up;
        var dir = transform.position + selfDir * 4;

        Gizmos.DrawLine(transform.position, targetPos);
        Gizmos.DrawLine(transform.position, dir);
    }
}
