using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class GraphDbRepository
{
    [JsonIgnore]
    public GraphDbRepositoryNamespaces GraphDbRepositoryNamespaces { get; private set; }
    [JsonIgnore]
    public GraphDbRepositoryDistantUris GraphDbRepositoryDistantUris { get; private set; }
    [JsonIgnore]
    public GraphDbRepositoryMedias GraphDbRepositoryMedias { get; private set; }

    [JsonIgnore]
    public GraphDBAPI GraphDBAPI { get; private set; }

    public string GraphDbUrl { get; private set; }
    public string GraphDbRepositoryId { get; private set; }

    public string OmekaURL;

    public string LastOmekaHash;


    [JsonIgnore]
    public string PathRepo { get; private set; }


    public GraphDbRepository(string graphDbUrl, string graphDbRepoId, string omekaURL)
    {
        GraphDbUrl = graphDbUrl;
        GraphDbRepositoryId = graphDbRepoId;
        OmekaURL = omekaURL;

        GraphDBAPI = new GraphDBAPI(this);


        PathRepo = Path.Combine(Settings.PersistentDataPath, "Data", GraphDbRepositoryId + "_" + Mathf.Abs(GraphDbUrl.GetHashCode()));


        if (!Directory.Exists(PathRepo))
            Directory.CreateDirectory(PathRepo);
        OmekaURL = omekaURL;
    }


    public async Task LoadChilds()
    {
        GraphDbRepositoryNamespaces = await GraphDbRepositoryNamespaces.Load(PathRepo);

        GraphDbRepositoryDistantUris = await GraphDbRepositoryDistantUris.Load(PathRepo);

        GraphDbRepositoryMedias = await GraphDbRepositoryMedias.Load(PathRepo);
    }


    public async Task DeleteFiles()
    {

        if(GraphDbRepositoryNamespaces != null)
            GraphDbRepositoryNamespaces = await GraphDbRepositoryNamespaces.Load(PathRepo);

        if (GraphDbRepositoryDistantUris != null)
            GraphDbRepositoryDistantUris = await GraphDbRepositoryDistantUris.Load(PathRepo);

        if (GraphDbRepositoryMedias != null)
            GraphDbRepositoryMedias = await GraphDbRepositoryMedias.Load(PathRepo);

        GraphDbRepositoryNamespaces?.DeleteFile();

        GraphDbRepositoryDistantUris?.DeleteFile();

        GraphDbRepositoryMedias?.DeleteMediasAndFile();
    }
}
