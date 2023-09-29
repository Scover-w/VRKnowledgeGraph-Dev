using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static GraphDbTests;

/// <summary>
/// Manage all the request to the GraphDb server.
/// </summary>
public class GraphDBAPI
{
    public string ServerUrl { get { return _serverUrl; } }

    public delegate void ErrorQuery(HttpResponseMessage responseMessage);
    public static ErrorQuery OnErrorQuery;

    private string _authenticationGDBToken;

    private string _username;
    private string _password;

    private string _serverUrl;
    private readonly string _repositoryId;


    public GraphDBAPI(GraphDbRepository graphDbRepository)
    {
        if(graphDbRepository == null)
        {
            _serverUrl = "https://rdf-epotec.univ-nantes.fr/";
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

    public async Task SetCredentials(string username, string password)
    {
        _username = username;
        _password = password;

        await GetGDBTokenAsync();
    }


    public async Task<bool> GetGDBTokenAsync()
    {
        // TODO : Only one HTTPClient instance for all
        HttpClient httpClient = new();
        HttpRequestMessage request = new(HttpMethod.Post, $"{_serverUrl}/rest/login/{_username}");
        request.Headers.Add("X-GraphDB-Password", _password);

        try
        {
            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            if (response.Headers.TryGetValues("Authorization", out var tokenValues))
            {
                var token = tokenValues.FirstOrDefault();
                _authenticationGDBToken = token.Replace("GDB ", "");
                return true;
            }
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

        return false;
    }


    public async Task<string> SelectQuery(string query, bool doInfer = false)
    {
        // Use Post instead of Get because Get request only allow to put the query in the url, which crash the request when the query is too long.And can't use StringContent or MultiPartFormDataContent with Get

        using HttpClient client = new();
        if (!string.IsNullOrEmpty(_authenticationGDBToken))
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("GDB", _authenticationGDBToken);

        HttpRequestMessage request = new(HttpMethod.Post, _serverUrl + "repositories/" + _repositoryId + "?infer=" + doInfer.ToString().ToLower()); 

        client.DefaultRequestHeaders.Add("Accept", "application/sparql-results+json");
        request.Content = new StringContent(query, Encoding.UTF8, "application/sparql-query");

        HttpResponseMessage response;

        try
        {
            response = await client.SendAsync(request);
        }
        catch (Exception e)
        {
            DebugDev.Log("SelectQuery error " + e.Message);
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

            Debug.Log("SelectQuery No success : " + error);

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



    /// <summary>
    /// For Insert or Delete query.
    /// </summary>
    /// <param name="sparqlQuery"></param>
    /// <returns></returns>
    public async Task<bool> UpdateQuery(string sparqlQuery)
    {
        using HttpClient client = new();

        if (!string.IsNullOrEmpty(_authenticationGDBToken))
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("GDB", _authenticationGDBToken);

        HttpRequestMessage request = new(HttpMethod.Post, _serverUrl + "repositories/" + _repositoryId + "/statements");
        MultipartFormDataContent multiPartContent = new()
        {
            // TODO : find a better solution 
            { new StringContent(sparqlQuery.Replace("\n", ""), Encoding.UTF8), "update" }  // \n make the request crash 
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

        if (!string.IsNullOrEmpty(_authenticationGDBToken))
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("GDB", _authenticationGDBToken);

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

    public async Task<string> StartTransaction()
    {
        using HttpClient client = new();

        if (!string.IsNullOrEmpty(_authenticationGDBToken))
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("GDB", _authenticationGDBToken);

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

        if (!string.IsNullOrEmpty(_authenticationGDBToken))
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("GDB", _authenticationGDBToken);

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

            if (!string.IsNullOrEmpty(_authenticationGDBToken))
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("GDB", _authenticationGDBToken);

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

    public async Task<RepositoryStatus> DoRepositoryExist()
    {

        using HttpClient client = new();
        HttpRequestMessage request = new(HttpMethod.Get, _serverUrl + "repositories");

        if (!string.IsNullOrEmpty(_authenticationGDBToken))
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("GDB", _authenticationGDBToken);

        client.DefaultRequestHeaders.Add("Accept", "application/sparql-results+json");

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
            return RepositoryStatus.CouldntConnect;
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


            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return RepositoryStatus.Unauthorized;

            return RepositoryStatus.Failed;
        }


        string answer = await response.Content.ReadAsStringAsync();


        return ContainsRepositoryId(answer, _repositoryId);
    }


    #region STATIC
    public static async Task<RepositoryStatus> DoRepositoryExist(string serverUrl, string repositoryId, string username = null, string password = null)
    {

        string token = null;
        if (!string.IsNullOrEmpty(serverUrl) && !string.IsNullOrEmpty(password))
            token = await GraphDBAPI.GetGDBToken(serverUrl, username, password);


        using HttpClient client = new();
        HttpRequestMessage request = new(HttpMethod.Get, serverUrl + "repositories");

        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("GDB", token);

        client.DefaultRequestHeaders.Add("Accept", "application/sparql-results+json");

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
            return RepositoryStatus.CouldntConnect;
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


            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return RepositoryStatus.Unauthorized;

            return RepositoryStatus.Failed;
        }


        string answer = await response.Content.ReadAsStringAsync();


        return ContainsRepositoryId(answer, repositoryId);
    }

    private static RepositoryStatus ContainsRepositoryId(string jsonString, string repositoryId)
    {
        JObject jsonObj = JObject.Parse(jsonString);

        if (jsonObj.ContainsKey("results") && jsonObj["results"] is JObject resultObj && resultObj.ContainsKey("bindings"))
        {
            JArray bindings = (JArray)jsonObj["results"]["bindings"];

            foreach (JObject binding in bindings)
            {
                if (!binding.ContainsKey("id"))
                    continue;


                if (!(binding["id"] is JObject jId && jId.ContainsKey("value")))
                    continue;

                string id = (string)jId["value"];
                if (id != repositoryId)
                    continue;

                if (!binding.ContainsKey("readable"))
                    continue;

                if (!(binding["readable"] is JObject jReadable && jReadable.ContainsKey("value")))
                    continue;

                bool isReadable = ((string)jReadable["value"]) == "true";

                return isReadable ? RepositoryStatus.Exist : RepositoryStatus.ExistButUnreadable;
            }
        }

        return RepositoryStatus.Nonexistent;
    }

    public static async Task<string> GetGDBToken(string serverUrl, string username, string password)
    {
        HttpClient httpClient = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, $"{serverUrl}/rest/login/{username}");
        request.Headers.Add("X-GraphDB-Password", password);

        try
        {
            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            if (response.Headers.TryGetValues("Authorization", out var tokenValues))
            {
                string token = tokenValues.FirstOrDefault();
                token = token.Replace("GDB ", "");
                return token;
            }
        }
        catch (Exception e)
        {
            HttpResponseMessage responseB = new()
            {
                StatusCode = HttpStatusCode.ServiceUnavailable,
                Content = new StringContent(e.Message)
            };

            OnErrorQuery?.Invoke(responseB);
            return null;
        }


        return null;
    }


    #endregion
}


public enum GraphDBAPIFileType
{
    Turtle,
    Rdf
}

public enum RepositoryStatus
{
    Exist,
    ExistButUnreadable,
    Nonexistent,
    Unauthorized,
    CouldntConnect,
    Failed
}