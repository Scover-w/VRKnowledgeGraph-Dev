using Azure;
using Azure.AI.OpenAI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
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

    [SerializeField]
    InputPropagatorManager _propagatorManager;

    [SerializeField]
    GraphConfigurationContainerSO _graphConfigurationContainerSO;

    GraphConfiguration _graphConfiguration;

    WhisperAPI _audioToTextAPI;
    OpenAIClient _gptClient;

    Dictionary<AIDENPrompts, string> _promptsDict;

    AIDENIntents _aidenIntents;

    string _gptModelName;
    int _stopPayloadId = -1;
    int _payloadCounterId = 0;

    readonly Mutex _payloadIdsMutex = new();

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
        _payloadIdsMutex.WaitOne();
        _stopPayloadId = _payloadCounterId;
        _payloadIdsMutex.ReleaseMutex();
    }


    public void Ask(AudioClip clip)
    {
        ThreadPool.QueueUserWorkItem(CreateRawPayload, clip);
    }


    private void CreateRawPayload(object clipObj)
    {
        _payloadIdsMutex.WaitOne();
        _stopPayloadId = _payloadCounterId;
        _payloadCounterId++;
        RawPayload payload = new(_payloadCounterId, (AudioClip)clipObj);

        _payloadIdsMutex.ReleaseMutex();

        TranscribeAudio(payload);
    }


    private async void TranscribeAudio(RawPayload rawPayload)
    {
        var clip = (AudioClip)rawPayload.Payload ;
        var audioStream = clip.EncodeToOggVorbisStream(true);

        var userVoiceText = await new WhisperAPI().TranscribeAudio(audioStream);

        AIDENChainPayload payload = new(rawPayload.Id);

        if (DoNeedStopPayload(payload))
            return;

        HandleTranscribedAudio(payload, userVoiceText);
    }

    public void HandleTranscribedAudio(AIDENChainPayload payload, string userVoiceText, int flowId = 0)
    {
        SplitSentence(payload, userVoiceText, flowId);
    }

   
    private void SplitSentence(AIDENChainPayload payload, string userVoiceText, int flowId)
    {
        ManualSplitSentence(payload, userVoiceText, flowId);
        AIDENSplitSentence(payload, userVoiceText, flowId);
    }

    private void ManualSplitSentence(AIDENChainPayload payload, string userVoiceText, int flowId)
    {
        // Cheap way to detect if need to call the splitIntentTexts function that add another api call
        string separator = TryGetSeparator(userVoiceText);

        //if (separator == null)
        //{
        //    payload.UserSentences.Add(userSentence);
        //    AIDENDetectMode(payload, flowId);
        //    return;
        //}
    }

    private string TryGetSeparator(string userIntentTxt)
    {
        if (userIntentTxt.Contains(","))
        {
            if (IsCommaASeparator(userIntentTxt)) // Mean that multiple intents are in the text
                return ",";
        }

        if (userIntentTxt.Contains(" et "))
        {
            if (IsAndASeparator(userIntentTxt)) // Mean that multiple intents are in the text
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

        for (int i = 1; i < nbSplit; i++)
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
            string split = splits[i].Replace(" ", "");

            if (!StartWithNextWord(split)) // probably a verb, so a new intent
                return true;
        }

        return false;

        bool StartWithNextWord(string words)
        {
            foreach (string nextWord in nextWords)
            {
                if (words.StartsWith(nextWord))
                    return true;
            }

            return false;
        }

    }

    private async void AIDENSplitSentence(AIDENChainPayload payload, string userVoiceText, int flowId)
    {
        ChatCompletionsOptions chat = GetChat(AIDENPrompts.SeparePhrase, userVoiceText);

        Response<ChatCompletions> response = await _gptClient.GetChatCompletionsAsync(
            deploymentOrModelName: _gptModelName,
        chat);

        if (DoNeedStopPayload(payload))
            return;

        if (payload.NeedStopThisFlow(flowId)) // Impossible to enter this because it's the first AIDEN function to check if manual has a wrong value. Keep it nonetheless to understand the stop flow pattern     
            return;

        var jsonAidenAnswer = response.Value.Choices[0].Message.Content;
        DebugDev.Log(jsonAidenAnswer);

        HandleAIDENSplitSentence(payload, jsonAidenAnswer, flowId);
    }

    private void HandleAIDENSplitSentence(AIDENChainPayload payload, string jsonAidenAnswer, int flowId)
    {
        JObject jObject = JObject.Parse(jsonAidenAnswer);

        var intents = jObject.GetValue("intentions");

        if (intents == null)
        {
            HandleNoSentence();
            return;
        }
        SplitSentencesPayload splitSentences = new();

        ExtractSentences();

        int nbSplitSetences = splitSentences.Count;

        if(nbSplitSetences == 0)
        {
            HandleNoSentence();
            return;
        }

        if (payload.NeedStopThisFlow(flowId)) // Impossible to enter this because it's the first AIDEN function to check if manual has a wrong value. Keep it nonetheless to understand the stop flow pattern     
            return;


        bool isManualValueCorrect = (payload.ManualNbSplitSentences == nbSplitSetences);

        payload.MutexAIDENProperties.WaitOne();
        payload.AIDENStateAnswer[FlowStep.SplitSentence] = true;
        payload.AIDENNbSplitSentences = nbSplitSetences;
        payload.AIDENSplitSentences = splitSentences;
        payload.MutexAIDENProperties.ReleaseMutex();

        FlowStepRelativeStatus flowStatus = payload.GetRelativeFlowStatus(FlowStep.SplitSentence, out FlowStep blockedFlowState);

        if (isManualValueCorrect)
        {
            HandleCorrectManualValue();
            return;
        }

        CreateNewFlow();

        void ExtractSentences()
        {
            JArray intentArray = (JArray)intents;

            foreach (var intentSentence in intentArray)
            {
                splitSentences.Add(intentSentence.ToString());
            }
        }

        void HandleNoSentence()
        {
            DebugDev.Log("HandleAIDENSplitSentence : 0 sentences detected, stop this chainPayload.");
            StopAllFlowsPayload(payload);
        }

        void HandleCorrectManualValue()
        {
            DebugDev.Log("HandleAIDENSplitSentence Manual has successfully detected the good number of sentences.");

            if (flowStatus == FlowStepRelativeStatus.FalseBefore || flowStatus == FlowStepRelativeStatus.NullBefore)
                return;

            if (flowStatus == FlowStepRelativeStatus.NullAfter)
                return;

            if (flowStatus == FlowStepRelativeStatus.FalseAfter)
            {
                CallAfterTrue(payload, blockedFlowState, flowId);
                return;
            }

            if (flowStatus == FlowStepRelativeStatus.AllTrue)
            {
                ValidateIntent(payload);
                return;
            }
        }

        void CreateNewFlow()
        {
            // All functions called under will be canceled with the flowId
            flowId++;

            payload.SetCurrentFlowId(flowId);

            payload.ClearAIDENStateAnswerAfter(FlowStep.SplitSentence);

            DetectType(payload, splitSentences, flowId);
        }
    }


    private void DetectType(AIDENChainPayload payload, SplitSentencesPayload splitSentences, int flowId)
    {
        ManualDetectType(payload, splitSentences, flowId);
        AIDENDetectType(payload, splitSentences, flowId);
    }

    private void ManualDetectType(AIDENChainPayload payload, SplitSentencesPayload splitSentences, int flowId)
    {
        if (DoNeedStopPayload(payload))
            return;

        if (payload.NeedStopThisFlow(flowId))
            return;
    }

    private async void AIDENDetectType(AIDENChainPayload payload, SplitSentencesPayload splitSentences, int flowId)
    {
        List<Task<Response<ChatCompletions>>> tasks = new();

        payload.MutexAIDENProperties.WaitOne();
        var userSentences = splitSentences.Sentences;

        foreach (string userIntent in userSentences)
        {
            tasks.Add(_gptClient.GetChatCompletionsAsync(_gptModelName, GetChat(AIDENPrompts.DetecteGroupe, userIntent)));
        }
        payload.MutexAIDENProperties.ReleaseMutex();

        await Task.WhenAll(tasks);

        if (DoNeedStopPayload(payload))
            return;

        if (payload.NeedStopThisFlow(flowId))
            return;

        int id = 0;

        List<(string, string)> sentenceAndJson = new();

        payload.MutexAIDENProperties.WaitOne();
        foreach (var task in tasks)
        {
            sentenceAndJson.Add((userSentences[id], task.Result.Value.Choices[0].Message.Content));
            id++;
        }
        payload.MutexAIDENProperties.ReleaseMutex();

        HandleAIDENDetectType(payload, sentenceAndJson, flowId);
    }

    private void HandleAIDENDetectType(AIDENChainPayload payload, List<(string, string)> sentenceAndJson, int flowId)
    {
        List<AIDENPrompts> intentModes = new();
        DetectedTypePayload detectedTypePayload = new();

        ExtractTypes();

        if (detectedTypePayload.Count == 0)
        {
            HandleNoTypes();
            return;
        }

        if (payload.NeedStopThisFlow(flowId))
            return;

        bool isManualValueCorrect = payload.AreSameMode(intentModes);

        payload.MutexAIDENProperties.WaitOne();
        payload.AIDENTypePayload = detectedTypePayload;
        payload.AIDENStateAnswer[FlowStep.DetectType] = isManualValueCorrect;
        payload.MutexAIDENProperties.ReleaseMutex();

        FlowStepRelativeStatus flowStatus = payload.GetRelativeFlowStatus(FlowStep.DetectType, out FlowStep blockedFlowState);


        if (isManualValueCorrect)
        {
            HandleCorrectManualValue();
            return;
        }


        if(flowStatus.IsBefore()) // Means that SplitSentence has not return its values yet, so don't create a new flow, let it do it
        {
            return;
        }

        // Wrong Manual Value detected
        CreateNewFlow();

        void ExtractTypes()
        {
            foreach ((string userSentence, string jsonItentType) in sentenceAndJson)
            {
                JObject jObject = JObject.Parse(jsonItentType);

                JToken intent = jObject.GetValue("intention");

                if (intent == null)
                {
                    DebugDev.LogWarning("HandleIntentGroupDetection : intent is null");
                    continue;
                }

                if (!intent.ToString().TryParseToEnum(out AIDENPrompts intentType))
                {
                    DebugDev.LogWarning("HandleIntentGroupDetection : Couldn't convert " + intent + " to an enum.");
                    continue;
                }

                intentModes.Add(intentType);
                detectedTypePayload.Add(intentType, userSentence);
            }
        }

        void HandleNoTypes()
        {
            DebugDev.Log("HandleIntentsGroupDetection : 0 types detected");

            var relativeFlowStatus = payload.GetRelativeFlowStatus(FlowStep.DetectType, out FlowStep _);

            payload.MutexAIDENProperties.WaitOne();
            payload.AIDENStateAnswer[FlowStep.DetectType] = false;
            payload.MutexAIDENProperties.ReleaseMutex();

            if (relativeFlowStatus.IsAfter()) // Means that the AIDEN head is here, so this function has been called by AIDEN flow and not by Manual
            {
                StopAllFlowsPayload(payload);
                return;
            }

            // Means that this function has been called by a Manual Flow, so AIDENSplitSentence hasn't returned its values
            // No need to do anything here
            return;
        }

        void HandleCorrectManualValue()
        {
            DebugDev.Log("HandleAIDENDetectMode Manual has successfully detected the good number of sentences.");
            if (flowStatus.IsBefore())
                return;

            if (flowStatus == FlowStepRelativeStatus.NullAfter)
                return;

            if (flowStatus == FlowStepRelativeStatus.FalseAfter) // AidenDetectParameters return before this, so need to call him, blockedFlowState will be DetectParameters
            {
                CallAfterTrue(payload, blockedFlowState, flowId);
                return;
            }

            if (flowStatus == FlowStepRelativeStatus.AllTrue)
            {
                ValidateIntent(payload);
                return;
            }
        }

        void CreateNewFlow()
        {
            flowId++;

            payload.SetCurrentFlowId(flowId);

            payload.ClearAIDENStateAnswerAfter(FlowStep.DetectType);

            DetectParameters(payload, detectedTypePayload, flowId);
        }
    }


    #region DetectParameters
    private void DetectParameters(AIDENChainPayload payload, DetectedTypePayload detectedTypePayload, int flowId)
    {
        if (DoNeedStopPayload(payload))
            return;

        if (payload.NeedStopThisFlow(flowId))
            return;

        ManualDetectParameters(payload, detectedTypePayload, flowId);
        _ = AIDENDetectParameters(payload, detectedTypePayload, flowId);
    }

    private void ManualDetectParameters(AIDENChainPayload payload, DetectedTypePayload detectedTypePayload, int flowId)
    {
        
    }

    private async Task AIDENDetectParameters(AIDENChainPayload payload, DetectedTypePayload detectedTypePayload, int flowId)
    {
        List<Task<Response<ChatCompletions>>> tasks = new();

        payload.MutexAIDENProperties.WaitOne();
        List<TypeAndSentence> typeAndSentences = detectedTypePayload.TypeAndSentence;

        foreach(TypeAndSentence typeAndSentence in typeAndSentences)
        {
            tasks.Add(_gptClient.GetChatCompletionsAsync(_gptModelName, GetChat(typeAndSentence.Type, typeAndSentence.UserIntentSentence)));
        }
        payload.MutexAIDENProperties.ReleaseMutex();

        await Task.WhenAll(tasks);

        if (DoNeedStopPayload(payload))
            return;

        if (payload.NeedStopThisFlow(flowId))
            return;


        List<(AIDENPrompts, string)> jtypeWithJsonParameters = new();

        int i = 0;

        payload.MutexAIDENProperties.WaitOne();
        foreach (var task in tasks)
        {
            jtypeWithJsonParameters.Add((typeAndSentences[i].Type, task.Result.Value.Choices[0].Message.Content));
            i++;
        }
        payload.MutexAIDENProperties.ReleaseMutex();

        HandleAIDENDetectParameters(payload, jtypeWithJsonParameters, flowId);
    }


    private void HandleAIDENDetectParameters(AIDENChainPayload payload, List<(AIDENPrompts, string)> jsonWithparameters, int flowId)
    {
        AIDENIntents aidenIntents = new();

        ExtractIntents();

        if (aidenIntents.Intents.Count == 0)
        {
            HandleNoIntents();
            return;
        }

        aidenIntents.Order();

        foreach (var intent in aidenIntents.Intents)
        {
            DebugDev.Log(intent.ToString());
        }

        bool isManualValueCorrect = payload.AreSameIntents(aidenIntents);

        payload.MutexAIDENProperties.WaitOne();
        payload.AIDENIntents = aidenIntents;
        payload.AIDENStateAnswer[FlowStep.SplitSentence] = isManualValueCorrect;
        payload.MutexAIDENProperties.ReleaseMutex();

        FlowStepRelativeStatus flowStatus = payload.GetRelativeFlowStatus(FlowStep.SplitSentence, out FlowStep blockedFlowState);


        if (isManualValueCorrect)
        {
            HandleCorrectManualValue();
            return;
        }


        if (flowStatus.IsBefore()) // Means that AIDENSplitSentence or/and AIDENDetectType has not return their values yet, so wait them
        {
            return;
        }

        ValidateIntent(payload, false);



        void ExtractIntents()
        {
            foreach ((AIDENPrompts type, string jsonParameter) in jsonWithparameters)
            {
                switch (type)
                {
                    case AIDENPrompts.Mode:
                        HandleAIDENParametersMode(jsonParameter, aidenIntents);
                        break;
                    case AIDENPrompts.Taille:
                        HandleAIDENParametersSize(jsonParameter, aidenIntents);
                        break;
                    case AIDENPrompts.Visibilite:
                        HandleAIDENParametersVisibility(jsonParameter, aidenIntents);
                        break;
                    case AIDENPrompts.Couleur:
                        HandleAIDENParametersColor(jsonParameter, aidenIntents);
                        break;
                    case AIDENPrompts.Ontologie:
                        HandleAIDENParametersOntology(jsonParameter, aidenIntents);
                        break;
                    case AIDENPrompts.Volume:
                        HandleAIDENParametersVolume(jsonParameter, aidenIntents);
                        break;
                    case AIDENPrompts.Interaction:
                        HandleAIDENParametersInteraction(jsonParameter, aidenIntents);
                        break;
                    case AIDENPrompts.Metrique:
                        HandleAIDENParametersMetric(jsonParameter, aidenIntents);
                        break;
                    case AIDENPrompts.Temps:
                        HandleAIDENParametersTime(jsonParameter, aidenIntents);
                        break;
                    case AIDENPrompts.Physique:
                        HandleAIDENParametersSimulation(jsonParameter, aidenIntents);
                        break;
                    case AIDENPrompts.Action:
                        HandleAIDENParametersAction(jsonParameter, aidenIntents);
                        break;
                    case AIDENPrompts.SeparePhrase:
                        Debug.Log("Weird to find a split sentence type here");
                        break;
                }
            }
        }

        void HandleNoIntents()
        {
            DebugDev.Log("HandleAIDENDetectParameters : 0 intents detected");

            var relativeFlowStatus = payload.GetRelativeFlowStatus(FlowStep.DetectParameters, out FlowStep _);

            payload.MutexAIDENProperties.WaitOne();
            payload.AIDENStateAnswer[FlowStep.DetectType] = false;
            payload.MutexAIDENProperties.ReleaseMutex();

            if (relativeFlowStatus == FlowStepRelativeStatus.AllTrue) // Means that the AIDEN head is here, so this function has been called by AIDENDetectType and not by Manual
            {
                StopAllFlowsPayload(payload);
                return;
            }

            // Means that this function has been called by a Manual Flow, so AIDENSplitSentence or AIDENDetectType hasn't returned its values
            // No need to do anything here
            return;
        }

        void HandleCorrectManualValue()
        {
            DebugDev.Log("HandleAIDENDetectParameters Manual has successfully detected the good number of sentences.");   

            if (flowStatus.IsBefore())
                return;

            if (flowStatus == FlowStepRelativeStatus.AllTrue)
            {
                ValidateIntent(payload);
                return;
            }
        }
    }

    private void HandleAIDENParametersMode(string jsonParameter, AIDENIntents aidenIntents)
    {
        var properties = new HashSet<string> { "mode", "type", "precision" };
        var propertiesDict = ExtractProperties(properties, jsonParameter);

        string mode = propertiesDict["mode"];
        string type = propertiesDict["type"];
        string precision = propertiesDict["precision"];


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
                    DebugDev.Log("HandleModeIntent GraphMode has already the same value : " + currentGraphMode);
                    return;
                }


                aidenIntents.Add(new AIDENIntent(GraphConfigKey.GraphMode, GraphMode.Desk.ToString(), currentGraphMode.ToString()));
                return;
            }
            else if (type == "immersion" || type == "gps")
            {
                if (currentGraphMode == GraphMode.Immersion)
                {
                    DebugDev.Log("HandleModeIntent GraphMode has already the same value : " + currentGraphMode);
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
                    DebugDev.Log("HandleModeIntent SelectionMode has already the value " + currentSelectionMode);
                    return;
                }

                aidenIntents.Add(new AIDENIntent(GraphConfigKey.SelectionMode, SelectionMode.Single.ToString(), currentSelectionMode.ToString()));
                return;
            }
            else if (type == "multiple")
            {
                if (currentSelectionMode == SelectionMode.Multiple)
                {
                    DebugDev.Log("HandleModeIntent SelectionMode has already the value " + currentSelectionMode);
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
                DebugDev.Log("HandleModeIntent GraphMetricType " + (isSize ? "size" : "color") + " has already the value " + newMetricType);
                return;
            }

            aidenIntents.Add(new AIDENIntent(isSize? GraphConfigKey.SelectedMetricTypeSize : GraphConfigKey.SelectedMetricTypeColor, newMetricType.ToString(), currentMetricType.ToString()));
            return;
        }


        LogWarning();

        void LogWarning()
        {
            DebugDev.LogWarning("Couldn't HandleModeIntent : " + mode + " " + type + " " + precision + "\n" + jsonParameter);

        }

    }

    private void HandleAIDENParametersSize(string jsonParameter, AIDENIntents aidenIntents)
    {
        var properties = new HashSet<string> { "sujet", "sujet_type", "valeur" };
        var propertiesDict = ExtractProperties(properties, jsonParameter);

        string subject = propertiesDict["sujet"];
        string subjectType = propertiesDict["sujet_type"];
        string valueS = propertiesDict["valeur"];

        GraphName graph = GraphName.Desk;

        if (subjectType == "loupe")
            graph = GraphName.Lense;
        else if (subjectType == "immersion")
            graph = GraphName.Immersion;
        else if (subjectType == "gps")
            graph = GraphName.GPS;
        else if (subjectType != "bureau")
        {
            // TODO : Do I set the current main graph as the graph to modify ?
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
                newSize = UpdateRelativeValue(currentSize, newSize);

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
                newSize = UpdateRelativeValue(currentSize, newSize);


            aidenIntents.Add(new AIDENIntent(GetTextKey(graph), newSize, currentSize));
            return;
        }

        if (subject == "noeud")
        {
            float currentSize = GetNodeSize(graph);

            if (!isAbsolute)
                newSize = UpdateRelativeValue(currentSize, newSize);

            aidenIntents.Add(new AIDENIntent(GetNodeKey(graph), newSize, currentSize));
            return;
        }

        if (subject == "arete")
        {
            float currentSize = GetEdgeSize(graph);

            if (!isAbsolute)
                newSize = UpdateRelativeValue(currentSize, newSize);

            aidenIntents.Add(new AIDENIntent(GetEdgeKey(graph), newSize, currentSize));
            return;
        }

        LogWarning();


        void LogWarning()
        {
            DebugDev.LogWarning("Couldn't HandleSizeIntent : " + subject + " " + subjectType + " " + valueS + "\n" + jsonParameter);

        }

        float GetGraphSize(GraphName graph)
        {
            return graph switch
            {
                GraphName.Desk => _graphConfiguration.DeskGraphSize,
                GraphName.Lense => _graphConfiguration.LensGraphSize,
                GraphName.Immersion => _graphConfiguration.ImmersionGraphSize,
                GraphName.GPS => _graphConfiguration.WatchGraphSize,
                _ => _graphConfiguration.DeskGraphSize,
            };
        }

        float GetTextSize(GraphName graph)
        {
            return graph switch
            {
                GraphName.Desk => _graphConfiguration.LabelNodeSizeDesk,
                GraphName.Lense => _graphConfiguration.LabelNodeSizeLens,
                GraphName.Immersion => _graphConfiguration.LabelNodeSizeImmersion,
                _ => _graphConfiguration.LabelNodeSizeDesk,
            };
        }

        float GetNodeSize(GraphName graph)
        {
            return graph switch
            {
                GraphName.Desk => _graphConfiguration.NodeSizeDesk,
                GraphName.Lense => _graphConfiguration.NodeSizeLens,
                GraphName.Immersion => _graphConfiguration.NodeSizeImmersion,
                GraphName.GPS => _graphConfiguration.NodeSizeWatch,
                _ => _graphConfiguration.NodeSizeDesk,
            };
        }

        float GetEdgeSize(GraphName graph)
        {
            return graph switch
            {
                GraphName.Desk => _graphConfiguration.EdgeThicknessDesk,
                GraphName.Lense => _graphConfiguration.EdgeThicknessLens,
                GraphName.Immersion => _graphConfiguration.EdgeThicknessImmersion,
                GraphName.GPS => _graphConfiguration.EdgeThicknessWatch,
                _ => _graphConfiguration.EdgeThicknessDesk,
            };
        }

        GraphConfigKey GetSizeKey(GraphName graph)
        {
            return graph switch
            {
                GraphName.Desk => GraphConfigKey.DeskGraphSize,
                GraphName.Lense => GraphConfigKey.LensGraphSize,
                GraphName.Immersion => GraphConfigKey.ImmersionGraphSize,
                GraphName.GPS => GraphConfigKey.WatchGraphSize,
                _ => GraphConfigKey.DeskGraphSize,
            };
        }

        GraphConfigKey GetTextKey(GraphName graph)
        {
            return graph switch
            {
                GraphName.Desk => GraphConfigKey.LabelNodeSizeDesk,
                GraphName.Lense => GraphConfigKey.LabelNodeSizeLens,
                GraphName.Immersion => GraphConfigKey.LabelNodeSizeImmersion,
                _ => GraphConfigKey.LabelNodeSizeDesk,
            };
        }

        GraphConfigKey GetNodeKey(GraphName graph)
        {
            return graph switch
            {
                GraphName.Desk => GraphConfigKey.NodeSizeDesk,
                GraphName.Lense => GraphConfigKey.NodeSizeLens,
                GraphName.Immersion => GraphConfigKey.NodeSizeImmersion,
                GraphName.GPS => GraphConfigKey.NodeSizeWatch,
                _ => GraphConfigKey.NodeSizeDesk,
            };
        }

        GraphConfigKey GetEdgeKey(GraphName graph)
        {
            return graph switch
            {
                GraphName.Desk => GraphConfigKey.EdgeThicknessDesk,
                GraphName.Lense => GraphConfigKey.EdgeThicknessLens,
                GraphName.Immersion => GraphConfigKey.EdgeThicknessImmersion,
                GraphName.GPS => GraphConfigKey.EdgeThicknessWatch,
                _ => GraphConfigKey.EdgeThicknessDesk,
            };
        }

    }

    private void HandleAIDENParametersVisibility(string jsonParameter, AIDENIntents aidenIntents)
    {
        var properties = new HashSet<string> { "affichage", "transparence", "valeur" };
        var propertiesDict = ExtractProperties(properties, jsonParameter);

        string affichage = propertiesDict["affichage"];
        string transparency = propertiesDict["transparence"];
        string valueS = propertiesDict["valeur"];


        bool isAlpha = false;

        if (affichage.Length > 0 && transparency.Length == 0)
        {
            isAlpha = false;
        }
        else if (transparency.Length > 0 && affichage.Length == 0)
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

            if (transparency == "noeud")
                alphaKey = GraphConfigKey.AlphaNodeColorUnPropagated;
            else if (transparency == "noeud_propage")
                alphaKey = GraphConfigKey.AlphaNodeColorPropagated;
            else if (transparency == "arete")
                alphaKey = GraphConfigKey.AlphaEdgeColorUnPropagated;
            else if (transparency == "arete_propage")
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
                DebugDev.Log("HandleVisibilityIntent newDisplay has the same value " + newDisplay);
                return;
            }

            aidenIntents.Add(new AIDENIntent(displayKey, newDisplay, currentDisplay));
            return;
        }


        void LogWarning()
        {
            DebugDev.LogWarning("Couldn't HandleVisibilityIntent : " + affichage + " " + transparency + " " + valueS + "\n" + jsonParameter);
        }

        float GetCurrentAlpha(GraphConfigKey alphaKey)
        {
            return alphaKey switch
            {
                GraphConfigKey.AlphaNodeColorUnPropagated => _graphConfiguration.AlphaNodeColorUnPropagated,
                GraphConfigKey.AlphaNodeColorPropagated => _graphConfiguration.AlphaNodeColorPropagated,
                GraphConfigKey.AlphaEdgeColorUnPropagated => _graphConfiguration.AlphaEdgeColorUnPropagated,
                GraphConfigKey.AlphaEdgeColorPropagated => _graphConfiguration.AlphaEdgeColorPropagated,
                _ => _graphConfiguration.AlphaNodeColorUnPropagated,
            };
        }

        bool GetCurrentDisplay(GraphConfigKey graphConfigKey)
        {
            return graphConfigKey switch
            {
                GraphConfigKey.ShowLabelImmersion => _graphConfiguration.ShowLabelImmersion,
                GraphConfigKey.ShowLabelDesk => _graphConfiguration.ShowLabelDesk,
                GraphConfigKey.ShowLabelLens => _graphConfiguration.ShowLabelLens,
                GraphConfigKey.ShowWatch => _graphConfiguration.ShowWatch,
                GraphConfigKey.DisplayEdges => _graphConfiguration.DisplayEdges,
                GraphConfigKey.DisplayInterSelectedNeighborEdges => _graphConfiguration.DisplayInterSelectedNeighborEdges,
                _ => _graphConfiguration.ShowLabelImmersion,
            };
        }
    }

    private void HandleAIDENParametersColor(string jsonParameter, AIDENIntents aidenIntents)
    {
        var properties = new HashSet<string> { "objet", "couleur" };
        var propertiesDict = ExtractProperties(properties, jsonParameter);

        string objet = propertiesDict["objet"];
        string colorS = propertiesDict["couleur"];


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
            DebugDev.Log("HandleColorIntent newColor is the same color as the old one : " + newColor);
            return;
        }

        aidenIntents.Add(new AIDENIntent(colorKey, newColor, currentColor));


        void LogWarning()
        {
            DebugDev.LogWarning("Couldn't HandleColorIntent : " + objet + " " + colorS + "\n" + jsonParameter);
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
            return colorKey switch
            {
                GraphConfigKey.NodeColor => _graphConfiguration.NodeColor,
                GraphConfigKey.NodeColorNoValueMetric => _graphConfiguration.NodeColorNoValueMetric,
                GraphConfigKey.EdgeColor => _graphConfiguration.EdgeColor,
                GraphConfigKey.PropagatedEdgeColor => _graphConfiguration.PropagatedEdgeColor,
                GraphConfigKey.NodeColorMappingColorA => _graphConfiguration.NodeColorMapping.ColorA,
                GraphConfigKey.NodeColorMappingColorB => _graphConfiguration.NodeColorMapping.ColorB,
                GraphConfigKey.NodeColorMappingColorC => _graphConfiguration.NodeColorMapping.ColorC,
                _ => _graphConfiguration.NodeColor,
            };
        }
    }

    private void HandleAIDENParametersOntology(string jsonParameter, AIDENIntents aidenIntents)
    {
        var properties = new HashSet<string> { "objet", "valeur" };
        var propertiesDict = ExtractProperties(properties, jsonParameter);

        string objet = propertiesDict["objet"];
        string valueS = propertiesDict["valeur"];

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
            newOntologyValue = UpdateRelativeValue(curentOntologyValue, newOntologyValue);


        aidenIntents.Add(new AIDENIntent(configKeyOntology, newOntologyValue, curentOntologyValue));


        void LogWarning()
        {
            DebugDev.LogWarning("Couldn't HandleOntologyIntent : " + objet + " " + valueS + "\n" + jsonParameter);
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
            return graphConfigKey switch
            {
                GraphConfigKey.NbOntologyColor => _graphConfiguration.NbOntologyColor,
                GraphConfigKey.MaxDeltaOntologyAlgo => _graphConfiguration.MaxDeltaOntologyAlgo,
                GraphConfigKey.SaturationOntologyColor => _graphConfiguration.SaturationOntologyColor,
                GraphConfigKey.ValueOntologyColor => _graphConfiguration.ValueOntologyColor,
                _ => _graphConfiguration.NbOntologyColor,
            };
        }

    }

    private void HandleAIDENParametersVolume(string jsonParameter, AIDENIntents aidenIntents)
    {
        var properties = new HashSet<string> { "objet", "valeur" };
        var propertiesDict = ExtractProperties(properties, jsonParameter);

        string objet = propertiesDict["objet"];
        string valueS = propertiesDict["valeur"];


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
            DebugDev.LogWarning("Couldn't HandleVolumeIntent : " + objet + " " + valueS + "\n" + jsonParameter);
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
            return graphConfigKey switch
            {
                GraphConfigKey.GlobalVolume => _graphConfiguration.GlobalVolume,
                GraphConfigKey.SoundEffectVolume => _graphConfiguration.SoundEffectVolume,
                GraphConfigKey.MusicVolume => _graphConfiguration.MusicVolume,
                GraphConfigKey.AidenVolume => _graphConfiguration.AidenVolume,
                _ => _graphConfiguration.GlobalVolume,
            };
        }

    }

    private void HandleAIDENParametersInteraction(string jsonParameter, AIDENIntents aidenIntents)
    {
        var properties = new HashSet<string> { "objet", "valeur" };
        var propertiesDict = ExtractProperties(properties, jsonParameter);

        string objet = propertiesDict["objet"];
        string valueS = propertiesDict["valeur"];


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
            DebugDev.Log("HandleInteractionIntent : CanSelectEdges has already the same value : " + newCanSelect);
            return;
        }


        aidenIntents.Add(new AIDENIntent(GraphConfigKey.CanSelectEdges, newCanSelect, curentCanSelectEdge));


        void LogWarning()
        {
            DebugDev.LogWarning("Couldn't HandleInteractionIntent : " + objet + " " + valueS + "\n" + jsonParameter);
        }

    }

    private void HandleAIDENParametersMetric(string jsonParameter, AIDENIntents aidenIntents)
    {
        var properties = new HashSet<string> { "metrique", "precision" };
        var propertiesDict = ExtractProperties(properties, jsonParameter);

        string metric = propertiesDict["metrique"];
        string precision = propertiesDict["precision"];

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
            DebugDev.LogWarning("Couldn't HandleMetriqueIntent : " + metric + " " + precision + "\n" + jsonParameter);
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

    private void HandleAIDENParametersTime(string jsonParameter, AIDENIntents aidenIntents)
    {
        var properties = new HashSet<string> { "temps", "valeur" };
        var propertiesDict = ExtractProperties(properties, jsonParameter);

        string time = propertiesDict["temps"];
        string valueS = propertiesDict["valeur"];


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
            newTimeFloat = UpdateRelativeValue(currentTimeFloat, newTimeFloat);

        aidenIntents.Add(new AIDENIntent(GraphConfigKey.GraphModeTransitionTime, newTimeFloat, currentTimeFloat));

        void LogWarning()
        {
            DebugDev.LogWarning("Couldn't HandleTimeIntent : " + time + " " + valueS + "\n" + jsonParameter);
        }
    }

    private void HandleAIDENParametersSimulation(string jsonParameter, AIDENIntents aidenIntents)
    {
        var properties = new HashSet<string> { "graphe", "propriete", "valeur" };
        var propertiesDict = ExtractProperties(properties, jsonParameter);

        string graphe = propertiesDict["graphe"];
        string property = propertiesDict["propriete"];
        string valueS = propertiesDict["valeur"];

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
            newSimuFloat = UpdateRelativeValue(currentSimuFloat, newSimuFloat);


        aidenIntents.Add(new AIDENIntent(GetSimuKey(isDefault, simuProperty), newSimuFloat, currentSimuFloat));


        void LogWarning()
        {
            DebugDev.LogWarning("Couldn't HandleTimeIntent : " + graphe + " " + property + " " +  valueS  + "\n" + jsonParameter);
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
            return simuProperty switch
            {
                SimuProperty.ForceSpring => isDefault ? _graphConfiguration.SimuParameters.SpringForce : _graphConfiguration.LensSimuParameters.SpringForce,
                SimuProperty.ForceCoulomb => isDefault ? _graphConfiguration.SimuParameters.CoulombForce : _graphConfiguration.LensSimuParameters.CoulombForce,
                SimuProperty.Damping => isDefault ? _graphConfiguration.SimuParameters.Damping : _graphConfiguration.LensSimuParameters.Damping,
                SimuProperty.DistanceSpring => isDefault ? _graphConfiguration.SimuParameters.SpringDistance : _graphConfiguration.LensSimuParameters.SpringDistance,
                SimuProperty.DistanceCoulomb => isDefault ? _graphConfiguration.SimuParameters.CoulombDistance : _graphConfiguration.LensSimuParameters.CoulombDistance,
                SimuProperty.MaxVelocity => isDefault ? _graphConfiguration.SimuParameters.MaxVelocity : _graphConfiguration.LensSimuParameters.MaxVelocity,
                SimuProperty.StopVelocity => isDefault ? _graphConfiguration.SimuParameters.StopVelocity : _graphConfiguration.LensSimuParameters.StopVelocity,
                _ => isDefault ? _graphConfiguration.SimuParameters.SpringForce : _graphConfiguration.LensSimuParameters.SpringForce,
            };
        }

        GraphConfigKey GetSimuKey(bool isDefault, SimuProperty simuproperty)
        {
            return simuProperty switch
            {
                SimuProperty.ForceSpring => isDefault ? GraphConfigKey.DefaultSpringForce : GraphConfigKey.LensSpringForce,
                SimuProperty.ForceCoulomb => isDefault ? GraphConfigKey.DefaultCoulombForce : GraphConfigKey.LensCoulombForce,
                SimuProperty.Damping => isDefault ? GraphConfigKey.DefaultDamping : GraphConfigKey.LensDamping,
                SimuProperty.DistanceSpring => isDefault ? GraphConfigKey.DefaultSpringDistance : GraphConfigKey.LensSpringDistance,
                SimuProperty.DistanceCoulomb => isDefault ? GraphConfigKey.DefaultCoulombDistance : GraphConfigKey.LensCoulombDistance,
                SimuProperty.MaxVelocity => isDefault ? GraphConfigKey.DefaultMaxVelocity : GraphConfigKey.LensMaxVelocity,
                SimuProperty.StopVelocity => isDefault ? GraphConfigKey.DefaultStopVelocity : GraphConfigKey.LensStopVelocity,
                _ => isDefault ? GraphConfigKey.DefaultSpringForce : GraphConfigKey.LensSpringForce,
            };
        }

    }

    private void HandleAIDENParametersAction(string jsonParameter, AIDENIntents aidenIntents)
    {
        var properties = new HashSet<string> { "action"};
        var propertiesDict = ExtractProperties(properties, jsonParameter);

        string action = propertiesDict["action"];

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
            DebugDev.LogWarning("Couldn't HandleTimeIntent : " + action + "\n" + jsonParameter);
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
    #endregion

    private void StopAllFlowsPayload(AIDENChainPayload payload)
    {

        payload.SetCurrentFlowId(int.MaxValue);

        

        // TODO : Feedback UI, no intent detected
    }

    private void CallAfterTrue(AIDENChainPayload payload, FlowStep flowStep, int flowId)
    {
        if(flowStep == FlowStep.DetectType)
        {
            payload.MutexAIDENProperties.WaitOne();
            SplitSentencesPayload splitSentences = payload.AIDENSplitSentences;


            if(splitSentences == null)
            {
                Debug.LogWarning("CallFalseAbove SplitSentencesPayload is null");
                payload.MutexAIDENProperties.ReleaseMutex();
                return;
            }
            payload.MutexAIDENProperties.ReleaseMutex();

            DetectType(payload, splitSentences, flowId);
            return;
        }


        if(flowStep == FlowStep.DetectParameters)
        {
            payload.MutexAIDENProperties.WaitOne();
            DetectedTypePayload detectedTypePayload = payload.AIDENTypePayload;

            if (detectedTypePayload == null)
            {
                Debug.LogWarning("CallFalseAbove DetectedTypePayload is null");
                payload.MutexAIDENProperties.ReleaseMutex();
                return;
            }

            payload.MutexAIDENProperties.ReleaseMutex();

            DetectParameters(payload, detectedTypePayload, flowId);
            return;
        }


        if (flowStep == FlowStep.SplitSentence)
            Debug.LogWarning("Weird : CallFalseAbove FlowStep.SplitSentence");
    }

    private void ValidateIntent(AIDENChainPayload payload, bool isManual = true)
    {
        if(isManual)
        {
            var intents = payload.ManualIntents;

            if(intents == null)
            {
                Debug.LogWarning("Intents are null in validateIntent.");
                return;
            }

            _aidenIntents = intents;
            Debug.Log("SetIntents : " + _aidenIntents);
            //_propagatorManager.SetNewValues(_aidenIntents);
            return;
        }


        var intentsB = payload.AIDENIntents;

        if (intentsB == null)
        {
            Debug.LogWarning("intentsB are null in validateIntent.");
            return;
        }

        Debug.Log("SetIntents B : " + _aidenIntents);
        _aidenIntents = intentsB;
        //_propagatorManager.SetNewValues(_aidenIntents);
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

    private Dictionary<string, string> ExtractProperties(HashSet<string> properties, string json)
    {
        Dictionary<string, string> propertiesDict = new();

        foreach(var property in properties) 
        {
            propertiesDict.Add(property, "");
        }

        JObject jObject = JObject.Parse(json);
        var jProperties = jObject.Properties();

        foreach (var prop in jProperties)
        {
            string propName = prop.Name.ToLower();
            string propValue = prop.Value.ToString().ToLower();

            propertiesDict[propName] = propValue;
        }

        return propertiesDict;

    }

    /// <summary>
    /// Stop this payload when a new one has been created because the user talked after or canceled it
    /// </summary>
    private bool DoNeedStopPayload(AIDENChainPayload promptPayload)
    {
        bool needStop = false;
        _payloadIdsMutex.WaitOne();

        if (promptPayload.Id <= _stopPayloadId)
        {
            DebugDev.Log("Stopped Payload " + promptPayload.Id);
            needStop = true;
        }

        _payloadIdsMutex.ReleaseMutex();

        return needStop;
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

    private float UpdateRelativeValue(float currentValue, float relativeValue)
    {
        if(relativeValue < 0)
        {
            relativeValue = 1f - relativeValue;
        }

        return currentValue * relativeValue;
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
