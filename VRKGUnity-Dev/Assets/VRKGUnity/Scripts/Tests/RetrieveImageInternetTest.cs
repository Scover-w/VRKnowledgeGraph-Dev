using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class RetrieveImageInternetTest : MonoBehaviour
{
    public string ImageUrl;

    [SerializeField]
    RawImage _image;

    [ContextMenu("RetrieveImage")]
    public async void RetrieveImage()
    {
        string savepath = Path.Combine(Application.dataPath, "VRKGUnity", "Data", "Tests");

        await DownloadImage(ImageUrl, savepath);
    }


    private async Task DownloadImage(string imageUrl, string savePath)
    {
        using HttpClient client = new();

        byte[] imageBytes = await client.GetByteArrayAsync(imageUrl);

        Guid guid = Guid.NewGuid();
        string uniqueId = guid.ToString();
        string fileName = Path.GetFileName(imageUrl);
        fileName = uniqueId + "_" + fileName;

        // Save the downloaded image as a PNG.
        string fullPath = Path.Combine(savePath, fileName);
        File.WriteAllBytes(fullPath, imageBytes);

        // Convert bytes to Texture2D
        Texture2D texture = new(2, 2);
        texture.LoadImage(imageBytes); //..this will auto-resize the texture dimensions.

        _image.texture = texture;
    }
}
