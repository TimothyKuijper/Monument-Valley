using UnityEngine;
using Yakanashe.Yautl;

public class CameraMover : MonoBehaviour
{
    public void MoveToNode(Node node)
    {
        transform.MoveTo(node.Position + new Vector3(-60, 60, -60), 2f, EaseType.InOutCubic);
    }
}
