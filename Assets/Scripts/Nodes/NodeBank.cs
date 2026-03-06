using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public static class NodeBank
{
    public static List<Node> SceneNodes => cachedNodes ??= Object.FindObjectsByType<Node>(FindObjectsSortMode.None).ToList();
    private static List<Node> cachedNodes;
    
    private const float overlapTolerance = 0.9f;

    public static void ResetNodeCache() => cachedNodes = null;

    public static void RebuildGraph(Camera camera)
    {
        foreach (var node in SceneNodes)
            node.ConnectedNodes.Clear();

        foreach (var baseNode in SceneNodes)
        {
            if (!IsWalkable(baseNode, true)) continue;
            var flatBase = baseNode.Position.Flatten(camera.transform);

            foreach (var comparerNode in SceneNodes)
            {
                if (baseNode == comparerNode || !IsWalkable(comparerNode, true)) continue; 

                var flatComparer = comparerNode.Position.Flatten(camera.transform);
                var delta = comparerNode.Position - baseNode.Position;

                var planarDistance = Vector2.Distance(new Vector2(flatBase.x, flatBase.z), new Vector2(flatComparer.x, flatComparer.z));

                if (planarDistance > overlapTolerance || Mathf.Abs(delta.x - delta.z) < 0.01f)
                    continue;

                baseNode.ConnectedNodes.Add(comparerNode);
            }
        }

        // AFTER updating the whole grid, every nodes send their rebuild event (ALWAYS perform after, since midgen events can cause false positives)
        foreach (var baseNode in SceneNodes) baseNode.onRebuild.Invoke();
    }

    public enum CanReachType
    {
        Unwalkable,
        Overlap,
        Free
    }

    public static CanReachType CanReach(this Node current, Node target, Camera camera)
    {
        if (!IsWalkable(target)) return CanReachType.Unwalkable;

        var flatA = current.Position.Flatten(camera.transform);
        var flatB = target.Position.Flatten(camera.transform);
        var planarDistance = Vector2.Distance(new Vector2(flatA.x, flatA.z), new Vector2(flatB.x, flatB.z));
        
        return planarDistance < overlapTolerance ? CanReachType.Free : CanReachType.Overlap;
    }

    private static bool IsWalkable(Node node, bool omitOccupancy = false)
    {
        if (node == null || !node.gameObject.activeInHierarchy || !node.Walkable || (!omitOccupancy && node.Occupied)) return false;

        return node.CurrentDirection == Direction.UP;
    }

    private static Vector3 Flatten(this Vector3 worldPos, Transform transform)
    {
        var camPos = transform.position;
        var camForward = transform.forward;
        var toPoint = worldPos - camPos;
        return worldPos - Vector3.Dot(toPoint, camForward) * camForward;
    }
}