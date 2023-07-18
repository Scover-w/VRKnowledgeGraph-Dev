using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelativeCanvasTest : MonoBehaviour
{
    public float ScaleSize;
    public float MinReadableFontSize;

    [Header("References")]
    [SerializeField]
    Transform _camTf;

    [SerializeField]
    RectTransform _canvasRect;

    [SerializeField]
    RectTransform _labelRect;

    [SerializeField]
    TMP_Text _labelTxt;

    [SerializeField]
    TMP_Text _valueTxt;

    [SerializeField]
    Slider _slider;

    Transform _canvasTf;
    Vector2 _baseSizeCanvas = Settings.BASE_SIZE_LABEL_CANVAS;
    float _baseFontSize = Settings.BASE_FONT_SIZE_LABEL;

    void Start()
    {
        _canvasTf = _canvasRect.transform;
    }


    public void OnValueChanged()
    {
        MinReadableFontSize = _slider.value;
        _valueTxt.text = _slider.value.ToString();
    }

    void Update()
    {
        Vector3 deltaDir = _canvasTf.position - _camTf.position;
        float distance = deltaDir.magnitude;

        float atomicFontSize = (_baseFontSize * ScaleSize) / distance;

        Vector2 sizeConvas;
        float fontSize;


        if (atomicFontSize < MinReadableFontSize) // Realtive Canvas Size Mode : Need to be readable even far
        {
            Debug.Log("Relative Canvas Mode");
            float relativeScale = (MinReadableFontSize * distance) / _baseFontSize;

            sizeConvas = _baseSizeCanvas * relativeScale;
            fontSize = _baseFontSize * relativeScale;
        }
        else // Normal Size
        {
            Debug.Log("Normal Canvas Mode");
            sizeConvas = _baseSizeCanvas * ScaleSize;
            fontSize = _baseFontSize * ScaleSize;
        }

        SetSize(sizeConvas, fontSize);

        _canvasTf.rotation = Quaternion.LookRotation(deltaDir);
    }

    private void SetSize(Vector2 sizeCanvas, float fontSize)
    {
        _canvasRect.sizeDelta = sizeCanvas;
        _labelRect.sizeDelta = sizeCanvas;

        _labelTxt.fontSize = fontSize;
    }
}
