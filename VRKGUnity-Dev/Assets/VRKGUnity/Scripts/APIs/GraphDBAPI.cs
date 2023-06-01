using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class GraphDBAPI
{

    public static GraphDbRepository GraphDbRepository { private get; set; }


    public delegate void ErrorQuery(HttpResponseMessage responseMessage);
    public static ErrorQuery OnErrorQuery;

    public async Task<string> Query(string query)
    {
        using (HttpClient client = new HttpClient())
        {
#if UNITY_EDITOR
            if (GraphDbRepository == null)
                GraphDbRepository = new GraphDbRepository("http://localhost:7200/", "cap44");
#endif


            string encodedQuery = WebUtility.UrlEncode(query);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, GraphDbRepository.ServerURL + "repositories/" + GraphDbRepository.RepositoryId + "?query=" + encodedQuery);
            request.Headers.Add("Accept", "application/sparql-results+json");

            HttpResponseMessage response;

            try
            {
                response = await client.SendAsync(request);
            }
            catch(Exception e)
            {
                var responseB = new HttpResponseMessage();
                responseB.StatusCode = HttpStatusCode.ServiceUnavailable;
                responseB.Content = new StringContent(e.Message);
                OnErrorQuery?.Invoke(responseB);
                return "";
            }

            if(!response.IsSuccessStatusCode)
            {
                OnErrorQuery?.Invoke(response);
                return "";
            }

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
    }
}
