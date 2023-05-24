using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExtensions
{
    public  static Color ToUnityColor(this System.Drawing.Color systemColor)
    {
        return new Color(
            systemColor.R / 255f,
            systemColor.G / 255f,
            systemColor.B / 255f,
            systemColor.A / 255f
        );
    }

    public static System.Drawing.Color ToSystemColor(this Color unityColor)
    {
        return System.Drawing.Color.FromArgb(
            Mathf.RoundToInt(unityColor.a * 255f),
            Mathf.RoundToInt(unityColor.r * 255f),
            Mathf.RoundToInt(unityColor.g * 255f),
            Mathf.RoundToInt(unityColor.b * 255f)
        );
    }
}
