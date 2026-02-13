using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yakanashe.Yautl;

public class NodeWalker : MonoBehaviour
{
    public float moveSpeed = 0.2f;

    private Node _currentNode;
    private Camera _camera;

    private void Start()
    {
        _camera = FindAnyObjectByType<Camera>();
        _currentNode = transform.FindClosestNode();
        transform.position = _currentNode.Position;
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        var target = NodeUtils.RaycastNode(_camera);
        if (target == null || target == _currentNode) return;

        NodeBank.RebuildGraph(_camera);

        var path = NodeUtils.BFS(_currentNode, target);
        if (path == null) return;

        StartCoroutine(MovePath(path));
    }


    private IEnumerator MovePath(List<Node> path)
    {
        foreach (var node in path)
        {
            transform.MoveTo(node.Position, moveSpeed, EaseType.Linear);
            yield return new WaitForSeconds(moveSpeed);
            _currentNode = node;
        }
    }
}