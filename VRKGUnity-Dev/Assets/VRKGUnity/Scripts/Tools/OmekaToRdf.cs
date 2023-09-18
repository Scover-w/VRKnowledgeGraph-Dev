using Codice.CM.Common;
using Newtonsoft.Json.Linq;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UnityEngine;
using VDS.RDF;
using VDS.RDF.Writing;

public class OmekaToRdf : MonoBehaviour
{
    public string JsonHash;
    public string UrlOmeka;

    private string _transactionId;


    [ContextMenu("StartTransaction")]
    private async void StartTransaction()
    {
        var api = new GraphDBAPI(null);

        _transactionId = await api.StartTransaction();

        Debug.Log(_transactionId);
    }

    [ContextMenu("AbortTransaction")]
    private async void AbortTransaction()
    {
        var api = new GraphDBAPI(null);

        bool succeed = await api.AbortTransaction(_transactionId);
        Debug.Log(succeed);
    }

    [ContextMenu("ExecuteTransaction")]
    private async void ExecuteTransaction()
    {
        var api = new GraphDBAPI(null);

        bool succeed = await api.CommitTransaction(_transactionId);
        Debug.Log(succeed);
    }

    [ContextMenu("RetrieveEditor")]
    private async void RetrieveEditor()
    {
        if (JsonHash == null || JsonHash.Length == 0)
            await RetrieveFirstTime();
        else
            await Retrieve();
    }

    [ContextMenu("RetrieveFirstTime")]
    private async Task RetrieveFirstTime()
    {
        (string ttlContent, string json) = await RetrieveFirstTimeTTL(UrlOmeka);

        var path = Path.Combine(Application.persistentDataPath, "Data", "Test", "LastOmekaRetrieval.json");
        Debug.Log(path);
        await FileHelper.SaveAsync(json, path);


        JsonHash = json.GetSha256Hash();

        var api = new GraphDBAPI(null);

        var hasDeletedAll = await api.UpdateQuery("DELETE\r\nWHERE {\r\n  ?s ?p ?o .\r\n}");

        if (!hasDeletedAll)
        {
            Debug.LogError("Data couldn't be deleted");
            return;
        }

        var hasUpdated = await api.LoadFileContentInDatabase(ttlContent, "<http://data>", GraphDBAPIFileType.Turtle);

        if (!hasUpdated)
        {
            Debug.LogError("Data in the Cap44 couldn't be updated");
            return;
        }

        Debug.Log("Data in the Cap44 has been reloaded !");
    }

