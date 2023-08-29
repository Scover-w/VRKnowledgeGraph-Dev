using Azure;
using Azure.AI.OpenAI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Utilities.Encoding.OggVorbis;

public class AIDENController : MonoBehaviour
{
    [SerializeField]
    AIDENUI _aidenUI;

    [SerializeField]
    TextAsset _prompts;

    GraphConfiguration _graphConfiguration;

    WhisperAPI _audioToTextAPI;
    OpenAIClient _gptClient;

    Dictionary<AIDENPrompts, string> _promptsDict;

    CancellationToken _cancelationToken;

    string _gptModelName;


    [ContextMenu("Start")]
    private void Start()
    {
        _audioToTextAPI = new();


        _gptClient = new OpenAIClient(OpenAIKey.ApiKey, new OpenAIClientOptions());

        _promptsDict = new();


        _promptsDict = JsonConvert.DeserializeObject<Dictionary<AIDENPrompts, string>>(_prompts.text);

        _gptModelName = "gpt-3.5-turbo";

        _graphConfiguration = GraphConfiguration.Instance;
    }

    public void Ask(AudioClip clip)
    {
        ThreadPool.QueueUserWorkItem(TranscribeAudio, clip);
    }


    private async void TranscribeAudio(object audioState)
    {
        var clip = (AudioClip)audioState;
        var audioStream = clip.EncodeToOggVorbisStream(true);

        var userIntentTxt = await new WhisperAPI().TranscribeAudio(audioStream);
        DetectIntentsGroup(userIntentTxt);
    }

    public async void DetectIntentsGroup(string userIntentTxt)
    {
        ChatCompletionsOptions chat = GetChat(AIDENPrompts.Groupe, userIntentTxt);

        Response<ChatCompletions> response = await _gptClient.GetChatCompletionsAsync(
            deploymentOrModelName: _gptModelName,
        chat);

        var aidenAnswer = response.Value.Choices[0].Message.Content;
        HandleIntentsGroupDetection(aidenAnswer);
    }

    private void HandleIntentsGroupDetection(string answer)
    {
        Debug.Log(answer);
        AIDENIntentGroupAnswers aidenAnswer = JsonConvert.DeserializeObject<AIDENIntentGroupAnswers>(answer);

        var intentions = aidenAnswer.Intentions;
        List<AIDENPromptPayload> payloads = new();

        foreach(var intent in intentions)
        {
            (string intentType, string sentenceChunk) = GetKeyValue(intent);

            if(intentType == "")
            {
                Debug.LogWarning("Couldn't get intentType or setnecenChunk.");
                continue;
            }

            if (!intentType.TryParseToEnum(out AIDENPrompts value))
            {
                Debug.LogWarning("Couldn't convert " + intent + " to an enum.");
                continue;
            }

            payloads.Add(new AIDENPromptPayload(value, sentenceChunk));
            Debug.Log(value.ToString() + " , " + sentenceChunk);
        }


        _ = DetectIntentsAsync(payloads);
    }



    private async Task DetectIntentsAsync(List<AIDENPromptPayload> payloads)
    {
        List<Task<Response<ChatCompletions>>> tasks = new List<Task<Response<ChatCompletions>>>();

        foreach(AIDENPromptPayload intent in payloads)
        {
            tasks.Add(_gptClient.GetChatCompletionsAsync(_gptModelName, GetChat(intent.Type, intent.Content)));
        }

        await Task.WhenAll(tasks);

        int i = 0;

        foreach (var task in tasks)
        {
            var payload = payloads[i];
            payload.Content = task.Result.Value.Choices[0].Message.Content;
            i++;
            Debug.Log(payload.Content);
        }

        HandleIntents(payloads);
    }


    private void HandleIntents(List<AIDENPromptPayload> payloads)
    {

        AIDENIntents aidenIntents = new();


        foreach(var payload in payloads)
        {
            switch (payload.Type)
            {
                case AIDENPrompts.Mode:
                    HandleModeIntent(payload, aidenIntents);
                    break;
                case AIDENPrompts.Taille:
                    HandleSizeIntent(payload, aidenIntents);
                    break;
                case AIDENPrompts.Visibilite:
                    HandleVisibilityIntent(payload, aidenIntents);
                    break;
                case AIDENPrompts.Couleur:
                    HandleColorIntent(payload, aidenIntents);
                    break;
                case AIDENPrompts.Ontologie:
                    HandleOntologyIntent(payload, aidenIntents);
                    break;
                case AIDENPrompts.Volume:
                    HandleVolumeIntent(payload, aidenIntents);
                    break;
                case AIDENPrompts.Interaction:
                    HandleInteractionIntent(payload, aidenIntents);
                    break;
                case AIDENPrompts.Metrique:
                    HandleMetriqueIntent(payload, aidenIntents);
                    break;
                case AIDENPrompts.Temps:
                    HandleTimeIntent(payload, aidenIntents);
                    break;
                case AIDENPrompts.Physique:
                    HandleSimulationIntent(payload, aidenIntents);
                    break;
            }
        }
    }

