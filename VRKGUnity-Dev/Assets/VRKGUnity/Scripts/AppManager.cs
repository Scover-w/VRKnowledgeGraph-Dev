using System.Runtime.CompilerServices;
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
        _referenceHolderSo.AppManagerSA.Value = this;

        await _graphConfigurationContainer.GetGraphConfiguration();
    }

    private void OnDisable()
    {
        _graphConfigurationContainer.RefreshWindowsEditor();
    }
}
