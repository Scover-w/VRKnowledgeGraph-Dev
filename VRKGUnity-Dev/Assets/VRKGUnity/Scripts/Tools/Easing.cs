using System;
using UnityEngine;


public delegate float EasingDel(float t);

public class Easing
{

    // https://easings.net/fr

    public static EasingDel GetEasing(EasingType easingType)
    {
        switch (easingType)
        {
            case EasingType.EaseInSine:
                return EaseInSine;
            case EasingType.EaseOutSine:
                return EaseOutSine;
            case EasingType.EaseInOutSine:
                return EaseInOutSine;
            case EasingType.EaseInQuad:
                return EaseInQuad;
            case EasingType.EaseOutQuad:
                return EaseOutQuad;
            case EasingType.EaseInOutQuad:
                return EaseInOutQuad;
            case EasingType.EaseInCubic:
                return EaseInCubic;
            case EasingType.EaseOutCubic:
                return EaseOutCubic;
            case EasingType.EaseInOutCubic:
                return EaseInOutCubic;
            case EasingType.EaseInQuart:
                return EaseInQuart;
            case EasingType.EaseOutQuart:
                return EaseOutQuart;
            case EasingType.EaseInOutQuart:
                return EaseInOutQuart;
            case EasingType.EaseInQuint:
                return EaseInQuint;
            case EasingType.EaseOutQuint:
                return EaseOutQuint;
            case EasingType.EaseInOutQuint:
                return EaseInOutQuint;
            case EasingType.EaseInExpo:
                return EaseInExpo;
            case EasingType.EaseOutExpo:
                return EaseOutExpo;
            case EasingType.EaseInOutExpo:
                return EaseInOutExpo;
            case EasingType.EaseInCirc:
                return EaseInCirc;
            case EasingType.EaseOutCirc:
                return EaseOutCirc;
            case EasingType.EaseInOutCirc:
                return EaseInOutCirc;
            case EasingType.EaseInBack:
                return EaseInBack;
            case EasingType.EaseOutBack:
                return EaseOutBack;
            case EasingType.EaseInOutBack:
                return EaseInOutBack;
            case EasingType.EaseInElastic:
                return EaseInElastic;
            case EasingType.EaseOutElastic:
                return EaseOutElastic;
            case EasingType.EaseInOutElastic:
                return EaseInOutElastic;
            case EasingType.EaseInBounce:
                return EaseInBounce;
            case EasingType.EaseOutBounce:
                return EaseOutBounce;
            case EasingType.EaseInOutBounce:
                return EaseInOutBounce;
            default:
                throw new ArgumentException("Invalid easing type.");
        }
    }

    public static float EaseInSine(float t)
    {
        return 1f - Mathf.Cos((t * Mathf.PI) / 2f);
    }


    public static float EaseOutSine(float t)
    {
        return Mathf.Sin((t * Mathf.PI) / 2f);
    }

    public static float EaseInOutSine(float t)
    {
        return -(Mathf.Cos(Mathf.PI * t) - 1) / 2f;
    }

    public static float EaseInQuad(float t)
    {
        return t * t;
    }

    public static float EaseOutQuad(float t)
    {
        return 1f - (1f - t) * (1f - t);
    }

    public static float EaseInOutQuad(float t)
    {
        return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
    }

