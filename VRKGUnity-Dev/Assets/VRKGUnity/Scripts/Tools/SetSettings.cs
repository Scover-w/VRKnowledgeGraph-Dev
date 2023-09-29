using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Placed in the first loaded scene. Allow to set Settings.PersistentDataPath for threads that can't access Application.persistentDataPath
/// </summary>
[DefaultExecutionOrder(-1)]
public class SetSettings : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Settings.SetPersistentDataPath(Application.persistentDataPath);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
