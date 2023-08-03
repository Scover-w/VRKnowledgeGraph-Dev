using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhysicalColorSelectorUI : MonoBehaviour, IPhysicalUI
{
    [SerializeField]
    ColorStateUI _color;

    [SerializeField]
    Image _img;

    [SerializeField]
    Image _colorImg;

    [SerializeField]
    Transform _keyboardPositionTf;

    [SerializeField]
    KeyboardControllerUI.KeyboardAlignment _keyboardAlignment;

    [SerializeField]
    Color _colorValue;

    Transform _touchTf;
    TouchInteraction _touchInter;


    bool _isActive = false;

    private void Start()
    {
        _colorImg.color = _colorValue;
    }

    public void TriggerEnter(bool isProximity, Collider touchCollider)
    {
        if (isProximity && touchCollider.CompareTag(Tags.ProximityUI))
        {
            _touchTf = touchCollider.transform.parent;
            _touchInter = _touchTf.GetComponent<TouchInteraction>();
            UpdateColor(InteractionStateUI.InProximity);
        }
        else if (!isProximity && touchCollider.CompareTag(Tags.InteractionUI))
        {
            TryActivate();
        }
    }

    private void TryActivate()
    {
        if (_isActive)
            return;

        var options = CreateKeyboardOptions();
        bool succeedUsingKeyboard = KeyboardControllerUI.Display(options);

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
        else if (!isProximity && touchCollider.CompareTag(Tags.InteractionUI))
        {
            if (_touchInter != null && _isActive)
                _touchInter.ActiveBtn(false, this);

            UpdateColor(InteractionStateUI.Normal);
        }
    }

    private void UpdateColor(InteractionStateUI interactionState)
    {
        if (_isActive)
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

    public void OnUpdateInput(object input)
    {
        if (input is not Color)
            return;

        _colorValue = (Color)input;
        _colorImg.color = _colorValue;
    }

    public void OnEnterInput(object input)
    {
        if (input is not Color)
            _colorValue = Color.white;
        else 
            _colorImg.color = _colorValue;

        _colorImg.color = _colorValue;

        _isActive = false;
        UpdateColor(InteractionStateUI.Normal);
    }

    private KeyboardUIOptions<Color> CreateKeyboardOptions()
    {

        return new KeyboardUIOptions<Color>(_keyboardPositionTf.position,
                                        _keyboardAlignment,
                                        OnUpdateInput,
                                        OnEnterInput,
                                        _colorValue);
    }
}
