using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LifeCycleSceneManager : MonoBehaviour
{
    [SerializeField]
    DeviceMode _deviceMode;

    private string _loadedScene = "";
    private string _sceneToLoad = "";

    private void Start()
    {
        Scenes.DeviceMode = _deviceMode;

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        Invoke(nameof(FirstLoad), .5f);
    }

    private void FirstLoad()
    {
        LoadScene(Scenes.MainMenu);
    }

    public void LoadScene(string sceneToLoad)
    {
        if(_loadedScene.Length > 0)
        {
            _sceneToLoad = sceneToLoad;
            SceneManager.UnloadSceneAsync(_loadedScene);
            return;
        }

        SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        var loadedSceneName = scene.name;

        if (loadedSceneName.Contains("Persistent"))
            return;

        _loadedScene = loadedSceneName;
    }


    private void OnSceneUnloaded(Scene scene)
    {
        _loadedScene = "";

        if (_sceneToLoad.Length == 0)
            return;

        SceneManager.LoadSceneAsync(_sceneToLoad, LoadSceneMode.Additive);
        _sceneToLoad = "";
    }

}
