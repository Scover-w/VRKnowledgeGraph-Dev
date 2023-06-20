using Codice.Client.BaseCommands;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;
using VDS.RDF;

public class AIDENToolsEditor : MonoBehaviour
{
    [MenuItem("AIDEN Tools/Scene/Play PC Persistent Scene")]
    private static void PlayPersistentScene()
    {
        var loadedSceneBeforePlay = EditorSceneManager.GetActiveScene();
        EditorPrefs.SetString("LoadedSceneBeforePlay", loadedSceneBeforePlay.path);

        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        EditorSceneManager.OpenScene("Assets/VRKGUnity/Scenes/PC_Persistent.unity", OpenSceneMode.Single);
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

        var hasUpdated = await api.LoadFileContentInDatabase(turtleContent, GraphDBAPIFileType.Turtle);

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
    }

}