    public static float EaseInOutQuart(float t)
    {
        return t < 0.5f ? 8f * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 4f) / 2f;
    }

    public static float EaseOutQuart(float t)
    {
        return 1f - Mathf.Pow(1f - t, 4f);
    }

    public static float EaseInQuart(float t)
    {
        return t * t * t * t;
    }

    public static float EaseInOutCubic(float t)
    {
        return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
    }

    public static float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }

    public static float EaseInCubic(float t)
    {
        return t * t * t;
    }

    public static float EaseInQuint(float t)
    {
        return t * t * t * t * t;
    }

    public static float EaseOutQuint(float t)
    {
        return 1f - Mathf.Pow(1f - t, 5f);
    }

    public static float EaseInOutQuint(float t)
    {
        return t < 0.5f ? 16f * t * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 5f) / 2f;
    }

    public static float EaseInExpo(float t)
    {
        return t == 0f ? 0f : Mathf.Pow(2f, 10f * t - 10f);
    }

    public static float EaseOutExpo(float t)
    {
        return t == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * t);
    }


    public static float EaseInOutExpo(float t)
    {
        if (t == 0f)
            return 0f;
        else if (t == 1f)
            return 1f;
        else if (t < 0.5f)
            return Mathf.Pow(2f, 20f * t - 10f) / 2f;
        else
            return (2f - Mathf.Pow(2f, -20f * t + 10f)) / 2f;
    }

    public static float EaseInOutBack(float t)
    {
        float c1 = 1.70158f;
        float c2 = c1 * 1.525f;

        if (t < 0.5f)
        {
            return (Mathf.Pow(2f * t, 2f) * ((c2 + 1f) * 2f * t - c2)) / 2f;
        }
        else
        {
            return (Mathf.Pow(2f * t - 2f, 2f) * ((c2 + 1f) * (2f * t - 2f) + c2) + 2f) / 2f;
        }
    }

    public static float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;

        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }

    public static float EaseInBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;

        return c3 * t * t * t - c1 * t * t;
    }

    public static float EaseInOutCirc(float t)
    {
        return t < 0.5f
            ? (1f - Mathf.Sqrt(1f - Mathf.Pow(2f * t, 2f))) / 2f
            : (Mathf.Sqrt(1f - Mathf.Pow(-2f * t + 2f, 2f)) + 1f) / 2f;
    }

    public static float EaseOutCirc(float t)
    {
        return Mathf.Sqrt(1f - Mathf.Pow(t - 1f, 2f));
    }

    public static float EaseInCirc(float t)
    {
        return 1f - Mathf.Sqrt(1f - Mathf.Pow(t, 2f));
    }


    public static float EaseInElastic(float t)
    {
        float c4 = (2f * Mathf.PI) / 3f;

        if (t == 0f)
            return 0f;
        else if (t == 1f)
            return 1f;
        else
            return -Mathf.Pow(2f, 10f * t - 10f) * Mathf.Sin((t * 10f - 10.75f) * c4);
    }

    public static float EaseOutElastic(float t)
    {
        float c4 = (2f * Mathf.PI) / 3f;

        if (t == 0f)
            return 0f;
        else if (t == 1f)
            return 1f;
        else
            return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * c4) + 1f;
    }

    public static float EaseInOutElastic(float t)
    {
        float c5 = (2f * Mathf.PI) / 4.5f;

        if (t == 0f)
            return 0f;
        else if (t == 1f)
            return 1f;
        else if (t < 0.5f)
            return -(Mathf.Pow(2f, 20f * t - 10f) * Mathf.Sin((20f * t - 11.125f) * c5)) / 2f;
        else
            return (Mathf.Pow(2f, -20f * t + 10f) * Mathf.Sin((20f * t - 11.125f) * c5)) / 2f + 1f;
    }

    public static float EaseOutBounce(float t)
    {
        const float n1 = 7.5625f;
        const float d1 = 2.75f;

        if (t < 1f / d1)
        {
            return n1 * t * t;
        }
        else if (t < 2f / d1)
        {
            return n1 * (t -= 1.5f / d1) * t + 0.75f;
        }
        else if (t < 2.5f / d1)
        {
            return n1 * (t -= 2.25f / d1) * t + 0.9375f;
        }
        else
        {
            return n1 * (t -= 2.625f / d1) * t + 0.984375f;
        }
    }


    public static float EaseInBounce(float t)
    {
        return 1f - EaseOutBounce(1f - t);
    }

    public static float EaseInOutBounce(float t)
    {
        return t < 0.5f
            ? (1f - EaseOutBounce(1f - 2f * t)) / 2f
            : (1f + EaseOutBounce(2f * t - 1f)) / 2f;
    }


}

public enum EasingType
{
    EaseInSine,
    EaseOutSine,
    EaseInOutSine,
    EaseInQuad,
    EaseOutQuad,
    EaseInOutQuad,
    EaseInCubic,
    EaseOutCubic,
    EaseInOutCubic,
    EaseInQuart,
    EaseOutQuart,
    EaseInOutQuart,
    EaseInQuint,
    EaseOutQuint,
    EaseInOutQuint,
    EaseInExpo,
    EaseOutExpo,
    EaseInOutExpo,
    EaseInCirc,
    EaseOutCirc,
    EaseInOutCirc,
    EaseInBack,
    EaseOutBack,
    EaseInOutBack,
    EaseInElastic,
    EaseOutElastic,
    EaseInOutElastic,
    EaseInBounce,
    EaseOutBounce,
    EaseInOutBounce
}
