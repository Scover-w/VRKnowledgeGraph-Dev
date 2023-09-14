using Newtonsoft.Json.Linq;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class OmekaToRdf : MonoBehaviour
{
    public string UrlOmeka;

    [ContextMenu("RetrieveEditor")]
    private void RetrieveEditor()
    {
        Retrieve(UrlOmeka);
    }


    public static async void Retrieve(string urlOmeka)
    {
        var voc = await RetrieveVocabulary(urlOmeka);


        var jsonData = await HttpHelper.Retrieve(urlOmeka);

        JToken root = JToken.Parse(jsonData);
        Debug.Log(root);
        Debug.Log(jsonData);


        foreach(var item in root) 
        {
            Debug.Log(item);
        }
    }

    private static async Task<Dictionary<string,string>> RetrieveVocabulary(string urlOmeka)
    {
        Dictionary<string, string> vocabulary = new();

        if (!urlOmeka.Contains("/api/"))
            return vocabulary;


        string vocUrl = urlOmeka.Split("/api/")[0] + "/api/vocabularies";

        Debug.Log(vocUrl);

        var jsonVoc = await HttpHelper.Retrieve(vocUrl);

        JToken root = JToken.Parse(jsonVoc);


        foreach(JToken item in root) 
        {
            if (item["o:prefix"] == null || item["o:namespace_uri"] == null)
                continue;

            string prefix = item["o:prefix"].ToString();
            string namespaceUri = item["o:namespace_uri"].ToString();
            vocabulary[prefix] = namespaceUri;
        }


        return vocabulary;
    }

}
