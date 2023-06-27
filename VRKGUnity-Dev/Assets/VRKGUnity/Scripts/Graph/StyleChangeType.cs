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

    MainGraph = 1 << 0,
    SubGraph = 1 << 1,

    DeskMode = 1 << 2,
    ImmersionMode = 1 << 3,

    Node = 1 << 4,
    Edge = 1 << 5,
    Label = 1 << 6,
    Propagation = 1 << 7,

    Color = 1 << 8,
    Size = 1 << 9,
    Visibility = 1 << 10,
    Position = 1 << 11,
    Collider = 1 << 12,

    All = MainGraph | SubGraph | DeskMode | ImmersionMode | Node | Edge | Label | Propagation | Color | Size | Visibility | Position | Collider
}