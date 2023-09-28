using AIDEN.TactileUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScrollItemUI : MonoBehaviour
{
    public GameObject Go { get { return _rect.gameObject; } }
    public float MaxY { get { return _maxY; } }

    [SerializeField]
    List<InteractiveGraphicUI> _interactiveGraphics;

    [SerializeField]
    UnityEvent _onClick;

    [SerializeField]
    UnityEvent _onDeselected;

    InteractionStateUI _interactionStateUI;

    RectTransform _rect;

    public delegate void Click();
    public Click OnClick;



    bool _inProximity = false;
    bool _isSelected = false;

    float _minY;
    float _maxY;


    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        _interactionStateUI = _isSelected ? InteractionStateUI.Active : InteractionStateUI.Normal;
        UpdateInteractionColor();
    }

    public void OnProximity(bool inProximity)
    {
        _inProximity = inProximity;
        if (_inProximity)
            _interactionStateUI = InteractionStateUI.InProximity;
        else
            _interactionStateUI = _isSelected ? InteractionStateUI.Active : InteractionStateUI.Normal;

        UpdateInteractionColor();
    }

    public void OnSelect(bool isSelected)
    {
        _isSelected = isSelected;

        if (_isSelected)
        {
            _interactionStateUI = InteractionStateUI.Active;
            OnClick?.Invoke();
            _onClick?.Invoke();
        }
        else
        {
            _interactionStateUI = _inProximity? InteractionStateUI.InProximity : InteractionStateUI.Normal;
            _onDeselected?.Invoke();
        }

        UpdateInteractionColor();
    }


    public bool IsInRange(float yPosition)
    {
        return yPosition > _minY && yPosition < _maxY;
    }


    public void RebuildLayout()
    {
        Debug.Log(_rect);
        if(_rect == null)
            _rect = GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rect);
    }

    public float RecomputeParameters(float minY)
    {
        _minY = minY;
        _maxY = _minY + _rect.sizeDelta.y;
        return _maxY;
    }

    private void UpdateInteractionColor()
    {
        _interactiveGraphics.UpdateColor(_interactionStateUI);
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        UpdateInteractionColor();
    }
#endif
}
