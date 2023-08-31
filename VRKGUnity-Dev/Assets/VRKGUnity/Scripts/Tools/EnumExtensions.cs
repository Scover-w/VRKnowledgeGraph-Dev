using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnumExtensions
{
    public static T ToEnum<T>(this string enumString) where T : struct, Enum
    {
        if (Enum.TryParse(enumString, true, out T result))

            return result;

        Debug.LogWarning($"StringToEnum<{typeof(T).Name}> couldn't parse the string: {enumString}");

        return default;
    }

    public static bool TryParseToEnum<T>(this string enumString, out T detectedEnum) where T : struct, Enum
    {
        if (Enum.TryParse(enumString, true, out detectedEnum))
            return true;

        Debug.LogWarning($"StringToEnum<{typeof(T).Name}> couldn't parse the string: {enumString}");

        return false;
    }
}