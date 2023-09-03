using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LabelScaleTest : MonoBehaviour
{
    [SerializeField]
    RectTransform _canvasRect;

    [SerializeField]
    RectTransform _labelRect;

    [SerializeField]
    TMP_Text _labelTxt;

    public Transform CamTf;
    public Transform CanvasTf;

    public float _scaleSize = 2f;
    bool _inRelativeMode = false;

    private void Update()
    {
        UpdateSize();
    }


    void UpdateSize()
    {

        float distance = (CamTf.position - CanvasTf.position).magnitude;

        float baseFontSize = Settings.BASE_FONT_SIZE_LABEL;
        Vector2 baseCanvasLabel = Settings.BASE_SIZE_LABEL_CANVAS;
        float minReadableFontSize = Settings.MIN_READABLE_FONT_SIZE;


        float atomicFontSize = (baseFontSize * _scaleSize) / distance;

        if (atomicFontSize < minReadableFontSize) // Realtive Canvas Size Mode : Need to be readable even far
        {
            _inRelativeMode = true;
            float relativeScale = (minReadableFontSize * distance) / baseFontSize;

            Vector2 sizeConvasA = baseCanvasLabel * relativeScale;
            float fontSizeA = baseFontSize * relativeScale;
            SetSize(_scaleSize, sizeConvasA, fontSizeA);
            return;
        }

        // Normal Size
        if (!_inRelativeMode)
            return;


        _inRelativeMode = false;
        Vector2 sizeConvasB = baseCanvasLabel * _scaleSize;
        float fontSizeB = baseFontSize * _scaleSize;
        SetSize(_scaleSize, sizeConvasB, fontSizeB);

    }

    public void SetSize(float scaleSize, Vector2 sizeCanvas, float fontSize)
    {
        _scaleSize = scaleSize;

        _canvasRect.sizeDelta = sizeCanvas;
        _labelRect.sizeDelta = sizeCanvas;

        _labelTxt.fontSize = fontSize;
    }
}
