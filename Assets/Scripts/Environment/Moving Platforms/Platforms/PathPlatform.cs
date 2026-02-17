using UnityEngine;

public class PathPlatform : MovingPlatform
{
    [Header("Pathing")]
    [SerializeField] protected float pathSnapDistance = .1f;

    public enum PlatformDirection
    {
        Left, Up, Right
    }

    [Header("Editing Settings")]
    [SerializeField] private PlatformDirection direction;
    public PlatformDirection Direction => direction;

    [SerializeField][Range(0, 100)] private int minDirection;
    [SerializeField][Range(0, 100)] private int maxDirection;

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

        _time = Time.deltaTime * dragSpeed;
        var positionLerp = Vector3.Lerp(transform.position, _nextPosition, _time);

        transform.position = positionLerp;

        if (Vector3.Distance(transform.position, _nextPosition) < pathSnapDistance)
        {
            transform.position = _nextPosition;
            isMoving = false;
            return;
        }
        isMoving = true;
    }

    public void SetNewPlatformPosition(float value, bool rounded = false)
    {
        _currentValue = rounded ? (int)value : value;
        _nextPosition = GetPositionAlongPath(_currentValue);
    }

    public void FinalizePlatformPosition() => SetNewPlatformPosition(_currentValue, true);



    private Vector3 GetDirectionPosition(Vector3 position, float value)
    {
        switch (direction)
        {
            case PlatformDirection.Left:
                position += Vector3.right * value;
                break;
            case PlatformDirection.Up:
                position += Vector3.up * value;
                break;
            case PlatformDirection.Right:
                position += Vector3.forward * value;
                break;
        }
        return position;
    }

    private Vector3 GetEndPosition(bool max = true)
    {
        var value = max == true ? maxDirection : -minDirection;
        var originPosition = Application.isEditor && Application.isPlaying == false ? transform.position : _startPosition;
        var position = GetDirectionPosition(originPosition, value);
        return position;
    }

    private Vector3 GetPositionAlongPath(float value = 1)
    {
        var clampedValue = Mathf.Clamp(value, -minDirection, maxDirection);
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
