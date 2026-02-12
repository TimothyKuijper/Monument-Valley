using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Node : MonoBehaviour
{
    public enum Directions
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        FORWARD,
        BACKWARD,
    }

    [SerializeField] private Directions _currentDirection = Directions.UP;
    [SerializeField] private float blockSize = 1;

    public Node ConnectedNode;
    public Vector3 Position => GetPosition();


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        var pos = GetPosition();
        Gizmos.DrawSphere(pos, blockSize / 4);
    }

    public Vector3 GetPosition()
    {
        var pos = _currentDirection switch
        {
            Directions.UP => blockSize * Vector3.up,
            Directions.DOWN => blockSize * Vector3.down,
            Directions.LEFT => blockSize * Vector3.left,
            Directions.RIGHT => blockSize * Vector3.right,
            Directions.FORWARD => blockSize * Vector3.forward,
            Directions.BACKWARD => blockSize * -Vector3.forward,
        };

        return transform.position + pos;
    }
}