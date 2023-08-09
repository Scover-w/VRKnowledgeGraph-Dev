using AIDEN.TactileUI;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class StringStepperUI : MonoBehaviour, ITouchUI, IValueUI<string>
{
    public bool Interactable
    {
        get
        {
            return _interactable;
        }
        set
        {
            _interactable = value;
            _buttonA.Interactable = value;
            _buttonB.Interactable = value;

            UpdateColliderActivation();
            TrySetNormalInteractionState();
            UpdateInteractionColor();
        }
    }

    public string Value
    {
        get
        {
            if (_selectedStepperValue == null)
                return "";

            return _selectedStepperValue.Value;
        }
        set
        {
            SetNewValueFromOutside(value);
        }
    }

    [SerializeField]
    bool _interactable = true;

    [SerializeField]
    List<InteractiveGraphicUI> _interactiveGraphics;

    [SerializeField]
    TMP_Text _label;

    [SerializeField]
    ButtonUI _buttonA;

    [SerializeField]
    ButtonUI _buttonB;

    [SerializeField]
    GameObject _interactionCollidersGo;

    [SerializeField, Space(5)]
    List<StringStepperValue> _optionsValues;

    [SerializeField]
    int _startSelected = 0;

    [SerializeField, Space(10)]
    UnityEvent<string> _onValueChanged;


    StringStepperValue _selectedStepperValue;
    int _selectedId = 0;


    Transform _touchTf;
    TouchInteractor _touchInter;

    InteractionStateUI _interactionStateUI;
    bool _canClick = true;


    private void Awake()
    {
        SetNewSelection(_startSelected);
    }

    private void OnEnable()
    {
        _canClick = true;

        UpdateColliderActivation();
        TrySetNormalInteractionState();
        UpdateInteractionColor();
    }

    private void OnDisable()
    {
        if (_touchInter != null)
            _touchInter.ActiveBtn(false, this);
    }


    public void TriggerEnter(bool isProximity, Transform touchTf)
    {
        if (isProximity)
        {
            _touchTf = touchTf;
            _touchInter = _touchTf.GetComponent<TouchInteractor>();
            _interactionStateUI = InteractionStateUI.InProximity;
            UpdateInteractionColor();
        }
        else if (!isProximity)
        {
            TryClick();
        }

    }

    private void TryClick()
    {
        if (!_canClick)
            return;

        _canClick = false;
        _interactionStateUI = InteractionStateUI.Active;
        UpdateInteractionColor();

        if (_touchInter != null)
            _touchInter.ActiveBtn(true, this);

        IncrementSelection();
    }

    public void TriggerExit(bool isProximity, Transform touchTf)
    {
        if (isProximity)
        {
            _canClick = true;
            _interactionStateUI = InteractionStateUI.Normal;
            UpdateInteractionColor();
        }
        else if (!isProximity)
        {
            if (_touchInter != null && !_canClick)
                _touchInter.ActiveBtn(false, this);

            _interactionStateUI = InteractionStateUI.Normal;
            UpdateInteractionColor();
        }

    }

    private void UpdateInteractionColor()
    {
        _interactiveGraphics.UpdateColor(_interactionStateUI);
    }


    public void AddOption(StringStepperValue stepperValue)
    {
        if (TryGetValueFromOptions(stepperValue.Value, out var v))
        {
            Debug.LogWarning("StringStepperValue Value already in.");
            return;
        }

        _optionsValues.Add(stepperValue);

        if (_selectedId == -1)
            SetNewSelection(0);
    }

    public void RemoveOption(StringStepperValue stepperValueToRemove)
    {
        if (!TryGetValueFromOptions(stepperValueToRemove.Value, out var stepperValue))
        {
            Debug.LogWarning("StringStepperValue Value already in.");
            return;
        }


        int idToRemove = _optionsValues.IndexOf(stepperValue);
        _optionsValues.Remove(stepperValue);

        if(_optionsValues.Count == 0)
        {
            SetNewSelection(-1);
            return;
        }

        int newSelectionId = idToRemove - 1;

        if(newSelectionId < 0)
            newSelectionId = _optionsValues.Count - 1;

        SetNewSelection(newSelectionId);
    }

    public void DecrementSelection()
    {
        int newSelectionId = _selectedId - 1;

        if(newSelectionId < 0)
            newSelectionId = _optionsValues.Count - 1;

        SetNewSelection(newSelectionId);
        
    }

    public void IncrementSelection()
    {
        int newSelectionId = _selectedId + 1;

        if(newSelectionId >= _optionsValues.Count)
            newSelectionId = 0;

        SetNewSelection(newSelectionId);
    }


    private void SetNewValueFromOutside(string newValue)
    {
        if(!TryGetValueFromOptions(newValue, out StringStepperValue stepperValue))
        {
            Debug.LogWarning("No StringStepperValue with this value : " + newValue);
            return;
        }

        var newSelectionId = _optionsValues.IndexOf(stepperValue);
        SetNewSelection(newSelectionId);
    }

    private void SetNewSelection(int selectionId)
    {
        if(selectionId == -1)
        {
            _selectedId = -1;
            _selectedStepperValue = null;
            _label.text = "";
            return;
        }


        _selectedId = selectionId;
        _selectedStepperValue = _optionsValues[_selectedId];
        _onValueChanged?.Invoke(_selectedStepperValue.Value);

        _label.text = _selectedStepperValue.LabelName;
    }


    private bool TryGetValueFromOptions(string value, out StringStepperValue stepperValueToGet)
    {
        foreach (StringStepperValue stepperValue in _optionsValues)
        {
            if (stepperValue.Value == value)
            {
                stepperValueToGet = stepperValue;
                return true;
            }
        }

        stepperValueToGet = default;
        return false;
    }


    private void UpdateColliderActivation()
    {
        _interactionCollidersGo.SetActive(_interactable);
    }

    private void TrySetNormalInteractionState()
    {
        if (_interactable)
            _interactionStateUI = InteractionStateUI.Normal;
        else
            _interactionStateUI = InteractionStateUI.Disabled;
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        _interactiveGraphics?.TrySetName();

        if (_optionsValues != null && _optionsValues.Count > 0)
        {
            if (_startSelected < 0)
                _startSelected = _optionsValues.Count - 1;

            if (_startSelected >= _optionsValues.Count)
                _startSelected = 0;

            _selectedId = _startSelected;
            _selectedStepperValue = _optionsValues[_selectedId];

            _label.text = _selectedStepperValue.LabelName;
        }
        else
        { 
            _startSelected = -1;
            _selectedId = -1;
        }

        Interactable = _interactable;

        UpdateColliderActivation();
        TrySetNormalInteractionState();
        UpdateInteractionColor();
    }
#endif

    [Serializable]
    public class StringStepperValue
    {
        public string LabelName;
        public string Value;
    }

}
