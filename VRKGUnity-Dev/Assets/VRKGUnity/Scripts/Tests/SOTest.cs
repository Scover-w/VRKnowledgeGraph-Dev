using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class SOTest
{
    public float FloatTest = 1f;

    public int IntTest = 1;
    public bool BoolTest = false;


    [JsonIgnore]
    public Color TestColor
    {
        get
        {
            return _testColor.ToUnityColor();
        }
        set
        {
            _testColor = value.ToSystemColor();
        }
    }

    [JsonProperty("TestColor_")]
    private System.Drawing.Color _testColor = System.Drawing.Color.White;

    public EnumTest EnumTest = EnumTest.TestA;

    private static string _soTestPath;

    public async static Task<SOTest> Load()
    {
        SetPath();

        if (File.Exists(_soTestPath))
        {
            string json = await File.ReadAllTextAsync(_soTestPath);
            var graphConfig = JsonConvert.DeserializeObject<SOTest>(json);
            return graphConfig;
        }


        var graphConfigB = new SOTest();
        await graphConfigB.Save();
        return graphConfigB;
    }

    public async Task Save()
    {
        SetPath();

        string json = JsonConvert.SerializeObject(this, Formatting.Indented);
        await File.WriteAllTextAsync(_soTestPath, json);
    }


    private static void SetPath()
    {
        var folderPath = Path.Combine(Application.dataPath, "VRKGUnity", "Data", "Test");

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        _soTestPath = Path.Combine(folderPath, "SoTest.json");
    }
}


public enum EnumTest
{
    TestA,
    TestB,
    TestC,
    TestD
}