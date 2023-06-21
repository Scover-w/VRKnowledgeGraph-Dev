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

    public static Color Lighten(this Color color, float amount)
    {
        Color lightenedColor = new Color(
                                Mathf.Clamp(color.r + amount, 0f, 1f),
                                Mathf.Clamp(color.g + amount, 0f, 1f),
                                Mathf.Clamp(color.b + amount, 0f, 1f),
                                color.a);
        return lightenedColor;
    }
}
