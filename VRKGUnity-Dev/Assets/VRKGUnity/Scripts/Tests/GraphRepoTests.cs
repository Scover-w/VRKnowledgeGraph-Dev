using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GraphRepoTests : MonoBehaviour
{

    public string Json;

    [ContextMenu("Load")]
    private void Load()
    {
        Json = FileHelper.Load("");
    }

    [ContextMenu("Test")]
    public async Task Test()
    {
        Settings.SetPersistentDataPath(Application.persistentDataPath);

        try
        {
            var repositories = await JsonConvertHelper.DeserializeObjectAsync<GraphDbRepositories>(Json);

            Debug.Log(repositories);

        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
    }
}
