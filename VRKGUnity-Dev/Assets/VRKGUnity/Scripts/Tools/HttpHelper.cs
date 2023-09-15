using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class HttpHelper
{
    public static async Task<string> RetrieveRdf(string uri)
    {
        try
        {
            using HttpClient client = new();

            client.Timeout = TimeSpan.FromSeconds(15);
            HttpRequestMessage request = new(HttpMethod.Get, uri);
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
                string error = await response.Content.ReadAsStringAsync();


                if (response.StatusCode == HttpStatusCode.Redirect
                    || response.StatusCode == HttpStatusCode.MovedPermanently
                    || response.StatusCode == HttpStatusCode.SeeOther)
                {
                    var newUri = response.Headers.Location;
                    return await RetrieveRdf(newUri.ToString());
                }

                return "";
            }

            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;

        }
        catch
        {

            return "";
        }
    }

    public static async Task<string> Retrieve(string url)
    {
        try
        {
            using HttpClient client = new();

            client.Timeout = TimeSpan.FromSeconds(30);
            HttpRequestMessage request = new(HttpMethod.Get, url);

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
                string error = await response.Content.ReadAsStringAsync();


                if (response.StatusCode == HttpStatusCode.Redirect
                    || response.StatusCode == HttpStatusCode.MovedPermanently
                    || response.StatusCode == HttpStatusCode.SeeOther)
                {
                    var newUri = response.Headers.Location;
                    return await Retrieve(newUri.ToString());
                }

                return "";
            }

            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;

        }
        catch
        {

            return "";
        }
    }

    public static async Task<bool> IsConnectedToInternet()
    {
        try
        {
            using HttpClient client = new();

            client.Timeout = TimeSpan.FromSeconds(10);
            HttpRequestMessage request = new(HttpMethod.Get, "http://www.google.com");

            HttpResponseMessage response;

            try
            {
                response = await client.SendAsync(request);
            }
            catch
            {
                return false;
            }

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;

        }
        catch
        {
            return false;
        }
    }

    public static async Task<bool> Ping(string url)
    {
        try
        {
            using HttpClient client = new();

            client.Timeout = TimeSpan.FromSeconds(10);
            HttpRequestMessage request = new(HttpMethod.Get, url);

            HttpResponseMessage response;

            try
            {
                response = await client.SendAsync(request);
            }
            catch
            {
                return false;
            }

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;

        }
        catch
        {
            return false;
        }
    }
}
