using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VDS.RDF.Query;

public class GraphDBAPI
{
    public delegate void ErrorQuery(HttpResponseMessage responseMessage);
    public static ErrorQuery OnErrorQuery;

    private string _serverUrl;
    private string _repositoryId;

    public GraphDBAPI(GraphDbRepository graphDbRepository)
    {
        if(graphDbRepository == null)
        {
            Debug.LogError("GraphDBAPI : graphDbRepository is null");
            _serverUrl = "http://localhost:7200/";
            _repositoryId = "cap44";
            return;
        }

        _serverUrl = graphDbRepository.ServerURL;
        _repositoryId = graphDbRepository.RepositoryId;
    }

    /// <summary>
    /// For Select query.
    /// Max sparqlQuery length is 8000.
    /// </summary>
    /// <param name="sparqlQuery"></param>
    /// <returns></returns>
    public async Task<string> SelectQuery(string sparqlQuery)
    {
        using (HttpClient client = new HttpClient())
        {

            string encodedQuery = WebUtility.UrlEncode(sparqlQuery);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _serverUrl + "repositories/" + _repositoryId + "?query=" + encodedQuery);
            request.Headers.Add("Accept", "application/sparql-results+json");

            HttpResponseMessage response;

            try
            {
                response = await client.SendAsync(request);
            }
            catch (Exception e)
            {
                var responseB = new HttpResponseMessage();
                responseB.StatusCode = HttpStatusCode.ServiceUnavailable;
                responseB.Content = new StringContent(e.Message);
                OnErrorQuery?.Invoke(responseB);
                return "";
            }

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                var responseC = new HttpResponseMessage();
                responseC.StatusCode = response.StatusCode;
                responseC.Content = new StringContent(error);
                OnErrorQuery?.Invoke(responseC);
                return "";
            }

            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
    }


    /// <summary>
    /// For Insert or Delete query.
    /// </summary>
    /// <param name="sparqlQuery"></param>
    /// <returns></returns>
    public async Task<bool> UpdateQuery(string sparqlQuery)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _serverUrl + "repositories/" + _repositoryId + "/statements");

            var multiPartContent = new MultipartFormDataContent();
            multiPartContent.Add(new StringContent(sparqlQuery), "update");

            request.Content = multiPartContent;

            HttpResponseMessage response;

            try
            {
                response = await client.SendAsync(request);
            }
            catch (Exception e)
            {
                var responseB = new HttpResponseMessage();
                responseB.StatusCode = HttpStatusCode.ServiceUnavailable;
                responseB.Content = new StringContent(e.Message);
                OnErrorQuery?.Invoke(responseB);
                return false;
            }

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                var responseC = new HttpResponseMessage();
                responseC.StatusCode = response.StatusCode;
                responseC.Content = new StringContent(error);
                OnErrorQuery?.Invoke(responseC);
                return false;
            }

            return true;
        }
    }

    public async Task<bool> LoadFileContentInDatabase(string fileContent, GraphDBAPIFileType type)
    {

        using (HttpClient client = new HttpClient())
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _serverUrl + "repositories/" + _repositoryId + "/statements");
            request.Content = new StringContent(fileContent, Encoding.UTF8, (type == GraphDBAPIFileType.Turtle) ? "text/turtle" : "application/rdf+xml");

            HttpResponseMessage response;

            try
            {
                response = await client.SendAsync(request);
            }
            catch (Exception e)
            {
                var responseB = new HttpResponseMessage();
                responseB.StatusCode = HttpStatusCode.ServiceUnavailable;
                responseB.Content = new StringContent(e.Message);
                OnErrorQuery?.Invoke(responseB);
                return false;
            }

            if (!response.IsSuccessStatusCode)
            {
                OnErrorQuery?.Invoke(response);
                return false;
            }

            response.EnsureSuccessStatusCode();

            return true;
        }
    }

    public static async Task<bool> DoRepositoryExist(string serverUrl, string repositoryId)
    {
        using (HttpClient client = new HttpClient())
        {

            string encodedQuery = WebUtility.UrlEncode("select * where { ?s ?p ?o .} LIMIT 1");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, serverUrl + "repositories/" + repositoryId + "?query=" + encodedQuery);
            request.Headers.Add("Accept", "application/sparql-results+json");

            HttpResponseMessage response;

            try
            {
                response = await client.SendAsync(request);
            }
            catch (Exception e)
            {
                var responseB = new HttpResponseMessage();
                responseB.StatusCode = HttpStatusCode.ServiceUnavailable;
                responseB.Content = new StringContent(e.Message);
                OnErrorQuery?.Invoke(responseB);
                return false;
            }

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                var responseC = new HttpResponseMessage();
                responseC.StatusCode = response.StatusCode;
                responseC.Content = new StringContent(error);
                OnErrorQuery?.Invoke(responseC);
                return false;
            }

            return true;
        }
    }
}


public enum GraphDBAPIFileType
{
    Turtle,
    Rdf
}