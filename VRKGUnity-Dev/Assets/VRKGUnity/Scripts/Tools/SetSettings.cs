using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
