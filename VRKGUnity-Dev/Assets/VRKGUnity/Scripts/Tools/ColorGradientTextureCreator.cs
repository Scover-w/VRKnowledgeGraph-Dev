using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ColorGradientTextureCreator : MonoBehaviour
{
    [SerializeField]
    RawImage _img;

    [Range(0f, 1f)]
    public float H;
    [Range(0f, 1f)]
    public float S;
    [Range(0f, 1f)]
    public float V;

    private void OnValidate()
    {
        GenerateRadialGradient(V, 1024);
    }



    async void GenerateRadialGradient(float v, int size)
    {
        Color[] colors = await Task.Run(() => ComputeGradient(v, size));
        Texture2D texture = new Texture2D(size, size);
        texture.SetPixels(colors);
        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();

        // Create a file and write the PNG data to it
        File.WriteAllBytes(Application.persistentDataPath + "/radialHSV.png", bytes);
        Debug.Log(Application.persistentDataPath + "/radialHSV.png");
        _img.texture = texture;
    }

    Color[] ComputeGradient(float v, int size)
    {
        Color[] colors = new Color[size * size];

        Vector2 center = new Vector2(size / 2, size / 2);
        float radius = size / 2;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 diff = new Vector2(x, y) - center;
                float distance = diff.magnitude;
                float angle = (Mathf.Atan2(diff.y, diff.x) + Mathf.PI) / (2 * Mathf.PI);

                int index = y * size + x;

                if (distance < radius)
                {
                    float s = distance / radius;
                    colors[index] = Color.HSVToRGB(angle, s, v);
                }
                else
                {
                    colors[index] = Color.clear;
                }
            }
        }

        return colors;
    }
}
