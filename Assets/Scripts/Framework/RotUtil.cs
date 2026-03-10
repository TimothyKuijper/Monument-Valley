using System.Linq;
using UnityEngine;

public static class RotUtil
{
    public static int[] RoundRotations = new int[4] { 0, 90, 180, 270 };

    public const float MaxRotation = 360f;
    public const float HalfRotation = 180f;

    public static float GetNearestRotation(float rotation)
    {
        if (rotation >= MaxRotation) return 0;
        return RoundRotations.OrderBy(x => Mathf.Abs(rotation - x)).First();
    }
}
