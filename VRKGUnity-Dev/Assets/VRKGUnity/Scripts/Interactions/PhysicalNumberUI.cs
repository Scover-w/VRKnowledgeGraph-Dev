using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PhysicalTabControllerUI;

public class PhysicalNumberUI : MonoBehaviour, IPhysicalUI
{
    [SerializeField]
    ColorTab _color;

    [SerializeField]
    Image _img;

    [SerializeField]
    TMP_Text _label;

    [SerializeField]
    Transform _keyboardPositionTf;

    Transform _touchTf;
    TouchInteraction _touchInter;

    float _value;

    bool _isActive = false; 

    public void TriggerEnter(bool isProximity, Collider touchCollider)
    {
        if (isProximity && touchCollider.CompareTag(Tags.ProximityUI))
        {
            _touchTf = touchCollider.transform.parent;
            _touchInter = _touchTf.GetComponent<TouchInteraction>();
            UpdateColor(InteractionStateUI.InProximity);
        }
        else if (!isProximity && touchCollider.CompareTag(Tags.ActiveUI))
        {
            TryActivate();
        }
    }

    private void TryActivate()
    {
        if (_isActive)
            return;

        var options = CreateKeyboardOptions();
        bool succeedUsingKeyboard = KeyboardControllerUI.DisplayNumpad(options);

        if (!succeedUsingKeyboard)
            return;

        _isActive = true;
        UpdateColor(InteractionStateUI.Active);

        if (_touchInter != null)
            _touchInter.ActiveBtn(true, this);
    }

    public void TriggerExit(bool isProximity, Collider touchCollider)
    {
        if (isProximity && touchCollider.CompareTag(Tags.ProximityUI))
        {
            UpdateColor(InteractionStateUI.Normal);
        }
        else if (!isProximity && touchCollider.CompareTag(Tags.ActiveUI))
        {
            if (_touchInter != null && _isActive)
                _touchInter.ActiveBtn(false, this);

            UpdateColor(InteractionStateUI.Normal);
        }
    }

    private void UpdateColor(InteractionStateUI interactionState)
    {
        if(_isActive)
        {
            _img.color = _color.ActivatedColor;
            return;
        }

        switch (interactionState)
        {
            case InteractionStateUI.Normal:
                _img.color = _color.NormalColor;
                break;
            case InteractionStateUI.InProximity:
                _img.color = _color.ProximityColor;
                break;
            case InteractionStateUI.Active:
                _img.color = _color.ActivatedColor;
                break;
        }
    }

    public void OnUpdateInput(string input)
    {
        _label.text = input;
        
        if(float.TryParse(input, out float newValue))
            _value = newValue;
        else
            _value = 0f;
        
    }

    public void OnEnterInput(string input)
    {
        _label.text = input;

        if (float.TryParse(input, out float newValue))
            _value = newValue;
        else
            _value = 0f;

        _isActive = false;
        UpdateColor(InteractionStateUI.Normal);
    }

    private KeyboardUIOptions CreateKeyboardOptions()
    {
        KeyboardUIOptions options = new(_keyboardPositionTf.position,
                                        KeyboardControllerUI.KeyboardAlignment.Left,
                                        OnUpdateInput,
                                        OnEnterInput,
                                        _value.ToString());

        return options;
    }

}
