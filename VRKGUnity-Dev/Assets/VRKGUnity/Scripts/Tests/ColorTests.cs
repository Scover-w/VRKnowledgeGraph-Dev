using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

public class ColorTests : MonoBehaviour
{
    [SerializeField]
    Color _colorA;

    [SerializeField]
    Color _colorB;

    [SerializeField]
    Color _colorC;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    float _boundaryColorA;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    float _boundaryColorB;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    float _boundaryColorC;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    float _value;

    [SerializeField]
    Color _resultColor;

    private void OnValidate()
    {
        if(_boundaryColorA > _boundaryColorB)
            _boundaryColorA = _boundaryColorB;

        if(_boundaryColorB > _boundaryColorC)
            _boundaryColorB = _boundaryColorC;

        if (_value < _boundaryColorA)
            _resultColor = _colorA;

        // Ensure value is within the boundaries
        float value = Math.Max(_boundaryColorA, Math.Min(_boundaryColorC, _value));

        if (value <= _boundaryColorB)
        {
            // Interpolate between colorA and colorB
            float t = (value - _boundaryColorA) / (_boundaryColorB - _boundaryColorA);
            _resultColor = Color.Lerp(_colorA, _colorB, t);
        }

        else
        {
            // Interpolate between colorB and colorC
            float t = (value - _boundaryColorB) / (_boundaryColorC - _boundaryColorB);
            _resultColor = Color.Lerp(_colorB, _colorC, t);
        }
    }
}
