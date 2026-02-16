using UnityEngine;

namespace Framework.Enemies
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private NodePath path;
        [SerializeField] private NodeWalker nodeWalker;
    
        private Camera _camera;
        private Node _target;

        private void Start()
        {
            nodeWalker.OnPathComplete.AddListener(ChangeTarget);
            ChangeTarget();
        }

        private void ChangeTarget()
        {
            _target = _target == path.endNode ? path.startNode : path.endNode;
            nodeWalker.MoveTo(_target);
        }
    }
}