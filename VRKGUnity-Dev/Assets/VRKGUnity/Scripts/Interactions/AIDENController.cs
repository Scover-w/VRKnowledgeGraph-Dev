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
using UnityEngine.UIElements;
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
                case AIDENPrompts.Action:
                    HandleActionIntent(payload, aidenIntents);
                    break;
            }
        }


        // TODO : todo
        foreach(var intent in aidenIntents.Intents)
        {
            Debug.Log(intent.ToString());
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

        if( (mode == "bureau" ||  mode == "loupe" || mode == "immersion" || mode == "gps") && type.Length == 0) // Case where user say switch to desk without graph
        {
            type = mode;
            mode = "graphe";
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



        if (!ParseIsAbsolute(unitValue, out bool isAbsolute))
        {
            LogWarning();
            return;
        }

        if (!ParseValue(valueS, isAbsolute, out float valueF))
        {
            LogWarning();
            return;
        }


        if (subject == "graphe")
        {

            if (!isAbsolute)
            {
                float currentSize = GetGraphSize(graph);
                valueF = currentSize * valueF;
            }

            aidenIntents.Add(new AIDENIntent(GetSizeKey(graph), valueF));
            return;
        }

        if(subject == "texte")
        {
            if(graph == GraphName.GPS)
            {
                LogWarning();
                return;
            }

            if (!isAbsolute)
            {
                float currentSize = GetTextSize(graph);
                valueF = currentSize * valueF;
            }

            aidenIntents.Add(new AIDENIntent(GetTextKey(graph), valueF));
            return;
        }

        if (subject == "noeud")
        {
            if (!isAbsolute)
            {
                float currentSize = GetNodeSize(graph);
                valueF = currentSize * valueF;
            }

            aidenIntents.Add(new AIDENIntent(GetNodeKey(graph), valueF));
            return;
        }

        if (subject == "arete")
        {
            if (!isAbsolute)
            {
                float currentSize = GetNodeSize(graph);
                valueF = currentSize * valueF;
            }

            aidenIntents.Add(new AIDENIntent(GetEdgeKey(graph), valueF));
            return;
        }

        LogWarning();


        void LogWarning()
        {
            Debug.LogWarning("Couldn't HandleSizeIntent : " + subject + " " + subjectType + " " + valueS + " " +  unitValue + "\n" + intent.Content);

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
        string unitValue = "";
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
            else if (propName == "valeur_unite")
                unitValue = propValue;
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
            AlphaType alphaType = AlphaType.Node;

            if (transparence == "noeud")
                alphaType = AlphaType.Node;
            else if (transparence == "noeud_propage")
                alphaType = AlphaType.PropagatedNode;
            else if (transparence == "arete")
                alphaType = AlphaType.Edge;
            else if (transparence == "arete_propage")
                alphaType = AlphaType.PropagatedEdge;
            else
            {
                LogWarning();
                return;
            }


            if(!ParseIsAbsolute(unitValue, out bool isAbsolute))
            {
                LogWarning();
                return;
            }


            if(!ParseValue(valueS, isAbsolute, out float valueF))
            {
                LogWarning();
                return;
            }

            if(!isAbsolute)
            {
                float currentSize = GetAlpha(alphaType);
                valueF = currentSize * valueF;
            }

            aidenIntents.Add(new AIDENIntent(GetAlphaKey(alphaType), valueF));
            return;

        }
        else
        {
            DisplayType displayType = DisplayType.TextImmersion;

            if (affichage == "texte_immersion")
                displayType = DisplayType.TextImmersion;
            else if (affichage == "texte_bureau")
                displayType = DisplayType.TextDesk;
            else if (affichage == "texte_loupe")
                displayType = DisplayType.TextLens;
            else if (affichage == "gps")
                displayType = DisplayType.GPS;
            else
            {
                LogWarning();
                return;
            }

            bool valueB = false;

            if (valueS == "vrai")
                valueB = true;
            else if (valueS != "faux")
            {
                LogWarning();
                return;
            }

            aidenIntents.Add(new AIDENIntent(GetDisplayKey(displayType), valueB));
            return;
        }


        void LogWarning()
        {
            Debug.LogWarning("Couldn't HandleVisibilityIntent : " + affichage + " " + transparence + " " + valueS + " " + unitValue + "\n" + intent.Content);
        }

        float GetAlpha(AlphaType type)
        {
            switch (type)
            {
                case AlphaType.Node:
                    return _graphConfiguration.AlphaNodeColorUnPropagated;
                case AlphaType.PropagatedNode:
                    return _graphConfiguration.AlphaNodeColorPropagated;
                case AlphaType.Edge:
                    return _graphConfiguration.AlphaEdgeColorUnPropagated;
                case AlphaType.PropagatedEdge:
                    return _graphConfiguration.AlphaEdgeColorPropagated;
                default:
                    return _graphConfiguration.AlphaNodeColorUnPropagated;
            }
        }

        GraphConfigKey GetDisplayKey(DisplayType type)
        {
            switch (type)
            {
                case DisplayType.TextImmersion:
                    return GraphConfigKey.ShowLabelImmersion;
                case DisplayType.TextDesk:
                    return GraphConfigKey.ShowLabelDesk;
                case DisplayType.TextLens:
                    return GraphConfigKey.ShowLabelLens;
                case DisplayType.GPS:
                    return GraphConfigKey.ShowWatch;
                case DisplayType.Edge:
                    return GraphConfigKey.DisplayEdges;
                case DisplayType.InterEdgeNeighboor:
                    return GraphConfigKey.DisplayInterSelectedNeighborEdges;
                default:
                    return GraphConfigKey.ShowLabelImmersion;
            }
        }

        GraphConfigKey GetAlphaKey(AlphaType type)
        {
            switch (type)
            {
                case AlphaType.Node:
                    return GraphConfigKey.AlphaNodeColorUnPropagated;
                case AlphaType.PropagatedNode:
                    return GraphConfigKey.AlphaNodeColorPropagated;
                case AlphaType.Edge:
                    return GraphConfigKey.AlphaEdgeColorUnPropagated;
                case AlphaType.PropagatedEdge:
                    return GraphConfigKey.AlphaEdgeColorPropagated;
                default:
                    return GraphConfigKey.AlphaNodeColorUnPropagated;
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


        if(!TryGetObjectColor(objet, out ObjectColor objectColor))
        {
            LogWarning();
            return;
        }


        if (!ColorUtility.TryParseHtmlString(colorS, out Color color))
        {
            LogWarning();
            return;
        }

        aidenIntents.Add(new AIDENIntent(GetColorKey(objectColor), color));


        void LogWarning()
        {
            Debug.LogWarning("Couldn't HandleColorIntent : " + objet + " " + colorS + " " + shade + "\n" + intent.Content);
        }

        bool TryGetObjectColor(string obj, out ObjectColor objectColor)
        {
            switch (obj) 
            {
                case "noeud":
                    objectColor = ObjectColor.Node;
                    return true;

                case "noeud_sans_valeur":
                    objectColor = ObjectColor.NodeNoValue;
                    return true;

                case "arete":
                    objectColor = ObjectColor.Edge;
                    return true;

                case "arete_propage":
                    objectColor = ObjectColor.PropagatedEdge;
                    return true;

                case "noeud_gradiant_1":
                    objectColor = ObjectColor.NodeMappingA;
                    return true;

                case "noeud_gradiant_2":
                    objectColor = ObjectColor.NodeMappingB;
                    return true;

                case "noeud_gradiant_3":
                    objectColor = ObjectColor.NodeMappingC;
                    return true;
                default:
                    objectColor = ObjectColor.Node;
                    return false;
            }

            
        }

        GraphConfigKey GetColorKey(ObjectColor objectColor)
        {
            switch (objectColor)
            {
                case ObjectColor.Node:
                    return GraphConfigKey.NodeColor;
                case ObjectColor.NodeNoValue:
                    return GraphConfigKey.NodeColorNoValueMetric;
                case ObjectColor.Edge:
                    return GraphConfigKey.EdgeColor;
                case ObjectColor.PropagatedEdge:
                    return GraphConfigKey.PropagatedEdgeColor;
                case ObjectColor.NodeMappingA:
                    return GraphConfigKey.NodeColorMappingColorA;
                case ObjectColor.NodeMappingB:
                    return GraphConfigKey.NodeColorMappingColorB;
                case ObjectColor.NodeMappingC:
                    return GraphConfigKey.NodeColorMappingColorC;
                default:
                    return GraphConfigKey.NodeColor;
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

        if (!TryGetObjectOntologty(objet, out ObjectOntology objectOntology))
        {
            LogWarning();
            return;
        }

        if (!ParseValue(valueS, true, out float valueF))
        {
            LogWarning();
            return;
        }


        aidenIntents.Add(new AIDENIntent(GetOntologyKey(objectOntology), valueF));



        void LogWarning()
        {
            Debug.LogWarning("Couldn't HandleOntologyIntent : " + objet + " " + valueS + "\n" + intent.Content);
        }

        bool TryGetObjectOntologty(string objet, out ObjectOntology objectOntology)
        {
            switch (objet)
            {
                case "nombre_color":
                    objectOntology = ObjectOntology.NbColor;
                    return true;
                case "maximum_delta":
                    objectOntology = ObjectOntology.MaxDelta;
                    return true;
                case "saturation":
                    objectOntology = ObjectOntology.Saturation;
                    return true;
                case "luminosite":
                    objectOntology = ObjectOntology.Luminosity;
                    return true;
                default:
                    objectOntology = ObjectOntology.NbColor;
                    return false;
            }
        }

        GraphConfigKey GetOntologyKey(ObjectOntology objectOntology)
        {
            switch (objectOntology)
            {
                case ObjectOntology.NbColor:
                    return GraphConfigKey.NbOntologyColor;
                case ObjectOntology.MaxDelta:
                    return GraphConfigKey.MaxDeltaOntologyAlgo;
                case ObjectOntology.Saturation:
                    return GraphConfigKey.SaturationOntologyColor;
                case ObjectOntology.Luminosity:
                    return GraphConfigKey.ValueOntologyColor;
                default:
                    return GraphConfigKey.NbOntologyColor;
            }
        }

    }

    private void HandleVolumeIntent(AIDENPromptPayload intent, AIDENIntents aidenIntents)
    {
        JObject jObject = JObject.Parse(intent.Content);

        string objet = "";
        string unitValue = "";
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
            else if (propName == "valeur_unite")
                unitValue = propValue;
        }

        if (!TryParseObjectVolume(objet, out ObjectVolume objectVolume))
        {
            LogWarning();
            return;
        }

        if(!ParseIsAbsolute(unitValue, out bool isAbsolute))
        {
            LogWarning();
            return;
        }


        if (!ParseValue(valueS, isAbsolute, out float valueF))
        {
            LogWarning();
            return;
        }

        if(!isAbsolute)
        {
            float currentVolume = GetVolume(objectVolume);
            valueF = currentVolume * valueF;
        }

        aidenIntents.Add(new AIDENIntent(GetVolumeKey(objectVolume), valueF));



        void LogWarning()
        {
            Debug.LogWarning("Couldn't HandleVolumeIntent : " + objet + " " + valueS +  " " + unitValue + "\n" + intent.Content);
        }

        bool TryParseObjectVolume(string objet, out ObjectVolume objectVolume)
        {
            switch(objet)
            {
                case "global":
                    objectVolume = ObjectVolume.Global;
                    return true;
                case "effet_sonore":
                    objectVolume = ObjectVolume.SoundEffect;
                    return true;
                case "musique":
                    objectVolume = ObjectVolume.Music;
                    return true;
                case "aiden":
                    objectVolume = ObjectVolume.AIDEN;
                    return true;
                default:
                    objectVolume = ObjectVolume.Global;
                    return false;
            }
        }

        GraphConfigKey GetVolumeKey(ObjectVolume objectVolume)
        {
            switch (objectVolume)
            {
                case ObjectVolume.Global:
                    return GraphConfigKey.GlobalVolume;
                case ObjectVolume.SoundEffect:
                    return GraphConfigKey.SoundEffectVolume;
                case ObjectVolume.Music:
                    return GraphConfigKey.MusicVolume;
                case ObjectVolume.AIDEN:
                    return GraphConfigKey.AidenVolume;
                default:
                    return GraphConfigKey.GlobalVolume;
            }
        }

        float GetVolume(ObjectVolume objectVolume)
        {
            switch (objectVolume)
            {
                case ObjectVolume.Global:
                    return _graphConfiguration.GlobalVolume;
                case ObjectVolume.SoundEffect:
                    return _graphConfiguration.SoundEffectVolume;
                case ObjectVolume.Music:
                    return _graphConfiguration.MusicVolume;
                case ObjectVolume.AIDEN:
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


        bool valueB = false;

        if (valueS == "vrai")
            valueB = true;
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


        aidenIntents.Add(new AIDENIntent(GraphConfigKey.CanSelectEdges, valueB));


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


        aidenIntents.Add(new AIDENIntent(isSize? GraphConfigKey.SelectedMetricTypeSize : GraphConfigKey.SelectedMetricTypeColor, graphMetricType.ToString()));

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

        if(!ParseValue(valueS, true, out float valueF))
        {
            LogWarning();
            return;
        }
        
        aidenIntents.Add(new AIDENIntent(GraphConfigKey.GraphModeTransitionTime, valueF));

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
        string unitValue = "";
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
            else if (propName == "valeur_unite")
                unitValue = propValue;
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

        if (!ParseIsAbsolute(unitValue, out bool isAbsolute))
        {
            LogWarning();
            return;
        }

        if (!ParseValue(valueS, isAbsolute, out float valueF))
        {
            LogWarning();
            return;
        }


        if (!isAbsolute)
        {
            float currentVolume = GetSimuValue(isDefault, simuProperty);
            valueF = currentVolume * valueF;
        }


        aidenIntents.Add(new AIDENIntent(GetSimuKey(isDefault, simuProperty), valueF));


        void LogWarning()
        {
            Debug.LogWarning("Couldn't HandleTimeIntent : " + graphe + " " + property + " " + unitValue + " " + valueS  + "\n" + intent.Content);
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

    private bool ParseIsAbsolute(string unitValue, out bool isAbsolute)
    {
        isAbsolute = true;

        if (unitValue == "relatif")
        {
            isAbsolute = false;
            return true;
        }
        else if (unitValue != "absolue")
            return false;

        return true;
    }



    enum DisplayType
    {
        TextImmersion,
        TextDesk,
        TextLens,
        GPS,
        Edge,
        InterEdgeNeighboor
    }

    enum AlphaType
    {
        Node,
        PropagatedNode,
        Edge,
        PropagatedEdge
    }

    enum ObjectColor
    {
        Node,
        NodeNoValue,
        Edge,
        PropagatedEdge,
        NodeMappingA,
        NodeMappingB,
        NodeMappingC
    }

    enum ObjectOntology
    {
        NbColor,
        MaxDelta,
        Saturation,
        Luminosity
    }

    enum ObjectVolume
    {
        Global,
        SoundEffect,
        Music,
        AIDEN
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

    public AIDENIntent(GraphActionKey graphConfigKey)
    {
        IsGraphConfig = false;
        GraphActionKey = graphConfigKey;
    }

    public override string ToString()
    {
        if(IsGraphConfig)
        {
            return GraphConfigKey + "  " + ValueB + " " + ValueC + " " + ValueS + " " + ValueF;
        }
        else
        {
            return GraphActionKey.ToString();
        }
    }
}