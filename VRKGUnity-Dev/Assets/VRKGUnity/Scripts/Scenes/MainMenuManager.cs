using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSO;


#if UNITY_EDITOR
    [SerializeField]
    bool _autoPlay = true;


    private void Start()
    {
        if(_autoPlay)
            Invoke(nameof(Play), .2f);
    }
#endif

    public void Play()
    {
        if (_referenceHolderSO.SelectedGraphDbRepository == null)
            return;


        var lifeCycleScene = _referenceHolderSO.LifeCycleSceneManagerSA.Value;
        lifeCycleScene.LoadScene(Scenes.DataSynchro);
    }
}