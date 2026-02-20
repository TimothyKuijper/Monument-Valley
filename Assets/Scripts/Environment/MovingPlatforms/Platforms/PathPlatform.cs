using System.Collections.Generic;
using UnityEngine;

public class PathPlatform : MovingPlatform
{
    [Header("Pathing")]
    [SerializeField] private float pathSnapDistance = .1f;

    public enum PlatformDirection
    {
        Left, Up, Right
    }

    [Header("Editing Settings")]
    [SerializeField] private PlatformDirection direction;
    public PlatformDirection Direction => direction;

    [SerializeField][Range(0, 100)] private int minDirection;
    [SerializeField][Range(0, 100)] private int maxDirection;

    [SerializeField] private List<PathPlatform> unisonPlatforms = new List<PathPlatform>();
    [SerializeField] private List<PathPlatform> oppositePlatforms = new List<PathPlatform>();

    private Vector3 _startPosition;
    private Vector3 _nextPosition;
    private float _currentValue = 0;


    private void Start()
    {
        _startPosition = transform.position;
        _nextPosition = _startPosition;
    }


    private void Update()
    {
        if (transform.position == _nextPosition) return;

        _time = Time.deltaTime * dragTime;
        var positionLerp = Vector3.Lerp(transform.position, _nextPosition, _time);

        transform.position = positionLerp;

        if (Vector3.Distance(transform.position, _nextPosition) < pathSnapDistance && _nextPosition == GetPositionAlongPath(_currentValue, true))
        {
            transform.position = _nextPosition;
            isMoving = false;
            return;
        }
        isMoving = true;
    }

    public void SetNewPlatformPosition(float value, bool rounded = false)
    {
        _currentValue = value;
        _nextPosition = GetPositionAlongPath(_currentValue, rounded);

        foreach (var platform in unisonPlatforms) platform.SetNewPlatformPosition(value, rounded);
        foreach (var platform in oppositePlatforms) platform.SetNewPlatformPosition(-value, rounded);
    }

    public void FinalizePlatformPosition() => SetNewPlatformPosition(_currentValue, true);



    public Vector3 GetDirectionPosition(Vector3 position, float value)
    {
        switch (direction)
        {
            case PlatformDirection.Left:
                return position += Vector3.right * value;
            case PlatformDirection.Up:
                return position += Vector3.up * value;
            case PlatformDirection.Right:
                return position += Vector3.forward * value;
        }
        return position;
    }

    public Vector3 GetDirectionVector()
    {
        switch (direction)
        {
            case PlatformDirection.Left:
                return Vector3.right;
            case PlatformDirection.Up:
                return Vector3.up;
            case PlatformDirection.Right:
                return Vector3.forward;
        }
        return Vector3.right;
    }

    public float GetPositionVector(Vector3 position)
    {
        switch (direction)
        {
            case PlatformDirection.Left:
                return position.x;
            case PlatformDirection.Up:
                return position.y;
            case PlatformDirection.Right:
                return position.z;
        }
        return position.x;
    }



    private Vector3 GetEndPosition(bool max = true)
    {
        var value = max == true ? maxDirection : -minDirection;
        var originPosition = Application.isEditor && Application.isPlaying == false ? transform.position : _startPosition;
        var position = GetDirectionPosition(originPosition, value);
        return position;
    }

    private Vector3 GetPositionAlongPath(float value = 1, bool rounded = false)
    {
        var usedValue = rounded ? (int)value : value;
        var clampedValue = Mathf.Clamp(usedValue, -minDirection, maxDirection);
        var position = GetDirectionPosition(_startPosition, clampedValue);
        return position;
    }
    


    private void OnDrawGizmos()
    {
        var startPos = GetEndPosition(false);
        var endPos = GetEndPosition();

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPos, endPos);
        Gizmos.DrawSphere(startPos, .1f);
        Gizmos.DrawSphere(endPos, .1f);
    }
}
