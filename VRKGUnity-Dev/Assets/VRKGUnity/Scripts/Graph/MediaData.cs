using Newtonsoft.Json;
using System;
using UnityEngine;

[Serializable]
public struct MediaData
{
    [JsonIgnore]
    public Vector2 Size
    { 
        get 
        { 
            return _size.ToVector2(); 
        }  

        set
        {
            _size = new JsonVector2(value);
        }
    }
 
    public MediaState State;
    public TextureFormat Format;

    [JsonProperty("Size_")]
    public JsonVector2 _size;

    public MediaData(MediaState state)
    {
        State = state;
        _size = new JsonVector2(Vector2.zero);
        Format = TextureFormat.Alpha8;
    }

    public MediaData(MediaState state, Vector2 size, TextureFormat format)
    {
        State = state;
        _size = new JsonVector2(size);
        Format = format;
    }
}

[Serializable]
public enum MediaState
{
    None,
    Loadable,
    Unloadable
}

public struct JsonVector2
{
    public float x;
    public float y;

    public JsonVector2(Vector2 vector2)
    {
        x = vector2.x;
        y = vector2.y;
    }


    public Vector2 ToVector2()
    {
        return new Vector2(x, y);
    }

}