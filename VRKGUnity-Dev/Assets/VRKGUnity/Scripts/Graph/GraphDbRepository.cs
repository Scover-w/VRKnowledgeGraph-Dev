using System;
using System.Collections;


[Serializable]
public class GraphDbRepository
{
    public string ServerURL;
    public string RepositoryId;

    public GraphDbRepository(string serverURL, string repositoryId)
    {
        ServerURL = serverURL;
        RepositoryId = repositoryId;
    }

}
