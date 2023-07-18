using System;
using System.Diagnostics;
using System.IO;
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
        IGraph d;

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
        var path = Path.Combine(Application.dataPath, "VRKGUnity", "Data", "cap44_1455283593");

        DirectoryInfo directoryInfo = new DirectoryInfo(path);

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
        var path = Path.Combine(Application.dataPath, "VRKGUnity", "Data", "cap44_1455283593");

        DirectoryInfo directoryInfo = new DirectoryInfo(path);

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


    [MenuItem("AIDEN Tools/Set Wifi IP")]
    private static void SetWifiIP()
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

        // Iterate through lines
        foreach (string line in lines)
        {
            // Check if line contains the adapter's name
            if (line.Contains(adapterName))
            {
                // Split line into components
                string[] components = line.Split(new[] { '{', '}', ',' }, StringSplitOptions.RemoveEmptyEntries);

                // The IPv4 address is the second component after the split (trimming to remove any leading/trailing spaces or quotes)
                string ipv4 = components[1].Trim().Trim('"');

                Debug.Log($"The IP address for the {adapterName} is {ipv4}");
            }
        }
    }
}
