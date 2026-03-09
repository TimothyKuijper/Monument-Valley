using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RotationPlatform : MovingPlatform
{
    public enum PlatformRotation
    {
        X, Y, Z
    }

    [Header("Editing Settings")]
    [SerializeField] private PlatformRotation rotationDir;
    public PlatformRotation RotationDir => rotationDir;

    [SerializeField] private List<RotationPlatform> unisonPlatforms = new List<RotationPlatform>();
    [SerializeField] private List<RotationPlatform> oppositePlatforms = new List<RotationPlatform>();

    private bool _isStraight;
    private Quaternion _nextRotation;
    private float _currentValue = 0;
    private float _previousNewRotation = 0;

    private int[] RoundRotations = new int[4] { 0, 90, 180, 270 };
    private const float MaxRotation = 360f;


    private void Start()
    {
        _nextRotation = transform.rotation;
    }



    private void Update()
    {
        if ((transform.rotation.eulerAngles == _nextRotation.eulerAngles || _time >= dragTime) && _isStraight == false)
        {
            if (isMoving == false) return;
            isMoving = false;

            transform.rotation = _nextRotation;
            NodeBank.RebuildGraph(_camera);
            return;
        }

        isMoving = true;
        if (_isStraight)
        {
            transform.rotation = _nextRotation;
            return;
        }

        _time += Time.deltaTime * dragTime;
        var rotationLerp = Quaternion.Lerp(transform.rotation, _nextRotation, _time);

        transform.rotation = rotationLerp;
    }


    public void SetNewPlatformRotation(float newRotation, bool add, bool rounded = false)
    {
        var rotation = newRotation;
        if (add == true && rounded == false)
        {
            if (newRotation == _previousNewRotation) return;

            var currentRotation = GetPlatformRotation(transform.rotation);
            rotation = Mathf.Repeat(_currentValue + (newRotation < _previousNewRotation ? 1 : -1), MaxRotation);
            _previousNewRotation = newRotation;
        }

        _time = Time.deltaTime;
        _isStraight = !rounded;
        _currentValue = rounded ? GetNearestRotation(rotation) : rotation;
        _nextRotation = GetPlatformQuaternion(_currentValue);

        foreach (var platform in unisonPlatforms) platform.SetNewPlatformRotation(newRotation, true, rounded);
        foreach (var platform in oppositePlatforms) platform.SetNewPlatformRotation(-newRotation, true, rounded);
    }

    public void SnapRotate() => SetNewPlatformRotation(_currentValue + 90, false, true);

    public void FinalizePlatformRotation() => SetNewPlatformRotation(_currentValue, false, true);



    public float GetNearestRotation(float rotation)
    {
        if (rotation >= MaxRotation) return 0;
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

    public Vector3 GetPlatformVectorDir()
    {
        switch (rotationDir)
        {
            case PlatformRotation.X:
                return Vector3.right;
            case PlatformRotation.Y:
                return Vector3.up;
            case PlatformRotation.Z:
                return Vector3.forward;
        }
        return Vector3.up;
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1);
    }
}
