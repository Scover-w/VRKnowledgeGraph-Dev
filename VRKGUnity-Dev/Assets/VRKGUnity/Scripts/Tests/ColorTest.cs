using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTest : MonoBehaviour
{

    public int NbColor;
    [Range(0f, 1f)]
    public float Saturation;
    [Range(0f, 1f)]
    public float Value;

    [Range(0f, 1f)]
    public float Delta;


    public List<Color> colors;




    [ContextMenu("Create Colors")]
    public void CreateColors()
    {
        colors = new List<Color>();

        float hue = 1;


        float deltaHue = 1f / NbColor;


        for (int i = 0; i < NbColor; i++)
        {
            colors.Add(Color.HSVToRGB((deltaHue * i + Delta) % 1f, Saturation, Value));
        }

    }

    private void OnValidate()
    {
        CreateColors();
    }
}
