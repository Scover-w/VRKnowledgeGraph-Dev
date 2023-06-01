using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class GraphDbRepository
{
    public string ServerURL { get; private set; }
    public string RepositoryId { get; private set; }

    [JsonIgnore]
    public string PathRepo { get; private set; }

    public GraphDbRepository(string serverURL, string repositoryId)
    {
        ServerURL = serverURL;
        RepositoryId = repositoryId;

        PathRepo = Path.Combine(Application.dataPath, "VRKGUnity", "Data", RepositoryId + "_" + Mathf.Abs(ServerURL.GetHashCode()));

        if (!Directory.Exists(PathRepo))
            Directory.CreateDirectory(PathRepo);
    }


    public async Task<(GraphDbRepositoryOntology ontology,object o)> LoadChilds()
    {
        var ontology = await GraphDbRepositoryOntology.Load(PathRepo);


        return (ontology, null);

    }
}
