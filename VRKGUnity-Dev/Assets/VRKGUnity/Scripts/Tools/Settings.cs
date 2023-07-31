using UnityEngine;

public class Settings
{
    public static readonly Vector2 BASE_SIZE_LABEL_CANVAS = new(450, 150);
    public static readonly float BASE_FONT_SIZE_LABEL = 36f;

#if PLATFORM_ANDROID && !UNITY_EDITOR // Android Standalone
    public static readonly float MIN_READABLE_FONT_SIZE = 10f;
#else // Unity Editor
    public static readonly float MIN_READABLE_FONT_SIZE = 10f;
#endif

    public static readonly GraphMode DEFAULT_GRAPH_MODE = GraphMode.Desk;


    public static readonly float TOUCH_RADIUS = .05f;


}
