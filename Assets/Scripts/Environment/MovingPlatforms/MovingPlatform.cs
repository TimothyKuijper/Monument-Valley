using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class MovingPlatform : MonoBehaviour
{
    public enum PlatformDirection
    {
        Left, Up, Right
    }
    [SerializeField] private PlatformDirection direction;
    public PlatformDirection Direction => direction;

    [SerializeField][Range(0, 100)] private int minDirection;
    [SerializeField][Range(0, 100)] private int maxDirection;

    private Vector3 _startPosition;
    private float currentValue = 0;


    private void Start()
    {
        _startPosition = transform.position;
    }



    public void SetNewPlatformPosition(float value, bool rounded = false)
    {
        currentValue = rounded ? (int)value : value;
        var position = GetPositionAlongPath(currentValue);
        transform.position = position;
    }

    public void FinalizePlatformPosition() => SetNewPlatformPosition(currentValue, true);



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
