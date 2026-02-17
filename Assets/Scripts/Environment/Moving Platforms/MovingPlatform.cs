using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
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
}
