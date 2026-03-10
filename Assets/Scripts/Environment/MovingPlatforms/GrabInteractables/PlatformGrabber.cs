using UnityEngine;

public class PlatformGrabber : PlatformInteractable
{
    private PathPlatform _platform;
    private PathPlatform.PlatformDirection _direction;

    private const float _defaultProjectionSize = 15f;


    private void Start()
    {
        if (transform.parent == null)
        {
            Debug.LogError("Please put path platform grabber " +  gameObject.name + " as a child of a MovingPlatform object.");
            return;
        }
        _platform = GetComponentInParent<PathPlatform>();
        _direction = _platform.Direction;

        stopInteractEvent.AddListener(() => _platform.FinalizePlatformPosition());
        _platform.onWalkOn.AddListener(SetWalkOn);
    }

    private void Update()
    {
        if (currentInteractable != this) return;

        var mousePos = Input.mousePosition - _camera.WorldToScreenPoint(transform.position);
        var finalPos = mousePos / _defaultProjectionSize * _camera.orthographicSize;
        switch (_direction)
        {
            case PathPlatform.PlatformDirection.Left:
                _platform.SetNewPlatformPosition(finalPos.x);
                break;
            case PathPlatform.PlatformDirection.Up:
                _platform.SetNewPlatformPosition(finalPos.y);
                break;
            case PathPlatform.PlatformDirection.Right:
                _platform.SetNewPlatformPosition(-finalPos.x);
                break;
        }
    }
}
