using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class GraphDbRepositoryMedias
{
    [JsonProperty("Medias_")]
    Dictionary<string, MediaData> _medias;

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

    public async Task AddMedia(string mediaUrl, MediaData mediaData)
    {
        if (_medias.ContainsKey(mediaUrl))
            return;

        _medias.Add(mediaUrl, mediaData);

        await Save();
    }

    public bool TryGetMediaData(string mediaUrl, out MediaData mediaData)
    {
        if (_medias.TryGetValue(mediaUrl, out mediaData))
            return true;

        return false;
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
            var repoMedias = await JsonConvertHelper.DeserializeObjectAsync<GraphDbRepositoryMedias>(json);

            return repoMedias;
        }


        var repoMediasB = new GraphDbRepositoryMedias();
        await repoMediasB.Save();
        return repoMediasB;
    }

    public async Task Save()
    {
        string json = await JsonConvertHelper.SerializeObjectAsync(this, Formatting.Indented);
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