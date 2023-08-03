using System;
using System.Diagnostics;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Windows;
using VDS.RDF;
using Debug = UnityEngine.Debug;

public class AIDENToolsEditor : MonoBehaviour
{
    [MenuItem("AIDEN Tools/Scene/Play PC Persistent Scene")]
    private static void PlayPCPersistentScene()
    {
        var loadedSceneBeforePlay = EditorSceneManager.GetActiveScene();
        EditorPrefs.SetString("LoadedSceneBeforePlay", loadedSceneBeforePlay.path);

        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        EditorSceneManager.OpenScene("Assets/VRKGUnity/Scenes/PC_Persistent.unity", OpenSceneMode.Single);
        EditorApplication.isPlaying = true;
    }

    [MenuItem("AIDEN Tools/Scene/Play VR Persistent Scene")]
    private static void PlayVRPersistentScene()
    {
        var loadedSceneBeforePlay = EditorSceneManager.GetActiveScene();
        EditorPrefs.SetString("LoadedSceneBeforePlay", loadedSceneBeforePlay.path);

        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        EditorSceneManager.OpenScene("Assets/VRKGUnity/Scenes/VR_Persistent.unity", OpenSceneMode.Single);
        EditorApplication.isPlaying = true;
    }

    [MenuItem("AIDEN Tools/Scene/Play Pre ForceTest Scene")]
    private static void PlayPreForceScene()
    {
        var loadedSceneBeforePlay = EditorSceneManager.GetActiveScene();
        EditorPrefs.SetString("LoadedSceneBeforePlay", loadedSceneBeforePlay.path);

        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        EditorSceneManager.OpenScene("Assets/VRKGUnity/Scenes/Tests/PreForceTest.unity", OpenSceneMode.Single);
        EditorApplication.isPlaying = true;
    }

