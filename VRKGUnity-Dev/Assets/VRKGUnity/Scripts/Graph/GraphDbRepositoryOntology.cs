using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class GraphDbRepositoryOntology
{
    [JsonIgnore]
    public IReadOnlyDictionary<string, UserNamespceType> UserNamepsceTypes => _userNamepsceTypes;

    [JsonProperty("UserNamepsceType_")]
    Dictionary<string, UserNamespceType> _userNamepsceTypes;

    private static string _path;

    public GraphDbRepositoryOntology()
    {
        _userNamepsceTypes = new();
    }

    public void Add(string namespce)
    {
        if(!_userNamepsceTypes.ContainsKey(namespce))
            _userNamepsceTypes.Add(namespce, UserNamespceType.None);

        Save();
    }

    public void AddWithoutSave(string namespce)
    {
        if (!_userNamepsceTypes.ContainsKey(namespce))
            _userNamepsceTypes.Add(namespce, UserNamespceType.None);

    }

    public UserNamespceType SwitchType(string namespce)
    {
        if (_userNamepsceTypes.ContainsKey(namespce))
        {
            var typeEnum = _userNamepsceTypes[namespce];
            var typeInt = (int)typeEnum;
            typeInt++;

            int nbValueInEnum = Enum.GetValues(typeof(UserNamespceType)).Length;

            typeInt %= nbValueInEnum;

            var newTypeEnum = (UserNamespceType)typeInt;
            _userNamepsceTypes[namespce] = newTypeEnum;

            return newTypeEnum;
        }

        return UserNamespceType.None;
    }

    public void SwitchType(string namespce, UserNamespceType type)
    {
        if(_userNamepsceTypes.ContainsKey(namespce)) 
        {
            _userNamepsceTypes[namespce] = type;
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
    DomainOntology,
    DeepOntology
}