    [ContextMenu("Retrieve")]
    private async Task Retrieve()
    {
        var omekaUrl = FormatOmekaUrl(UrlOmeka);
        
        var currentJsonData = await HttpHelper.Retrieve(omekaUrl);

        var currentJsonHash = currentJsonData.GetSha256Hash();

        if(currentJsonHash == JsonHash)
        {
            Debug.Log("No changes detected");
            return;
        }

        var path = Path.Combine(Application.persistentDataPath, "Data", "Test", "LastOmekaRetrieval.json");
        Debug.Log(path);

        var previousJsonData = await FileHelper.LoadAsync(path);

        var vocabulary = await RetrieveVocabulary(omekaUrl);

        (var currentTriples, var currentMedias) = RetrieveRDFTriples(currentJsonData, omekaUrl, vocabulary);
        (var previousTriples, var previousMedias) = RetrieveRDFTriples(previousJsonData, omekaUrl, vocabulary);

        var (newTriples, deletedTriples) = GetTripleDiffs(currentTriples, previousTriples);
        var (newMediasDict, deletedMediasDict) = GetMediaDiffs(currentMedias, previousMedias);


        var newTripleMedias = await RetrieveMedias(newMediasDict);
        var deleteTripleMedia = MediaDictDeleteToTriples(deletedMediasDict);

        var api = new GraphDBAPI(null);


        string transactionId = await api.StartTransaction();

        bool succeedUpdating = true;

        if(transactionId == null)
        {
            Debug.LogWarning("Couldn't start transaction");
            // TODO : transaction failed to start
            return;
        }

        if (newTriples.Count > 0)
        {
            string insertTripleQuery = BuildQuery(newTriples, true);
            bool succeedA = await api.UpdateQuery(insertTripleQuery);

            succeedUpdating &= succeedA;
        }

        if (deletedTriples.Count > 0)
        {
            string deleteTripleQuery = BuildQuery(deletedTriples, false);
            bool succeedB = await api.UpdateQuery(deleteTripleQuery);

            succeedUpdating &= succeedB;
        }

        if (newTripleMedias.Count > 0)
        {
            string insertTripleMediaQuery = BuildQuery(newTripleMedias, true);
            bool succeedC = await api.UpdateQuery(insertTripleMediaQuery);

            succeedUpdating &= succeedC;
        }

        if (deleteTripleMedia.Count > 0)
        {
            string deleteTripleMediaQuery = BuildQuery(deleteTripleMedia, false);
            bool succeedD = await api.UpdateQuery(deleteTripleMediaQuery);

            succeedUpdating &= succeedD;
        }

        if(!succeedUpdating)
        {
            Debug.LogWarning("Couldn't update some query");
            await api.AbortTransaction(transactionId);
            // TODO : Couldn't update some query
            return;
        }

        bool succeedTransaction = await api.CommitTransaction(transactionId);

        if(!succeedUpdating)
        {
            Debug.LogWarning("Couldn't commit transaction");
            // TODO : Couldn't commit transaction
            return;
        }

        JsonHash = currentJsonHash;
        await FileHelper.SaveAsync(currentJsonData, path);


        (List<RDFTriple> newTriples, List<RDFTriple> deletedTriples) GetTripleDiffs(Dictionary<string, RDFTriple> currentTriples, Dictionary<string, RDFTriple> previousTriples)
        {
            List<RDFTriple> newTriples = new();
            List<RDFTriple> deletedTriples = new();

            foreach (var kvp in currentTriples)
            {
                if (previousTriples.ContainsKey(kvp.Key))
                    continue;

                newTriples.Add(kvp.Value);
            }

            foreach (var kvp in previousTriples)
            {
                if (currentTriples.ContainsKey(kvp.Key))
                    continue;

                deletedTriples.Add(kvp.Value);
            }

            return (newTriples, deletedTriples);
        }

        (Dictionary<string, List<string>> newMedias, Dictionary<string, List<string>> deletedMedias) GetMediaDiffs(Dictionary<string, List<string>> currentMedias, Dictionary<string, List<string>> previousMedias)
        {
            Dictionary<string, List<string>> newMedias = new();
            Dictionary<string, List<string>> deletedMedias = new();

            foreach (var kvp in currentMedias)
            {
                if (!previousMedias.ContainsKey(kvp.Key))
                {
                    newMedias.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (var kvp in previousMedias)
            {
                if (!currentMedias.ContainsKey(kvp.Key))
                {
                    deletedMedias.Add(kvp.Key, kvp.Value);
                }
            }

            return (newMedias, deletedMedias);
        }
    }

    private string BuildQuery(List<RDFTriple> triples, bool doInsert)
    {
        StringBuilder queryBuilder = new();

        if (doInsert)
            queryBuilder.Append("INSERT DATA { GRAPH <http://data> {");
        else
            queryBuilder.AppendLine("DELETE DATA { GRAPH <http://data> {");

        foreach (var triple in triples)
        {
            queryBuilder.AppendLine(GetStringQueryValue(triple.Subject) + " " + GetStringQueryValue(triple.Predicate) + " " + GetStringQueryValue(triple.Object) + ".");
        }

        queryBuilder.AppendLine("} }");

        return queryBuilder.ToString();



        string GetStringQueryValue(string v)
        {
            return v.StartsWith("http") ? $"<{v}>" : $"\"{v}\"";
        }
    }

    public static async Task<(string ttlContent, string json)> RetrieveFirstTimeTTL(string omekaUrl)
    {
        omekaUrl = FormatOmekaUrl(omekaUrl);

        var vocabulary = await RetrieveVocabulary(omekaUrl);
        var jsonData = await HttpHelper.Retrieve(omekaUrl);


        var graph = new VDS.RDF.Graph();

        JToken root = JToken.Parse(jsonData);

        foreach (JToken itemR in root)
        {
            await ExtractTriplesAsync(itemR);
        }


        System.IO.StringWriter textWriter = new();
        CompressingTurtleWriter turtleWriter = new();
        turtleWriter.Save(graph, textWriter);
        string ttlContent = textWriter.ToString();

        return (ttlContent, jsonData);

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
            INode s = graph.CreateUriNode(new Uri(subject));
            INode p = graph.CreateUriNode(new Uri(predicate));
            INode o = objectValue.StartsWith("http")? graph.CreateUriNode(new Uri(objectValue)) : graph.CreateLiteralNode(objectValue);

            graph.Assert(new Triple(s, p, o));
        }
    }

    public static (Dictionary<string, RDFTriple> triples, Dictionary<string, List<string>> mediaTokens) RetrieveRDFTriples(string jsonData, string omekaUrl, Dictionary<string, string> vocabulary)
    {
        Dictionary<string, RDFTriple> triples = new();
        Dictionary<string, List<string>> mediaTokens = new();


        JToken root = JToken.Parse(jsonData);


        foreach(JToken itemR in root) 
        {
            try
            {
                ExtractTriplesItem(itemR);
            }
            catch (Exception ex)
            {
                DebugDev.Log($"Exception message: {ex.Message}");
            }
        }


        return (triples, mediaTokens);

        void ExtractTriplesItem(JToken item)
        {

            string itemUri = item["@id"].ToString();

            AddTriple(itemUri, "http://www.w3.org/2000/01/rdf-schema#label", item["o:title"].ToString());

            //if (item["o:item_set"] != null)
            //    ExtractItemSet(itemUri, item);


            foreach (var key in item.Children<JProperty>())
            {
                string prefix = key.Name.Split(':')[0];

                if (key.Name.Contains(":") && !key.Name.StartsWith("o:") && !key.Name.StartsWith("o-module"))
                    ExtractTriples(prefix, itemUri, key);


                else if (key.Name.Contains(":") && key.Name.StartsWith("o:media"))
                    ExtractMedias(itemUri, key);
            }


            if (item["@type"] != null)
                ExtractType(itemUri, item);
 
        }

        void ExtractItemSet(string itemUri, JToken item)
        {
            foreach (var item_set in item["o:item_set"])
            {
                AddTriple(itemUri, "http://omeka.org/s/vocabs/o#item_set", item_set["@id"].ToString().Trim());
            }
        }

        void ExtractTriples(string prefix, string itemUri, JProperty key)
        {
            if (!vocabulary.ContainsKey(prefix))
            {
                return;
            }

            foreach (var element in key.Value)
            {
                string predicate = vocabulary[prefix] + key.Name.Substring(prefix.Length + 1);

                if (element["@value"] != null)
                {
                    AddTriple(itemUri, predicate, element["@value"].ToString());
                }

                if (element["@id"] != null)
                {
                    string objectId = element["@id"].ToString().Trim();
                    if (objectId.StartsWith("http"))
                    {
                        AddTriple(itemUri, predicate, objectId);
                    }
                    else
                    {
                        AddTriple(itemUri, predicate, objectId);
                    }
                }
            }
        }

        void ExtractMedias(string itemUri, JProperty key)
        {
            foreach (var mediaFromItem in key.Value)
            {
                string urlMedia = mediaFromItem["@id"].ToString();

                if (mediaTokens.TryGetValue(itemUri, out List<string> urlMedias))
                {
                    urlMedias.Add(urlMedia);
                    continue;
                }


                List<string> urlMediasB = new() { urlMedia };
                mediaTokens.Add(itemUri, urlMediasB);
            }
        }

        void ExtractType(string itemUri, JToken item)
        {
            foreach (var type in item["@type"])
            {
                string prefix = type.ToString().Split(':')[0];
                if (vocabulary.ContainsKey(prefix))
                {
                    string obj = vocabulary[prefix] + type.ToString().Substring(prefix.Length + 1);
                    AddTriple(itemUri, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", obj);
                }
            }
        }

        void AddTriple(string subject, string predicate, string objectValue)
        {
            triples.Add(subject + predicate + objectValue, new RDFTriple (subject,predicate,objectValue));
        }
    }

    private static async Task<Dictionary<string,string>> RetrieveVocabulary(string urlOmeka)
    {
        Dictionary<string, string> vocabulary = new();

        if (!urlOmeka.Contains("/api/"))
            return vocabulary;


        string vocUrl = urlOmeka.Split("/api/")[0] + "/api/vocabularies";

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

    private async Task<List<RDFTriple>> RetrieveMedias(Dictionary<string, List<string>> mediaUrlsDict)
    {

        List<RDFTriple> mediaTriples = new();

        foreach(var kvp in mediaUrlsDict) 
        {
            string itemUri = kvp.Key;

            List<string> mediaUrls = kvp.Value;

            foreach(string mediaUrl in mediaUrls)
            {
                string mediaJson = await HttpHelper.Retrieve(mediaUrl);

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

                AddTriple(itemUri, "http://xmlns.com/foaf/0.1/depiction", urlPicture);

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

        return mediaTriples;

        void AddTriple(string subject, string predicate, string objectValue)
        {
            mediaTriples.Add(new RDFTriple(subject, predicate, objectValue));
        }
    }

    private List<RDFTriple> MediaDictDeleteToTriples(Dictionary<string, List<string>> toDeleteMediasDict)
    {
        List<RDFTriple> mediaToDelete = new();

        foreach(var kvp in toDeleteMediasDict)
        {
            string itemUri = kvp.Key;
            List<string> mediaUrls = kvp.Value;

            foreach(string mediaUrl in mediaUrls) 
            {
                mediaToDelete.Add(new RDFTriple(itemUri, "http://xmlns.com/foaf/0.1/depiction", mediaUrl));
            }
        }

        return mediaToDelete;
    }

    private static string FormatOmekaUrl(string omekaUrl)
    {
        if (!omekaUrl.Contains("/api"))
        {
            omekaUrl += ((omekaUrl[omekaUrl.Length - 1] == '/') ? "api" : "/api");
        }

        if (!omekaUrl.Contains("/items"))
        {
            omekaUrl += ((omekaUrl[omekaUrl.Length - 1] == '/') ? "items" : "/items");
        }

        return omekaUrl;
    }

    public class RDFTriple
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