    [DidReloadScripts]
    private static void OnEditorReload()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            var scene = EditorPrefs.GetString("LoadedSceneBeforePlay");
            if (scene == null || scene.Length == 0)
                return;
            EditorSceneManager.OpenScene(EditorPrefs.GetString("LoadedSceneBeforePlay"), OpenSceneMode.Single);
        }

        EditorPrefs.SetString("LoadedSceneBeforePlay", null);
    }


    [MenuItem("AIDEN Tools/GraphDb/Reload Data Cap44")]
    private async static void ReloadDataCap44()
    {
        var api = new GraphDBAPI(null);

        var hasDeletedAll = await api.UpdateQuery("DELETE\r\nWHERE {\r\n  ?s ?p ?o .\r\n}");

        if (!hasDeletedAll)
        {
            Debug.LogError("Data in the Cap44 couldn't be deleted");
            return;
        }

        var turtleContent = await FileHelper.LoadAsync(Application.dataPath, "VRKGUnity", "Data", "Tools", "cap44_items.ttl");

        var hasUpdated = await api.LoadFileContentInDatabase(turtleContent, "<http://data>", GraphDBAPIFileType.Turtle);

        if(!hasUpdated) 
        {
            Debug.LogError("Data in the Cap44 couldn't be updated");
            return;
        }

        Debug.Log("Data in the Cap44 has been reloaded !");
    }

    [MenuItem("AIDEN Tools/Folders/Reload Data Cap44")]
    private static void RemoveDataFromCap44Folder()
    {
        var path = Path.Combine(Application.persistentDataPath, "Data", "cap44_1455283593");

        DirectoryInfo directoryInfo = new(path);

        foreach (FileInfo file in directoryInfo.GetFiles())
        {
            file.Delete();
        }

        // If you also want to remove all subdirectories and files in them
        foreach (DirectoryInfo dir in directoryInfo.GetDirectories())
        {
            dir.Delete(true);
        }

        Debug.Log("Cap44 data has been removed !");
    }

    [MenuItem("AIDEN Tools/Folders/Reload Data Cap44 Except Distant Uris")]
    private static void RemoveDataFromCap44FolderExceptUris()
    {
        var path = Path.Combine(Application.persistentDataPath, "Data", "cap44_1455283593");

        DirectoryInfo directoryInfo = new(path);

        foreach (FileInfo file in directoryInfo.GetFiles())
        {
            if (file.Name == "GraphDbRepositoryDistantUris.json" || file.Name == "GraphDbRepositoryDistantUris.json.meta")
                continue;
            file.Delete();
        }

        // If you also want to remove all subdirectories and files in them
        foreach (DirectoryInfo dir in directoryInfo.GetDirectories())
        {
            dir.Delete(true);
        }

        Debug.Log("Cap44 data has been removed !");
    }

    [MenuItem("AIDEN Tools/Folders/Open Data Folder")]
    private static void OpenDataFolder()
    {
        string path = Path.Combine(Application.persistentDataPath, "Data");

        Process.Start(new ProcessStartInfo()
        {
            FileName = path,
            UseShellExecute = true,
            Verb = "open"
        });
    }


    [MenuItem("AIDEN Tools/Set Wifi IP")]
    private static void SetWifiIP()
    {
        string newIp = GetWifiIP();

        if(newIp == null)
        {
            Debug.LogWarning("The Wifi Key IP couldn't be retrieved.");
            return;
        }

        string scriptPath = Application.dataPath + "/VRKGUnity/Scripts/Scenes/MainMenuManager.cs";
        string scriptText = System.IO.File.ReadAllText(scriptPath);

        string[] lines = scriptText.Split("graphDbApi.OverrideForTest(\"");
        string oldIp = lines[1].Split('"')[0]; // http://130....


        scriptText = scriptText.Replace(oldIp, "http://" + newIp + ":7200/");


        System.IO.File.WriteAllText(scriptPath, scriptText);
        AssetDatabase.Refresh();

        Debug.Log("Wifi Ip set as " + newIp);
    }



    private static string GetWifiIP() 
    {
        string adapterName = "TP-Link Wireless USB Adapter";

        var process = new Process()
        {
            StartInfo = new ProcessStartInfo("cmd.exe")
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();

        process.StandardInput.WriteLine("wmic nicconfig where \"IPEnabled=True\" get Caption, IPAddress");
        process.StandardInput.Close(); // Important to close input stream

        string result = process.StandardOutput.ReadToEnd();

        process.WaitForExit();


        // Split input into lines
        string[] lines = result.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);


        foreach (string line in lines)
        {
            if (line.Contains(adapterName))
            {
                // Split line into components
                string[] components = line.Split(new[] { '{', '}', ',' }, StringSplitOptions.RemoveEmptyEntries);

                // The IPv4 address is the second component after the split (trimming to remove any leading/trailing spaces or quotes)
                string ipv4 = components[1].Trim().Trim('"');

                return ipv4;
            }
        }

        return null;
    }


    [MenuItem("AIDEN Tools/Add Basic Interaction UI GOs")]
    private static void AddBasicInteractionGOs()
    {
        GameObject selectedObject = Selection.activeGameObject;

        if (selectedObject == null)
        {
            Debug.LogWarning("No gameobject selected.");
            return;
        }

        MonoBehaviour physicalUIScript = GetPhysicalUIScript();

        if(physicalUIScript == null ) 
        {
            Debug.LogWarning("No Monobehavior that implement IPhysicalUI founded.");
            return;
        }

        Transform selectedTf = selectedObject.transform;
        RectTransform selectedRect = selectedObject.GetComponent<RectTransform>();

        Transform proximityTf = new GameObject("Proximity").transform;
        ResetTf(proximityTf);



        proximityTf.tag = Tags.ProximityUI;
        var sphereCol = proximityTf.AddComponent<SphereCollider>();
        sphereCol.isTrigger = true;
        var triggerA = proximityTf.AddComponent<TriggerPhysicalUI>();
        SerializedObject serializedTriggerA = new SerializedObject(triggerA);
        SerializedProperty isProximityProp = serializedTriggerA.FindProperty("_isProximity");
        SerializedProperty physicalUIScriptPropA = serializedTriggerA.FindProperty("_physicalUIScript");
        isProximityProp.boolValue = true;
        physicalUIScriptPropA.objectReferenceValue = physicalUIScript;

        serializedTriggerA.ApplyModifiedProperties();

        float x = selectedRect.rect.width;
        float y = selectedRect.rect.height;
        float maxXY = (x > y) ? x : y;
        sphereCol.radius = maxXY * .5f;


        Transform activeTf = new GameObject("Interaction").transform;
        ResetTf(activeTf);

        activeTf.tag = Tags.InteractionUI;
        var boxCol = activeTf.AddComponent<BoxCollider>();
        boxCol.isTrigger = true;
        var triggerB = activeTf.AddComponent<TriggerPhysicalUI>();
        SerializedObject serializedTriggerB = new SerializedObject(triggerB);
        SerializedProperty physicalUIScriptPropB = serializedTriggerB.FindProperty("_physicalUIScript");
        physicalUIScriptPropB.objectReferenceValue = physicalUIScript;

        serializedTriggerB.ApplyModifiedProperties();

        boxCol.size = new Vector3(x, y, 80f);
        boxCol.center = new Vector3(0, 0, 80f / 2);

        Debug.Log("Basic Interaction UI GOs added !");


        void ResetTf(Transform tf)
        {
            tf.parent = selectedTf;
            tf.localPosition = Vector3.zero;
            tf.localRotation = Quaternion.identity;
            tf.localScale = new Vector3(1f, 1f, 1f);

            tf.gameObject.layer = Layers.UI;
        }

        MonoBehaviour GetPhysicalUIScript()
        {
            var monos = selectedObject.GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour script in monos)
            {
                if (script is IPhysicalUI physicalUI)
                {
                    return script;
                }
            }

            return null;
        }
    }
}
