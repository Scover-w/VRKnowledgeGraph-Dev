using Newtonsoft.Json;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SerializeVectorTest : MonoBehaviour
{

    [ContextMenu("Serialize")]
    public void Serialize()
    {
        var v = new Vector2TestSeria();
        v.TheV = Vector2.left;


        string path = Path.Combine(Application.persistentDataPath, "VRKGUnity-Dev", "Data", "Tests", "bipbop.json");

        string fileContent = JsonConvert.SerializeObject(v);

        var theNewV = JsonConvert.DeserializeObject<Vector2TestSeria>(fileContent);

        Debug.Log("Hope");


    }


}


public struct Vector2TestSeria
{
    public Vector2 TheV;
}
