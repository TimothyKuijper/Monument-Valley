using System.Collections.Generic;
using UnityEngine;

public class RotationPlatform : MovingPlatform
{
    [Header("Rotating")]
    [SerializeField] private float rotationSpeed = 270f;

    public enum PlatformRotation
    {
        X, Y, Z
    }

    [Header("Editing Settings")]
    [SerializeField] private PlatformRotation rotationDir;
    public PlatformRotation RotationDir => rotationDir;

    [SerializeField] private List<RotationPlatform> unisonPlatforms = new List<RotationPlatform>();
    [SerializeField] private List<RotationPlatform> oppositePlatforms = new List<RotationPlatform>();

    private bool _isDone;
    private Quaternion _nextRotation;
    private float _currentValue = 0;
    private float _previousNewRotation = 0;


    private void Start()
    {
        _nextRotation = transform.rotation;
    }



    private void FixedUpdate()
    {
        if (_isDone == true) return;
        if (_time >= dragTime)
        {
            if (isMoving == false) return;
            isMoving = false;

            transform.rotation = _nextRotation;
            NodeBank.RebuildGraph(_camera);
            return;
        }
        _time += Time.fixedDeltaTime / dragTime;
        var rotationLerp = Quaternion.Lerp(transform.rotation, _nextRotation, _time);

        transform.rotation = rotationLerp;
    }


    public void SetNewPlatformRotation(float newRotation, bool rounded = false)
    {
        isMoving = true;
        var rotation = newRotation;
        if (rounded == false)
        {
            if (newRotation == _previousNewRotation) return;

            _currentValue = Mathf.Repeat(_currentValue + (newRotation < _previousNewRotation ? rotationSpeed : -rotationSpeed) * Time.deltaTime, RotUtil.MaxRotation);
            _nextRotation = GetPlatformQuaternion(_currentValue);
            _previousNewRotation = newRotation;
            
            _isDone = true;
            transform.rotation = _nextRotation;
        }
        else
        {
            _isDone = false;
            _time = 0;
            _currentValue = RotUtil.GetNearestRotation(rotation);
            _nextRotation = GetPlatformQuaternion(_currentValue);
        }

        foreach (var platform in unisonPlatforms) platform.SetNewPlatformRotation(newRotation, rounded);
        foreach (var platform in oppositePlatforms) platform.SetNewPlatformRotation(-newRotation, rounded);
    }

    public void SnapRotate() => SetNewPlatformRotation(_currentValue + 90, true);

    public void FinalizePlatformRotation() => SetNewPlatformRotation(_currentValue, true);


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
