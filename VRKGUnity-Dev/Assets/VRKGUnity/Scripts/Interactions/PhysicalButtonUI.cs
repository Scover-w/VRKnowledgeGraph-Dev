using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PhysicalButtonUI : MonoBehaviour, IPhysicalUI
{
    [SerializeField]
    List<ColorStateUI> _colorStates;

    [SerializeField]
    List<Image> _imgs;

    [SerializeField, Space(10)]
    UnityEvent _onClick;

    Transform _touchTf;
    TouchInteraction _touchInter;

    bool _canClick = true;

    public void TriggerEnter(bool isProximity, Collider touchCollider)
    {
        if(isProximity && touchCollider.CompareTag(Tags.ProximityUI))
        {
            _touchTf = touchCollider.transform.parent;
            _touchInter = _touchTf.GetComponent<TouchInteraction>();
            UpdateColor(InteractionStateUI.InProximity);
        }
        else if(!isProximity && touchCollider.CompareTag(Tags.InteractionUI))
        {
            TryClick();
        }
        
    }

    private void TryClick()
    {
        if (!_canClick)
            return;

        _canClick = false;
        UpdateColor(InteractionStateUI.Active);

        if (_touchInter != null)
            _touchInter.ActiveBtn(true, this);

        _onClick?.Invoke();
    }

    public void TriggerExit(bool isProximity, Collider touchCollider)
    {
        if(isProximity && touchCollider.CompareTag(Tags.ProximityUI))
        {
            _canClick = true;
            UpdateColor(InteractionStateUI.Normal);
        }
        else if (!isProximity && touchCollider.CompareTag(Tags.InteractionUI))
        {
            if (_touchInter != null && !_canClick)
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
}


public enum InteractionStateUI
{
    Normal,
    InProximity,
    Active
}

