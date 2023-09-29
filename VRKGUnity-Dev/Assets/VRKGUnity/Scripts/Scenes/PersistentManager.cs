using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

/// <summary>
/// GameManager of the PersistentManager Scene
/// </summary>
public class PersistentManager : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSO;

    [SerializeField]
    Camera _cam;

    [SerializeField]
    Transform _xrOriginTf;
    
    [SerializeField]
    Transform _wristTf;

    [SerializeField]
    DynamicMoveProvider _dynamicMoveProvider;



    private void Start()
    {
        _referenceHolderSO.HMDCamSA.Value = _cam;
        _referenceHolderSO.WristTf.Value = _wristTf;
        _referenceHolderSO.XrOriginTf.Value = _xrOriginTf;
        _referenceHolderSO.DynamicMoveProvider.Value = _dynamicMoveProvider;
    }
}
