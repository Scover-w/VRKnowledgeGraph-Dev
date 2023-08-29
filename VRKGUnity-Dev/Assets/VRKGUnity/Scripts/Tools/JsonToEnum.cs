using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class JsonToEnum
{
    
    public static void RefreshEnum(TextAsset textAsset)
    {
        string nameFile = Path.GetFileNameWithoutExtension(textAsset.name);

        if(Path.GetExtension(textAsset.name) != ".json")
        {
            Debug.LogWarning("Only json format is currently implemented");
            return;
        }

        string path = Path.Combine(Application.dataPath, "VRKGUnity", "Scripts","Tools", nameFile + ".cs");

        string text = textAsset.text;

        var obj = JsonConvert.DeserializeObject<JObject>(text);

        var properties = obj.Properties();

        if (properties.Count() == 0)
            return;


        using StreamWriter writer = new(path);

        writer.WriteLine("public enum " + nameFile);
        writer.WriteLine("{");

        foreach (var property in properties)
        {
            string key = property.Name;
            writer.WriteLine("    " + key + ",");
        }

        writer.WriteLine("}");
    }

}