    private void HandleModeIntent(AIDENPromptPayload intent, AIDENIntents aidenIntents)
    {
        JObject jObject = JObject.Parse(intent.Content);

        string mode = "";
        string type = "";
        string precision = "";

        var props = jObject.Properties();

        foreach (var prop in props)
        {
            string propName = prop.Name.ToLower();
            string propValue = prop.Value.ToString().ToLower();

            if(propName == "mode")
                mode = propValue;
            else if(propName == "type")
                type = propValue;
            else if(propName == "precision")
                precision = propValue;
        }

        if(mode == "graphe")
        {

            if (type == "bureau" || type == "loupe")
            {
                aidenIntents.Add(new AIDENIntent(GraphConfigKey.GraphMode, GraphMode.Desk.ToString()));
            }
            else if (type == "immersion" || type == "gps")
            {
                aidenIntents.Add(new AIDENIntent(GraphConfigKey.GraphMode, GraphMode.Immersion.ToString()));
            }
            return;
        }


        if(mode == "selection")
        {
            if (type == "simple")
            {
                aidenIntents.Add(new AIDENIntent(GraphConfigKey.SelectionMode, SelectionMode.Single.ToString()));
            }
            else if (type == "multiple")
            {
                aidenIntents.Add(new AIDENIntent(GraphConfigKey.SelectionMode, SelectionMode.Multiple.ToString()));
            }
            return;
        }


        if(mode == "metrique")
        {
            if(precision.Length == 0)
            {
                LogWarning();
                return;
            }

            bool isSize = false;

            if (precision == "taille")
                isSize = true;
            else if(precision != "couleur")
            {
                LogWarning();
                return;
            }

            GraphMetricType metricType = GraphMetricType.None;

            if (type == "normal")
                metricType = GraphMetricType.None;
            else if(type == "degre")
                metricType = GraphMetricType.Degree;
            else if (type == "regroupement_local")
                metricType = GraphMetricType.LocalClusteringCoefficient;
            else if (type == "chemin_court")
                metricType = GraphMetricType.AverageShortestPath;
            else if (type == "centralite_intermediaire")
                metricType = GraphMetricType.BetweennessCentrality;
            else if (type == "centralite_proximite")
                metricType = GraphMetricType.ClosenessCentrality;
            else
            {
                LogWarning();
                return;
            }

            aidenIntents.Add(new AIDENIntent(isSize? GraphConfigKey.SelectedMetricTypeSize : GraphConfigKey.SelectedMetricTypeColor, metricType.ToString()));
            return;
        }


        LogWarning();

        void LogWarning()
        {
            Debug.LogWarning("Couldn't HandleModeIntent : " + mode + " " + type + " " + precision + "\n" + intent.Content);

        }

    }

    private void HandleSizeIntent(AIDENPromptPayload intent, AIDENIntents aidenIntents)
    {
        JObject jObject = JObject.Parse(intent.Content);

        string subject = "";
        string subjectType = "";
        string unitValue = "";
        string valueS = "";

        var props = jObject.Properties();

        foreach (var prop in props)
        {
            string propName = prop.Name.ToLower();
            string propValue = prop.Value.ToString().ToLower();

            if (propName == "sujet")
                subject = propValue;
            else if (propName == "sujet_type")
                subjectType = propValue;
            else if (propName == "valeur_unite")
                unitValue = propValue;
            else if (propName == "valeur")
                valueS = propValue;
        }

        int subjectId = 0;

        if (subjectType == "loupe")
            subjectId = 1;
        else if (subjectType == "immersion")
            subjectId = 2;
        else if (subjectType == "gps")
            subjectId = 3;
        else if(subjectType != "bureau")
        {
            LogWarning();
            return;
        }


        bool isAbsolute = true;
        if (unitValue == "relatif")
            isAbsolute = false;
        else if(unitValue != "absolue")
        {
            LogWarning();
            return;
        }

        float valueV = 0f;

        if(isAbsolute)
        {
            if(!float.TryParse(valueS, out valueV))
            {
                LogWarning();
                return;
            }
        }
        else
        {
            if(valueS.Contains("%"))
            {
                valueS = valueS.Replace("%", "");

                if (!float.TryParse(valueS, out valueV))
                {
                    LogWarning();
                    return;
                }

                valueV *= 0.01f;
            }
            else
            {
                if (!float.TryParse(valueS, out valueV))
                {
                    LogWarning();
                    return;
                }
            }
        }


        if (subject == "graphe")
        {
            if(subjectId == 0)
            {
                if(isAbsolute)
                {
                    aidenIntents.Add(new AIDENIntent(GraphConfigKey.GraphMode, GraphMode.Desk.ToString()));
                }
            }


            return;
        }

        if(subject == "texte")
        {
            return;
        }

        if (subject == "noeud")
        {
            return;
        }

        if (subject == "arete")
        {
            return;
        }

        LogWarning();


        void LogWarning()
        {
            Debug.LogWarning("Couldn't HandleSizeIntent : " + subject + " " + subjectType + " " + value + " " +  unitValue + "\n" + intent.Content);

        }


    }

