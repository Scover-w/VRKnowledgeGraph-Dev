using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class GraphDbRepositories
{
    [JsonIgnore]
    public int Count { 
        get
        {
            if (_repositories == null)
                return 0;

            return _repositories.Count;
        } 
    }

    public int LastSelectedId { get { return _lastSelectedId; } }

    [JsonIgnore]
    public IReadOnlyList<GraphDbRepository> Repositories => _repositories;

    [JsonProperty("Repositories_")]
    private List<GraphDbRepository> _repositories;

    [JsonProperty("LastSelectedId_")]
    private int _lastSelectedId = -1; 

    private static string _gdbRepositoriesPath;

    public GraphDbRepositories()
    {
        _repositories = new();
    }

    public async static Task<GraphDbRepositories> Load()
    {
        SetPath();

        if (File.Exists(_gdbRepositoriesPath))
        {
            string json = await File.ReadAllTextAsync(_gdbRepositoriesPath);
            var repositories = JsonConvert.DeserializeObject<GraphDbRepositories>(json);


            if(repositories.Repositories.Count == 0)
            {;
                repositories.Add(new("http://localhost:7200/", "cap44"));

                await repositories.Save();
            }

            return repositories;
        }


        var repositoriesB = new GraphDbRepositories();
        repositoriesB.Add(new("http://localhost:7200/", "cap44"));

        await repositoriesB.Save();
        return repositoriesB;
    }

    public async Task Save()
    {
        SetPath();

        string json = JsonConvert.SerializeObject(this, Formatting.Indented);
        await File.WriteAllTextAsync(_gdbRepositoriesPath, json);
    }

    private static void SetPath()
    {
#if PLATFORM_ANDROID && !UNITY_EDITOR
        var folderPath = Path.Combine(Application.persistentDataPath, "VRKGUnity", "Data");
#else
        var folderPath = Path.Combine(Application.dataPath, "VRKGUnity", "Data");
#endif
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        _gdbRepositoriesPath = Path.Combine(folderPath, "GraphDbRespositories.json");
    }

    public void Add(GraphDbRepository repository)
    {
        _repositories.Add(repository);

        Save();
    }

    public void Remove(GraphDbRepository repository) 
    { 
        if(_lastSelectedId != -1)
        {
            int idToRemove = _repositories.IndexOf(repository);

            if (idToRemove < _lastSelectedId)
                _lastSelectedId--;
            else if (idToRemove == _lastSelectedId)
                _lastSelectedId = -1;
        }

        _repositories.Remove(repository);
        Save();
    }

    public void Select(GraphDbRepository repository)
    {
        _lastSelectedId = _repositories.IndexOf(repository);

        Debug.Log("Select Repo : " + repository.ServerURL + " , " + repository.RepositoryId);

        Save();
    }

    public GraphDbRepository Select(int id)
    {
        if (_repositories.Count < id)
            return null;

        var repo = _repositories[id];
        Select(repo);
        return repo;
    }

    public GraphDbRepository AutoSelect()
    {
        if(_lastSelectedId != -1 && _lastSelectedId < _repositories.Count)
            return _repositories[_lastSelectedId];

        if (_repositories.Count == 0)
            return null;

        return _repositories[0];
    }
}