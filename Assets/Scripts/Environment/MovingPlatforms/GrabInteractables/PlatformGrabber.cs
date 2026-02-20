using UnityEngine;

public class PlatformGrabber : PlatformInteractable
{
    private PathPlatform platform;
    private PathPlatform.PlatformDirection direction;


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
        switch (direction)
        {
            case PathPlatform.PlatformDirection.Left:
                platform.SetNewPlatformPosition(mousePos.x);
                break;
            case PathPlatform.PlatformDirection.Up:
                platform.SetNewPlatformPosition(mousePos.y);
                break;
            case PathPlatform.PlatformDirection.Right:
                platform.SetNewPlatformPosition(-mousePos.x);
                break;
        }
    }
}
