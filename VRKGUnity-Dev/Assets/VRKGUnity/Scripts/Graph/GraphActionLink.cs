using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphActionLink : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;

    [SerializeField]
    GraphActionKey _graphActionKey;

    User _user;


    private void Awake()
    {
        _user = _referenceHolderSo.User;
    }


    public void OnClick()
    {

    }


}
