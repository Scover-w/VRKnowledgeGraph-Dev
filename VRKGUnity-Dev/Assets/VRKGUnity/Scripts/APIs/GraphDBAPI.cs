using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Writing;
using VDS.RDF.Writing.Formatting;
using static Codice.CM.WorkspaceServer.WorkspaceTreeDataStore;

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

    public async Task<string> SelectQuery(string sparqlQuery)
    {
        using (HttpClient client = new HttpClient())
        {

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _serverUrl + "repositories/" + _repositoryId);
            request.Headers.Add("Accept", "application/sparql-results+json");

            var parameters = new Dictionary<string, string> { { "query", sparqlQuery } };
            var encodedContent = new FormUrlEncodedContent(parameters);
            request.Content = encodedContent;


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

            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
    }

    public async Task<bool> InsertQuery(string sparqlQuery)
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
                OnErrorQuery?.Invoke(response);
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
}


public enum GraphDBAPIFileType
{
    Turtle,
    Rdf
}