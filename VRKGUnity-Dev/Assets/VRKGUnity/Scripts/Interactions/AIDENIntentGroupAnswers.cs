using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class AIDENIntentGroupAnswers
{
    [JsonProperty("intentions")]
    public List<JObject> Intentions;
}
