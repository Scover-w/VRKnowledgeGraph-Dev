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


    // TODO : Place in a more obfuscated file
    public string EncryptedUsername;
    public string EncryptedPassword;

    public string StartQuery;


    [JsonIgnore]
    public string PathRepo { get; private set; }


    public GraphDbRepository(string graphDbUrl, string graphDbRepoId, string encryptedUsername, string encryptedPassword)
    {
        GraphDbUrl = graphDbUrl;
        GraphDbRepositoryId = graphDbRepoId;

        EncryptedUsername = encryptedUsername;
        EncryptedPassword = encryptedPassword;

        GraphDBAPI = new GraphDBAPI(this);

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
