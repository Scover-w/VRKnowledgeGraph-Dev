using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientEdgeTest : MonoBehaviour
{
    [SerializeField]
    LineRenderer _lineRenderer;

    [SerializeField]
    Color _colorA;

    [SerializeField]
    Color _colorB;


    private void Start()
    {
        SetTest();
    }


    private void SetTest()
    {
        // Create a new gradient
        var gradient = new Gradient();

        // Set up the color keys
        var colorKey = new GradientColorKey[2];
        colorKey[0].color = Color.red;
        colorKey[0].time = 0f;

        // Set up the alpha keys
        var alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.5f;
        alphaKey[1].alpha = 0.0f;
        alphaKey[1].time = 1.0f;

        // Apply the color and alpha keys to the gradient
        gradient.SetKeys(colorKey, alphaKey);

        _lineRenderer.colorGradient = gradient;
    }
}
