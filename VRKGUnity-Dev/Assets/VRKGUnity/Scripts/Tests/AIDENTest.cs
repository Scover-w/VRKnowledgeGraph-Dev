using Newtonsoft.Json.Linq;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIDENTest : MonoBehaviour
{
    [SerializeField]
    AIDENController _aiden;


    public string UserSentence;



    [ContextMenu("Ask")]
    public void Ask()
    {
        _aiden.DetectIntentsGroup(0, UserSentence);
    }

    [ContextMenu("Test order")]
    public void Test()
    {
        List<AIDENIntent> Intents = new();


        for (int i = 0; i < 100; i++)
        {

            bool isAction = Random.Range(0f, 1f) > .9f;

            if (isAction)
                Intents.Add(new AIDENIntent((GraphActionKey)Random.Range(0, 6)));
            else
                Intents.Add(new AIDENIntent((GraphConfigKey)Random.Range(0, 79), 2f, 2f));

        }

        foreach (AIDENIntent intent in Intents)
        {
            Debug.Log(intent.IsGraphConfig ? (int)intent.GraphConfigKey : (int)intent.GraphActionKey * 1000);
        }


        Debug.Log("-----\n\n---------");

        Intents = Intents
        .OrderBy(intent => !intent.IsGraphConfig)
        .ThenBy(intent => intent.IsGraphConfig ? (int)intent.GraphConfigKey : (int)intent.GraphActionKey)
        .ToList();

        foreach(AIDENIntent intent in Intents) 
        {
            Debug.Log(intent.IsGraphConfig ? intent.GraphConfigKey : intent.GraphActionKey);
        }
    }

    [ContextMenu("TestTest")]
    public void TestTest()
    {
        string bobo = "{\"intentions\": [{\"TAILLE\": \"Augmente le graphe bureau de 25%\"}]}";

        JObject jToken = JObject.Parse(bobo);

        (string key, string value) = GetKeyValue(jToken);

        Debug.Log(key + " " + value);   
 
    }

    private (string key, string value) GetKeyValue(JObject jObject)
    {
        try
        {
            var props = jObject.Properties();

            foreach(var prop in props) 
            {
                Debug.Log(prop);
                Debug.Log(prop.Name);
                Debug.Log(prop.Value);
            }

            string intentType = jObject.Properties().First().Name;
            string sentenceChunck = jObject[intentType].ToString();
            return (intentType, sentenceChunck);
        }
        catch (Exception ex)
        {
            return ("", "");
        }
    }

}
