using Codice.CM.Common;
using Newtonsoft.Json.Linq;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using VDS.RDF;

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
        if(!urlOmeka.Contains("/api"))
        {
            urlOmeka += ((urlOmeka[urlOmeka.Length - 1] == '/') ? "api" : "/api");
        }

        if (!urlOmeka.Contains("/items"))
        {
            urlOmeka += ((urlOmeka[urlOmeka.Length - 1] == '/') ? "items" : "/items");
        }

        var vocabulary = await RetrieveVocabulary(urlOmeka);


        var jsonData = await HttpHelper.Retrieve(urlOmeka);

        string jsonHash = jsonData.GetSha256Hash();
        Debug.Log("Hash : " +  jsonHash);


        List<RDFTriple> triples = new List<RDFTriple>();
        JToken root = JToken.Parse(jsonData);


        foreach(JToken itemR in root) 
        {
            await ExtractTriplesAsync(itemR);
        }

        Debug.Log(triples);
        

        foreach(var triple in triples) 
        {
            Debug.Log(triple.Subject + "  " + triple.Predicate + "  " + triple.Object);

            if (!triple.Predicate.StartsWith("http"))
                Debug.Log(triple);
        }

        Debug.Log("Count : " + triples.Count);

        async Task ExtractTriplesAsync(JToken item)
        {
            try
            {
                string item_uri = item["@id"].ToString();

                AddTriple(item_uri, "http://www.w3.org/2000/01/rdf-schema#label", item["o:title"].ToString());

                //if (item["o:item_set"] != null)
                //{
                //    foreach (var item_set in item["o:item_set"])
                //    {
                //        AddTriple(item_uri, "http://omeka.org/s/vocabs/o#item_set", item_set["@id"].ToString().Trim());
                //    }
                //}

                foreach (var key in item.Children<JProperty>())
                {
                    string prefix = key.Name.Split(':')[0];

                    if (key.Name.Contains(":") && !key.Name.StartsWith("o:") && !key.Name.StartsWith("o-module"))
                    {
                        if (!vocabulary.ContainsKey(prefix))
                        {
                            continue;
                        }

                        foreach (var element in key.Value)
                        {
                            string predicate = vocabulary[prefix] + key.Name.Substring(prefix.Length + 1);

                            if (element["@value"] != null)
                            {
                                AddTriple(item_uri, predicate, element["@value"].ToString());
                            }

                            if (element["@id"] != null)
                            {
                                string objectId = element["@id"].ToString().Trim();
                                if (objectId.StartsWith("http"))
                                {
                                    AddTriple(item_uri, predicate, objectId);
                                }
                                else
                                {
                                    AddTriple(item_uri, predicate, objectId);
                                }
                            }
                        }
                    }
                    else if (key.Name.Contains(":") && key.Name.StartsWith("o:media"))
                    {
                        foreach (var mediaFromItem in key.Value)
                        {
                            string urlMedia = mediaFromItem["@id"].ToString();
                            string mediaJson = await HttpHelper.Retrieve(urlMedia);

                            if (mediaJson.Length == 0)
                            {
                                continue;
                            }

                            var media = JObject.Parse(mediaJson);

                            if (media["o:original_url"] == null)
                            {
                                continue;
                            }

                            string urlPicture = media["o:original_url"].ToString();
                            string predicate = vocabulary["foaf"] + "depiction";

                            AddTriple(item_uri, predicate, urlPicture);

                            // TODO :

                            //AddTriple(item_uri, ?, urlMedia);
                            //AddTriple(urlMedia, ?, urlPicture);

                            //AddTriple(urlMedia, "http://www.w3.org/2000/01/rdf-schema#label", media["o:title"].ToString());

                            //if (media["o:source"] != null)
                            //{
                            //    AddTriple(urlMedia, "o:source", media["o:source"].ToString());
                            //}
                        }
                    }
                }

                if (item["@type"] != null)
                {
                    foreach (var type in item["@type"])
                    {
                        string prefix = type.ToString().Split(':')[0];
                        if (vocabulary.ContainsKey(prefix))
                        {
                            string obj = vocabulary[prefix] + type.ToString().Substring(prefix.Length + 1);
                            AddTriple(item_uri, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", obj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred for item with id: {item["@id"]}");
                Console.WriteLine($"Exception message: {ex.Message}");
            }
        }

        void AddTriple(string subject, string predicate, string objectValue)
        {
            triples.Add(new RDFTriple (subject,predicate,objectValue));
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


    private class RDFTriple
    {
        public string Subject;
        public string Predicate;
        public string Object;

        public RDFTriple(string subject, string predicate, string @object)
        {
            Subject = subject;
            Predicate = predicate;
            Object = @object;
        }
    }

}
