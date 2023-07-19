using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class MediaAPI
{
    private async Task<Texture2D> DownloadAndSaveImage(string imageUrl, string savePath)
    {
        string extension = Path.GetExtension(imageUrl).ToLower();

        if (!(extension.Contains(".jpg") || extension.Contains(".png")))
            return null;

        using HttpClient client = new();

        try
        {
            byte[] imageBytes = await client.GetByteArrayAsync(imageUrl);


            await File.WriteAllBytesAsync(savePath, imageBytes);

            Texture2D texture = new(2, 2);
            texture.LoadImage(imageBytes); // Auto-resize the texture dimensions.

            return texture;
        }
        catch(Exception e)
        {
            HttpResponseMessage responseB = new()
            {
                StatusCode = HttpStatusCode.ServiceUnavailable,
                Content = new StringContent(e.Message)
            };

            GraphDBAPI.OnErrorQuery?.Invoke(responseB);
            return null;
        }
    }
}
