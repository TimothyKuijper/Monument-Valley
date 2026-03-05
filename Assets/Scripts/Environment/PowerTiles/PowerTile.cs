using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
[RequireComponent(typeof(Node))]
public class PowerTile : MonoBehaviour
{
    [SerializeField][Tooltip("Start as an emitter of power that activates other tiles")] private bool startEmitter;

    private enum OccupyMode
    {
        Off,
        OnPower,
        OffPower,
        Always,
        Never
    }
    [SerializeField][Tooltip("If should set occupied on power")] private OccupyMode occupyMode = OccupyMode.Off;

    private bool _isEmitter;
    public bool IsEmitter
    {
        get => _isEmitter;
        set
        {
            _isEmitter = value;
            UpdatePowerTiles(value);

            if (_node == null) return;
            if (value == true)
            {
                _node.onRebuild.AddListener(() => UpdatePowerTiles(true));
                return;
            }
            _node.onRebuild.RemoveListener(() => UpdatePowerTiles(true));
        }
    }

    private bool _isPowered = false;
    public bool IsPowered
    {
        get => _isPowered;
        set
        {
            _isPowered = value;
            onPowered.Invoke(value);

            if (_node == null) return;
            switch (occupyMode)
            {
                case OccupyMode.Off: break;
                case OccupyMode.OnPower:
                    if (value) _node.SetChildrenToAdjacentNode();
                    _node.Occupied = value;
                    break;
                case OccupyMode.OffPower:
                    if (!value) _node.SetChildrenToAdjacentNode();
                    _node.Occupied = !value;
                    break;
                case OccupyMode.Always:
                    _node.SetChildrenToAdjacentNode();
                    _node.Occupied = true;
                    break;
                case OccupyMode.Never:
                    _node.Occupied = false;
                    break;
            }
        }
    }
    public UnityEvent<bool> onPowered;

    private Node _node;



    private void Awake()
    {
        _node = GetComponent<Node>();
        IsPowered = false;
    }

    private void Start()
    {
        _node.onChangeWalkable.AddListener((value) => UpdatePowerTiles(false));
        IsEmitter = startEmitter;
    }


    public void UpdatePowerTiles(bool newIsPowered, List<Node> checkedNodes = null)
    {
        if (newIsPowered == false && IsEmitter) return;
        if (checkedNodes != null && checkedNodes.Contains(_node)) return;
        IsPowered = newIsPowered;

        if (checkedNodes != null) checkedNodes = new List<Node>(checkedNodes);
        else checkedNodes = new List<Node>();
        checkedNodes.Add(_node);

        foreach (var adjacentNode in _node.ConnectedNodes)
        {
            PowerTile tile;
            if (adjacentNode.TryGetComponent<PowerTile>(out tile)) tile.UpdatePowerTiles(newIsPowered, checkedNodes);
        }
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = IsPowered ? Color.teal : Color.red;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
}
