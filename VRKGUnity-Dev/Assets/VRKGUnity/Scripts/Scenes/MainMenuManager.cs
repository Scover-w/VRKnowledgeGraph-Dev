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

    private async void AutoLoadGraphScene()
    {
        var graphDbRepositories = await GraphDbRepositories.Load();
        var repositories = graphDbRepositories.Repositories;

        if (repositories.Count == 0)
            return;

        var repository = repositories[0];

        _referenceHolderSO.SelectedGraphDbRepository = repository;
        repository.SetGraphDbCredentials();

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