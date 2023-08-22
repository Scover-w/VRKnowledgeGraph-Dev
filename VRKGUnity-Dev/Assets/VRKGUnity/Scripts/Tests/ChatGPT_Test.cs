using Azure.AI.OpenAI;
using Azure;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.PackageManager;

public class ChatGPT_Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [ContextMenu("Test")]
    public async void Test()
    {

        var client = new OpenAIClient(OpenAIKey.ApiKey, new OpenAIClientOptions());
        var chatCompletionsOptions = new ChatCompletionsOptions()
        {
            Messages =
            {
                new ChatMessage(ChatRole.System, "You are a helpful assistant. You will talk like a pirate."),
                new ChatMessage(ChatRole.User, "Can you help me?"),
                new ChatMessage(ChatRole.Assistant, "Arrrr! Of course, me hearty! What can I do for ye?"),
                new ChatMessage(ChatRole.User, "What's the best way to train a parrot?"),
            }
        };

        Response<StreamingChatCompletions> response = await client.GetChatCompletionsStreamingAsync(
            deploymentOrModelName: "gpt-3.5-turbo",
            chatCompletionsOptions);
        using StreamingChatCompletions streamingChatCompletions = response.Value;

        await foreach (StreamingChatChoice choice in streamingChatCompletions.GetChoicesStreaming())
        {
            await foreach (ChatMessage message in choice.GetMessageStreaming())
            {
                Debug.Log(message.Content);
            }
            
        }
    }
}
