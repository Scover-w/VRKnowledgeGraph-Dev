using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class JsonThreadTest : MonoBehaviour
{

    private void Start()
    {
        Settings.SetPersistentDataPath(Application.persistentDataPath);
    }

    public async void Test()
    {

        var repos = await Task.Run(async () =>
        {
            return await GraphDbRepositories.Load();
        });


        
    }
}
