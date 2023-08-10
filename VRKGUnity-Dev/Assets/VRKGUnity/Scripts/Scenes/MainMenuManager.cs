using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSO;



    [SerializeField]
    bool _autoPlay = true;


    private void Start()
    {
        if(_autoPlay)
            Invoke(nameof(AutoLoadGraphScene), .2f);
    }

    private void AutoLoadGraphScene()
    {
        if (_referenceHolderSO.SelectedGraphDbRepository == null)
            return;

        var graphDbApi = _referenceHolderSO.SelectedGraphDbRepository.GraphDBAPI;


#if UNITY_ANDROID && !UNITY_EDITOR
        graphDbApi.OverrideForTest("http://130.66.203.189:7200/");
#endif

        var lifeCycleScene = _referenceHolderSO.LifeCycleSceneManagerSA.Value;
        lifeCycleScene.LoadScene(Scenes.DataSynchro);
    }

    public void LoadGraphScene()
    {
        var lifeCycleScene = _referenceHolderSO.LifeCycleSceneManagerSA.Value;
        lifeCycleScene.LoadScene(Scenes.DataSynchro);
    }

    public void LoadTutorialScene()
    {
        var lifeCycleScene = _referenceHolderSO.LifeCycleSceneManagerSA.Value;
        lifeCycleScene.LoadScene(Scenes.Tutorial);
    }
}