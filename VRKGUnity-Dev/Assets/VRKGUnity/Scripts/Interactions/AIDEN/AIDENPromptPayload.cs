using System.Collections.Generic;

public class AIDENPromptPayload
{
    public int Id;
    public string UserSentence;
    public string AIDENAnswer;

    public List<string> UserSentences = new();
    public List<string> AIDENAnswers = new();

    public List<AIDENIntentPayload> IntentsPayloads = new();


    // Manual Values Detected 
    public int NbSentences = -1;
    public List<AIDENPrompts> PromptTypes = new();
    public List<AIDENIntent> AidenIntents = new();

    public Dictionary<FlowStep, bool> AIDENStateAnswer = new();

    public FlowStep CurentManualStep = FlowStep.None;
    

    public AIDENPromptPayload(int id, string userSentence, string aidenAnswer)
    {
        Id = id;
        UserSentence = userSentence;
        AIDENAnswer = aidenAnswer;
    }

    public AIDENPromptPayload(int id, string userSentence)
    {
        Id = id;
        UserSentence = userSentence;
    }
}