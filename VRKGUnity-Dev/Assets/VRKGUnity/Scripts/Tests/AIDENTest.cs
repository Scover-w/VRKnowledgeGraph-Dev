using Newtonsoft.Json.Linq;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIDENTest : MonoBehaviour
{
    [SerializeField]
    AIDENController _aiden;


    public string UserSentence;

    [ContextMenu("Ask")]
    public void Ask()
    {
        _aiden.DetectIntentsGroup(UserSentence);
    }

    [ContextMenu("Test")]
    public void Test()
    {
        string json = "{ \"TAILLE\": \"augmente la taille du noeud\" }";
        JObject jToken = JObject.Parse(json);


        JObject jsonObj = JObject.Parse(json);

       
    }

    [ContextMenu("TestTest")]
    public void TestTest()
    {
        string bobo = "{\r\n    \"mode\": \"SELECTION\",\r\n    \"type\": \"MULTIPLE\"\r\n}";

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
