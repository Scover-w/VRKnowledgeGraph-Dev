using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneTest : MonoBehaviour
{
    [SerializeField]
    LifeCycleSceneManager _lifeCycleSceneManager;


    public string SceneToLoad;

    [ContextMenu("LoadScene")]
    public void LoadScene()
    {
        _lifeCycleSceneManager.LoadScene(SceneToLoad);
    }
}
