using UnityEngine;

namespace Environment.Enemies
{
    [ExecuteAlways]
    public class SnapToNode : MonoBehaviour
    {
        public float snapDistance = 0.5f;
        public float offset = 0.5f;

        private Transform currentNode;

        void Update()
        {
            if (!transform.hasChanged)
                return;

            transform.hasChanged = false;

            var nodes = NodeBank.sceneNodes;
            if (nodes == null || nodes.Count == 0)
                return;

            float snapDistanceSqr = snapDistance * snapDistance;
            Vector3 myPos = transform.position;

            Transform closestNode = null;
            float closestDist = float.MaxValue;

            foreach (var node in nodes)
            {
                Vector3 nodePos = node.Position;
                float dist = (nodePos - myPos).sqrMagnitude;

                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestNode = node.gameObject.transform;
                }
            }

            if (currentNode != null)
            {
                float distFromCurrent = (currentNode.position - myPos).sqrMagnitude;

                if (distFromCurrent > snapDistanceSqr)
                {
                    currentNode = null;
                }
            }

            if (currentNode == null && closestNode != null && closestDist <= snapDistanceSqr)
            {
                currentNode = closestNode;
                transform.position = currentNode.position;
                var position = transform.position;
                position.y += offset;
                transform.position = position;
            }
            else if (currentNode != null)
            {
                transform.position = currentNode.position;
            }
        }
    }
}