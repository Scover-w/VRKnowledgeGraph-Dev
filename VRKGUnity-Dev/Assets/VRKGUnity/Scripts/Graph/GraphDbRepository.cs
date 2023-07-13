using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class GraphDbRepository
{
    public GraphDbRepositoryNamespaces GraphDbRepositoryNamespaces { get; private set; }
    public GraphDbRepositoryDistantUris GraphDbRepositoryDistantUris { get; private set; }

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


#if PLATFORM_ANDROID && !UNITY_EDITOR
        PathRepo = Path.Combine(Application.persistentDataPath, "VRKGUnity", "Data", RepositoryId + "_" + Mathf.Abs(ServerURL.GetHashCode()));
#else
        PathRepo = Path.Combine(Application.dataPath, "VRKGUnity", "Data", RepositoryId + "_" + Mathf.Abs(ServerURL.GetHashCode()));
#endif


        if (!Directory.Exists(PathRepo))
            Directory.CreateDirectory(PathRepo);
    }


    public async Task LoadChilds()
    {
        GraphDbRepositoryNamespaces = await GraphDbRepositoryNamespaces.Load(PathRepo);

        GraphDbRepositoryDistantUris = await GraphDbRepositoryDistantUris.LoadAsync(PathRepo);
    }
}
