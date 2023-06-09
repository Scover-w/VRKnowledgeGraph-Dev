using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;

public class AIDENToolsEditor : MonoBehaviour
{
    [MenuItem("AIDEN Tools/Play PC Persistent Scene")]
    private static void PlayPersistentScene()
    {
        var loadedSceneBeforePlay = EditorSceneManager.GetActiveScene();
        EditorPrefs.SetString("LoadedSceneBeforePlay", loadedSceneBeforePlay.path);

        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        EditorSceneManager.OpenScene("Assets/VRKGUnity/Scenes/PC_Persistent.unity", OpenSceneMode.Single);
        EditorApplication.isPlaying = true;
    }

    [MenuItem("AIDEN Tools/Play Pre ForceTest Scene")]
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

}
