using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

public class Node : MonoBehaviour
{
    public Direction CurrentDirection = Direction.UP;
    public List<Node> ConnectedNodes = new();
    public bool Occupied;

    [SerializeField] private bool _walkable = true;
    public bool Walkable
    {
        get => _walkable;
        set
        {
            _walkable = value;
            onChangeWalkable.Invoke(value);
        }
    }

    public UnityEvent onEnter;
    public UnityEvent onExit;
    public UnityEvent onRebuild;
    public UnityEvent<bool> onChangeWalkable;


    public Vector3 Position
    {
        get
        {
            var pos = CurrentDirection switch
            {
                Direction.UP => verticalOffset * Vector3.up,
                Direction.DOWN => verticalOffset * Vector3.down,
                Direction.LEFT => verticalOffset * Vector3.left,
                Direction.RIGHT => verticalOffset * Vector3.right,
                Direction.FORWARD => verticalOffset * Vector3.forward,
                Direction.BACKWARD => verticalOffset * -Vector3.forward,
                _ => throw new ArgumentOutOfRangeException()
            };

            return transform.position + pos;
        }
    }

    [SerializeField] private float verticalOffset = 0.8f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Walkable && !Occupied ? Color.green : Color.red;
        Gizmos.DrawSphere(Position, 0.2f);

        if (ConnectedNodes.Count == 0) return;
        foreach (var neighbour in ConnectedNodes)
        {
            if (!neighbour) continue;
            Debug.DrawLine(Position, neighbour.Position, Color.white, 0.01f);
        }
    }
}