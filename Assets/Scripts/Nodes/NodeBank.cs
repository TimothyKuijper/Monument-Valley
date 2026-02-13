using UnityEngine;

public static class NodeBank
{
    private const float overlapTolerance = 0.9f;

    public static void RebuildGraph(Camera camera)
    {
        var nodes = UnityEngine.Object.FindObjectsByType<Node>(FindObjectsSortMode.None);

        foreach (var n in nodes)
            n.ConnectedNodes.Clear();

        foreach (var A in nodes)
        {
            if (!IsWalkable(A)) continue;
            var flatA = A.Position.Flatten(camera.transform);

            foreach (var B in nodes)
            {
                if (A == B || !IsWalkable(B)) continue;

                var flatB = B.Position.Flatten(camera.transform);
                var delta = B.Position - A.Position;

                float planarDistance = Vector2.Distance(new Vector2(flatA.x, flatA.z), new Vector2(flatB.x, flatB.z));

                if (planarDistance > overlapTolerance || Mathf.Abs(delta.x - delta.z) < 0.01f || !B.Walkable)
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

    private static Vector3 Flatten(this Vector3 worldPos, Transform transform)
    {
        var camPos = transform.position;
        var camForward = transform.forward;
        var toPoint = worldPos - camPos;
        return worldPos - Vector3.Dot(toPoint, camForward) * camForward;
    }
}