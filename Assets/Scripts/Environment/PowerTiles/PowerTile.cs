using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Node))]
public class PowerTile : MonoBehaviour
{
    [SerializeField] private bool startEmitter;

    private bool _isEmitter;
    public bool IsEmitter
    {
        get => _isEmitter;
        set
        {
            _isEmitter = value;
            UpdatePowerTiles(value);
        }
    }

    public bool IsPowered = false;

    private Node _node;



    private void Awake()
    {
        _node = GetComponent<Node>();
        IsPowered = false;
    }

    private void Start()
    {
        _node.onChangeWalkable.AddListener((value) => IsPowered = false);

        if (startEmitter == false) return;
        _node.Occupied = true;
        IsEmitter = true;

        _node.onRebuild.AddListener(() => UpdatePowerTiles(true));
    }


    public void UpdatePowerTiles(bool newIsPowered, List<Node> checkedNodes = null)
    {
        IsPowered = newIsPowered;

        var connectedNodes = _node.ConnectedNodes;
        foreach (var adjacentNode in connectedNodes)
        {
            if (checkedNodes != null && checkedNodes.Contains(adjacentNode)) continue;

            PowerTile tile;
            if (adjacentNode.TryGetComponent<PowerTile>(out tile))
            {
                if (checkedNodes != null) checkedNodes = new List<Node>(checkedNodes);
                else checkedNodes = new List<Node>();

                checkedNodes.Add(adjacentNode);
                tile.UpdatePowerTiles(newIsPowered, checkedNodes);
            }
        }
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = IsPowered ? Color.teal : Color.red;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
}
