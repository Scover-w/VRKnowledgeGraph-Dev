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
using VDS.RDF.Writing;

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

    public async Task<string> Query(string query)
    {
        using (HttpClient client = new HttpClient())
        {

            string encodedQuery = WebUtility.UrlEncode(query);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _serverUrl + "repositories/" + _repositoryId + "?query=" + encodedQuery);
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

    public async Task<bool> LoadOntologyInDatabase(IGraph graph)
    {
        var writer = new CompressingTurtleWriter();
        var serializedRdf = new System.IO.StringWriter();
        writer.Save(graph, serializedRdf);

        string rdfData = serializedRdf.ToString();

        using (HttpClient client = new HttpClient())
        {
            // Create the request message
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, _serverUrl + "repositories/" + _repositoryId + "/statements");

            // Set the content of the request message to be the serialized RDF graph
            request.Content = new StringContent(rdfData, Encoding.UTF8, "text/turtle");

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
