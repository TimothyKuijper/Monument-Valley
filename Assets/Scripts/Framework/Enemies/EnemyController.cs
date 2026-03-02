using System;
using System.Collections;
using UnityEngine;

namespace Framework.Enemies
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private NodePath path;
        [SerializeField] private NodeWalker nodeWalker;
        
        public Node _target;
    
        private Camera _camera;
        private Node _tempNode;

        private void Start()
        {
            nodeWalker.OnPathComplete.AddListener(ChangeTarget);
            ChangeTarget();
        }

        private void ChangeTarget(Node node = null)
        {
            _target = _target == path.endNode ? path.startNode : path.endNode;
            _tempNode = node;
            if (node != null && node.Occupied)
            {
                return;
            }
            nodeWalker.MoveTo(_target);
        }
    }
}