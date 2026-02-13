using System.Collections;
using Framework.Enemies;
using UnityEngine;
using Yakanashe.Yautl;

public class EnemyNodeWalker : MonoBehaviour
{
    public float moveSpeed = 0.2f;
    public NodePath nodePath;

    private Node _currentNode;
    private Coroutine _moveRoutine;

    private void Start()
    {
        if (nodePath == null || nodePath.startNode == null || nodePath.endNode == null)
        {
            Debug.LogWarning("EnemyNodeWalker missing NodePath or nodes.");
            enabled = false;
            return;
        }

        _currentNode = nodePath.startNode;
        transform.position = _currentNode.Position;

        _moveRoutine = StartCoroutine(Patrol());
    }

    private IEnumerator Patrol()
    {
        while (true)
        {
            Node target = (_currentNode == nodePath.startNode)
                ? nodePath.endNode
                : nodePath.startNode;

            yield return MoveToNode(target);

            _currentNode = target;
        }
    }

    private IEnumerator MoveToNode(Node target)
    {
        transform.MoveTo(target.Position, moveSpeed, EaseType.Linear);
        yield return new WaitForSeconds(moveSpeed);
    }
}