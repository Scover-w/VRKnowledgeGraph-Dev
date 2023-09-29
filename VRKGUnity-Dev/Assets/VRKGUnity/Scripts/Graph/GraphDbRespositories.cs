using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;


/// <summary>
/// JSONAble. Store the list of all the saved repositories in the app.
/// </summary>
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

    [JsonIgnore]
    public int LastSelectedId { get { return _lastSelectedId; } }

    [JsonIgnore]
    public IReadOnlyList<GraphDbRepository> Repositories => _repositories;

    [JsonProperty("Repositories_")]
    private List<GraphDbRepository> _repositories;

    [JsonProperty("LastSelectedId_")]
    private int _lastSelectedId = -1;

    [JsonIgnore]
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

            DebugDev.Log("GraphDbRepositories");
            DebugDev.Log(json);

            var repositories = await JsonConvertHelper.DeserializeObjectAsync<GraphDbRepositories>(json);

            var repos = repositories.Repositories;
            foreach (var repo in repos)
            {
                DebugDev.Log("----");
                DebugDev.Log(repo.GraphDbUrl);
                DebugDev.Log(repo.GraphDbRepositoryId);
                DebugDev.Log("----");
            }

            return repositories;
        }

        var repositoriesB = new GraphDbRepositories();

        await repositoriesB.Save();
        return repositoriesB;
    }

    public async Task Save()
    {
        string json = await JsonConvertHelper.SerializeObjectAsync(this, Formatting.Indented);
        await File.WriteAllTextAsync(_gdbRepositoriesPath, json);
    }

    private static void SetPath()
    {
        var folderPath = Path.Combine(Settings.PersistentDataPath, "Data");

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        _gdbRepositoriesPath = Path.Combine(folderPath, "GraphDbRespositories.json");

        Debug.Log(_gdbRepositoriesPath);
    }

    public async Task Add(GraphDbRepository repository)
    {
        _repositories.Add(repository);

        await Save();
    }

    public async Task Remove(GraphDbRepository repository) 
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
        await repository.DeleteFiles();

        _ = Save();
    }

    public void Select(GraphDbRepository repository)
    {
        _lastSelectedId = _repositories.IndexOf(repository);

        Debug.Log("Select Repo : " + repository.GraphDbUrl + ", " + repository.GraphDbRepositoryId);

        _ = Save();
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