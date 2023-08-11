using AIDEN.TactileUI;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class StringStepperUI : BaseTouch, IValueUI<string>
{
    public override bool Interactable
    {
        get
        {
            return _interactable;
        }
        set
        {
            base.Interactable = value;

            _buttonA.Interactable = value;
            _buttonB.Interactable = value;
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
    TMP_Text _label;

    [SerializeField]
    ButtonUI _buttonA;

    [SerializeField]
    ButtonUI _buttonB;

    [SerializeField, Space(5)]
    List<StringStepperValue> _optionsValues;

    [SerializeField]
    int _startSelected = 0;

    [SerializeField, Space(10)]
    UnityEvent<string> _onValueChanged;


    StringStepperValue _selectedStepperValue;
    int _selectedId = 0;



    private void Awake()
    {
        SetNewSelection(_startSelected);
    }


    protected override void TryActivate()
    {
        if (!base.CanActivate())
            return;

        base.Activate();

        _touchInter.ActivateHaptic();
        IncrementSelection();
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



#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

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
    }
#endif

    [Serializable]
    public class StringStepperValue
    {
        public string LabelName;
        public string Value;
    }

}
