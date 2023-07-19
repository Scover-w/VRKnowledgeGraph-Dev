using UnityEngine;

[DefaultExecutionOrder(-1)]
public class ForceTestManager : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSO;

    [SerializeField]
    Camera _cam;



    private void Awake()
    {
        _referenceHolderSO.HMDCamSA.Value = _cam;
    }
}
