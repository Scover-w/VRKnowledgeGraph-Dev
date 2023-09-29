using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// JSONAble. Manage a repo persistent data, with the help of <see cref="GraphDbRepositoryNamespaces"/>, <see cref="GraphDbRepositoryDistantUris"/>, <see cref="GraphDbRepositoryMedias"/>
/// </summary>
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


    // TODO : Place in a more obfuscated file
    public string EncryptedUsername { get; private set; }
    public string EncryptedPassword { get; private set; }

    public string StartQuery { get; private set; }


    [JsonIgnore]
    public string PathRepo { get; private set; }


    public GraphDbRepository(string graphDbUrl, string graphDbRepositoryId, string encryptedUsername, string encryptedPassword)
    {
        GraphDbUrl = graphDbUrl;
        GraphDbRepositoryId = graphDbRepositoryId;

        EncryptedUsername = encryptedUsername;
        EncryptedPassword = encryptedPassword;

        GraphDBAPI = new GraphDBAPI(this);

        StartQuery = "select * where { ?s ?p ?o . } limit 5000";

        PathRepo = Path.Combine(Settings.PersistentDataPath, "Data", GraphDbRepositoryId + "_" + Mathf.Abs(GraphDbUrl.GetHashCode()));


        if (!Directory.Exists(PathRepo))
            Directory.CreateDirectory(PathRepo);
    }

    public async Task SetGraphDbCredentials()
    {
        if (string.IsNullOrEmpty(EncryptedUsername) || string.IsNullOrEmpty(EncryptedPassword))
            return;

        string key = SystemInfo.deviceUniqueIdentifier + SystemInfo.graphicsDeviceID.ToString();

        string username = CryptographyHelper.Decrypt(EncryptedUsername, key);
        string password = CryptographyHelper.Decrypt(EncryptedPassword, key);

        await GraphDBAPI.SetCredentials(username, password);
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
