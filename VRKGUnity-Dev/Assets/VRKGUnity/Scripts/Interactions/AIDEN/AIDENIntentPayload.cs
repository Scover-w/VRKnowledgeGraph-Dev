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