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

    public string ServerURL { get; private set; }
    public string RepositoryId { get; private set; }

    [JsonIgnore]
    public string PathRepo { get; private set; }


    public GraphDbRepository(string serverURL, string repositoryId)
    {
        ServerURL = serverURL;
        RepositoryId = repositoryId;

        GraphDBAPI = new GraphDBAPI(this);


        PathRepo = Path.Combine(Application.persistentDataPath, "Data", RepositoryId + "_" + Mathf.Abs(ServerURL.GetHashCode()));


        if (!Directory.Exists(PathRepo))
            Directory.CreateDirectory(PathRepo);
    }


    public async Task LoadChilds()
    {
        GraphDbRepositoryNamespaces = await GraphDbRepositoryNamespaces.Load(PathRepo);

        GraphDbRepositoryDistantUris = await GraphDbRepositoryDistantUris.LoadAsync(PathRepo);

        GraphDbRepositoryMedias = await GraphDbRepositoryMedias.Load(PathRepo);
    }
}
