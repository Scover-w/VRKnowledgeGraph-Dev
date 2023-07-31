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

    public static Color New(string hexColor)
    {
        if (hexColor.IndexOf('#') != -1)
            hexColor = hexColor.Replace("#", "");

        if (hexColor.Length != 6)
            return Color.white;

        byte r = byte.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        return new Color32(r, g, b, 255);
    }
}
