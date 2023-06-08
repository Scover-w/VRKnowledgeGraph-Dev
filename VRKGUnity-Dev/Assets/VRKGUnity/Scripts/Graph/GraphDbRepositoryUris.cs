using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using System.Threading.Tasks;
using UnityEngine;

public class GraphDbRepositoryUris
{
    [JsonProperty("Uris")]
    Dictionary<string, OntologyUri> _uris;

    [JsonIgnore]
    private static string _fullpathFile;

    [JsonIgnore]
    private static string _pathRepo;

    public GraphDbRepositoryUris()
    {
        _uris = new();
    }

    public async Task RetrieveNewUris(JObject data,GraphDBAPI graphDBAPI)
    {
        HashSet<string> uris = new();

        // Detect all uris
        foreach (JToken binding in data["results"]["bindings"])
        {
            string sType = binding["s"]["type"].Value<string>();
            string sValue = binding["s"]["value"].Value<string>();

            string pType = binding["p"]["type"].Value<string>();
            string pValue = binding["p"]["value"].Value<string>();

            string oType = binding["o"]["type"].Value<string>();
            string oValue = binding["o"]["value"].Value<string>();


            if (sType == "uri" && sValue.StartsWith("http"))
            {
                var uri = sValue.ExtractUri();
                uris.Add(uri.namespce);
            }

            if (pType == "uri" && pValue.StartsWith("http"))
            {
                var uri = pValue.ExtractUri();
                uris.Add(uri.namespce);
            }

            if (oType == "uri" && oValue.StartsWith("http"))
            {
                var uri = oValue.ExtractUri();
                uris.Add(uri.namespce);
            }
        }


        foreach(string uri in uris)
        {
            if(_uris.ContainsKey(uri))
            {
                continue;
            }

            // For each new uris
            var ontoUri = new OntologyUri(uri);
            await ontoUri.TryCreateOntologyAndLoadInDatabase(graphDBAPI, _pathRepo);

            _uris.Add(uri, ontoUri);
        }

        await Save();

    }


    #region SAVE_LOAD
    public async static Task<GraphDbRepositoryUris> Load(string pathRepo)
    {
        _pathRepo = pathRepo;
        SetPath(_pathRepo);

        if (File.Exists(_fullpathFile))
        {
            string json = await File.ReadAllTextAsync(_fullpathFile);
            var graphOnto = JsonConvert.DeserializeObject<GraphDbRepositoryUris>(json);

            await graphOnto.ReloadExistingOntologies();

            return graphOnto;
        }


        var graphOntoB = new GraphDbRepositoryUris();
        await graphOntoB.Save();
        return graphOntoB;
    }

    public async Task Save()
    {
        string json = JsonConvert.SerializeObject(this, Formatting.Indented);
        await File.WriteAllTextAsync(_fullpathFile, json);
    }

    private static void SetPath(string pathRepo)
    {
        _fullpathFile = Path.Combine(pathRepo, "GraphDbRepositoryUris.json");
    }

    public async Task ReloadExistingOntologies()
    {
        foreach (var uri in _uris) 
        {
            await uri.Value.ReloadOntology(_pathRepo);
        }
    }
    #endregion

}

public enum UriType
{
    None,
    Ontology
}