    private void HandleVisibilityIntent(AIDENPromptPayload intent, AIDENIntents aidenIntents)
    {

    }

    private void HandleColorIntent(AIDENPromptPayload intent, AIDENIntents aidenIntents)
    {

    }

    private void HandleOntologyIntent(AIDENPromptPayload intent, AIDENIntents aidenIntents)
    {

    }

    private void HandleVolumeIntent(AIDENPromptPayload intent, AIDENIntents aidenIntents)
    {

    }

    private void HandleInteractionIntent(AIDENPromptPayload intent, AIDENIntents aidenIntents)
    {

    }

    private void HandleMetriqueIntent(AIDENPromptPayload intent, AIDENIntents aidenIntents)
    {

    }

    private void HandleTimeIntent(AIDENPromptPayload intent, AIDENIntents aidenIntents)
    {

    }

    private void HandleSimulationIntent(AIDENPromptPayload intent, AIDENIntents aidenIntents)
    {

    }

    private ChatCompletionsOptions GetChat(AIDENPrompts aidenPrompts, string transcribedText)
    {
        string asistantPrompt = _promptsDict[aidenPrompts];

        Debug.Log("GetChat : " + aidenPrompts.ToString() + " : " + asistantPrompt + " / " + transcribedText);
        var chat = new ChatCompletionsOptions()
        {
            Messages =
            {
                new ChatMessage(ChatRole.Assistant, asistantPrompt),
                new ChatMessage(ChatRole.User, transcribedText)
            }
        };

        chat.Temperature = 0;

        return chat;
    }

    private (string key,string value) GetKeyValue(JObject jObject)
    {
        try
        {
            string intentType = jObject.Properties().First().Name;
            string sentenceChunck = jObject[intentType].ToString();
            return (intentType, sentenceChunck);
        }
        catch(Exception ex) 
        {
            return ("", "");
        }
    }

    [ContextMenu("RefreshPrompts")]
    private void RefreshPrompts()
    {
        JsonToEnum.RefreshEnum(_prompts);
    }



}



public class AIDENIntentGroupAnswers
{
    [JsonProperty("intentions")]
    public List<JObject> Intentions;
}


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


public class AIDENAnswer
{
    [JsonProperty("objet")]
    public string Objet;
    [JsonProperty("précision objet")]
    public string PrecisionObjet;
    [JsonProperty("action")]
    public string Action;
    [JsonProperty("propriété")]
    public string Propriété;
    [JsonProperty("valeur")]
    public string Value;
}

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
}

public class AIDENIntent
{
    public bool IsGraphConfig { get; private set; }

    public GraphConfigKey GraphConfigKey { get; private set; }
    public GraphActionKey GraphActionKey { get; private set; }

    public string ValueS { get; private set; }
    public float ValueF { get; private set; }
    public bool ValueB { get; private set; }
    public Color ValueC { get; private set; }

    public AIDENIntent(GraphConfigKey graphConfigKey, string v)
    {
        IsGraphConfig = true;
        GraphConfigKey = graphConfigKey;
        ValueS = v;
    }
    public AIDENIntent(GraphConfigKey graphConfigKey, float v)
    {
        IsGraphConfig = true;
        GraphConfigKey = graphConfigKey;
        ValueF = v;
    }
    public AIDENIntent(GraphConfigKey graphConfigKey, bool v)
    {
        IsGraphConfig = true;
        GraphConfigKey = graphConfigKey;
        ValueB = v;
    }
    public AIDENIntent(GraphConfigKey graphConfigKey, Color v)
    {
        IsGraphConfig = true;
        GraphConfigKey = graphConfigKey;
        ValueC = v;
    }

    public AIDENIntent(GraphActionKey graphConfigKey, string v)
    {
        IsGraphConfig = true;
        GraphActionKey = graphConfigKey;
        ValueS = v;
    }

    public AIDENIntent(GraphActionKey graphConfigKey, float v)
    {
        IsGraphConfig = true;
        GraphActionKey = graphConfigKey;
        ValueF = v;
    }

    public AIDENIntent(GraphActionKey graphConfigKey, bool v)
    {
        IsGraphConfig = true;
        GraphActionKey = graphConfigKey;
        ValueB = v;
    }

    public AIDENIntent(GraphActionKey graphConfigKey, Color v)
    {
        IsGraphConfig = true;
        GraphActionKey = graphConfigKey;
        ValueC = v;
    }
}