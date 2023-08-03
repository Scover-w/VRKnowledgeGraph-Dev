using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PhysicalTabControllerUI;

public class PhysicalLongToggleUI : MonoBehaviour, IPhysicalUI
{
    [SerializeField]
    List<ColorStateUI> _colorStates;

    [SerializeField]
    List<Image> _imgs;

    [SerializeField]
    RectTransform _toggleRect;

    [SerializeField]
    RectTransform _knobRect;

    [SerializeField]
    TMP_Text _text;

    [SerializeField]
    string _disabledValue;

    [SerializeField]
    string _enabledValue;

    RectTransform _rectText;
    Transform _touchTf;
    TouchInteraction _touchInter;

    float _xDeltaKnob;
    float _xDeltaText;

    bool _isEnable = false;
    bool _canSwitch = true;


    private void Start()
    {
        float knobWidth = _knobRect.rect.width;
        float toggleWidth = _toggleRect.rect.width;

        _xDeltaKnob = (toggleWidth - 8f - knobWidth * .5f);
        _xDeltaText = (toggleWidth - 4f - knobWidth) * .5f;

        _xDeltaKnob -= (toggleWidth * .5f);
        _xDeltaText -= (toggleWidth * .5f);
        //_xDeltaText += (_xDeltaText * .5f);

        _rectText = _text.GetComponent<RectTransform>();
        UpdateKnobPosition();
        UpdateColor(InteractionStateUI.Normal);
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
            TryClick();
        }
    }

    private void TryClick()
    {
        if (!_canSwitch)
            return;

        _canSwitch = false;
        _isEnable = !_isEnable;
        UpdateKnobPosition();
        UpdateColor(InteractionStateUI.Active);

        if (_touchInter != null)
            _touchInter.ActiveBtn(true, this);

        // TODO : Link the the true datas
        Debug.Log("Click");
    }

    public void TriggerExit(bool isProximity, Collider touchCollider)
    {
        if (isProximity && touchCollider.CompareTag(Tags.ProximityUI))
        {
            _canSwitch = true;
            UpdateColor(InteractionStateUI.Normal);
        }
        else if (!isProximity && touchCollider.CompareTag(Tags.InteractionUI))
        {
            if (_touchInter != null && !_canSwitch)
                _touchInter.ActiveBtn(false, this);

            UpdateColor(InteractionStateUI.Normal);
        }
    }

    private void UpdateColor(InteractionStateUI interactionState)
    {
        int nbImg = _imgs.Count;

        for (int i = 0; i < nbImg; i++)
        {
            Image img = _imgs[i];
            ColorStateUI colorState = _colorStates[i];

            switch (interactionState)
            {
                case InteractionStateUI.Normal:
                    img.color = colorState.NormalColor;
                    break;
                case InteractionStateUI.InProximity:
                    img.color = colorState.ProximityColor;
                    break;
                case InteractionStateUI.Active:
                    img.color = colorState.ActivatedColor;
                    break;
            }
        }
    }

    private void UpdateKnobPosition()
    {
        Vector3 posA = _knobRect.localPosition;
        posA.x = _xDeltaKnob * (_isEnable ? 1 : -1);
        _knobRect.localPosition = posA;

        _text.text = _isEnable ? _enabledValue : _disabledValue;
        Vector3 posB = _rectText.localPosition;
        posB.x = _xDeltaText * (_isEnable ? 1 : -1);
        _rectText.localPosition = posB;
    }
}
