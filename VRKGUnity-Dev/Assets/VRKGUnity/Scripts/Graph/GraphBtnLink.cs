using AIDEN.TactileUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphBtnLink : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;

    [SerializeField]
    GraphActionKey _graphActionKey;

    [SerializeField]
    MonoBehaviour _tactileUIScript;

    User _user;

    ITouchUI _iTouchUI;


    private void Awake()
    {
        _iTouchUI = _tactileUIScript.GetComponent<ITouchUI>();
    }

    private void OnEnable()
    {
        Invoke(nameof(DelayedOnEnable), .5f);
    }

    private void OnDisable()
    {
        _user.UnRegister(_graphActionKey, OnFeedbackActionStateFromUser);
    }

    private void DelayedOnEnable()
    {
        if(_user == null)
            _user = _referenceHolderSo.User;

        _iTouchUI.Interactable = _user.GetInteractableState(_graphActionKey);
        _user.Register(_graphActionKey, OnFeedbackActionStateFromUser);
    }

    public void OnClick()
    {
        _user.InitiateNewAction(_graphActionKey);
    }

    public void OnFeedbackActionStateFromUser(bool interactable)
    {
        _iTouchUI.Interactable = interactable;
    }



    private void OnValidate()
    {
        if (_tactileUIScript == null)
            return;

        var interfaces = _tactileUIScript.GetType().GetInterfaces();
        bool implementsITouchUI = false;

        foreach (Type interfaceType in interfaces)
        {
            if (!(interfaceType == typeof(ITouchUI)))
                continue;

            implementsITouchUI = true;
            break;
        }

        if (implementsITouchUI)
            return;

        _tactileUIScript = null;
        Debug.LogError("The script does not implement ITouchUI !");
    }

    public static IEnumerable<Type> GetAllInterfaces(Type type)
    {
        var interfaces = new List<Type>();

        while (type != null)
        {
            interfaces.AddRange(type.GetInterfaces());
            type = type.BaseType;
        }

        return interfaces.Distinct();
    }

}
