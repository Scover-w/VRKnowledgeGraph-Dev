using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGraph : MonoBehaviour
{

    public Transform Tf { get {  return _tf; } }

    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;

    [SerializeField]
    Transform _tf;

    Transform _playerTf;

    MainGraphMode _mainGraphMode;

    void Start()
    {
        _mainGraphMode = MainGraphMode.Desk;
        _playerTf = _referenceHolderSo.HMDCamSA.Value.transform;
    }


    private void Update()
    {
        
    }




    public void SwitchMode(GraphMode graphMode)
    {
        if (graphMode == GraphMode.Desk && _mainGraphMode == MainGraphMode.Desk ||
            graphMode == GraphMode.Immersion && _mainGraphMode == MainGraphMode.Desk)
            return;



    }


    enum MainGraphMode
    {
        Desk,
        Immersion
    }

}