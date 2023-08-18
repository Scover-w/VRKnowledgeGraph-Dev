using UnityEngine;

public class PersistentManager : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSO;

    [SerializeField]
    Camera _cam;

    [SerializeField]
    Transform _wristTf;



    private void Start()
    {
        _referenceHolderSO.HMDCamSA.Value = _cam;
        _referenceHolderSO.WristTf.Value = _wristTf;
    }
}
