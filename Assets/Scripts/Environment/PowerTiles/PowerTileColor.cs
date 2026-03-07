using System;
using UnityEngine;

public class PowerTileColor : MonoBehaviour
{
    private Material _material => GetComponentInChildren<Renderer>().material;
    private const string propertyName = "_EmissionPatternMultiplier";

    public void OnPowered(bool powered)
    {
        _material.SetFloat(propertyName, powered ? 1 : 0);
    }
}
