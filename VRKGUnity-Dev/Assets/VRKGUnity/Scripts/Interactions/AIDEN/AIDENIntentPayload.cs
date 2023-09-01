using System.Collections.Generic;

public class AIDENIntentPayload
{
    public AIDENPrompts Type;
    public string UserIntentSentence;
    public string AIDENAnswer;

    public AIDENIntentPayload(string userIntentSentence, string aidenAnswer)
    {
        UserIntentSentence = userIntentSentence;
        AIDENAnswer = aidenAnswer;
    }
}

public class SplitSentencesPayload
{
    public int Count { get { return Sentences.Count; } }

    public List<string> Sentences = new();

    public void Add(string splitSentence)
    {
        Sentences.Add(splitSentence);
    }

}

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
