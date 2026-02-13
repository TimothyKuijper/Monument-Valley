using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public static class NodeUtils
{
    public static List<Node> BFS(Node start, Node goal)
    {
        var queue = new Queue<Node>();
        var cameFrom = new Dictionary<Node, Node>();

        queue.Enqueue(start);
        cameFrom[start] = null;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current == goal)
                break;

            foreach (var neighbor in current.ConnectedNodes)
            {
                if (cameFrom.ContainsKey(neighbor)) continue;
                queue.Enqueue(neighbor);
                cameFrom[neighbor] = current;
            }
        }

        if (!cameFrom.ContainsKey(goal))
            return null;

        var path = new List<Node>();
        var temp = goal;

        while (temp != null)
        {
            path.Add(temp);
            temp = cameFrom[temp];
        }

        path.Reverse();
        return path;
    }
    
    public static Node FindClosestNode(this Transform transform)
    {
        var nodes = Object.FindObjectsByType<Node>(FindObjectsSortMode.None);
        var min = float.MaxValue;
        Node closest = null;

        foreach (var n in nodes)
        {
            float d = Vector3.Distance(transform.position, n.Position);
            if (!(d < min)) continue;
            
            min = d;
            closest = n;
        }

        return closest;
    }
    
    public static Node RaycastNode(Camera cam)
    {
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        return !Physics.Raycast(ray, out RaycastHit hit) ? null : hit.collider.GetComponent<Node>();
    }
}