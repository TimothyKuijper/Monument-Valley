#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Node Builder/Palette")]
public class NodePalette : ScriptableObject
{
    public List<GameObject> prefabs = new();
}
#endif