using UnityEngine;

public class AppManager : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;


    [SerializeField]
    GraphConfigurationContainerSO _graphConfigurationContainer;

    [SerializeField]
    LifeCycleSceneManager _lifeCycleSceneManager;


    void Start()
    {
        _referenceHolderSo.AppManagerSA.Value = this;
        
    }

    private void OnDisable()
    {
        _graphConfigurationContainer.RefreshWindowsEditor();
    }
}
