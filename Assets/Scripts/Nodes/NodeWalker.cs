using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yakanashe.Yautl;

public class NodeWalker : MonoBehaviour
{
    public float moveSpeed = 0.3f;
    public float overlapTolerance = 0.1f;
    public new Camera camera; // rider cried so i added new

    private Node currentNode;

    private void Start()
    {
        currentNode = transform.FindClosestNode();
        transform.position = currentNode.Position;
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        var target = NodeUtils.FindClosestNodeToMouse(camera);
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
            var flatA = Flatten(A.Position);

            foreach (var B in nodes)
            {
                if (A == B || !IsWalkable(B)) continue;

                var flatB = Flatten(B.Position);
                var delta = B.Position - A.Position;

                float planarDistance = Vector2.Distance(new Vector2(flatA.x, flatA.z), new Vector2(flatB.x, flatB.z));
                
                if (planarDistance > overlapTolerance || Math.Abs(delta.x - delta.z) < 0.01f)
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
        var camPos = camera.transform.position;
        var camForward = camera.transform.forward;
        var toPoint = worldPos - camPos;
        return worldPos - Vector3.Dot(toPoint, camForward) * camForward;
    }


    private IEnumerator MovePath(List<Node> path)
    {
        foreach (var node in path)
        {
            transform.MoveTo(node.Position, moveSpeed, EaseType.Linear);
            yield return new WaitForSeconds(moveSpeed);
            currentNode = node;
        }
    }
}