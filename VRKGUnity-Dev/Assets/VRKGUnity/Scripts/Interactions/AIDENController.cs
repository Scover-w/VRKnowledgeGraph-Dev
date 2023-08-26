using Azure;
using Azure.AI.OpenAI;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor.PackageManager;
using UnityEngine;
using Utilities.Encoding.OggVorbis;

public class AIDENController : MonoBehaviour
{
    [SerializeField]
    AIDENUI _aidenUI;

    WhisperAPI _audioToTextAPI;
    OpenAIClient _gptClient;

    CancellationToken _cancelationToken;

    string _gptModelName;


    [ContextMenu("Start")]
    private void Start()
    {
        _audioToTextAPI = new();


        _gptClient = new OpenAIClient(OpenAIKey.ApiKey, new OpenAIClientOptions());

        _gptModelName = "gpt-3.5-turbo";
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
        GenerateResponse(userIntentTxt);
    }

    public async void GenerateResponse(string userIntentTxt)
    {
        ChatCompletionsOptions chat = GetChat(userIntentTxt);

        Response<ChatCompletions> response = await _gptClient.GetChatCompletionsAsync(
            deploymentOrModelName: _gptModelName,
        chat);

        string json = JsonConvert.SerializeObject(response, Formatting.Indented);

        Debug.Log(json);

        var aidenAnswer = response.Value.Choices[0].Message.Content;
        HandleAIDENAnswer(aidenAnswer);
    }

    private void HandleAIDENAnswer(string answer)
    {
        Debug.Log(answer);
        AIDENIntentGroupAnswer aidenAnswer = JsonConvert.DeserializeObject<AIDENIntentGroupAnswer>(answer);

        List<string> intentions = aidenAnswer.Intentions;
        List<IntentType> intentsType = new();

        foreach(string intent in  intentions)
        {
            if (!intent.TryParseToEnum<IntentType>(out IntentType value))
            {
                Debug.LogWarning("Couldn't convert " + intent + " to an enum.");
                continue;
            }

            intentsType.Add(value);
            Debug.Log(value.ToString());
        }



    }

    private ChatCompletionsOptions GetChat(IntentType intentType, string transcribedText)
    {
        var chat = new ChatCompletionsOptions()
        {
            Messages =
            {
                new ChatMessage(ChatRole.Assistant, "L'utilisateur interagit avec l'application AIDEN, une application VR qui permet d'analyser un graphe de connaissances en 3D. Les utilisateurs peuvent modifier des param�tres ou ex�cuter des actions dans l'application. Veuillez d�tecter les intentions de l'utilisateur.\r\nR�pondez au format JSON uniquement, en utilisant la technique du Slot-Filling : Je veux que vous r�pondiez avec les param�tres suivants : objet, pr�cision objet, action, propri�t�, valeur.\r\nSeuls les objets suivants sont disponibles : graphe, noeuds, ar�te, selection, metrique, labels, volume, ontologie, simulation.\r\nSeules les actions suivantes sont disponibles : commutation, modification de la valeur, ex�cution, red�marrage.\r\nExemple :\r\n{\r\n  \"objet\": \"x\", \r\n  \"pr�cision objet\": \"x\",\r\n \"action\": \"x\",\r\n  \"propri�t�\": \"x\",\r\n  \"valeur\": \"x\" // mots interdits\r\n}"),
                new ChatMessage(ChatRole.User, transcribedText)
            }
        };

        chat.Temperature = 0;

        return chat;
    }


    private ChatCompletionsOptions GetChat(string transcribedText)
    {
        var chat = new ChatCompletionsOptions()
        {
            Messages =
            {
                new ChatMessage(ChatRole.Assistant, "Mon r�le est de d�tecter les intentions de l'utilisateur. Celui-ci peut changer des modes (MODE), changer des tailles (TAILLE), changer la visibilit� (VISIBILITE), changer les couleurs (COULEUR), changer des param�tres d'une ontologie (ONTOLOGIE), modifier le volume (VOLUME), modifier des param�tres d'interaction (INTERACTION), changer des m�triques (METRIQUE), modifier des param�tres temps (TEMPS), modifier des param�tres de physique (PHYSIQUE). R�pond en JSON, en fournissant uniquement la liste des intentions d�tect�es.\r\nExemple format :\r\n{\r\n    \"intentions\":[\"MODE\", \"METRIQUE\"]\r\n}"),
                new ChatMessage(ChatRole.User, transcribedText)
            }
        };

        chat.Temperature = 0;

        return chat;
    }

}


public enum ChatType
{
    None,
    DetectIntentGroup,
    DetectIntent
}


public enum IntentType
{
    NONE,
    MODE,
    TAILLE,
    VISIBILITE,
    COULEUR,
    ONTOLOGIE,
    VOLUME,
    INTERACTION,
    METRIQUE,
    TEMPS,
    PHYSIQUE
}


public class AIDENIntentGroupAnswer
{
    [JsonProperty("objet")]
    public List<string> Intentions;
}

public class AIDENAnswer
{
    [JsonProperty("objet")]
    public string Objet;
    [JsonProperty("pr�cision objet")]
    public string PrecisionObjet;
    [JsonProperty("action")]
    public string Action;
    [JsonProperty("propri�t�")]
    public string Propri�t�;
    [JsonProperty("valeur")]
    public string Value;
}
