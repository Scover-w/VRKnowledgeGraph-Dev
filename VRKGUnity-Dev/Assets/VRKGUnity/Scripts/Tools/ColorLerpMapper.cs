using Newtonsoft.Json;
using System;
using UnityEngine;


[Serializable]
public class ColorLerpMapper
{

    [JsonIgnore]
    public Color ColorA
    {
        get
        {
            return _colorA.ToUnityColor();
        }
        set
        {
            _colorA = value.ToSystemColor();
        }
    }

    [JsonIgnore]
    public Color ColorB
    {
        get
        {
            return _colorB.ToUnityColor();
        }
        set
        {
            _colorB = value.ToSystemColor();
        }
    }

    [JsonIgnore]
    public Color ColorC
    {
        get
        {
            return _colorC.ToUnityColor();
        }
        set
        {
            _colorC = value.ToSystemColor();
        }
    }

    [JsonIgnore]
    public float BoundaryColorA
    {
        get 
        { 
            return _boundaryColorA; 
        }

        set
        {
            _boundaryColorA = value;

            if (_boundaryColorA > _boundaryColorB)
                _boundaryColorA = _boundaryColorB;
        }
    }

    [JsonIgnore]
    public float BoundaryColorB
    {
        get
        {
            return _boundaryColorB;
        }

        set
        {
            _boundaryColorB = value;

            if (_boundaryColorB > _boundaryColorC)
                _boundaryColorB = _boundaryColorC;

            if (_boundaryColorB < _boundaryColorA)
                _boundaryColorB = _boundaryColorA;
        }
    }

    [JsonIgnore]
    public float BoundaryColorC
    {
        get
        {
            return _boundaryColorC;
        }

        set
        {
            _boundaryColorC = value;

            if (_boundaryColorC < _boundaryColorB)
                _boundaryColorC = _boundaryColorB;
        }
    }


    [JsonProperty("ColorA_")]
    [SerializeField]
    System.Drawing.Color _colorA = System.Drawing.Color.FromArgb(255, 0, 0);

    [JsonProperty("ColorB_")]
    [SerializeField]
    System.Drawing.Color _colorB = System.Drawing.Color.FromArgb(0, 255, 0);

    [JsonProperty("ColorC_")]
    [SerializeField]
    System.Drawing.Color _colorC = System.Drawing.Color.FromArgb(0, 0, 255);

    [JsonProperty("BoundaryColorA_")]
    [SerializeField]
    [Range(0f, 1f)]
    float _boundaryColorA = 0f;
    [JsonProperty("BoundaryColorB_")]
    [SerializeField]
    [Range(0f, 1f)]
    float _boundaryColorB = 0.5f;
    [JsonProperty("BoundaryColorC_")]
    [SerializeField]
    [Range(0f, 1f)]
    float _boundaryColorC = 1f;



    public Color Lerp(float value)
    {

        value = Math.Max(_boundaryColorA, Math.Min(_boundaryColorC, value));

        if (value <= _boundaryColorB)
        {
            // Interpolate between colorA and colorB
            float t = (value - _boundaryColorA) / (_boundaryColorB - _boundaryColorA);
            return Color.Lerp(ColorA, ColorB, t);
        }

        else
        {
            // Interpolate between colorB and colorC
            float t = (value - _boundaryColorB) / (_boundaryColorC - _boundaryColorB);
            return Color.Lerp(ColorB, ColorC, t);
        }
    }

}
