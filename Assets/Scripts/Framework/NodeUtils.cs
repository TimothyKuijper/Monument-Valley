using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public static class NodeUtils
{
    public static List<Node> BFS(Node start, Node destination)
    {
        var queue = new Queue<Node>();
        var cameFrom = new Dictionary<Node, Node>();

        queue.Enqueue(start);
        cameFrom[start] = null;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current == destination)
                break;

            foreach (var neighbor in current.ConnectedNodes)
            {
                if (cameFrom.ContainsKey(neighbor)) continue;
                queue.Enqueue(neighbor);
                cameFrom[neighbor] = current;
            }
        }

        if (!cameFrom.ContainsKey(destination))
            return null;

        var path = new List<Node>();
        var temp = destination;

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
        var nodes = NodeBank.SceneNodes;
        var min = float.MaxValue;
        Node closest = null;

        foreach (var node in nodes)
        {
            float distance = Vector3.Distance(transform.position, node.Position);
            if (!(distance < min)) continue;
            
            min = distance;
            closest = node;
        }

        return closest;
    }
    
    public static Node RaycastNode(Camera cam)
    {
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        return !Physics.Raycast(ray, out RaycastHit hit) ? null : hit.collider.GetComponent<Node>();
    }
}