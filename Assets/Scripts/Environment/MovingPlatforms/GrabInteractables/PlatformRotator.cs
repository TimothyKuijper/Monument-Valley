using UnityEngine;

public class PlatformRotator : PlatformInteractable
{
    [SerializeField] private bool clickRotate = true;

    private RotationPlatform _platform;
    private RotationPlatform.PlatformRotation _rotation;


    private void Start()
    {
        if (transform.parent == null)
        {
            Debug.LogError("Please put rotating platform rotator " + gameObject.name + " as a child of a MovingPlatform object.");
            return;
        }
        _platform = GetComponentInParent<RotationPlatform>();
        _rotation = _platform.RotationDir;

        if (clickRotate) startInteractEvent.AddListener(() => _platform.SnapRotate());
        stopInteractEvent.AddListener(() => _platform.FinalizePlatformRotation());
        _platform.onWalkOn.AddListener(SetWalkOn);
    }



    private void FixedUpdate() // Consistent addup across framerates
    {
        if (clickRotate) return;
        if (currentInteractable != this) return;

        var center = _collider.center + transform.position;
        var rotateAxis = _camera.WorldToScreenPoint(center + _platform.GetPlatformVectorDir());
        var targetPos = Input.mousePosition - _camera.WorldToScreenPoint(center);
        var dotProduct = Vector2.Dot(rotateAxis, _platform.GetPlatformVectorDir());
        var angle = Vector2.SignedAngle(rotateAxis, targetPos) + 180f;

        _platform.SetNewPlatformRotation(angle, true);
    }



    private new void OnDrawGizmos()
    {
        if (Application.isPlaying == false)
        {
            base.OnDrawGizmos();
            return;
        }

        var center = _collider.center + transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(center, DebugGizmoSize);

        if (clickRotate) return;
        if (currentInteractable != this) return;

        Gizmos.color = Color.deepPink;

        var targetPos = _camera.ScreenToWorldPoint(Input.mousePosition);
        var selfDir = _rotation == RotationPlatform.PlatformRotation.Y ? Vector3.forward : Vector3.up;
        var dir = transform.position + selfDir * 4;

        Gizmos.DrawLine(center, targetPos);
        Gizmos.DrawLine(center, dir);
    }
}
