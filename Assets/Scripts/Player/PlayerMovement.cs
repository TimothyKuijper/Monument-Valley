using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private NodeWalker nodeWalker;
    
    private Camera _camera;

    private void Start()
    {
        _camera = FindAnyObjectByType<Camera>();
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        var target = NodeUtils.RaycastNode(_camera);
        if (target == null) return;

        NodeBank.RebuildGraph(_camera);
        nodeWalker.MoveTo(target);
    }
}
