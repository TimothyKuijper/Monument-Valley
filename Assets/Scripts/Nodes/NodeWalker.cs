using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yakanashe.Yautl;

public class NodeWalker : MonoBehaviour
{
    public float moveSpeed = 0.3f;
    public float stepHeight = 0.6f; // bit over half a unit for stairs
    public float overlapTolerance = 0.1f;
    public new Camera camera; // rider cried so i added new

    [Space(30)] public Node currentNode;

    private void Start()
    {
        currentNode = transform.FindClosestNode();
        transform.position = currentNode.Position;
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Debug.Log("Clicked!");
        Node target = NodeUtils.FindClosestNodeToMouse(camera);
        if (target == null || target == currentNode) return;

        RebuildGraph();

        var path = NodeUtils.BFS(currentNode, target);
        if (path == null) return;

        StartCoroutine(MovePath(path));
    }

    private void RebuildGraph()
    {
        var nodes = FindObjectsByType<Node>(FindObjectsSortMode.None);

        foreach (var n in nodes)
            n.ConnectedNodes.Clear();

        foreach (var A in nodes)
        {
            if (!IsWalkable(A)) continue;
            Vector3 flatA = Flatten(A.Position);

            foreach (var B in nodes)
            {
                if (A == B || !IsWalkable(B)) continue;

                Vector3 flatB = Flatten(B.Position);
                Vector3 delta = flatB - flatA;
                Vector3 flatUp = Vector3.Cross(camera.transform.right, camera.transform.forward);

                float planarDistance = Vector2.Distance(new Vector2(flatA.x, flatA.z), new Vector2(flatB.x, flatB.z));
                
                if (planarDistance > overlapTolerance || Mathf.Abs(Vector3.Dot(delta, flatUp)) < 0.01f)
                    continue;
                
                A.ConnectedNodes.Add(B);
                Debug.DrawLine(A.Position, B.Position, Color.green, 1f);
            }
        }
    }

    private static bool IsWalkable(Node n)
    {
        if (n == null) return false;
        if (!n.gameObject.activeInHierarchy) return false;

        return n.CurrentDirection == Direction.UP;
    }


    private Vector3 Flatten(Vector3 worldPos)
    {
        Vector3 camPos = camera.transform.position;
        Vector3 camForward = camera.transform.forward;

        Vector3 toPoint = worldPos - camPos;

        // remove depth relative to camera
        return worldPos - Vector3.Dot(toPoint, camForward) * camForward;
    }


    private IEnumerator MovePath(List<Node> path)
    {
        Debug.Log("Moved!");

        foreach (var node in path)
        {
            transform.MoveTo(node.Position, moveSpeed, EaseType.Linear);
            yield return new WaitForSeconds(moveSpeed);
            currentNode = node;
        }
    }
}