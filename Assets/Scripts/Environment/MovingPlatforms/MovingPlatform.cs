using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class MovingPlatform : MonoBehaviour
{
    enum PlatformDirection
    {
        Left, Up, Right
    }
    [SerializeField] private PlatformDirection direction;

    [SerializeField][Range(0, 100)] private int minDirection;
    [SerializeField][Range(0, 100)] private int maxDirection;

    private Vector3 _startPosition;
    private int currentValue = 0;


    private void Start()
    {
        _startPosition = transform.position;
    }


    private void Update() // TEST
    {
        if (Input.GetKeyDown(KeyCode.W)) SetNewPlatformPosition(1);
        if (Input.GetKeyDown(KeyCode.S)) SetNewPlatformPosition(-1);
    }



    public void SetNewPlatformPosition(int directionValue = 1)
    {
        currentValue = Mathf.Clamp(currentValue + directionValue, -minDirection, maxDirection);
        var position = GetPositionAlongPath(currentValue);
        transform.position = position;
    }



    private Vector3 GetDirectionPosition(Vector3 position, int value)
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

    private Vector3 GetPositionAlongPath(int value = 1)
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
