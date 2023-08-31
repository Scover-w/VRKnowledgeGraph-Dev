using Azure;
using Azure.AI.OpenAI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities.Encoding.OggVorbis;

public class AIDENController : MonoBehaviour
{
    [SerializeField]
    AIDENUI _aidenUI;

    [SerializeField]
    TextAsset _prompts;

    [SerializeField]
    InputPropagatorManager _propagatorManager;

    [SerializeField]
    GraphConfigurationContainerSO _graphConfigurationContainerSO;

    GraphConfiguration _graphConfiguration;

    WhisperAPI _audioToTextAPI;
    OpenAIClient _gptClient;

    Dictionary<AIDENPrompts, string> _promptsDict;

    CancellationToken _cancelationToken;

    AIDENIntents _aidenIntents;

    string _gptModelName;
    int _stopPayloadId = -1;
    int _payloadCounterId = 0;



    [ContextMenu("Start")]
    private async void Start()
    {
        _audioToTextAPI = new();


        _gptClient = new OpenAIClient(OpenAIKey.ApiKey, new OpenAIClientOptions());

        _promptsDict = new();


        _promptsDict = JsonConvert.DeserializeObject<Dictionary<AIDENPrompts, string>>(_prompts.text);

        _gptModelName = "gpt-3.5-turbo";

        _graphConfiguration = await _graphConfigurationContainerSO.GetGraphConfiguration();
    }

    public void CancelActions()
    {
        if (_aidenIntents == null || _aidenIntents.Intents.Count == 0)
            return;

        _propagatorManager.SetOldValues(_aidenIntents);

        _aidenIntents = new();
    }

    public void Stop()
    {
        _stopPayloadId = _payloadCounterId;
    }


    public void Ask(AudioClip clip)
    {
        _stopPayloadId = _payloadCounterId;

        _payloadCounterId++;
        RawPayload payload = new(_payloadCounterId, clip);

        ThreadPool.QueueUserWorkItem(TranscribeAudio, payload);
    }



    private async void TranscribeAudio(object rawPayloadObj)
    {
        RawPayload rawPayload = (RawPayload)rawPayloadObj;

        var clip = (AudioClip)rawPayload.Payload ;
        var audioStream = clip.EncodeToOggVorbisStream(true);

        var userIntentTxt = await new WhisperAPI().TranscribeAudio(audioStream);

        if (rawPayload.Id <= _stopPayloadId)
        {
            Debug.Log("Stopped Payload " + rawPayload.Id + " at TranscribeAudio.");
            return;
        }

        // Cheap way to detect if need to call the splitIntentTexts function that add another api call
        string separator = TryGetSeparator(userIntentTxt);

        if (separator == null)
        {
            DetectIntentsGroup(rawPayload.Id, userIntentTxt);
            return;
        }

        // Check if common 



        SplitIntentsText(rawPayload.Id, userIntentTxt);
            
    }


    public string TryGetSeparator(string userIntentTxt)
    {
        if(userIntentTxt.Contains(","))
        {
            if (IsCommaASeparator(userIntentTxt)) // Mean that multiple intents are in the text
                return ",";
        }

        if(userIntentTxt.Contains(" et "))
        {
            if(IsAndASeparator(userIntentTxt)) // Mean that multiple intents are in the text
                return "et";
        }

        if (userIntentTxt.Contains(" puis ")) // Mean that multiple intents are in the text
        { 
            return "puis";
        }

        return null;
    }

    private bool IsCommaASeparator(string userIntentTxt)
    {
        string[] splits = userIntentTxt.Split(",");

        int nbSplit = splits.Length;

        if (nbSplit == 1)
            return false;

        for(int i = 1; i < nbSplit; i++)
        {
            string split = splits[i];

            if (!char.IsDigit(split[0]))
                return true;
        }

        return false;
    }

    private bool IsAndASeparator(string userIntentTxt)
    {
        HashSet<string> nextWords = new() // new word that are used when it's the same intent in the sentence (it's a sub intent)
        {
            "le",
            "la",
            "les",
            "l'",
            "du",
            "des"
        };

        string[] splits = userIntentTxt.Split("et");

        int nbSplit = splits.Length;

        if (nbSplit == 1)
            return false;

        for (int i = 1; i < nbSplit; i++)
        {
            string split = splits[i].Replace(" ","");

            if (!StartWithNextWord(split)) // probably a verb, so a new intent
                return true;
        }

        return false;

        bool StartWithNextWord(string words)
        {
            foreach(string nextWord in nextWords) 
            {
                if (words.StartsWith(nextWord))
                    return true;
            }

            return false;
        }

    }

    public async void SplitIntentsText(int payloadId, string userIntentTxt)
    {
        ChatCompletionsOptions chat = GetChat(AIDENPrompts.SeparePhrase, userIntentTxt);

        Response<ChatCompletions> response = await _gptClient.GetChatCompletionsAsync(
            deploymentOrModelName: _gptModelName,
        chat);

        if (payloadId <= _stopPayloadId)
        {
            Debug.Log("Stopped Payload " + payloadId + " at SplitIntentsText.");
            return;
        }

        var aidenAnswer = response.Value.Choices[0].Message.Content;
        HandleSplitIntentText(payloadId, aidenAnswer);
    }

    public async void HandleSplitIntentText(int payloadId, string aidenAnswer)
    {
        JObject jObject = JObject.Parse(aidenAnswer);

        var intents = jObject.GetValue("intentions");

        if (intents == null)
        {
            Debug.Log("Couldn't Deserialize Intentions Split in HandleSplitIntentText.");
            // TODO : FeedbackUI
            return;
        }

        List<string> sentencesIntents = new();

        JArray intentArray = (JArray)intents;
        foreach (var intent in intentArray)
        {
            sentencesIntents.Add(intent.ToString());
        }


    }


