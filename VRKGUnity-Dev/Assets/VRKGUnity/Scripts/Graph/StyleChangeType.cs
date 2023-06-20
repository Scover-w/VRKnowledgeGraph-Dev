using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct StyleChange
{
    private StyleChangeType _changes;

    public StyleChange Add(StyleChangeType type)
    {
        _changes |= type;
        return this;
    }

    public bool HasChanged(StyleChangeType type)
    {
        return (_changes & type) != 0;
    }
}


public enum StyleChangeType
{
    None = 0,

    MegaGraph = 1 << 0,
    MiniGraph = 1 << 1,

    Node = 1 << 2,
    Edge = 1 << 3,

    Color = 1 << 4,
    Size = 1 << 5,
    Position = 1 << 6,
    Collider = 1 << 7,

    All = MegaGraph | MiniGraph | Node | Edge | Color | Size | Position | Collider
}