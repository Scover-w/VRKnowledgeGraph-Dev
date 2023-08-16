using AIDEN.TactileUI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseTouch : MonoBehaviour, ITouchUI
{

    public virtual bool Interactable
    {
        get
        {
            return _interactable;
        }
        set
        {
            _interactable = value;

            UpdateColliderInteraction();
            TrySetNormalInteractionState();
            UpdateInteractionColor();
        }
    }

    [SerializeField]
    protected bool _interactable = true;

    [SerializeField]
    protected List<InteractiveGraphicUI> _interactiveGraphics;

    [SerializeField]
    protected GameObject _interactionCollidersGo;

    protected Transform _touchTf;
    protected TouchInteractor _touchInter;

    protected InteractionStateUI _interactionStateUI;

    protected bool _inProximity = false;
    protected int _proximityFrameCount;

    protected float _rebounceDelay = .5f;
    float _rebounceTime = 0f;

    protected virtual void OnEnable()
    {
        _inProximity = false;

        UpdateColliderInteraction();
        TrySetNormalInteractionState();
        UpdateInteractionColor();
    }

    protected virtual void OnDisable()
    {
        if (_touchInter != null)
            _touchInter.ActiveBtn(false, this);
    }

    public void TriggerEnter(bool isProximity, Transform touchTf)
    {
        if (isProximity)
        {
            _inProximity = true;
            _proximityFrameCount = Time.frameCount;
            _touchTf = touchTf;
            _touchInter = _touchTf.GetComponent<TouchInteractor>();
            _interactionStateUI = InteractionStateUI.InProximity;
            UpdateInteractionColor();
        }
        else if (!isProximity)
        {
            TryActivate();
        }
    }


    public virtual void TriggerExit(bool isProximity, Transform touchTf)
    {
        if (isProximity)
        {
            _inProximity = false;
            _interactionStateUI = InteractionStateUI.Normal;
            UpdateInteractionColor();
        }
        else // interaction
        {
            if (_touchInter != null)
                _touchInter.ActiveBtn(false, this);

            _interactionStateUI = InteractionStateUI.Normal;
            UpdateInteractionColor();
        }

    }

    protected bool CanActivate(bool doCheckRebound = true)
    {
        if (!_inProximity)
            return false;

        if (Time.frameCount == _proximityFrameCount)
            return false;

        if (doCheckRebound && Time.time < _rebounceTime)
            return false;

        return true;
    }

    protected void Activate()
    {
        _rebounceTime = Time.time + _rebounceDelay;

        _interactionStateUI = InteractionStateUI.Active;
        UpdateInteractionColor();

        if (_touchInter != null)
            _touchInter.ActiveBtn(true, this);
    }

    protected virtual void TryActivate()
    {
        throw new NotImplementedException("Child classes should override this method.");
    }

    protected virtual void UpdateInteractionColor()
    {
        _interactiveGraphics.UpdateColor(_interactionStateUI);
    }

    protected virtual void UpdateColliderInteraction()
    {
        _interactionCollidersGo.SetActive(_interactable);
    }

    protected void TrySetNormalInteractionState()
    {
        if (_interactable)
            _interactionStateUI = InteractionStateUI.Normal;
        else
            _interactionStateUI = InteractionStateUI.Disabled;
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        _interactiveGraphics?.TrySetName();

        UpdateColliderInteraction();
        TrySetNormalInteractionState();
        UpdateInteractionColor();
    }
#endif
}
