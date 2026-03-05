using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Yakanashe.Yautl;

public class NodeWalker : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.3f;
    [SerializeField] private bool rebuildOnStart;

    private Node _currentNode;
    private Camera _camera;
    private Coroutine _moveRoutine;

    public Vector3 Direction = new Vector3();

    public UnityEvent OnStartMoving = new();
    public UnityEvent OnEnter = new();
    public UnityEvent OnExit = new();
    public UnityEvent<Node> OnPathComplete = new();
    
    private void Awake()
    {
        _camera = FindAnyObjectByType<Camera>();
        _currentNode = transform.FindClosestNode();
        transform.position = _currentNode.Position;
    }


    private void Start()
    {
        if (rebuildOnStart) StartCoroutine(DelayStart());
    }

    private IEnumerator DelayStart() // Build graph when scene is fully loaded
    {
        yield return 0;
        NodeBank.RebuildGraph(_camera);
    }


    public void MoveTo(Node destination)
    {
        _currentNode.Occupied = false;

        NodeBank.RebuildGraph(_camera);

        var path = NodeUtils.BFS(_currentNode, destination);
        if (path == null) return;
        OnStartMoving.Invoke();

        if (_moveRoutine != null) StopCoroutine(_moveRoutine);
        TweenRunner.Instance.KillAllFrom(transform);
        _moveRoutine = StartCoroutine(MovePath(path));
    }

    private IEnumerator MovePath(List<Node> path)
    {
        for (var index = 0; index < path.Count; index++)
        {
            var node = path[index];
            var currentY = transform.position.y;
            var targetY = node.Position.y;
            
            if (!_currentNode.CanReach(node, _camera)) break;
            
            if (node != _currentNode) Direction = _currentNode.Position - node.Position; // Do not change direction to current node

            transform.parent = node.transform;
            OnExit.Invoke();
            _currentNode.Occupied = false;
            _currentNode.onExit.Invoke();
            _currentNode.onChangeWalkable.RemoveListener(ResetMovement);

            OnEnter.Invoke();
            _currentNode = node;
            _currentNode.Occupied = true;
            _currentNode.onEnter.Invoke();
            _currentNode.onChangeWalkable.AddListener(ResetMovement);

            if (Mathf.Abs(currentY - targetY) > 0.01f)
            {
                var currentScreen = _camera.WorldToViewportPoint(transform.position);
                var targetScreen = _camera.WorldToViewportPoint(node.Position);
                (targetScreen.z, currentScreen.z) = (currentScreen.z, targetScreen.z);

                var projected = _camera.ViewportToWorldPoint(targetY > currentY ? currentScreen : targetScreen);
                if (targetY > currentY) transform.position = projected;
                transform.MoveTo(targetY < currentY ? projected : node.Position, moveSpeed, EaseType.Linear);
                yield return new WaitForSeconds(moveSpeed);
                if (targetY < currentY) transform.position = node.Position;
            }
            else
            {
                transform.MoveTo(node.Position, moveSpeed, EaseType.Linear);
                yield return new WaitForSeconds(moveSpeed);
                transform.position = node.Position;
            }
        }

        var nextNodeIndex = path.IndexOf(_currentNode) + 1;
        if (nextNodeIndex > path.Count - 1)
        {
            OnPathComplete.Invoke(path[path.IndexOf(_currentNode) - 1]);
        }
        else
        {
            OnPathComplete.Invoke(path[nextNodeIndex]);
        }
    }



    public void DirectSetNode(Node newNode)
    {
        _currentNode = newNode;
        transform.position = newNode.Position;
        transform.parent = newNode.transform;
    }



    private void ResetMovement(bool isWalkable)
    {
        if (isWalkable) return;

        if (_moveRoutine != null) StopCoroutine(_moveRoutine);
        TweenRunner.Instance.KillAllFrom(transform);
        transform.position = _currentNode.Position;
        OnPathComplete.Invoke(_currentNode);
    }
}