using UnityEngine;
using UnityEngine.Events;

public class MovingPlatform : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] protected float dragTime = 0.1f;
    protected float _time;

    [SerializeField] private bool _disableOnPlatform;

    private bool moving;
    public bool isMoving
    {
        get => moving;
        set
        {
            if (value == moving) return;

            for (int childIdx = 0; childIdx < transform.childCount; childIdx++)
            {
                var child = transform.GetChild(childIdx);
                Node childNode;
                if (child.gameObject.TryGetComponent<Node>(out childNode)) childNode.Walkable = !value;
            }
            moving = value;
        }
    }

    public UnityEvent<bool> onWalkOn = new UnityEvent<bool>();
    
    protected Camera _camera; // Used to rebuild Nodegraph when moved



    private void Awake()
    {
        SetDisableOnEnter();
        _camera = FindFirstObjectByType<Camera>();
    }

    private void SetDisableOnEnter()
    {
        if (_disableOnPlatform == false) return;

        for (var childIdx = 0; childIdx < transform.childCount; childIdx++)
        {
            var child = transform.GetChild(childIdx);
            Node childNode;
            if (child.gameObject.TryGetComponent<Node>(out childNode))
            {
                childNode.onEnter.AddListener(() => onWalkOn.Invoke(true));
                childNode.onExit.AddListener(() => onWalkOn.Invoke(false));
            }
        }
    }


    // Use this to send disable from the outside, possibly with a UnityEvent. WARNING: If this platform has _disableOnPlatform enabled, this will always override the status
    public void SendDisableEvent(bool disable) => onWalkOn.Invoke(disable); 
}
