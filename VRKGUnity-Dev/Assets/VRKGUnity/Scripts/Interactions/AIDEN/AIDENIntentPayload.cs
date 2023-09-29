using System.Collections.Generic;


/// <summary>
/// Payload used by <see cref="SplitSentencesPayload"/> to pass data between functions
/// </summary>
public class SplitSentencesPayload
{
    public int Count { get { return Sentences.Count; } }

    public List<string> Sentences = new();

    public void Add(string splitSentence)
    {
        Sentences.Add(splitSentence);
    }

}

/// <summary>
/// Payload used by <see cref="SplitSentencesPayload"/> to pass data between functions
/// </summary>
public class DetectedTypePayload
{
    public int Count { get { return TypeAndSentence.Count; } }
    public List<TypeAndSentence> TypeAndSentence = new();

    public void Add(TypeAndSentence promptAndSentence)
    {
        TypeAndSentence.Add(promptAndSentence);
    }

    public void Add(AIDENPrompts type, string userIntentSentence)
    {
        TypeAndSentence.Add(new(type, userIntentSentence));
    }
}

/// <summary>
/// Payload used by <see cref="SplitSentencesPayload"/> to pass data between functions
/// </summary>
public class TypeAndSentence
{
    public AIDENPrompts Type;
    public string UserIntentSentence;

    public TypeAndSentence(AIDENPrompts type, string userIntentSentence)
    {
        Type = type;
        UserIntentSentence = userIntentSentence;
    }

}
