using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class RetrieveImageInternetTest : MonoBehaviour
{
    public string ImageUrl;

    [SerializeField]
    RawImage _image;

    [SerializeField]
    Texture2D _imageTexture;

    [ContextMenu("RetrieveImage")]
    public async void RetrieveImage()
    {
        string savepath = Path.Combine(Application.persistentDataPath, "VRKGUnity", "Data", "Tests");

        Debug.Log(savepath);

        if(!Directory.Exists(savepath))
            Directory.CreateDirectory(savepath);

        _image.texture = null;
        await DownloadImage(ImageUrl, savepath);
    }

    [ContextMenu("RetrieveImageCoroutine")]
    public void RetrieveImageCoroutine()
    {
        string savepath = Path.Combine(Application.persistentDataPath, "VRKGUnity", "Data", "Tests");

        Debug.Log(savepath);

        if (!Directory.Exists(savepath))
            Directory.CreateDirectory(savepath);

        _image.texture = null;
        StartCoroutine(DownloadLocalTexture(ImageUrl));
    }


    private async Task DownloadImage(string imageUrl, string savePath)
    {
        using HttpClient client = new();

        int milli = DateTime.Now.Millisecond;

        byte[] imageBytes = await client.GetByteArrayAsync(imageUrl);

        //milli = DateTime.Now.Millisecond;
        //Guid guid = Guid.NewGuid();
        //string uniqueId = guid.ToString();
        //string fileName = Path.GetFileName(imageUrl);
        //fileName = uniqueId + "_" + fileName;

        // Save the downloaded image as a PNG.
        //string fullPath = Path.Combine(savePath, fileName);

        //await File.WriteAllBytesAsync(fullPath, imageBytes);


        //Stopwatch stopwatch = new();
        //stopwatch.Start();

        // Convert bytes to Texture2D
        Texture2D texture = new(2, 2);
        texture.LoadImage(imageBytes);



        //texture.LoadImage(imageBytes); //..this will auto-resize the texture dimensions.

        //Debug.Log("B : " + stopwatch.Elapsed.TotalMilliseconds);
        //stopwatch.Restart();

        _image.texture = texture;
        //Debug.Log("C : " + stopwatch.Elapsed.TotalMilliseconds);
        //Drawing(System.Drawing.Color.White); 
    }



    IEnumerator DownloadLocalTexture(string filePath)
    {
        using UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(filePath);

        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(uwr.error);
            yield break;
        }


        //Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
        //Texture2D texture = DownloadHandler.GetCheckedDownloader<DownloadHandlerTexture>(uwr).texture;
        Texture2D texture = ((DownloadHandlerTexture)uwr.downloadHandler).texture;


        string savepath = Path.Combine(Application.persistentDataPath, "VRKGUnity", "Data", "Tests", "bb.bin");
        string savepath2 = Path.Combine(Application.persistentDataPath, "VRKGUnity", "Data", "Tests", "bb.jpg");

        yield return new WaitForSeconds(2f);


        Stopwatch stopwatch = new();
        stopwatch.Start();

        byte[] rawData = texture.GetRawTextureData();
        File.WriteAllBytes(savepath, rawData);
        Debug.Log(stopwatch.ElapsedMilliseconds + "----------");

        Debug.Log("--------------/-----------------");

        byte[] rawData2 = File.ReadAllBytes(savepath);
        Debug.Log(stopwatch.ElapsedMilliseconds + "------------");

        Texture2D loadedTexture = new Texture2D(texture.width, texture.height, texture.format, false);
        loadedTexture.LoadRawTextureData(rawData2);
        Debug.Log(stopwatch.ElapsedMilliseconds);

        loadedTexture.Apply();
        Debug.Log(stopwatch.ElapsedMilliseconds);

        _image.texture = loadedTexture;
        stopwatch.Stop();
        Debug.Log("a : " + stopwatch.ElapsedMilliseconds);

        yield return new WaitForSeconds(5f);
        _image.texture = null;
        yield return new WaitForSeconds(5f);
        Debug.Log("-------------");
        stopwatch.Reset();
        stopwatch.Restart();
        byte[] bytes;

        //if (Path.GetExtension(savepath2).ToLower() == ".png")
        //    bytes = texture.EncodeToPNG();
        //else

        Task.Run(() =>
        {
            bytes = ImageConversion.EncodeToJPG(texture);//texture.EncodeToJPG();

            // Note that you should not interact with Unity's API from here.
        });
        //bytes = ImageConversion.EncodeToJPG(texture);//texture.EncodeToJPG();

        Debug.Log(stopwatch.ElapsedMilliseconds + " <---- Encode");

        //File.WriteAllBytes(savepath2, bytes);
        Debug.Log(stopwatch.ElapsedMilliseconds + "-----------");
        using UnityWebRequest uwr2 = UnityWebRequestTexture.GetTexture(savepath2);

        Debug.Log("--------------/-----------------");

        yield return uwr2.SendWebRequest();
        Debug.Log(stopwatch.ElapsedMilliseconds);
        if (uwr2.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(uwr2.error);
            yield break;
        }

        Texture2D textureB = ((DownloadHandlerTexture)uwr2.downloadHandler).texture;

        //Stopwatch stopwatch = new();
        //stopwatch.Start();
        //Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
        //Debug.Log("B : " + stopwatch.Elapsed.TotalMilliseconds);
        //stopwatch.Restart();
        _image.texture = textureB;
        stopwatch.Stop();
        Debug.Log("b : " + stopwatch.ElapsedMilliseconds);
        //Debug.Log("C : " + stopwatch.Elapsed.TotalMilliseconds);
    }

    [ContextMenu("Test")]
    private void Test()
    {
        string savepath = Path.Combine(Application.persistentDataPath, "VRKGUnity", "Data", "Tests", "chateau.png");


        Task.Run(() =>
        {
            try
            {
                var bytes = ImageConversion.EncodeToPNG(_imageTexture);//texture.EncodeToJPG();
                File.WriteAllBytes(savepath, bytes);
            }
            catch (Exception ex) 
            {
                Debug.Log(ex);
            }

        });
    }
}
