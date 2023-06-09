using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static GraphDBAPI;
using System.Net.Http;
using System.Net;
using System.Text;
using System;
using UnityEditor.Graphs;

public class GraphDBAPITest
{

    [Test]
    public async void QueryTest()
    {
        var api = new GraphDBAPI(null);

        var jsonText = await api.Query("PREFIX imdb: <http://academy.ontotext.com/imdb/>\r\n" +
                                    "PREFIX schema: <http://schema.org/>\r\n\r\n" +
                                    "SELECT ?movie ?director\r\n" +
                                    "WHERE {\r\n  " +
                                    "?movie a schema:Movie .\r\n  " +
                                    "?movie schema:director ?director .\r\n}");

        Debug.Log(jsonText);



        File.WriteAllText(Application.dataPath + "/KGVRUnity/Data/query.json", jsonText);

        var data = JsonConvert.DeserializeObject<JObject>(jsonText);

        // (string)obj["name"]
        Debug.Log(data);
    }

    [Test]
    public async void InsertTest()
    {
        string path = Path.Combine(Application.dataPath, "VRKGUnity", "Data", "cap44_1455283593", "wwwcidoc-crmorgcidoc-crm.rdf");
        string content = await FileHelper.LoadAsync(path);


        string sparqlQuery = "INSERT DATA {\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2002/07/owl#Ontology> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#> <http://purl.org/dc/elements/1.1/description> \"This is the RDF Schema for the RDF vocabulary terms in the RDF Namespace, defined in RDF 1.1 Concepts.\" .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#> <http://purl.org/dc/elements/1.1/date> \"2019-12-16\" .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#> <http://purl.org/dc/elements/1.1/title> \"The RDF Concepts Vocabulary (RDF)\" .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#value> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/1999/02/22-rdf-syntax-ns#Property> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#value> <http://www.w3.org/2000/01/rdf-schema#range> <http://www.w3.org/2000/01/rdf-schema#Resource> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#value> <http://www.w3.org/2000/01/rdf-schema#isDefinedBy> <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#value> <http://www.w3.org/2000/01/rdf-schema#domain> <http://www.w3.org/2000/01/rdf-schema#Resource> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#HTML> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2000/01/rdf-schema#Datatype> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#HTML> <http://www.w3.org/2000/01/rdf-schema#seeAlso> <http://www.w3.org/TR/rdf11-concepts/#section-html> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#HTML> <http://www.w3.org/2000/01/rdf-schema#subClassOf> <http://www.w3.org/2000/01/rdf-schema#Literal> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#HTML> <http://www.w3.org/2000/01/rdf-schema#isDefinedBy> <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#PlainLiteral> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2000/01/rdf-schema#Datatype> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#PlainLiteral> <http://www.w3.org/2000/01/rdf-schema#subClassOf> <http://www.w3.org/2000/01/rdf-schema#Literal> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#PlainLiteral> <http://www.w3.org/2000/01/rdf-schema#seeAlso> <http://www.w3.org/TR/rdf-plain-literal/> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#PlainLiteral> <http://www.w3.org/2000/01/rdf-schema#isDefinedBy> <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#JSON> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2000/01/rdf-schema#Datatype> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#JSON> <http://www.w3.org/2000/01/rdf-schema#seeAlso> <https://www.w3.org/TR/json-ld11/#the-rdf-json-datatype> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#JSON> <http://www.w3.org/2000/01/rdf-schema#subClassOf> <http://www.w3.org/2000/01/rdf-schema#Literal> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#JSON> <http://www.w3.org/2000/01/rdf-schema#isDefinedBy> <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#object> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/1999/02/22-rdf-syntax-ns#Property> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#object> <http://www.w3.org/2000/01/rdf-schema#range> <http://www.w3.org/2000/01/rdf-schema#Resource> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#object> <http://www.w3.org/2000/01/rdf-schema#domain> <http://www.w3.org/1999/02/22-rdf-syntax-ns#Statement> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#object> <http://www.w3.org/2000/01/rdf-schema#isDefinedBy> <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#langString> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2000/01/rdf-schema#Datatype> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#langString> <http://www.w3.org/2000/01/rdf-schema#seeAlso> <http://www.w3.org/TR/rdf11-concepts/#section-Graph-Literal> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#langString> <http://www.w3.org/2000/01/rdf-schema#subClassOf> <http://www.w3.org/2000/01/rdf-schema#Literal> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#langString> <http://www.w3.org/2000/01/rdf-schema#isDefinedBy> <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/1999/02/22-rdf-syntax-ns#Property> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2000/01/rdf-schema#domain> <http://www.w3.org/2000/01/rdf-schema#Resource> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2000/01/rdf-schema#range> <http://www.w3.org/2000/01/rdf-schema#Class> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2000/01/rdf-schema#isDefinedBy> <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#Alt> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2000/01/rdf-schema#Class> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#Alt> <http://www.w3.org/2000/01/rdf-schema#subClassOf> <http://www.w3.org/2000/01/rdf-schema#Container> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#Alt> <http://www.w3.org/2000/01/rdf-schema#isDefinedBy> <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#predicate> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/1999/02/22-rdf-syntax-ns#Property> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#predicate> <http://www.w3.org/2000/01/rdf-schema#range> <http://www.w3.org/2000/01/rdf-schema#Resource> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#predicate> <http://www.w3.org/2000/01/rdf-schema#isDefinedBy> <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#predicate> <http://www.w3.org/2000/01/rdf-schema#domain> <http://www.w3.org/1999/02/22-rdf-syntax-ns#Statement> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#first> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/1999/02/22-rdf-syntax-ns#Property> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#first> <http://www.w3.org/2000/01/rdf-schema#domain> <http://www.w3.org/1999/02/22-rdf-syntax-ns#List> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#first> <http://www.w3.org/2000/01/rdf-schema#isDefinedBy> <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#first> <http://www.w3.org/2000/01/rdf-schema#range> <http://www.w3.org/2000/01/rdf-schema#Resource> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#Bag> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2000/01/rdf-schema#Class> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#Bag> <http://www.w3.org/2000/01/rdf-schema#subClassOf> <http://www.w3.org/2000/01/rdf-schema#Container> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#Bag> <http://www.w3.org/2000/01/rdf-schema#isDefinedBy> <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#rest> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/1999/02/22-rdf-syntax-ns#Property> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#rest> <http://www.w3.org/2000/01/rdf-schema#range> <http://www.w3.org/1999/02/22-rdf-syntax-ns#List> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#rest> <http://www.w3.org/2000/01/rdf-schema#isDefinedBy> <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#rest> <http://www.w3.org/2000/01/rdf-schema#domain> <http://www.w3.org/1999/02/22-rdf-syntax-ns#List> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#subject> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/1999/02/22-rdf-syntax-ns#Property> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#subject> <http://www.w3.org/2000/01/rdf-schema#isDefinedBy> <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#subject> <http://www.w3.org/2000/01/rdf-schema#range> <http://www.w3.org/2000/01/rdf-schema#Resource> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#subject> <http://www.w3.org/2000/01/rdf-schema#domain> <http://www.w3.org/1999/02/22-rdf-syntax-ns#Statement> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#direction> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/1999/02/22-rdf-syntax-ns#Property> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#direction> <http://www.w3.org/2000/01/rdf-schema#isDefinedBy> <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#direction> <http://www.w3.org/2000/01/rdf-schema#seeAlso> <https://www.w3.org/TR/json-ld11/#the-rdf-compoundliteral-class-and-the-rdf-language-and-rdf-direction-properties> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#direction> <http://www.w3.org/2000/01/rdf-schema#domain> <http://www.w3.org/1999/02/22-rdf-syntax-ns#CompoundLiteral> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#language> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/1999/02/22-rdf-syntax-ns#Property> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#language> <http://www.w3.org/2000/01/rdf-schema#seeAlso> <https://www.w3.org/TR/json-ld11/#the-rdf-compoundliteral-class-and-the-rdf-language-and-rdf-direction-properties> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#language> <http://www.w3.org/2000/01/rdf-schema#domain> <http://www.w3.org/1999/02/22-rdf-syntax-ns#CompoundLiteral> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#language> <http://www.w3.org/2000/01/rdf-schema#isDefinedBy> <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#Seq> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2000/01/rdf-schema#Class> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#Seq> <http://www.w3.org/2000/01/rdf-schema#isDefinedBy> <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#Seq> <http://www.w3.org/2000/01/rdf-schema#subClassOf> <http://www.w3.org/2000/01/rdf-schema#Container> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#nil> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/1999/02/22-rdf-syntax-ns#List> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#nil> <http://www.w3.org/2000/01/rdf-schema#isDefinedBy> <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#XMLLiteral> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2000/01/rdf-schema#Datatype> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#XMLLiteral> <http://www.w3.org/2000/01/rdf-schema#subClassOf> <http://www.w3.org/2000/01/rdf-schema#Literal> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#XMLLiteral> <http://www.w3.org/2000/01/rdf-schema#isDefinedBy> <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#List> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2000/01/rdf-schema#Class> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#List> <http://www.w3.org/2000/01/rdf-schema#isDefinedBy> <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#List> <http://www.w3.org/2000/01/rdf-schema#subClassOf> <http://www.w3.org/2000/01/rdf-schema#Resource> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#CompoundLiteral> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2000/01/rdf-schema#Class> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#CompoundLiteral> <http://www.w3.org/2000/01/rdf-schema#subClassOf> <http://www.w3.org/2000/01/rdf-schema#Resource> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#CompoundLiteral> <http://www.w3.org/2000/01/rdf-schema#isDefinedBy> <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#CompoundLiteral> <http://www.w3.org/2000/01/rdf-schema#seeAlso> <https://www.w3.org/TR/json-ld11/#the-rdf-compoundliteral-class-and-the-rdf-language-and-rdf-direction-properties> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#Statement> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2000/01/rdf-schema#Class> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#Statement> <http://www.w3.org/2000/01/rdf-schema#subClassOf> <http://www.w3.org/2000/01/rdf-schema#Resource> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#Statement> <http://www.w3.org/2000/01/rdf-schema#isDefinedBy> <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#Property> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2000/01/rdf-schema#Class> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#Property> <http://www.w3.org/2000/01/rdf-schema#isDefinedBy> <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .\r\n  <http://www.w3.org/1999/02/22-rdf-syntax-ns#Property> <http://www.w3.org/2000/01/rdf-schema#subClassOf> <http://www.w3.org/2000/01/rdf-schema#Resource> .\r\n}\r\n";
        string encodedQuery = WebUtility.UrlEncode(sparqlQuery);
        using (HttpClient client = new HttpClient())
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:7200/" + "repositories/" + "TestOntology" + "/statements");
            //request.Content = new StringContent(sparqlQuery, Encoding.UTF8);
            //request.Content = new StringContent(content, Encoding.UTF8, "application/rdf+xml");
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
                return;
            }

