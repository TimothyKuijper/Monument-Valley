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



    private void Awake()
    {
        SetDisableOnEnter();
    }

    private void SetDisableOnEnter()
    {
        if (_disableOnPlatform == false) return;

        for (var childIdx = 0; childIdx < transform.childCount; childIdx++)
        {
            var child = transform.GetChild(childIdx);
            Node childNode;
            // if (child.gameObject.TryGetComponent<Node>(out childNode)) // ADD LATER
            // childNode.onEnter.AddListener(() => onWalkOn.Invoke(true));
            // childNode.onExit.AddListener(() => onWalkOn.Invoke(false));
            // nodes.Add(childNode);
        }
    }
}
