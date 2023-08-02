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
    List<InteractiveColorImage> _interactiveImgs;

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
        else if(!isProximity && touchCollider.CompareTag(Tags.ActiveUI))
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
        else if (!isProximity && touchCollider.CompareTag(Tags.ActiveUI))
        {
            if (_touchInter != null && !_canClick)
                _touchInter.ActiveBtn(false, this);

            UpdateColor(InteractionStateUI.Normal);
        }
       
    }

    private void UpdateColor(InteractionStateUI interactionState)
    {
        foreach(var interactionColor in _interactiveImgs)
        {
            switch (interactionState)
            {
                case InteractionStateUI.Normal:
                    interactionColor.Img.color = interactionColor.NormalColor;
                    break;
                case InteractionStateUI.InProximity:
                    interactionColor.Img.color = interactionColor.ProximityColor;
                    break;
                case InteractionStateUI.Active:
                    interactionColor.Img.color = interactionColor.ActivatedColor;
                    break;
            }
            
        }
    }

    private void OnValidate()
    {
        if (_interactiveImgs == null)
            return;

        for(int i = 0; i < _interactiveImgs.Count; i++) 
        { 
            var interactiveImg = _interactiveImgs[i];
            var img = interactiveImg.Img;
            interactiveImg.Name = (img == null) ? "None" : img.name;
        }
    }

    [Serializable]
    public class InteractiveColorImage
    {
        public Image Img { get { return _img; } }
        public Color NormalColor { get { return _normalColor; } }
        public Color ProximityColor { get { return _proximityColor; } }
        public Color ActivatedColor { get { return _activatedColor; } }

        [HideInInspector]
        public string Name;

        [SerializeField]
        Image _img;

        [SerializeField]
        Color _normalColor;

        [SerializeField]
        Color _proximityColor;

        [SerializeField]
        Color _activatedColor;
    }
}


public enum InteractionStateUI
{
    Normal,
    InProximity,
    Active
}

