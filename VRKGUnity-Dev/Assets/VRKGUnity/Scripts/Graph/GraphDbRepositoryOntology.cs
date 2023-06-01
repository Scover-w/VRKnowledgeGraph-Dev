using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class GraphDbRepositoryOntology
{
    [JsonIgnore]
    public IReadOnlyDictionary<string, UserNamespceType> UserNamepsceType => _userNamepsceType;

    [JsonProperty("UserNamepsceType_")]
    Dictionary<string, UserNamespceType> _userNamepsceType;

    private static string _path;

    public GraphDbRepositoryOntology()
    {
        _userNamepsceType = new();
    }

    public void Add(string namespce)
    {
        if(!_userNamepsceType.ContainsKey(namespce))
            _userNamepsceType.Add(namespce, UserNamespceType.None);

        Save();
    }

    public void AddWithoutSave(string namespce)
    {
        if (!_userNamepsceType.ContainsKey(namespce))
            _userNamepsceType.Add(namespce, UserNamespceType.None);

    }

    public void SwitchType(string namespce, UserNamespceType type)
    {
        if(_userNamepsceType.ContainsKey(namespce)) 
        {
            _userNamepsceType[namespce] = type;
        }
    }

    public async static Task<GraphDbRepositoryOntology> Load(string path)
    {
        SetPath(path);

        if (File.Exists(_path))
        {
            string json = await File.ReadAllTextAsync(_path);
            var graphOnto = JsonConvert.DeserializeObject<GraphDbRepositoryOntology>(json);
            return graphOnto;
        }


        var graphOntoB = new GraphDbRepositoryOntology();
        await graphOntoB.Save();
        return graphOntoB;
    }

    public async Task Save()
    {
        string json = JsonConvert.SerializeObject(this, Formatting.Indented);
        await File.WriteAllTextAsync(_path, json);
    }

    private static void SetPath(string path)
    {
        _path = Path.Combine(path, "GraphDbRepositoryOntology.json");
    }

}

public enum UserNamespceType
{
    None,
    Ontology,
    DeepOntology
}
