using UnityEditor;
using UnityEngine;

public class PlatformGrabber : PlatformInteractable
{
    private PathPlatform _platform;
    private PathPlatform.PlatformDirection _direction;



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

    private void LateUpdate()
    {
        if (currentInteractable != this) return;

        var plane = new Plane(Vector3.up, transform.position);
        var finalPos = Vector3.zero;
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out var distance)) finalPos = _platform.StartPosition - ray.GetPoint(distance) + transform.localPosition;
        print(finalPos);
        switch (_direction)
        {
            case PathPlatform.PlatformDirection.Left:
                _platform.SetNewPlatformPosition(-finalPos.x);
                break;
            case PathPlatform.PlatformDirection.Up:
                _platform.SetNewPlatformPosition(-finalPos.x + -finalPos.z);
                break;
            case PathPlatform.PlatformDirection.Right:
                _platform.SetNewPlatformPosition(-finalPos.z);
                break;
        }
    }
}
