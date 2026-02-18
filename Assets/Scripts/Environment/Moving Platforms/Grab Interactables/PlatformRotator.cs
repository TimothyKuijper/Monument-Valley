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
    }



    private void Update()
    {
        if (clickRotate) return;
        if (currentInteractable != this) return;

        var rotateAxis = Camera.main.WorldToViewportPoint(transform.position + GetVectorDir());
        var targetPos = Camera.main.ScreenToViewportPoint(Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position));
        var dotProduct = Vector2.Dot(rotateAxis, GetVectorDir());
        var angle = Vector2.SignedAngle(rotateAxis, targetPos) + 180f;

        platform.SetNewPlatformRotation(angle);
    }

    public Vector3 GetVectorDir()
    {
        switch (rotation)
        {
            case RotationPlatform.PlatformRotation.X:
                return Vector3.right;
            case RotationPlatform.PlatformRotation.Y:
                return Vector3.up;
            case RotationPlatform.PlatformRotation.Z:
                return Vector3.forward;
        }
        return Vector3.up;
    }

    public Vector3 GetVectorSelfDir()
    {
        if (rotation == RotationPlatform.PlatformRotation.Y) return Vector3.forward;
        return Vector3.up;
    }



    private void OnDrawGizmos()
    {
        if (clickRotate) return;
        if (currentInteractable != this) return;

        Gizmos.color = Color.deepPink;
        var targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position;
        var dir = transform.position + GetVectorSelfDir() * 4;
        var dir3 = Camera.main.WorldToViewportPoint(dir);

        Gizmos.DrawLine(transform.position, targetPos);
        Gizmos.DrawLine(transform.position, dir);
    }
}
