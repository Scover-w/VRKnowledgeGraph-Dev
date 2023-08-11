using UnityEngine;

public class AppManager : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;


    [SerializeField]
    GraphConfigurationContainerSO _graphConfigurationContainer;

    [SerializeField]
    LifeCycleSceneManager _lifeCycleSceneManager;


    async void Start()
    {
        Settings.SetPersistentDataPath(Application.persistentDataPath);
        _referenceHolderSo.AppManagerSA.Value = this;

        await _graphConfigurationContainer.GetGraphConfiguration();
    }

    private void OnDisable()
    {
        _graphConfigurationContainer.RefreshWindowsEditor();
    }

    public void ReloadKG()
    {
        _lifeCycleSceneManager.LoadScene(Scenes.KG);
    }
}
