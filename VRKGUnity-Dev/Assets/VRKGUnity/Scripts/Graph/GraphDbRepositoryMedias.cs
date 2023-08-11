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
    Dictionary<string, MediaState> _medias;

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

    public async Task AddMedia(string mediaUrl, MediaState state)
    {
        if (_medias.ContainsKey(mediaUrl))
            return;

        _medias.Add(mediaUrl, state);

        await Save();
    }

    public MediaState TryGetMediaState(string mediaUrl)
    {
        if (_medias.TryGetValue(mediaUrl, out var mediaState))
            return mediaState;

        return MediaState.None;
    }

    public string GetPath(string mediaUrl)
    {
        string fileName = Path.GetFileName(mediaUrl);
        return Path.Combine(_mediaPath, fileName);
    }

    #region SAVE_LOAD
    public async static Task<GraphDbRepositoryMedias> Load(string pathRepo)
    {
        _pathRepo = pathRepo;
        SetPaths(_pathRepo);

        if (File.Exists(_fullpathFile))
        {
            string json = await File.ReadAllTextAsync(_fullpathFile);
            var repoMedias = JsonConvert.DeserializeObject<GraphDbRepositoryMedias>(json);

            return repoMedias;
        }


        var repoMediasB = new GraphDbRepositoryMedias();
        await repoMediasB.Save();
        return repoMediasB;
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

    /// <summary>
    /// /!\ Be carefull with this function, it will permanently delete all the medias and the file associated to this class
    /// </summary>
    public void DeleteMediasAndFile()
    {
        if (!Directory.Exists(_mediaPath))
            return;

        Directory.Delete(_mediaPath, true);
    }
    #endregion
}

public enum MediaState
{
    None, 
    Loadable,
    Unloadable
}