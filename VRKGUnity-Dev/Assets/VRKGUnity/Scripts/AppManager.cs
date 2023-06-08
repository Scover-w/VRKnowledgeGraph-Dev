using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;

    [SerializeField]
    LifeCycleSceneManager _lifeCycleSceneManager;


    void Start()
    {
        _referenceHolderSo.AppManagerSA.Value = this;
        
    }
}
