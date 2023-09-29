/// <summary>
/// Used to pass data between threads for the AIDEN (voice interface/chatgpt) feature
/// </summary>
public class RawPayload
{
    public int Id { get; private set; }

    public object Payload { get; private set; }

    public RawPayload(int id,  object payload)
    {
        Id = id; 
        Payload = payload;
    }
}
