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



    //private void Update()
    //{
    //    if (clickRotate) return;
    //    if (currentInteractable != this) return;

    //    var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position - transform.localPosition;
    //    var mouseAngle = Vector3.Angle(transform.localPosition, mousePos);
    //    print(mouseAngle);
    //    switch (rotation)
    //    {
    //        case RotationPlatform.PlatformRotation.X:
    //            platform.SetNewPlatformRotation(mouseAngle);
    //            break;
    //        case RotationPlatform.PlatformRotation.Y:
    //            platform.SetNewPlatformRotation(mouseAngle);
    //            break;
    //        case RotationPlatform.PlatformRotation.Z:
    //            platform.SetNewPlatformRotation(mouseAngle);
    //            break;
    //    }
    //}
}