    public async void DetectIntentsGroup(int payloadId, string userIntentTxt)
    {
        ChatCompletionsOptions chat = GetChat(AIDENPrompts.Groupe, userIntentTxt);

        Response<ChatCompletions> response = await _gptClient.GetChatCompletionsAsync(
            deploymentOrModelName: _gptModelName,
        chat);

        if (payloadId <= _stopPayloadId)
        {
            Debug.Log("Stopped Payload " + payloadId + " at DetectIntentsGroup.");
            return;
        }

        var aidenAnswer = response.Value.Choices[0].Message.Content;
        HandleIntentsGroupDetection(payloadId, aidenAnswer);
    }

    private void HandleIntentsGroupDetection(int payloadId, string answer)
    {
        Debug.Log(answer);

        AIDENIntentGroupAnswers aidenAnswer = new();
        try
        {
            aidenAnswer = JsonConvert.DeserializeObject<AIDENIntentGroupAnswers>(answer);
        }
        catch
        {
            Debug.Log("Couldn't Deserialize AIDENIntentGroupAnswers in HandleIntentsGroupDetection.");
            // TODO : FeedbackUI
            return;
        }

        var intentions = aidenAnswer.Intentions;
        List<AIDENPromptPayload> payloads = new();

        foreach(var intent in intentions)
        {
            (string intentType, string sentenceChunk) = GetKeyValue(intent);

            if(intentType == "")
            {
                Debug.LogWarning("Couldn't get intentType or sentenceChunk.");
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


        _ = DetectIntentsAsync(payloadId, payloads);
    }



    private async Task DetectIntentsAsync(int payloadId, List<AIDENPromptPayload> payloads)
    {
        List<Task<Response<ChatCompletions>>> tasks = new List<Task<Response<ChatCompletions>>>();

        foreach(AIDENPromptPayload intent in payloads)
        {
            tasks.Add(_gptClient.GetChatCompletionsAsync(_gptModelName, GetChat(intent.Type, intent.Content)));
        }

        await Task.WhenAll(tasks);


        if(payloadId <= _stopPayloadId)
        {
            Debug.Log("Stopped Payload " + payloadId + " at DetectIntentsAsync.");
            return;
        }


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

        _aidenIntents = new();


        foreach(var payload in payloads)
        {
            switch (payload.Type)
            {
                case AIDENPrompts.Mode:
                    HandleModeIntent(payload, _aidenIntents);
                    break;
                case AIDENPrompts.Taille:
                    HandleSizeIntent(payload, _aidenIntents);
                    break;
                case AIDENPrompts.Visibilite:
                    HandleVisibilityIntent(payload, _aidenIntents);
                    break;
                case AIDENPrompts.Couleur:
                    HandleColorIntent(payload, _aidenIntents);
                    break;
                case AIDENPrompts.Ontologie:
                    HandleOntologyIntent(payload, _aidenIntents);
                    break;
                case AIDENPrompts.Volume:
                    HandleVolumeIntent(payload, _aidenIntents);
                    break;
                case AIDENPrompts.Interaction:
                    HandleInteractionIntent(payload, _aidenIntents);
                    break;
                case AIDENPrompts.Metrique:
                    HandleMetriqueIntent(payload, _aidenIntents);
                    break;
                case AIDENPrompts.Temps:
                    HandleTimeIntent(payload, _aidenIntents);
                    break;
                case AIDENPrompts.Physique:
                    HandleSimulationIntent(payload, _aidenIntents);
                    break;
                case AIDENPrompts.Action:
                    HandleActionIntent(payload, _aidenIntents);
                    break;
            }
        }

        if(_aidenIntents.Intents.Count == 0)
        {
            // TODO : UI feedback
            Debug.Log("No intents detected");
            return;
        }

        _aidenIntents.Order();

        foreach (var intent in _aidenIntents.Intents)
        {
            Debug.Log(intent.ToString());
        }

        //_propagatorManager.SetNewValues(_aidenIntents);
        
        // TODO : UI feedback
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

        if( (mode == "bureau" ||  mode == "loupe" || mode == "immersion" || mode == "gps") && type.Length == 0) // Case where user say switch to desk without graph
        {
            type = mode;
            mode = "graphe";
        }


        if(mode == "graphe")
        {
            GraphMode currentGraphMode = _graphConfiguration.GraphMode;

            // TODO : Check if can switch mode

            if (type == "bureau" || type == "loupe")
            {
                if(currentGraphMode == GraphMode.Desk)
                {
                    Debug.Log("HandleModeIntent GraphMode has already the same value : " + currentGraphMode);
                    return;
                }


                aidenIntents.Add(new AIDENIntent(GraphConfigKey.GraphMode, GraphMode.Desk.ToString(), currentGraphMode.ToString()));
                return;
            }
            else if (type == "immersion" || type == "gps")
            {
                if (currentGraphMode == GraphMode.Immersion)
                {
                    Debug.Log("HandleModeIntent GraphMode has already the same value : " + currentGraphMode);
                    return;
                }


                aidenIntents.Add(new AIDENIntent(GraphConfigKey.GraphMode, GraphMode.Immersion.ToString(), currentGraphMode.ToString()));
                return;
            }

            LogWarning();
            return;
        }


        if(mode == "selection")
        {
            SelectionMode currentSelectionMode = _graphConfiguration.SelectionMode;

            if (type == "simple")
            {
                if(currentSelectionMode == SelectionMode.Single)
                {
                    Debug.Log("HandleModeIntent SelectionMode has already the value " + currentSelectionMode);
                    return;
                }

                aidenIntents.Add(new AIDENIntent(GraphConfigKey.SelectionMode, SelectionMode.Single.ToString(), currentSelectionMode.ToString()));
                return;
            }
            else if (type == "multiple")
            {
                if (currentSelectionMode == SelectionMode.Multiple)
                {
                    Debug.Log("HandleModeIntent SelectionMode has already the value " + currentSelectionMode);
                    return;
                }

                aidenIntents.Add(new AIDENIntent(GraphConfigKey.SelectionMode, SelectionMode.Multiple.ToString(), currentSelectionMode.ToString()));
                return;
            }

            LogWarning();
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

            GraphMetricType newMetricType = GraphMetricType.None;

            if (type == "normal")
                newMetricType = GraphMetricType.None;
            else if(type == "degre")
                newMetricType = GraphMetricType.Degree;
            else if (type == "regroupement_local")
                newMetricType = GraphMetricType.LocalClusteringCoefficient;
            else if (type == "chemin_court")
                newMetricType = GraphMetricType.AverageShortestPath;
            else if (type == "centralite_intermediaire")
                newMetricType = GraphMetricType.BetweennessCentrality;
            else if (type == "centralite_proximite")
                newMetricType = GraphMetricType.ClosenessCentrality;
            else
            {
                LogWarning();
                return;
            }

            GraphMetricType currentMetricType = isSize ? _graphConfiguration.SelectedMetricTypeSize : _graphConfiguration.SelectedMetricTypeColor;

            if(currentMetricType == newMetricType)
            {
                Debug.Log("HandleModeIntent GraphMetricType " + (isSize ? "size" : "color") + " has already the value " + newMetricType);
                return;
            }

            aidenIntents.Add(new AIDENIntent(isSize? GraphConfigKey.SelectedMetricTypeSize : GraphConfigKey.SelectedMetricTypeColor, newMetricType.ToString(), currentMetricType.ToString()));
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
            else if (propName == "valeur")
                valueS = propValue;
        }

        GraphName graph = GraphName.Desk;

        if (subjectType == "loupe")
            graph = GraphName.Lense;
        else if (subjectType == "immersion")
            graph = GraphName.Immersion;
        else if (subjectType == "gps")
            graph = GraphName.GPS;
        else if (subjectType != "bureau")
        {
            LogWarning();
            return;
        }



        if (!ParseIsAbsolute(valueS, out bool isAbsolute))
        {
            LogWarning();
            return;
        }

        if (!ParseValue(valueS, isAbsolute, out float newSize))
        {
            LogWarning();
            return;
        }


        if (subject == "graphe")
        {
            float currentSize = GetGraphSize(graph);

            if (!isAbsolute)
                newSize = currentSize * newSize;

            aidenIntents.Add(new AIDENIntent(GetSizeKey(graph), newSize, currentSize));
            return;
        }

        if(subject == "texte")
        {
            if(graph == GraphName.GPS)
            {
                LogWarning();
                return;
            }

            float currentSize = GetTextSize(graph);

            if (!isAbsolute)
                newSize = currentSize * newSize;


            aidenIntents.Add(new AIDENIntent(GetTextKey(graph), newSize, currentSize));
            return;
        }

        if (subject == "noeud")
        {
            float currentSize = GetNodeSize(graph);

            if (!isAbsolute)
                newSize = currentSize * newSize;

            aidenIntents.Add(new AIDENIntent(GetNodeKey(graph), newSize, currentSize));
            return;
        }

        if (subject == "arete")
        {
            float currentSize = GetEdgeSize(graph);

            if (!isAbsolute)
                newSize = currentSize * newSize;

            aidenIntents.Add(new AIDENIntent(GetEdgeKey(graph), newSize, currentSize));
            return;
        }

        LogWarning();


        void LogWarning()
        {
            Debug.LogWarning("Couldn't HandleSizeIntent : " + subject + " " + subjectType + " " + valueS + "\n" + intent.Content);

        }

        float GetGraphSize(GraphName graph)
        {
            switch (graph)
            {
                case GraphName.Desk:
                    return _graphConfiguration.DeskGraphSize;
                case GraphName.Lense:
                    return _graphConfiguration.LensGraphSize;
                case GraphName.Immersion:
                    return _graphConfiguration.ImmersionGraphSize;
                case GraphName.GPS:
                    return _graphConfiguration.WatchGraphSize;
            }

            return _graphConfiguration.DeskGraphSize;
        }

        float GetTextSize(GraphName graph)
        {
            switch (graph)
            {
                case GraphName.Desk:
                    return _graphConfiguration.LabelNodeSizeDesk;
                case GraphName.Lense:
                    return _graphConfiguration.LabelNodeSizeLens;
                case GraphName.Immersion:
                    return _graphConfiguration.LabelNodeSizeImmersion;
            }

            return _graphConfiguration.LabelNodeSizeDesk;
        }

        float GetNodeSize(GraphName graph)
        {
            switch (graph)
            {
                case GraphName.Desk:
                    return _graphConfiguration.NodeSizeDesk;
                case GraphName.Lense:
                    return _graphConfiguration.NodeSizeLens;
                case GraphName.Immersion:
                    return _graphConfiguration.NodeSizeImmersion;
                case GraphName.GPS:
                    return _graphConfiguration.NodeSizeWatch;
            }

            return _graphConfiguration.NodeSizeDesk;
        }

        float GetEdgeSize(GraphName graph)
        {
            switch (graph)
            {
                case GraphName.Desk:
                    return _graphConfiguration.EdgeThicknessDesk;
                case GraphName.Lense:
                    return _graphConfiguration.EdgeThicknessLens;
                case GraphName.Immersion:
                    return _graphConfiguration.EdgeThicknessImmersion;
                case GraphName.GPS:
                    return _graphConfiguration.EdgeThicknessWatch;
            }

            return _graphConfiguration.EdgeThicknessDesk;
        }

        GraphConfigKey GetSizeKey(GraphName graph)
        {
            switch (graph)
            {
                case GraphName.Desk:
                    return GraphConfigKey.DeskGraphSize;
                case GraphName.Lense:
                    return GraphConfigKey.LensGraphSize;
                case GraphName.Immersion:
                    return GraphConfigKey.ImmersionGraphSize;
                case GraphName.GPS:
                    return GraphConfigKey.WatchGraphSize;
            }

            return GraphConfigKey.DeskGraphSize;
        }

        GraphConfigKey GetTextKey(GraphName graph)
        {
            switch (graph)
            {
                case GraphName.Desk:
                    return GraphConfigKey.LabelNodeSizeDesk;
                case GraphName.Lense:
                    return GraphConfigKey.LabelNodeSizeLens;
                case GraphName.Immersion:
                    return GraphConfigKey.LabelNodeSizeImmersion;
            }

            return GraphConfigKey.LabelNodeSizeDesk;
        }

        GraphConfigKey GetNodeKey(GraphName graph)
        {
            switch (graph)
            {
                case GraphName.Desk:
                    return GraphConfigKey.NodeSizeDesk;
                case GraphName.Lense:
                    return GraphConfigKey.NodeSizeLens;
                case GraphName.Immersion:
                    return GraphConfigKey.NodeSizeImmersion;
                case GraphName.GPS:
                    return GraphConfigKey.NodeSizeWatch;
            }

            return GraphConfigKey.NodeSizeDesk;
        }

        GraphConfigKey GetEdgeKey(GraphName graph)
        {
            switch (graph)
            {
                case GraphName.Desk:
                    return GraphConfigKey.EdgeThicknessDesk;
                case GraphName.Lense:
                    return GraphConfigKey.EdgeThicknessLens;
                case GraphName.Immersion:
                    return GraphConfigKey.EdgeThicknessImmersion;
                case GraphName.GPS:
                    return GraphConfigKey.EdgeThicknessWatch;
            }

            return GraphConfigKey.EdgeThicknessDesk;
        }

    }

    private void HandleVisibilityIntent(AIDENPromptPayload intent, AIDENIntents aidenIntents)
    {
        JObject jObject = JObject.Parse(intent.Content);

        string affichage = "";
        string transparence = "";
        string valueS = "";

        var props = jObject.Properties();

        foreach (var prop in props)
        {
            string propName = prop.Name.ToLower();
            string propValue = prop.Value.ToString().ToLower();

            if (propName == "affichage")
                affichage = propValue;
            else if (propName == "transparence")
                transparence = propValue;
            else if (propName == "valeur")
                valueS = propValue;
        }

        bool isAlpha = false;

        if (affichage.Length > 0 && transparence.Length == 0)
        {
            isAlpha = false;
        }
        else if (transparence.Length > 0 && affichage.Length == 0)
        {
            isAlpha = true;
        }
        else
        {
            LogWarning();
            return;
        }


        if (isAlpha)
        {
            GraphConfigKey alphaKey;

            if (transparence == "noeud")
                alphaKey = GraphConfigKey.AlphaNodeColorUnPropagated;
            else if (transparence == "noeud_propage")
                alphaKey = GraphConfigKey.AlphaNodeColorPropagated;
            else if (transparence == "arete")
                alphaKey = GraphConfigKey.AlphaEdgeColorUnPropagated;
            else if (transparence == "arete_propage")
                alphaKey = GraphConfigKey.AlphaEdgeColorPropagated;
            else
            {
                LogWarning();
                return;
            }


            if(!ParseIsAbsolute(valueS, out bool isAbsolute))
            {
                LogWarning();
                return;
            }


            if(!ParseValue(valueS, isAbsolute, out float newAlpha))
            {
                LogWarning();
                return;
            }

            float currentAlpha = GetCurrentAlpha(alphaKey);

            if(!isAbsolute)
            {
                newAlpha = currentAlpha * newAlpha;
            }

            aidenIntents.Add(new AIDENIntent(alphaKey, newAlpha, currentAlpha));
            return;

        }
        else
        {
            GraphConfigKey displayKey;

            if (affichage == "texte_immersion")
                displayKey = GraphConfigKey.ShowLabelImmersion;
            else if (affichage == "texte_bureau")
                displayKey = GraphConfigKey.ShowLabelDesk;
            else if (affichage == "texte_loupe")
                displayKey = GraphConfigKey.ShowLabelLens;
            else if (affichage == "gps")
                displayKey = GraphConfigKey.ShowWatch;
            else if (affichage == "arete")
                displayKey = GraphConfigKey.DisplayEdges;
            else if (affichage == "inter_arete_voisin")
                displayKey = GraphConfigKey.DisplayInterSelectedNeighborEdges;
            else
            {
                LogWarning();
                return;
            }

            bool newDisplay = false;

            if (valueS == "vrai")
                newDisplay = true;
            else if (valueS != "faux")
            {
                LogWarning();
                return;
            }

            bool currentDisplay = GetCurrentDisplay(displayKey);

            if(currentDisplay == newDisplay)
            {
                Debug.Log("HandleVisibilityIntent newDisplay has the same value " + newDisplay);
                return;
            }

            aidenIntents.Add(new AIDENIntent(displayKey, newDisplay, currentDisplay));
            return;
        }


        void LogWarning()
        {
            Debug.LogWarning("Couldn't HandleVisibilityIntent : " + affichage + " " + transparence + " " + valueS + "\n" + intent.Content);
        }

        float GetCurrentAlpha(GraphConfigKey alphaKey)
        {
            switch (alphaKey)
            {
                case GraphConfigKey.AlphaNodeColorUnPropagated:
                    return _graphConfiguration.AlphaNodeColorUnPropagated;
                case GraphConfigKey.AlphaNodeColorPropagated:
                    return _graphConfiguration.AlphaNodeColorPropagated;
                case GraphConfigKey.AlphaEdgeColorUnPropagated:
                    return _graphConfiguration.AlphaEdgeColorUnPropagated;
                case GraphConfigKey.AlphaEdgeColorPropagated:
                    return _graphConfiguration.AlphaEdgeColorPropagated;
                default:
                    return _graphConfiguration.AlphaNodeColorUnPropagated;
            }
        }

        bool GetCurrentDisplay(GraphConfigKey graphConfigKey)
        {
            switch (graphConfigKey)
            {
                case GraphConfigKey.ShowLabelImmersion:
                    return _graphConfiguration.ShowLabelImmersion;
                case GraphConfigKey.ShowLabelDesk:
                    return _graphConfiguration.ShowLabelDesk;
                case GraphConfigKey.ShowLabelLens:
                    return _graphConfiguration.ShowLabelLens;
                case GraphConfigKey.ShowWatch:
                    return _graphConfiguration.ShowWatch;
                case GraphConfigKey.DisplayEdges:
                    return _graphConfiguration.DisplayEdges;
                case GraphConfigKey.DisplayInterSelectedNeighborEdges:
                    return _graphConfiguration.DisplayInterSelectedNeighborEdges;
                default:
                    return _graphConfiguration.ShowLabelImmersion;
            }
        }
    }

    private void HandleColorIntent(AIDENPromptPayload intent, AIDENIntents aidenIntents)
    {
        JObject jObject = JObject.Parse(intent.Content);

        string objet = "";
        string colorS = "";
        string shade = "";

        var props = jObject.Properties();

        foreach (var prop in props)
        {
            string propName = prop.Name.ToLower();
            string propValue = prop.Value.ToString().ToLower();

            if (propName == "objet")
                objet = propValue;
            else if (propName == "couleur")
                colorS = propValue;
        }


        if(!TryGetObjectColor(objet, out GraphConfigKey colorKey))
        {
            LogWarning();
            return;
        }


        if (!ColorUtility.TryParseHtmlString(colorS, out Color newColor))
        {
            LogWarning();
            return;
        }

        Color currentColor = GetCurentColor(colorKey);


        if(newColor == currentColor)
        {
            Debug.Log("HandleColorIntent newColor is the same color as the old one : " + newColor);
            return;
        }

        aidenIntents.Add(new AIDENIntent(colorKey, newColor, currentColor));


        void LogWarning()
        {
            Debug.LogWarning("Couldn't HandleColorIntent : " + objet + " " + colorS + " " + shade + "\n" + intent.Content);
        }

        bool TryGetObjectColor(string obj, out GraphConfigKey colorKey)
        {
            switch (obj) 
            {
                case "noeud":
                    colorKey = GraphConfigKey.NodeColor;
                    return true;

                case "noeud_sans_valeur":
                    colorKey = GraphConfigKey.NodeColorNoValueMetric;
                    return true;

                case "arete":
                    colorKey = GraphConfigKey.EdgeColor;
                    return true;

                case "arete_propage":
                    colorKey = GraphConfigKey.PropagatedEdgeColor;
                    return true;

                case "noeud_gradiant_1":
                    colorKey = GraphConfigKey.NodeColorMappingColorA;
                    return true;

                case "noeud_gradiant_2":
                    colorKey = GraphConfigKey.NodeColorMappingColorB;
                    return true;

                case "noeud_gradiant_3":
                    colorKey = GraphConfigKey.NodeColorMappingColorC;
                    return true;
                default:
                    colorKey = GraphConfigKey.NodeColor;
                    return false;
            }

            
        }


        Color GetCurentColor(GraphConfigKey colorKey)
        {
            switch (colorKey)
            {
                case GraphConfigKey.NodeColor:
                    return _graphConfiguration.NodeColor;
                case GraphConfigKey.NodeColorNoValueMetric:
                    return _graphConfiguration.NodeColorNoValueMetric;
                case GraphConfigKey.EdgeColor:
                    return _graphConfiguration.EdgeColor;
                case GraphConfigKey.PropagatedEdgeColor:
                    return _graphConfiguration.PropagatedEdgeColor;
                case GraphConfigKey.NodeColorMappingColorA:
                    return _graphConfiguration.NodeColorMapping.ColorA;
                case GraphConfigKey.NodeColorMappingColorB:
                    return _graphConfiguration.NodeColorMapping.ColorB;
                case GraphConfigKey.NodeColorMappingColorC:
                    return _graphConfiguration.NodeColorMapping.ColorC;
                default:
                    return _graphConfiguration.NodeColor;
            }
        }
    }

    private void HandleOntologyIntent(AIDENPromptPayload intent, AIDENIntents aidenIntents)
    {
        JObject jObject = JObject.Parse(intent.Content);

        string objet = "";
        string valueS = "";

        var props = jObject.Properties();

        foreach (var prop in props)
        {
            string propName = prop.Name.ToLower();
            string propValue = prop.Value.ToString().ToLower();

            if (propName == "objet")
                objet = propValue;
            else if (propName == "valeur")
                valueS = propValue;
        }

        if (!TryGetObjectOntologty(objet, out GraphConfigKey configKeyOntology))
        {
            LogWarning();
            return;
        }

        if (!ParseIsAbsolute(valueS, out bool isAbsolute))
        {
            LogWarning();
            return;
        }


        if (!ParseValue(valueS, isAbsolute, out float newOntologyValue))
        {
            LogWarning();
            return;
        }

        float curentOntologyValue = GetCurrentOntologyValue(configKeyOntology);

        if (!isAbsolute)
        {
            newOntologyValue = curentOntologyValue * newOntologyValue;
        }


        aidenIntents.Add(new AIDENIntent(configKeyOntology, newOntologyValue, curentOntologyValue));


        void LogWarning()
        {
            Debug.LogWarning("Couldn't HandleOntologyIntent : " + objet + " " + valueS + "\n" + intent.Content);
        }

        bool TryGetObjectOntologty(string objet, out GraphConfigKey graphConfigKey)
        {
            switch (objet)
            {
                case "nombre_color":
                    graphConfigKey = GraphConfigKey.NbOntologyColor;
                    return true;
                case "maximum_delta":
                    graphConfigKey = GraphConfigKey.MaxDeltaOntologyAlgo;
                    return true;
                case "saturation":
                    graphConfigKey = GraphConfigKey.SaturationOntologyColor;
                    return true;
                case "luminosite":
                    graphConfigKey = GraphConfigKey.ValueOntologyColor;
                    return true;
                default:
                    graphConfigKey = GraphConfigKey.NbOntologyColor;
                    return false;
            }
        }

        float GetCurrentOntologyValue(GraphConfigKey graphConfigKey)
        {
            switch (graphConfigKey)
            {
                case GraphConfigKey.NbOntologyColor:
                    return _graphConfiguration.NbOntologyColor;
                case GraphConfigKey.MaxDeltaOntologyAlgo:
                    return _graphConfiguration.MaxDeltaOntologyAlgo;
                case GraphConfigKey.SaturationOntologyColor:
                    return _graphConfiguration.SaturationOntologyColor;
                case GraphConfigKey.ValueOntologyColor:
                    return _graphConfiguration.ValueOntologyColor;
                default:
                    return _graphConfiguration.NbOntologyColor;
            }
        }

    }

    private void HandleVolumeIntent(AIDENPromptPayload intent, AIDENIntents aidenIntents)
    {
        JObject jObject = JObject.Parse(intent.Content);

        string objet = "";
        string valueS = "";

        var props = jObject.Properties();

        foreach (var prop in props)
        {
            string propName = prop.Name.ToLower();
            string propValue = prop.Value.ToString().ToLower();

            if (propName == "objet")
                objet = propValue;
            else if (propName == "valeur")
                valueS = propValue;
        }

        if (!TryParseObjectVolume(objet, out GraphConfigKey configVolumeKey))
        {
            LogWarning();
            return;
        }

        if(!ParseIsAbsolute(valueS, out bool isAbsolute))
        {
            LogWarning();
            return;
        }


        if (!ParseValue(valueS, isAbsolute, out float newVolume))
        {
            LogWarning();
            return;
        }

        float currentVolume = GetVolume(configVolumeKey);


        if(!isAbsolute)
        {
            newVolume = currentVolume * newVolume;
        }

        aidenIntents.Add(new AIDENIntent(configVolumeKey, newVolume, currentVolume));



        void LogWarning()
        {
            Debug.LogWarning("Couldn't HandleVolumeIntent : " + objet + " " + valueS + "\n" + intent.Content);
        }

        bool TryParseObjectVolume(string objet, out GraphConfigKey graphConfigKey)
        {
            switch(objet)
            {
                case "global":
                    graphConfigKey = GraphConfigKey.GlobalVolume;
                    return true;
                case "effet_sonore":
                    graphConfigKey = GraphConfigKey.SoundEffectVolume;
                    return true;
                case "musique":
                    graphConfigKey = GraphConfigKey.MusicVolume;
                    return true;
                case "aiden":
                    graphConfigKey = GraphConfigKey.AidenVolume;
                    return true;
                default:
                    graphConfigKey = GraphConfigKey.GlobalVolume;
                    return false;
            }
        }

        float GetVolume(GraphConfigKey graphConfigKey)
        {
            switch (graphConfigKey)
            {
                case GraphConfigKey.GlobalVolume:
                    return _graphConfiguration.GlobalVolume;
                case GraphConfigKey.SoundEffectVolume:
                    return _graphConfiguration.SoundEffectVolume;
                case GraphConfigKey.MusicVolume:
                    return _graphConfiguration.MusicVolume;
                case GraphConfigKey.AidenVolume:
                    return _graphConfiguration.AidenVolume;
                default:
                    return _graphConfiguration.GlobalVolume;
            }
        }

    }

    private void HandleInteractionIntent(AIDENPromptPayload intent, AIDENIntents aidenIntents)
    {
        JObject jObject = JObject.Parse(intent.Content);

        string objet = "";
        string valueS = "";

        var props = jObject.Properties();

        foreach (var prop in props)
        {
            string propName = prop.Name.ToLower();
            string propValue = prop.Value.ToString().ToLower();

            if (propName == "objet")
                objet = propValue;
            else if (propName == "valeur")
                valueS = propValue;
        }


        bool newCanSelect = false;

        if (valueS == "vrai")
            newCanSelect = true;
        else if(valueS != "faux")
        {
            LogWarning();
            return;
        }


        if(objet != "arete")
        {
            LogWarning();
            return;
        }

        bool curentCanSelectEdge = _graphConfiguration.CanSelectEdges;

        if(curentCanSelectEdge == newCanSelect)
        {
            Debug.Log("HandleInteractionIntent : CanSelectEdges has already the same value : " + newCanSelect);
            return;
        }


        aidenIntents.Add(new AIDENIntent(GraphConfigKey.CanSelectEdges, newCanSelect, curentCanSelectEdge));


        void LogWarning()
        {
            Debug.LogWarning("Couldn't HandleInteractionIntent : " + objet + " " + valueS + "\n" + intent.Content);
        }

    }

    private void HandleMetriqueIntent(AIDENPromptPayload intent, AIDENIntents aidenIntents)
    {
        JObject jObject = JObject.Parse(intent.Content);

        string metric = "";
        string precision = "";

        var props = jObject.Properties();

        foreach (var prop in props)
        {
            string propName = prop.Name.ToLower();
            string propValue = prop.Value.ToString().ToLower();

            if (propName == "metrique")
                metric = propValue;
            else if (propName == "precision")
                precision = propValue;
        }


        bool isSize = true;

        if (precision == "couleur")
            isSize = false;
        else if(precision != "taille")
        {
            LogWarning();
            return;
        }


        if(!TryParseMetric(metric, out GraphMetricType graphMetricType))
        {
            LogWarning();
            return;
        }

        GraphMetricType currentSelectedMetric = isSize ? _graphConfiguration.SelectedMetricTypeSize : _graphConfiguration.SelectedMetricTypeColor;

        aidenIntents.Add(new AIDENIntent(isSize? GraphConfigKey.SelectedMetricTypeSize : GraphConfigKey.SelectedMetricTypeColor, graphMetricType.ToString(), currentSelectedMetric.ToString()));

        void LogWarning()
        {
            Debug.LogWarning("Couldn't HandleMetriqueIntent : " + metric + " " + precision + "\n" + intent.Content);
        }


        bool TryParseMetric(string valueS, out GraphMetricType objectMetrique)
        {
            switch(valueS)
            {
                case "normal":
                    objectMetrique = GraphMetricType.None;
                    return true;
                case "degre":
                    objectMetrique = GraphMetricType.Degree;
                    return true;
                case "regroupement_local":
                    objectMetrique = GraphMetricType.LocalClusteringCoefficient;
                    return true;
                case "chemin_court":
                    objectMetrique = GraphMetricType.AverageShortestPath;
                    return true;
                case "centralite_intermediaire":
                    objectMetrique = GraphMetricType.BetweennessCentrality;
                    return true;
                case "centralite_proximite":
                    objectMetrique = GraphMetricType.ClosenessCentrality;
                    return true;
                default:
                    objectMetrique = GraphMetricType.None;
                    return false;
            }
        }

    }

    private void HandleTimeIntent(AIDENPromptPayload intent, AIDENIntents aidenIntents)
    {
        JObject jObject = JObject.Parse(intent.Content);

        string time = "";
        string valueS = "";

        var props = jObject.Properties();

        foreach (var prop in props)
        {
            string propName = prop.Name.ToLower();
            string propValue = prop.Value.ToString().ToLower();

            if (propName == "temps")
                time = propValue;
            else if (propName == "valeur")
                valueS = propValue;
        }

        if(time != "transition_graphe")
        {
            LogWarning();
            return;
        }


        if (!ParseIsAbsolute(valueS, out bool isAbsolute))
        {
            LogWarning();
            return;
        }

        if (!ParseValue(valueS, isAbsolute, out float newTimeFloat))
        {
            LogWarning();
            return;
        }

        float currentTimeFloat = _graphConfiguration.GraphModeTransitionTime;

        if (!isAbsolute)
        {
            newTimeFloat = currentTimeFloat * newTimeFloat;
        }

        aidenIntents.Add(new AIDENIntent(GraphConfigKey.GraphModeTransitionTime, newTimeFloat, currentTimeFloat));

        void LogWarning()
        {
            Debug.LogWarning("Couldn't HandleTimeIntent : " + time + " " + valueS + "\n" + intent.Content);
        }
    }

    private void HandleSimulationIntent(AIDENPromptPayload intent, AIDENIntents aidenIntents)
    {
        JObject jObject = JObject.Parse(intent.Content);

        string graphe = "";
        string property = "";
        string valueS = "";

        var props = jObject.Properties();

        foreach (var prop in props)
        {
            string propName = prop.Name.ToLower();
            string propValue = prop.Value.ToString().ToLower();

            if (propName == "graphe")
                graphe = propValue;
            else if (propName == "propriete")
                property = propValue;
            else if (propName == "valeur")
                valueS = propValue;
        }

        bool isDefault = true;
        if (graphe == "loupe")
            isDefault = false;

        if(!TryExtractProperty(property, out SimuProperty simuProperty))
        {
            LogWarning();
            return;
        }

        if (!ParseIsAbsolute(valueS, out bool isAbsolute))
        {
            LogWarning();
            return;
        }

        if (!ParseValue(valueS, isAbsolute, out float newSimuFloat))
        {
            LogWarning();
            return;
        }


        float currentSimuFloat = GetSimuValue(isDefault, simuProperty);

        if (!isAbsolute)
        {
            newSimuFloat = currentSimuFloat * newSimuFloat;
        }


        aidenIntents.Add(new AIDENIntent(GetSimuKey(isDefault, simuProperty), newSimuFloat, currentSimuFloat));


        void LogWarning()
        {
            Debug.LogWarning("Couldn't HandleTimeIntent : " + graphe + " " + property + " " +  valueS  + "\n" + intent.Content);
        }

        bool TryExtractProperty(string property, out SimuProperty simuProperty)
        {
            switch(property)
            {
                case "force_ressort":
                    simuProperty = SimuProperty.ForceSpring;
                    return true;
                case "force_coulomb":
                    simuProperty = SimuProperty.ForceCoulomb;
                    return true;
                case "amortissement":
                    simuProperty = SimuProperty.Damping;
                    return true;
                case "distance_ressort":
                    simuProperty = SimuProperty.DistanceSpring;
                    return true;
                case "distance_coulomb":
                    simuProperty = SimuProperty.DistanceCoulomb;
                    return true;
                default:
                    simuProperty = SimuProperty.ForceSpring;
                    return false;
            }
        }

        float GetSimuValue(bool isDefault, SimuProperty simuproperty)
        {
            switch (simuProperty)
            {
                case SimuProperty.ForceSpring:
                    return isDefault? _graphConfiguration.SimuParameters.SpringForce : _graphConfiguration.LensSimuParameters.SpringForce;
                case SimuProperty.ForceCoulomb:
                    return isDefault ? _graphConfiguration.SimuParameters.CoulombForce : _graphConfiguration.LensSimuParameters.CoulombForce;
                case SimuProperty.Damping:
                    return isDefault ? _graphConfiguration.SimuParameters.Damping : _graphConfiguration.LensSimuParameters.Damping;
                case SimuProperty.DistanceSpring:
                    return isDefault ? _graphConfiguration.SimuParameters.SpringDistance : _graphConfiguration.LensSimuParameters.SpringDistance;
                case SimuProperty.DistanceCoulomb:
                    return isDefault ? _graphConfiguration.SimuParameters.CoulombDistance : _graphConfiguration.LensSimuParameters.CoulombDistance;
                case SimuProperty.MaxVelocity:
                    return isDefault ? _graphConfiguration.SimuParameters.MaxVelocity : _graphConfiguration.LensSimuParameters.MaxVelocity;
                case SimuProperty.StopVelocity:
                    return isDefault ? _graphConfiguration.SimuParameters.StopVelocity : _graphConfiguration.LensSimuParameters.StopVelocity;
                default:
                    return isDefault ? _graphConfiguration.SimuParameters.SpringForce : _graphConfiguration.LensSimuParameters.SpringForce;
            }
        }

        GraphConfigKey GetSimuKey(bool isDefault, SimuProperty simuproperty)
        {
            switch (simuProperty)
            {
                case SimuProperty.ForceSpring:
                    return isDefault ? GraphConfigKey.DefaultSpringForce: GraphConfigKey.LensSpringForce;
                case SimuProperty.ForceCoulomb:
                    return isDefault ? GraphConfigKey.DefaultCoulombForce : GraphConfigKey.LensCoulombForce;
                case SimuProperty.Damping:
                    return isDefault ? GraphConfigKey.DefaultDamping : GraphConfigKey.LensDamping;
                case SimuProperty.DistanceSpring:
                    return isDefault ? GraphConfigKey.DefaultSpringDistance : GraphConfigKey.LensSpringDistance;
                case SimuProperty.DistanceCoulomb:
                    return isDefault ? GraphConfigKey.DefaultCoulombDistance : GraphConfigKey.LensCoulombDistance;
                case SimuProperty.MaxVelocity:
                    return isDefault ? GraphConfigKey.DefaultMaxVelocity : GraphConfigKey.LensMaxVelocity;
                case SimuProperty.StopVelocity:
                    return isDefault ? GraphConfigKey.DefaultStopVelocity : GraphConfigKey.LensStopVelocity;
                default:
                    return isDefault ? GraphConfigKey.DefaultSpringForce : GraphConfigKey.LensSpringForce;
            }
        }

    }

    private void HandleActionIntent(AIDENPromptPayload intent, AIDENIntents aidenIntents)
    {
        JObject jObject = JObject.Parse(intent.Content);

        string action = "";


        var props = jObject.Properties();

        foreach (var prop in props)
        {
            string propName = prop.Name.ToLower();
            string propValue = prop.Value.ToString().ToLower();

            if (propName == "action")
                action = propValue;
        }

        if(!TryParseAction(action, out GraphActionKey actionKey))
        {
            LogWarning();
            return;
        }

        // TODO : check if can execute ation here 
        // return;


        aidenIntents.Add(new AIDENIntent(actionKey));


        void LogWarning()
        {
            Debug.LogWarning("Couldn't HandleTimeIntent : " + action + "\n" + intent.Content);
        }

        bool TryParseAction(string action, out GraphActionKey actionType)
        {
            switch(action) 
            {
                case "relance_graphe_simulation":
                    actionType = GraphActionKey.Simulate;
                    return true;
                case "filtre_selection":
                    actionType = GraphActionKey.FilterSelected;
                    return true;
                case "filtre_voisin":
                    actionType = GraphActionKey.FilterPropagated;
                    return true;
                case "filtre_non_selection":
                    actionType = GraphActionKey.FilterUnselected;
                    return true;
                case "filtre_non_voisin":
                    actionType = GraphActionKey.FilterUnpropagated;
                    return true;
                case "annuler_filtre":
                    actionType = GraphActionKey.UndoFilter;
                    return true;
                case "retablir_filtre":
                    actionType = GraphActionKey.RedoFilter;
                    return true;
                default:
                    actionType = GraphActionKey.Simulate;
                    return false;
            }
        }

    }

    private ChatCompletionsOptions GetChat(AIDENPrompts aidenPrompts, string transcribedText)
    {
        string asistantPrompt = _promptsDict[aidenPrompts];

        var chat = new ChatCompletionsOptions()
        {
            Messages =
            {
                new ChatMessage(ChatRole.Assistant, asistantPrompt),
                new ChatMessage(ChatRole.User, transcribedText)
            }
        };

        chat.Temperature = 0;
        chat.MaxTokens = 256;

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
        catch 
        {
            return ("", "");
        }
    }

    [ContextMenu("RefreshPrompts")]
    private void RefreshPrompts()
    {
        JsonToEnum.RefreshEnum(_prompts);
    }

    private bool ParseValue(string valueS, bool isAbsolute, out float valueF)
    {
        valueS = valueS.Replace(".", ",");
        if (isAbsolute)
        {
            return float.TryParse(valueS, out valueF);
        }

        if (valueS.Contains("%"))
        {
            valueS = valueS.Replace("%", "");

            if (!float.TryParse(valueS, out valueF))
            {
                return false;
            }

            valueF *= 0.01f;
            return true;
        }
        else
        {
            return float.TryParse(valueS, out valueF);
        }
    }

    private bool ParseIsAbsolute(string value, out bool isAbsolute)
    {
        isAbsolute = true;

        if (value.Contains("%"))
        {
            isAbsolute = false;
            return true;
        }

        return true;
    }


    enum SimuProperty
    {
        ForceSpring,
        ForceCoulomb,
        Damping,
        DistanceSpring,
        DistanceCoulomb,
        MaxVelocity,
        StopVelocity
    }
}