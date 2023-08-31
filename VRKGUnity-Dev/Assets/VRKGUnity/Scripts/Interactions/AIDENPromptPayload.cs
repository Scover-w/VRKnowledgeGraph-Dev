public class AIDENPromptPayload
{
    public AIDENPrompts Type { get; private set; }
    public string Content;
    
    public AIDENPromptPayload(AIDENPrompts type, string content)
    {
        Type = type;
        Content = content;
    }
}