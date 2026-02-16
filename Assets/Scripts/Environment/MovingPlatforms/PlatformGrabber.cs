using UnityEngine;

public class PlatformGrabber : PlatformInteractable
{
    private MovingPlatform platform;
    private MovingPlatform.PlatformDirection direction;


    private void Start()
    {
        if (transform.parent == null)
        {
            Debug.LogError("Please put moving platform " +  gameObject.name + " as a child of a MovingPlatform object.");
            return;
        }
        platform = GetComponentInParent<MovingPlatform>();
        direction = platform.Direction;

        stopInteractEvent.AddListener(() => platform.FinalizePlatformPosition());
    }

    private void Update()
    {
        if (currentInteractable != this) return;

        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position - transform.localPosition;
        switch (direction)
        {
            case MovingPlatform.PlatformDirection.Left:
                platform.SetNewPlatformPosition(mousePos.x);
                break;
            case MovingPlatform.PlatformDirection.Up:
                platform.SetNewPlatformPosition(mousePos.y);
                break;
            case MovingPlatform.PlatformDirection.Right:
                platform.SetNewPlatformPosition(mousePos.z);
                break;
        }
    }
}
