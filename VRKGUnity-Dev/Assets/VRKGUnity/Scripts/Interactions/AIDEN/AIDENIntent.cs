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

    public string ValueString { get; private set; }
    public float ValueFloat { get; private set; }
    public bool ValueBoolean { get; private set; }
    public Color ValueColor { get; private set; }


    public string OldValueString { get; private set; }
    public float OldValueFloat { get; private set; }
    public bool OldValueBoolean { get; private set; }
    public Color OldValueColor { get; private set; }

    public AIDENValueType ValueType { get; private set; }


    public AIDENIntent(GraphConfigKey graphConfigKey, string newString, string oldString)
    {
        IsGraphConfig = true;
        GraphConfigKey = graphConfigKey;
        ValueString = newString;
        OldValueString = oldString;

        ValueType = AIDENValueType.String;
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

            switch (ValueType)
            {
                case AIDENValueType.String:
                    return ValueString == aidenIntent.ValueString;
                case AIDENValueType.Float:
                    return ValueFloat == aidenIntent.ValueFloat;
                case AIDENValueType.Boolean:
                    return ValueBoolean == aidenIntent.ValueBoolean;
                case AIDENValueType.Color:
                    return ValueColor == aidenIntent.ValueColor;
                default: 
                    return false;
            }
        }

        return GraphActionKey == aidenIntent.GraphActionKey;
    }

    public override string ToString()
    {
        if(IsGraphConfig)
        {
            switch(ValueType)
            {
                case AIDENValueType.String:
                    return GraphConfigKey + " " + ValueString;
                case AIDENValueType.Float:
                    return GraphConfigKey + " " + ValueFloat;
                case AIDENValueType.Boolean:
                    return GraphConfigKey + "  " + ValueBoolean;
                case AIDENValueType.Color:
                    return GraphConfigKey + " " + ValueColor;
                default:
                    return GraphConfigKey + " " + ValueString;
            }
        }
        else
        {
            return GraphActionKey.ToString();
        }
    }
}
