using Newtonsoft.Json;
using PlasticGui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;

public class NodeUriRetriever
{
    Dictionary<string, (string,string)> _uriLabels;
    int _nbFinishedThread;
    int _nbNodes;

    string _uriLabelPath;

    public NodeUriRetriever()
    {
        var folderPath = Path.Combine(Application.dataPath, "VRKGUnity","Data");

        if(!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        _uriLabelPath = Path.Combine(folderPath, "UriLabels.json");
        _uriLabels = LoadData();
    }


    private Dictionary<string, (string, string)> LoadData()
    {
        if (File.Exists(_uriLabelPath))
        {
            string json = File.ReadAllText(_uriLabelPath);
            var data = JsonConvert.DeserializeObject<Dictionary<string, (string, string)>>(json);
            return data ?? new Dictionary<string, (string, string)>();
        }
        else
        {
            return new Dictionary<string, (string, string)>();
        }
    }

    public async Task SaveData()
    {
        string json = JsonConvert.SerializeObject(_uriLabels, Newtonsoft.Json.Formatting.Indented);
        await File.WriteAllTextAsync(_uriLabelPath, json);
    }


    public async Task RetrieveNames(Dictionary<int, Node> idAndNodes)
    {
        idAndNodes = idAndNodes.GetNoLabeledNodes();


        if (idAndNodes.Count == 0)
            return;

        _nbFinishedThread = 0;

        _nbNodes = idAndNodes.Count;

        Debug.Log("RetrieveNames Count " + _nbNodes);

        var tasks = new List<Task>();

        SemaphoreSlim semaphore = new SemaphoreSlim(0);

        foreach (var idAndNode in idAndNodes)
        {
            tasks.Add(Task.Run(async () =>
            {
                await RetrieveName(idAndNode.Value);

                // Increment the finished thread count atomically
                if (Interlocked.Increment(ref _nbFinishedThread) == _nbNodes)
                {
                    // Signal the semaphore when all threads have finished
                    semaphore.Release();
                }
            }));
        }

        await semaphore.WaitAsync();

        await SaveData();
        Debug.Log("Finished");
    }

    private async Task RetrieveName(object obj)
    {

        try
        {
            var node = (Node)obj;


        
            lock (_uriLabels)
            {
                if (_uriLabels.TryGetValue(node.Value, out var propAndValue))
                {

                    if(propAndValue.Item1 != "-1")
                        node.Properties.Add(propAndValue.Item1, propAndValue.Item2);

                    EndThread();
                    return;
                }
            }


            string xmlContent = await RetrieveRdf(node.Value);

            if (xmlContent == null || xmlContent.Length == 0)
            {
                lock (_uriLabels)
                {
                    _uriLabels.Add(node.Value, ("-1", "-1"));
                }
                EndThread();
                return;
            }

            if (ExtractName(xmlContent, out string property, out string value))
            {
                Debug.Log("Extracted " + node.Value + "  , " + property + " " + value);
                node.Properties.Add(property, value);


                lock (_uriLabels)
                {
                    _uriLabels.Add(node.Value, (property, value));
                }
            }
            else
            {
                lock (_uriLabels)
                {
                    _uriLabels.Add(node.Value, ("-1", "-1"));
                }
            }


            EndThread();
        }
        catch(Exception ex) 
        {

        }

        void EndThread()
        {
            //_nbFinishedThread++;
        }
    }


    private async Task<string> RetrieveRdf(string uri)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);

                request.Headers.Add("Accept", "application/rdf+xml");

                HttpResponseMessage response;

                try
                {
                    response = await client.SendAsync(request);
                }
                catch(Exception e)
                {
                    return "";
                }

                if(!response.IsSuccessStatusCode) 
                {
                    return "";
                }
                
                string responseBody = await response.Content.ReadAsStringAsync();

                return responseBody;
            }

        }
        catch (Exception e)
        {
            return "";
        }
    }

    private bool ExtractName(string xmlContent, out string property, out string value)
    {
        property = "";
        value = "";


        try
        {
            using (StringReader stringReader = new StringReader(xmlContent))

            using (XmlReader xmlReader = XmlReader.Create(stringReader))
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType != XmlNodeType.Element)
                        continue;

                    var name = xmlReader.Name.ToLower();

                    if (!(name.Contains("label") || name.Contains("title") || name.Contains("name")))
                        continue;

                    property = xmlReader.Name;
                    value = xmlReader.ReadElementContentAsString();

                    return true;
                }
            }
        }
        catch(Exception e) 
        {
            return false;    
        }
        

        return false;
    }
}
