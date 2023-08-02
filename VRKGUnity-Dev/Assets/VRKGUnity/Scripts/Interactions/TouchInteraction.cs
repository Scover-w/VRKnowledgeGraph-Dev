using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInteraction : MonoBehaviour
{
    [SerializeField]
    Color _normalColor;

    [SerializeField]
    Color _proximityColor;

    [SerializeField]
    Color _activeColor;

    [SerializeField]
    MeshRenderer _renderer;

    HashSet<MonoBehaviour> _activeButtons;

    Material _sharedMat;

    int _nbInProximity = 0;


    private void Start()
    {
        _activeButtons = new();
        _sharedMat = _renderer.sharedMaterial;
        UpdateColor();
    }

    public void TriggerProximityEnter()
    {
        _nbInProximity++;
        UpdateColor();
    }

    public void TriggerProximityExit()
    {
        _nbInProximity--;
        UpdateColor();
    }

    public void ActiveBtn(bool doActive, MonoBehaviour button)
    {
        if (doActive && !_activeButtons.Contains(button))
        {
            _activeButtons.Add(button);
        }
        else if(!doActive && _activeButtons.Contains(button))
        {
            _activeButtons.Remove(button);
        }

        UpdateColor();
    }

    private void UpdateColor()
    {
        int nbInActive = _activeButtons.Count;

        if ((nbInActive + _nbInProximity) == 0)
            _sharedMat.color = _normalColor;
        else if(nbInActive > 0)
            _sharedMat.color = _activeColor;
        else
            _sharedMat.color = _proximityColor;
    }

    private void OnDisable()
    {
        _sharedMat.color = _normalColor;
    }
}
