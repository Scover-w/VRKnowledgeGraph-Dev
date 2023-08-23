using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UnityEngine;

public class WhisperAPI
{
    private readonly string _apiBaseUrl = "https://api.openai.com/v1";
    private readonly string _apiKey;

    private string _dataPath;
    public static string LanguageFormat = "";

    public WhisperAPI()
    {
        _apiKey = OpenAIKey.ApiKey;
        _dataPath = Application.dataPath;
    }

    public async Task<string> TranscribeAudio(string filePath, bool retry = true)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        client.Timeout = TimeSpan.FromSeconds(20f);


        var formData = new MultipartFormDataContent();

        await using var stream = File.OpenRead(filePath);
        formData.Add(new StreamContent(stream), "file", Path.GetFileName(filePath));
        formData.Add(new StringContent("whisper-1"), "model");
        formData.Add(new StringContent("text"), "response_format");

        if (LanguageFormat.Length > 0)
            formData.Add(new StringContent(LanguageFormat), "language");

        var requestUrl = new Uri(_apiBaseUrl + "/audio/transcriptions");


        try
        {
            var response = await client.PostAsync(requestUrl, formData);

            var text = await response.Content.ReadAsStringAsync();


            if (!response.IsSuccessStatusCode)
            {


                Debug.LogError($"Failed to transcribe audio: {response.StatusCode} {response.ReasonPhrase}");
                return "";
            }

            return text;
        }
        catch (TaskCanceledException tastCalceledException)
        {
            if (retry)
            {
                string result = await TranscribeAudio(filePath, false);
                return result;
            }

            Debug.Log("Failed Whisper Transcription ");
            Debug.Log(tastCalceledException);

            return "";
        }
        catch (Exception e)
        {
            Debug.Log("Failed Whisper Transcription");
            return "";
        }
    }


    public async Task<string> TranscribeAudio(Stream stream)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        client.Timeout = TimeSpan.FromSeconds(20f);


        var formData = new MultipartFormDataContent();
        stream.Position = 0;
        formData.Add(new StreamContent(stream), "file", "audio.ogg");
        formData.Add(new StringContent("whisper-1"), "model");
        formData.Add(new StringContent("text"), "response_format");

        if (LanguageFormat.Length > 0)
            formData.Add(new StringContent(LanguageFormat), "language");

        var requestUrl = new Uri(_apiBaseUrl + "/audio/transcriptions");


        try
        {
            var response = await client.PostAsync(requestUrl, formData);

            var text = await response.Content.ReadAsStringAsync();


            if (!response.IsSuccessStatusCode)
            {
                Debug.LogError($"Failed to transcribe audio: {response.StatusCode} {response.ReasonPhrase}");
                return "";
            }

            return text;
        }
        catch (TaskCanceledException tastCalceledException)
        {
            Debug.Log("Failed Whisper Transcription ");
            Debug.Log(tastCalceledException);

            return "";
        }
        catch (Exception e)
        {
            Debug.Log("Failed Whisper Transcription");
            return "";
        }
    }

    private class Transcription
    {
        public string Text { get; set; }
    }
}