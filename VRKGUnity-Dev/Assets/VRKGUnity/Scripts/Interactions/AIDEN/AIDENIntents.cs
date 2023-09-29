using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Store the differents <see cref="AIDENIntent"/> detected by <see cref="AIDENController"/> 
/// </summary>
public class AIDENIntents
{
    public List<AIDENIntent> Intents;


    public AIDENIntents()
    {
        Intents = new();
    }


    public void Add(AIDENIntent intent)
    {
        Intents.Add(intent);
    }

    public void Order()
    {
        Intents = Intents
        .OrderBy(intent => !intent.IsGraphConfig)
        .ThenBy(intent => intent.IsGraphConfig ? (int)intent.GraphConfigKey : (int)intent.GraphActionKey)
        .ToList();
    }
}
