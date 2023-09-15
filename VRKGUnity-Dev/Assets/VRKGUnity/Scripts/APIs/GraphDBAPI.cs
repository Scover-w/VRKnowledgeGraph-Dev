using Codice.Client.BaseCommands;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using UnityEditor.PackageManager;
using UnityEngine;

public class GraphDBAPI
{
    public string ServerUrl { get { return _serverUrl; } }

    public delegate void ErrorQuery(HttpResponseMessage responseMessage);
    public static ErrorQuery OnErrorQuery;

    private string _serverUrl;
    private readonly string _repositoryId;

    public GraphDBAPI(GraphDbRepository graphDbRepository)
    {
        if(graphDbRepository == null)
        {
            _serverUrl = "http://localhost:7200/";
            _repositoryId = "cap44";

            if (!Application.isPlaying)
                return;

            Debug.LogError("GraphDBAPI : graphDbRepository is null");
            return;
        }

        _serverUrl = graphDbRepository.GraphDbUrl;
        _repositoryId = graphDbRepository.GraphDbRepositoryId;

        if (_serverUrl[_serverUrl.Length - 1] != '/')
            _serverUrl += "/";

    }

    public void OverrideForTest(string serverUrl)
    {
        _serverUrl = serverUrl;
    }

    /// <summary>
    /// For Select query.
    /// Max sparqlQuery length is 8000.
    /// </summary>
    /// <param name="sparqlQuery"></param>
    /// <returns></returns>
    public async Task<string> SelectQuery(string sparqlQuery, bool doInfer = false)
    {
        using HttpClient client = new();
        Debug.Log(_serverUrl + " " + _repositoryId);
        string encodedQuery = WebUtility.UrlEncode(sparqlQuery);
        HttpRequestMessage request = new(HttpMethod.Get, _serverUrl + "repositories/" + _repositoryId + "?query=" + encodedQuery + "&infer=" + doInfer.ToString().ToLower());
        request.Headers.Add("Accept", "application/sparql-results+json");

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

        string responseBody = await response.Content.ReadAsStringAsync();
        return responseBody;
    }


    /// <summary>
    /// For Insert or Delete query.
    /// </summary>
    /// <param name="sparqlQuery"></param>
    /// <returns></returns>
    public async Task<bool> UpdateQuery(string sparqlQuery)
    {
        using HttpClient client = new();

        HttpRequestMessage request = new(HttpMethod.Post, _serverUrl + "repositories/" + _repositoryId + "/statements");

        MultipartFormDataContent multiPartContent = new()
        {
            { new StringContent(sparqlQuery), "update" }
        };

        request.Content = multiPartContent;

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
            return false;
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
            return false;
        }

        return true;
    }

    public async Task<bool> LoadFileContentInDatabase(string fileContent, string graphName, GraphDBAPIFileType type)
    {

        using HttpClient client = new();

        HttpRequestMessage request = new(HttpMethod.Post, _serverUrl + "repositories/" + _repositoryId + "/statements?context=" + graphName)
        {
            Content = new StringContent(fileContent, Encoding.UTF8, (type == GraphDBAPIFileType.Turtle) ? "text/turtle" : "application/rdf+xml")
        };

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
            return false;
        }

        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync();
            OnErrorQuery?.Invoke(response);
            return false;
        }

        return true;
    }

    public static async Task<bool> DoRepositoryExist(string serverUrl, string repositoryId)
    {
        using HttpClient client = new();

        string encodedQuery = WebUtility.UrlEncode("select * where { ?s ?p ?o .} LIMIT 1");
        HttpRequestMessage request = new(HttpMethod.Get, serverUrl + "repositories/" + repositoryId + "?query=" + encodedQuery);
        request.Headers.Add("Accept", "application/sparql-results+json");
        client.Timeout = TimeSpan.FromSeconds(30);

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
            return false;
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
            return false;
        }

        return true;
    }


    public async Task<string> StartTransaction()
    {
        using HttpClient client = new();

        try
        {
            var response = await client.PostAsync(_serverUrl + "repositories/" + _repositoryId + "/transactions", null);

            if (response.IsSuccessStatusCode)
            {
                var locationHeader = response.Headers.GetValues("location").FirstOrDefault();
                var transactionID = locationHeader?.Split('/').Last();

                return transactionID;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> AbortTransaction(string transactionId)
    {
        using HttpClient client = new();

        try
        {
            var response = await client.DeleteAsync(_serverUrl + "repositories/" + _repositoryId + "/transactions/" + transactionId);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> CommitTransaction(string transactionId)
    {
        try
        {
            using HttpClient client = new();

            var response = await client.PutAsync(_serverUrl + "repositories/" + _repositoryId + "/transactions/" + transactionId + "?action=COMMIT", null);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

}


public enum GraphDBAPIFileType
{
    Turtle,
    Rdf
}