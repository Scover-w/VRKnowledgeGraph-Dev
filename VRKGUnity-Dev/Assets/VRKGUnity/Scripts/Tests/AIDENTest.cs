using AIDEN.TactileUI;
using Newtonsoft.Json.Linq;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Windows;
using Random = UnityEngine.Random;

public class AIDENTest : MonoBehaviour
{
    [SerializeField]
    AIDENController _aiden;


    public string UserSentence;

    [Space(10)]
    public string SplitSentenceTest;




    [ContextMenu("Ask")]
    public void Ask()
    {

        AIDENChainPayload payload = new(0);

        _aiden.HandleTranscribedAudio(payload, UserSentence);
    }


    [ContextMenu("ManualSplitTest")]
    public void ManualSplitTest()
    {
        string[] splitByPeriod = Regex.Split(SplitSentenceTest, @"[\.,](?!\d)");

        List<string> finalList = new List<string>();

        foreach (string s in splitByPeriod)
        {
            string[] splitByAnd = Regex.Split(s.Trim(), @"\s+et\s+(?!le|la|les|l'|du|des)");
            foreach (string andSplit in splitByAnd)
            {
                string[] splitByThen = Regex.Split(andSplit.Trim(), @"\s+puis\s+");


                foreach (string thenSplit in splitByThen)
                {
                    string[] splitByOtherwise = Regex.Split(thenSplit.Trim(), @"\s+sinon\s+");
                    finalList.AddRange(splitByOtherwise);
                }

            }
        }

        foreach (string ss in finalList)
        {
            Debug.Log(ss.Trim());
        }
    }

    [ContextMenu("ManualSplitTest2")]
    public void ManualSplitTest2()
    {
        string[] delimiters = { @"[\.,](?!\d)", @"\s+et\s+(?!le|la|les|l'|du|des)", @"\s+puis\s+", @"\s+sinon\s+" };
        List<string> finalList = new List<string> { SplitSentenceTest };

        foreach (string delimiter in delimiters)
        {
            finalList = finalList
                .SelectMany(str => Regex.Split(str.Trim(), delimiter))
                .ToList();
        }

        finalList.ForEach(str => Debug.Log(ManualDetectTypeInSentence(str.Trim().ToLower().RemoveAccents()) + " -> " + str.Trim()));
    }

    private AIDENPrompts ManualDetectTypeInSentence(string sentence)
    {
        IDetectType[] detectors = { new DetectNumberType(), new DetectTimeType(),new DetectVolumeType(), new DetectSimulationType(),
                                    new DetectOntologyType(), new DetectSizeType(), new DetectActionType(), new DetectMetricType(),
                                    new DetectColorType(),new DetectVisibilityType(), new DetectInteractionType(), new DetectModeType()};

        List<AIDENPrompts> possibleAidenTypes = Enum.GetValues(typeof(AIDENPrompts))
                                        .Cast<AIDENPrompts>()
                                        .Where(e => e != AIDENPrompts.SeparePhrase && e != AIDENPrompts.DetecteGroupe)
                                        .ToList();

        foreach (var detector in detectors)
        {
            var detectedType = detector.DetectType(sentence, possibleAidenTypes);
            if (detectedType != null)
            {
                return (AIDENPrompts)detectedType;
            }
        }


        if (possibleAidenTypes.Count == 1)
            return possibleAidenTypes[0];


        return AIDENPrompts.Mode;
    }

    [ContextMenu("RegexTest")]
    public void RegexTest()
    {
        //Debug.Log(Regex.IsMatch(SplitSentenceTest, @"\b(volume|accoustique|audio|sonore|muet|le son|shorte\w*|reduc\w*)\b"));
        Debug.Log(Regex.IsMatch(SplitSentenceTest, @"\b(ontologie)\b"));

        Debug.Log("jésouis un 156.3516%** fesfs éèûûûûûû^^uuu".RemoveAccents());

        var steps = Enum.GetValues(typeof(FlowStep));
        Debug.Log(steps);
    }

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

        foreach (AIDENIntent intent in Intents)
        {
            Debug.Log(intent.IsGraphConfig ? intent.GraphConfigKey : intent.GraphActionKey);
        }
    }


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

            foreach (var prop in props)
            {
                Debug.Log(prop);
                Debug.Log(prop.Name);
                Debug.Log(prop.Value);
            }

            string intentType = jObject.Properties().First().Name;
            string sentenceChunck = jObject[intentType].ToString();
            return (intentType, sentenceChunck);
        }
        catch
        {
            return ("", "");
        }
    }




    private void TokenTest()
    {
        string test = "{\r\n  \"intentions\": [\r\n    \"Passe en mode chemin court pour la taille\",\r\n    \"Réduit la taille du graphe de 25%\",\r\n    \"Réduit la taille des arêtes de 25%\",\r\n    \"Colore les nœuds en bleu\"\r\n  ]\r\n}";

        JObject jObject = JObject.Parse(test);
        var props = jObject.Properties();

        var intents = jObject.GetValue("intentions");

        JArray intentArray = (JArray)intents;
        foreach (var intent in intentArray)
        {
            string singleIntent = intent.ToString();
            // Do something with singleIntent
        }

    }


    private void TestSplit()
    {
        string[] splits = SplitSentenceTest.Split(",");

        Debug.Log(splits.Length);

        foreach(string split in splits)
        {
            Debug.Log(split);
        }

    }
}
