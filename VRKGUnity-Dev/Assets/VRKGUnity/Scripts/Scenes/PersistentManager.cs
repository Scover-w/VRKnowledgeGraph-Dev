using UnityEngine;

public class PersistentManager : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSO;

    [SerializeField]
    Camera _cam;



    private void Start()
    {
        _referenceHolderSO.HMDCamSA.Value = _cam;
    }
}
