using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public static class HttpHelper
{
    public static async Task<string> RetrieveRdf(string uri)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);

                request.Headers.Add("Accept", "application/rdf+xml");

                HttpResponseMessage response;

                try
                {
                    response = await client.SendAsync(request);
                }
                catch (Exception e)
                {
                    return "";
                }

                if (!response.IsSuccessStatusCode)
                {
                    return "";
                }

                string responseBody = await response.Content.ReadAsStringAsync();

                return responseBody;
            }

        }
        catch (Exception e)
        {
            return "";
        }
    }
}
