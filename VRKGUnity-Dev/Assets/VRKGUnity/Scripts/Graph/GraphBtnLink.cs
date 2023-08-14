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

    InputPropagatorManager _inputPropagatorManager;

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
        _inputPropagatorManager.UnRegister(_graphActionKey, OnFeedbackActionStateFromUser);
    }

    private void DelayedOnEnable()
    {
        if (_inputPropagatorManager == null)
            _inputPropagatorManager = _referenceHolderSo.InputPropagatorManager;

        _iTouchUI.Interactable = _inputPropagatorManager.GetInteractableState(_graphActionKey);
        _inputPropagatorManager.Register(_graphActionKey, OnFeedbackActionStateFromUser);
    }

    public void OnClick()
    {
        _inputPropagatorManager.InitiateNewAction(_graphActionKey);
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
