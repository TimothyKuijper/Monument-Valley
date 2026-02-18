using System.Linq;
using UnityEngine;
using static PathPlatform;

public class RotationPlatform : MovingPlatform
{
    public enum PlatformRotation
    {
        X, Y, Z
    }

    [Header("Editing Settings")]
    [SerializeField] private PlatformRotation rotationDir;
    public PlatformRotation RotationDir => rotationDir;

    private Quaternion _nextRotation;
    private float _currentValue = 0;

    private int[] RoundRotations = new int[4] { 0, 90, 180, 270 };


    private void Start()
    {
        _nextRotation = transform.rotation;
    }



    private void Update()
    {
        if (transform.rotation.eulerAngles == _nextRotation.eulerAngles || _time >= dragSpeed)
        {
            isMoving = false;
            return;
        }

        _time += Time.deltaTime * dragSpeed;
        var rotationLerp = Quaternion.Lerp(transform.rotation, _nextRotation, _time);

        transform.rotation = rotationLerp;
        isMoving = true;
    }


    public void SetNewPlatformRotation(float newRotation, bool rounded = false)
    {
        _time = Time.deltaTime;
        _currentValue = rounded ? GetNearestRotation(newRotation) : newRotation;
        _nextRotation = GetPlatformQuaternion(_currentValue);
    }

    public void SnapRotate() => SetNewPlatformRotation(_currentValue + 90, true);

    public void FinalizePlatformRotation() => SetNewPlatformRotation(_currentValue, true);



    public float GetNearestRotation(float rotation)
    {
        if (rotation >= 360f) return 0;
        return RoundRotations.OrderBy(x => Mathf.Abs(rotation - x)).First();
    }

    public Quaternion GetPlatformQuaternion(float rotation)
    {
        switch (rotationDir)
        {
            case PlatformRotation.X:
                return Quaternion.Euler(rotation, 0, 0);
            case PlatformRotation.Y:
                return Quaternion.Euler(0, rotation, 0);
            case PlatformRotation.Z:
                return Quaternion.Euler(0, 0, rotation);
        }
        return Quaternion.Euler(Vector3.zero);
    }

    public float GetPlatformRotation(Quaternion rotation)
    {
        switch (rotationDir)
        {
            case PlatformRotation.X:
                return rotation.eulerAngles.x;
            case PlatformRotation.Y:
                return rotation.eulerAngles.y;
            case PlatformRotation.Z:
                return rotation.eulerAngles.z;
        }
        return rotation.eulerAngles.x;
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1);
    }
}
