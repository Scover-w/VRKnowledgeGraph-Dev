using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;
using static GraphDBAPI;

public static class HttpHelper
{
    public static async Task<string> RetrieveRdf(string uri)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(15);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);

                request.Headers.Add("Accept", "application/rdf+xml");

                HttpResponseMessage response;

                try
                {
                    response = await client.SendAsync(request);
                }
                catch (Exception e)
                {
                    //Debug.Log("HttpHelper : " + e + " \n" + e.Message);
                    return "";
                }

                if (!response.IsSuccessStatusCode)
                {
                    //string error = await response.Content.ReadAsStringAsync();
                    //Debug.Log("HttpHelper : " + error);
                    return "";
                }

                string responseBody = await response.Content.ReadAsStringAsync();

                return responseBody;
            }

        }
        catch (Exception e)
        {
            //Debug.Log("HttpHelper : " + e);
            return "";
        }
    }
}
