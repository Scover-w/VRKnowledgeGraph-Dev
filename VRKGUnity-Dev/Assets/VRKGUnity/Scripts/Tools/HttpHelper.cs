using System;
using System.Net.Http;
using System.Threading.Tasks;

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
                DebugDev.Log(uri);
                DebugDev.Log("HttpHelper : " + e + " \n" + e.Message);
                return "";
            }

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                DebugDev.Log(uri);
                DebugDev.Log("HttpHelper : " + error);
                return "";
            }

            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;

        }
        catch (Exception e)
        {
            DebugDev.Log(uri);
            DebugDev.Log("HttpHelper : " + e);
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
