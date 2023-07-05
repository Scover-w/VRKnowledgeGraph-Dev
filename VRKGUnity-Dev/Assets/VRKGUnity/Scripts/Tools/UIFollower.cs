using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollower : MonoBehaviour
{

    [SerializeField]
    Transform _uiTf;

    [SerializeField]
    ReferenceHolderSO _referenceholderSo;


    Transform _camTf;
    Vector3 _deltaFromCam;

    void Start()
    {
        _camTf = _referenceholderSo.HMDCamSA.Value.transform;
        _deltaFromCam = _uiTf.position - _camTf.position;
    }

    void Update()
    {
        _uiTf.position = _camTf.position + _deltaFromCam;
    }
}