            if (!response.IsSuccessStatusCode)
            {
                OnErrorQuery?.Invoke(response);
                return;
            }

            response.EnsureSuccessStatusCode();

            return;
        }

    }

    //[Test]
    //public async void GetNodes()
    //{
    //    var api = new GraphDBAPI();
    //    var json = await api.Query("PREFIX imdb: <http://academy.ontotext.com/imdb/>\r\nPREFIX schema: <http://schema.org/>\r\n\r\nSELECT ?movie ?director\r\nWHERE {\r\n  ?movie a schema:Movie .\r\n  ?movie schema:director ?director .\r\n} LIMIT 50");


    //    var data = JsonConvert.DeserializeObject<JObject>(json);

    //    var nodes = new List<Node>();
    //    var edges = new List<Edge>();


    //    string directorEdgeUri = "http://schema.org";
    //    string directorEdgeLabel = "director";

    //    foreach (JToken binding in data["results"]["bindings"])
    //    {
    //        var movieUriFull = binding["movie"]["value"].Value<string>();
    //        var directorUriFull = binding["director"]["value"].Value<string>();

    //        int movieLastSlashId = movieUriFull.LastIndexOf('/');
    //        string movieUri = movieUriFull.Substring(0, movieLastSlashId);
    //        string movieLabel = movieUriFull.Substring(movieLastSlashId + 1);

    //        int directorLastSlashId = directorUriFull.LastIndexOf('/');
    //        string directorUri = directorUriFull.Substring(0, directorLastSlashId);
    //        string directorLabel = directorUriFull.Substring(directorLastSlashId + 1);

    //        var movie = new Node("Movie", movieLabel, movieUri);
    //        var director = new Node("Director", directorLabel, directorUri);
    //        nodes.Add(movie);
    //        nodes.Add(director);


    //        edges.Add(new Edge(directorEdgeLabel, directorEdgeUri, movie, director));
    //    }



    //    Debug.Log(nodes + " " + edges);
    //}

}
