using UnityEngine;

public class AIDENIntent
{
    public bool IsGraphConfig { get; private set; }

    public int IdKey
    {
        get
        {
            int idKey = 0;
            if (IsGraphConfig)
                idKey = (int)GraphConfigKey;
            else
                idKey = (int)GraphActionKey + 1000;
            return idKey;
        }
    }

    public GraphConfigKey GraphConfigKey { get; private set; }
    public GraphActionKey GraphActionKey { get; private set; }

    public int ValueInt { get; private set; }
    public float ValueFloat { get; private set; }
    public bool ValueBoolean { get; private set; }
    public Color ValueColor { get; private set; }


    public int OldValueInt { get; private set; }
    public float OldValueFloat { get; private set; }
    public bool OldValueBoolean { get; private set; }
    public Color OldValueColor { get; private set; }

    public AIDENValueType ValueType { get; private set; }


    public AIDENIntent(GraphConfigKey graphConfigKey, int newInt, int oldInt)
    {
        IsGraphConfig = true;
        GraphConfigKey = graphConfigKey;
        ValueInt = newInt;
        OldValueInt = oldInt;

        ValueType = AIDENValueType.Int;
    }
    public AIDENIntent(GraphConfigKey graphConfigKey, float newFloat, float oldFloat)
    {
        IsGraphConfig = true;
        GraphConfigKey = graphConfigKey;
        ValueFloat = newFloat;
        OldValueFloat = oldFloat;

        ValueType = AIDENValueType.Float;
    }
    public AIDENIntent(GraphConfigKey graphConfigKey, bool newBoolean, bool oldBoolean)
    {
        IsGraphConfig = true;
        GraphConfigKey = graphConfigKey;
        ValueBoolean = newBoolean;
        OldValueBoolean = oldBoolean;

        ValueType = AIDENValueType.Boolean;
    }
    public AIDENIntent(GraphConfigKey graphConfigKey, Color newColor, Color oldColor)
    {
        IsGraphConfig = true;
        GraphConfigKey = graphConfigKey;
        ValueColor = newColor;
        OldValueColor = oldColor;

        ValueType = AIDENValueType.Color;
    }

    public AIDENIntent(GraphActionKey graphConfigKey)
    {
        IsGraphConfig = false;
        GraphActionKey = graphConfigKey;
    }

    public bool AreSame(AIDENIntent aidenIntent)
    {
        if (IsGraphConfig != aidenIntent.IsGraphConfig)
            return false;


        if(IsGraphConfig)
        {
            if(GraphConfigKey != aidenIntent.GraphConfigKey)
                return false;

            return ValueType switch
            {
                AIDENValueType.Int => ValueInt == aidenIntent.ValueInt,
                AIDENValueType.Float => ValueFloat == aidenIntent.ValueFloat,
                AIDENValueType.Boolean => ValueBoolean == aidenIntent.ValueBoolean,
                AIDENValueType.Color => ValueColor == aidenIntent.ValueColor,
                _ => false,
            };
        }

        return GraphActionKey == aidenIntent.GraphActionKey;
    }

    public override string ToString()
    {
        if(IsGraphConfig)
        {
            return ValueType switch
            {
                AIDENValueType.Int => GraphConfigKey + " " + ValueInt,
                AIDENValueType.Float => GraphConfigKey + " " + ValueFloat,
                AIDENValueType.Boolean => GraphConfigKey + "  " + ValueBoolean,
                AIDENValueType.Color => GraphConfigKey + " " + ValueColor,
                _ => GraphConfigKey + " " + ValueInt,
            };
        }
        else
        {
            return GraphActionKey.ToString();
        }
    }
}
