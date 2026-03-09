using UnityEngine;

public class PlatformGrabber : PlatformInteractable
{
    private PathPlatform platform;
    private PathPlatform.PlatformDirection direction;

    private const float defaultProjectionSize = 15f;

    private void Start()
    {
        if (transform.parent == null)
        {
            Debug.LogError("Please put path platform grabber " +  gameObject.name + " as a child of a MovingPlatform object.");
            return;
        }
        platform = GetComponentInParent<PathPlatform>();
        direction = platform.Direction;

        stopInteractEvent.AddListener(() => platform.FinalizePlatformPosition());
        platform.onWalkOn.AddListener(SetWalkOn);
    }

    private void Update()
    {
        if (currentInteractable != this) return;

        var mousePos = Input.mousePosition - _camera.WorldToScreenPoint(transform.position);
        var finalPos = mousePos / defaultProjectionSize * _camera.orthographicSize;
        switch (direction)
        {
            case PathPlatform.PlatformDirection.Left:
                platform.SetNewPlatformPosition(finalPos.x);
                break;
            case PathPlatform.PlatformDirection.Up:
                platform.SetNewPlatformPosition(finalPos.y);
                break;
            case PathPlatform.PlatformDirection.Right:
                platform.SetNewPlatformPosition(-finalPos.x);
                break;
        }
    }
}
