public static class StyleChangeExtensions
{
    public static StyleChange Add(this StyleChange change, StyleChange addedType)
    {
        change |= addedType;
        return change;
    }

    public static bool HasChanged(this StyleChange change, StyleChange filterType)
    {
        return (change & filterType) != 0;
    }
}

// Max X for 1 << X is 31

/// <summary>
/// Store all the Styling changes in bits.
/// </summary>
public enum StyleChange
{
    None = 0,

    MainGraph = 1 << 0,
    SubGraph = 1 << 1,
    BothGraph = MainGraph | SubGraph,

    DeskMode = 1 << 2,
    ImmersionMode = 1 << 3,
    BothMode = DeskMode | ImmersionMode,


    Node = 1 << 4,
    Edge = 1 << 5,
    Nodge = Node | Edge,

    Label = 1 << 6,
    Propagation = 1 << 7,

    Color = 1 << 8,
    Size = 1 << 9,
    Visibility = 1 << 10,
    Position = 1 << 11,
    Collider = 1 << 12,
    Selection = 1 << 13,

    All = BothGraph | BothMode | Nodge | Label | Propagation | Color | Size | Visibility | Position | Collider
}