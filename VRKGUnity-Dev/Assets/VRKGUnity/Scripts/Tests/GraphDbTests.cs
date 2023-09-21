using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static GraphDBAPI;

public class GraphDbTests : MonoBehaviour
{
    public string Token;
    public string ValueToEncrypt;



    [ContextMenu("Test")]
    public async void Test()
    {
        Debug.Log("Test");
        var query = await SelectQuery("https://rdf-epotec.univ-nantes.fr/", "cap44", "SELECT ?p (COUNT(?p) as ?count) WHERE    {  ?s ?p ?o .   }GROUP BY ?p ORDER BY DESC(?count)");

    }

    [ContextMenu("SelectOnOpenBioDiv")]
    public async void SelectOnOpenBioDiv()
    {

        using HttpClient client = new();
        HttpRequestMessage request = new(HttpMethod.Post, "https://graph.openbiodiv.net/" + "repositories/OpenBiodiv" + "?infer=" + false.ToString().ToLower());

        client.DefaultRequestHeaders.Add("Accept", "application/sparql-results+json");
        request.Content = new StringContent("select * where { ?s ?p ?o .} limit 10", Encoding.UTF8, "application/sparql-query");

        HttpResponseMessage response;

        try
        {
            response = await client.SendAsync(request);
        }
        catch (Exception e)
        {
            HttpResponseMessage responseB = new()
            {
                StatusCode = HttpStatusCode.ServiceUnavailable,
                Content = new StringContent(e.Message)
            };

            OnErrorQuery?.Invoke(responseB);
            return;
        }

        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync();

            HttpResponseMessage responseC = new()
            {
                StatusCode = response.StatusCode,
                Content = new StringContent(error)
            };

            OnErrorQuery?.Invoke(responseC);
            return;
        }


        string answer = await response.Content.ReadAsStringAsync();

        Debug.Log(answer);
    }
    

    public async Task<string> SelectQuery(string _serverUrl, string _repositoryId, string query, bool doInfer = false)
    {
        // Use Post instead of Get because Get request only allow to put the query in the url, which crash the request when the query is too long.And can't use StringContent or MultiPartFormDataContent with Get



        using HttpClient client = new();
        HttpRequestMessage request = new(HttpMethod.Post, _serverUrl + "repositories/" + _repositoryId + "?infer=" + doInfer.ToString().ToLower());

        client.DefaultRequestHeaders.Add("Accept", "application/sparql-results+json");
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("GDB", Token);
        request.Content = new StringContent(query, Encoding.UTF8, "application/sparql-query");

        HttpResponseMessage response;

        try
        {
            response = await client.SendAsync(request);
        }
        catch (Exception e)
        {
            HttpResponseMessage responseB = new()
            {
                StatusCode = HttpStatusCode.ServiceUnavailable,
                Content = new StringContent(e.Message)
            };

            OnErrorQuery?.Invoke(responseB);
            return "";
        }

        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync();

            HttpResponseMessage responseC = new()
            {
                StatusCode = response.StatusCode,
                Content = new StringContent(error)
            };

            OnErrorQuery?.Invoke(responseC);
            return "";
        }


        string answer = await response.Content.ReadAsStringAsync();

        return answer;
    }
}
