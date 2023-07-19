using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class GraphDbRepositoryMedias
{
    [JsonProperty("Medias_")]
    Dictionary<string, Media> _medias;

    [JsonIgnore]
    private static string _fullpathFile;
    [JsonIgnore]
    private static string _pathRepo;
    [JsonIgnore]
    private static string _mediaPath;


    public GraphDbRepositoryMedias() 
    {
        _medias = new();
    }

    public async Task AddMedia(string mediaUrl)
    {

        if (_medias.ContainsKey(mediaUrl))
            return;


        _medias.Add(mediaUrl, new Media(mediaUrl));

        await Save();
    }

    #region SAVE_LOAD
    public async static Task<GraphDbRepositoryNamespaces> Load(string pathRepo)
    {
        _pathRepo = pathRepo;
        SetPaths(_pathRepo);

        if (File.Exists(_fullpathFile))
        {
            string json = await File.ReadAllTextAsync(_fullpathFile);
            var repoNamespaces = JsonConvert.DeserializeObject<GraphDbRepositoryNamespaces>(json);

            return repoNamespaces;
        }


        var repoNamespacesB = new GraphDbRepositoryNamespaces();
        await repoNamespacesB.Save();
        return repoNamespacesB;
    }

    public async Task Save()
    {
        string json = JsonConvert.SerializeObject(this, Formatting.Indented);
        await File.WriteAllTextAsync(_fullpathFile, json);
    }

    private static void SetPaths(string pathRepo)
    {
        _fullpathFile = Path.Combine(pathRepo, "GraphDbRepositoryMedias.json");

        _mediaPath = Path.Combine(pathRepo, "Medias");

        if(Directory.Exists(_mediaPath))
            return;

        Directory.CreateDirectory(_mediaPath);
    }
    #endregion
}

public class Media
{
    string _nameInFolder;

    public Media(string mediaUrl)
    {
        Guid guid = Guid.NewGuid();
        string uniqueId = guid.ToString();

        _nameInFolder = uniqueId + "_" + mediaUrl;
    }
}